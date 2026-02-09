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
    [SerializeField] private float laneDistance = 2.5f; // Distancia entre carriles
    [SerializeField] private float laneChangeSpeed = 10f;

    private int currentLane = 0; // -1 = izquierda, 0 = centro, 1 = derecha
    private Vector3 targetPosition;

    [Header("Slide Collider")]
    [SerializeField] private float slideHeight = 1f;
    [SerializeField] private Vector3 slideCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private float slideDuration = 1f;

    private float originalHeight;
    private Vector3 originalCenter;

    private bool isGrounded = true;
    private bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        originalHeight = capsule.height;
        originalCenter = capsule.center;

        animator.applyRootMotion = false;

        targetPosition = transform.position;
    }

    private void Update()
    {
        if (isDead) return;

        // Movimiento hacia adelante
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Movimiento suave hacia el carril objetivo
        Vector3 newPos = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPos, laneChangeSpeed * Time.deltaTime);
    }

    // -----------------------------
    //      INPUTS
    // -----------------------------

    public void MoveLeft(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (currentLane > -1)
        {
            currentLane--;
            UpdateLanePosition();
        }
    }

    public void MoveRight(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (currentLane < 1)
        {
            currentLane++;
            UpdateLanePosition();
        }
    }

    private void UpdateLanePosition()
    {
        targetPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!isGrounded || isDead) return;

        isGrounded = false;
        animator.SetTrigger("Jump");

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    public void Slide(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (isDead) return;

        animator.SetTrigger("Slide");

        capsule.height = slideHeight;
        capsule.center = slideCenter;

        CancelInvoke(nameof(ResetCollider));
        Invoke(nameof(ResetCollider), slideDuration);
    }

    private void ResetCollider()
    {
        capsule.height = originalHeight;
        capsule.center = originalCenter;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
