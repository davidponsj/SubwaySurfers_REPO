using UnityEngine;

public class SuperJumpBoost : MonoBehaviour
{
    [Header("Configuración del Boost")]
    [SerializeField] private float jumpMultiplier = 2f;      // Multiplicador del salto (2x = doble altura)
    [SerializeField] private float boostDuration = 5f;       // Duración del boost en segundos
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Visual (Opcional)")]
    [SerializeField] private GameObject boostEffect;         // Partículas o efecto visual en el jugador

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip collectSound;

    private void Update()
    {
        // Rotar el power-up
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el jugador
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Activar el boost en el jugador
        player.ActivateSuperJump(jumpMultiplier, boostDuration);

        // Reproducir sonido
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Destruir el power-up
        Destroy(gameObject);
    }
}