using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider capsule;

    [Header("Movimiento")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float forwardSpeed = 5f;

    [Header("Carriles")]
    [SerializeField] private float laneDistance = 2.5f;
    [SerializeField] private float laneChangeSpeed = 10f;

    private int currentLane = 0;
    private Vector3 targetPosition;

    [Header("Slide Collider")]
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private Vector3 slideCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private float slideDuration = 1f;

    private float originalHeight;
    private Vector3 originalCenter;

    [Header("Muerte - Solo Visual")]
    [SerializeField] private float deathMoveBackDistance = 2f;
    [SerializeField] private float deathFallDistance = 3f;
    [SerializeField] private float deathAnimationSpeed = 2f;

    private bool isGrounded = true;
    private bool isDead = false;
    private bool isPlayingDeathAnimation = false;
    private Vector3 visualStartPosition;
    private Vector3 visualTargetPosition;
    private float deathAnimationProgress = 0f;

    private Transform visualContainer;

    private bool isJumping = false;
    private bool isSliding = false;

    [Header("Super Jump Boost")]
    private bool hasSuperJump = false;
    private float superJumpMultiplier = 1f;
    private float superJumpTimeRemaining = 0f;
    [SerializeField] private GameObject superJumpEffect;  // Efecto visual opcional

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        originalHeight = capsule.height;
        originalCenter = capsule.center;

        animator.applyRootMotion = false;

        targetPosition = transform.position;

        CreateVisualContainer();
    }

    private void CreateVisualContainer()
    {
        GameObject container = new GameObject("VisualContainer");
        container.transform.SetParent(transform);
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;
        visualContainer = container.transform;

        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        foreach (Transform child in children)
        {
            if (child == visualContainer) continue;
            child.SetParent(visualContainer);
        }
    }

    private void Update()
    {
        if (isDead)
        {
            if (isPlayingDeathAnimation)
            {
                deathAnimationProgress += Time.deltaTime * deathAnimationSpeed;
                visualContainer.localPosition = Vector3.Lerp(visualStartPosition, visualTargetPosition, deathAnimationProgress);

                if (deathAnimationProgress >= 1f)
                {
                    isPlayingDeathAnimation = false;
                }
            }
            return;
        }

        // Actualizar timer del super jump
        if (hasSuperJump)
        {
            superJumpTimeRemaining -= Time.deltaTime;
            if (superJumpTimeRemaining <= 0f)
            {
                DeactivateSuperJump();
            }
        }

        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        Vector3 newPos = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPos, laneChangeSpeed * Time.deltaTime);
    }

    // ---------------------------------------------------------
    //   BLOQUEO DE CARRIL POR TREN
    // ---------------------------------------------------------

    private bool IsLaneBlockedByTrain(int targetLane)
    {
        foreach (var train in Train.ActiveTrains)
        {
            if (train.lane != targetLane)
                continue;

            float playerZ = transform.position.z;

            if (playerZ >= train.BackZ && playerZ <= train.FrontZ)
                return true;
        }

        return false;
    }

    // ---------------------------------------------------------
    //   INPUTS
    // ---------------------------------------------------------

    public void MoveLeft(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        int targetLane = currentLane - 1;
        if (targetLane < -1) return;

        if (IsLaneBlockedByTrain(targetLane))
            return;

        currentLane = targetLane;
        UpdateLanePosition();
    }

    public void MoveRight(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        int targetLane = currentLane + 1;
        if (targetLane > 1) return;

        if (IsLaneBlockedByTrain(targetLane))
            return;

        currentLane = targetLane;
        UpdateLanePosition();
    }

    private void UpdateLanePosition()
    {
        targetPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
    }

    // ---------------------------------------------------------
    //   SALTO Y SLIDE
    // ---------------------------------------------------------

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!isGrounded || isDead) return;

        isGrounded = false;
        isJumping = true;

        animator.SetTrigger("Jump");

        // Aplicar multiplicador de super jump si está activo
        float currentJumpForce = jumpForce * (hasSuperJump ? superJumpMultiplier : 1f);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, currentJumpForce, rb.linearVelocity.z);

        Invoke(nameof(ResetJump), 0.6f);
    }

    private void ResetJump() => isJumping = false;

    public void Slide(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (isDead) return;

        isSliding = true;
        animator.SetTrigger("Slide");

        capsule.height = slideHeight;
        capsule.center = slideCenter;

        Invoke(nameof(ResetSlide), slideDuration);
    }

    private void ResetSlide()
    {
        isSliding = false;
        capsule.height = originalHeight;
        capsule.center = originalCenter;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    // ---------------------------------------------------------
    //   DETECCIÓN DE MUERTE POR OBSTÁCULOS
    // ---------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Obstacle>(out var obs))
            return;

        switch (obs.type)
        {
            case Obstacle.Type.Train:
                if (obs.lane != currentLane) return;
                if (isJumping) return;
                Die();
                break;

            case Obstacle.Type.Jump:
                if (obs.lane != currentLane) return;
                if (!isJumping) Die();
                break;

            case Obstacle.Type.Slide:
                if (obs.lane != currentLane) return;
                if (!isSliding) Die();
                break;
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("Dead", true);
        forwardSpeed = 0;

        isPlayingDeathAnimation = true;
        deathAnimationProgress = 0f;
        visualStartPosition = visualContainer.localPosition;

        visualTargetPosition = visualStartPosition + new Vector3(
            0,
            -deathFallDistance,
            -deathMoveBackDistance
        );

        // Notificar al Game Manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    // ---------------------------------------------------------
    //   SUPER JUMP BOOST
    // ---------------------------------------------------------

    public void ActivateSuperJump(float multiplier, float duration)
    {
        hasSuperJump = true;
        superJumpMultiplier = multiplier;
        superJumpTimeRemaining = duration;

        // Activar efecto visual si existe
        if (superJumpEffect != null)
        {
            superJumpEffect.SetActive(true);
        }

        Debug.Log($"¡Super Jump activado! x{multiplier} durante {duration}s");
    }

    private void DeactivateSuperJump()
    {
        hasSuperJump = false;
        superJumpMultiplier = 1f;

        // Desactivar efecto visual
        if (superJumpEffect != null)
        {
            superJumpEffect.SetActive(false);
        }

        Debug.Log("Super Jump desactivado");
    }

    public bool HasSuperJump() => hasSuperJump;
    public float GetSuperJumpTimeRemaining() => superJumpTimeRemaining;
}