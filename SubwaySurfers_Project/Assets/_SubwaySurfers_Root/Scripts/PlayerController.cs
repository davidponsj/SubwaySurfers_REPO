using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    private Animator animator;
    private Rigidbody rb;
    private CapsuleCollider capsule;

    [Header("Movimiento")]
    [SerializeField] private float jumpForce = 7f;

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

        // Guardamos el tamaño original del collider
        originalHeight = capsule.height;
        originalCenter = capsule.center;

        // Evita que la animación mueva al personaje verticalmente
        animator.applyRootMotion = false;
    }

    // -----------------------------
    //      ACCIONES DEL PLAYER
    // -----------------------------

    public void Jump()
    {
        if (!isGrounded || isDead) return;

        isGrounded = false;
        animator.SetTrigger("Jump");

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            jumpForce,
            rb.linearVelocity.z
        );
    }

    public void Slide(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (isDead) return;

        animator.SetTrigger("Slide");

        capsule.height = slideHeight;
        capsule.center = slideCenter;

        CancelInvoke(nameof(ResetCollider));
        Invoke(nameof(ResetCollider), slideDuration);
    }


    public void Die()
    {
        isDead = true;
        animator.SetBool("Dead", true);
    }

    // -----------------------------
    //      COLISIONES
    // -----------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    // -----------------------------
    //      FUNCIONES INTERNAS
    // -----------------------------

    private void ResetCollider()
    {
        capsule.height = originalHeight;
        capsule.center = originalCenter;
    }
}
