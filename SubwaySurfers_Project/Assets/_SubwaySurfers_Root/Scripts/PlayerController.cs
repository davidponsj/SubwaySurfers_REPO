using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    public float jumpForce = 7f;
    private bool isGrounded = true;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // --- LLAMADOS DESDE UNITY EVENTS ---

    public void Jump()
    {
        if (!isGrounded || isDead) return;

        isGrounded = false;
        animator.SetTrigger("Jump");
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    public void Slide()
    {
        if (isDead) return;
        animator.SetTrigger("Slide");
    }

    public void Die()
    {
        isDead = true;
        animator.SetBool("Dead", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
