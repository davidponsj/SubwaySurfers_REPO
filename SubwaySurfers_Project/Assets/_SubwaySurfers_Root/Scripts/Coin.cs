using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int coinValue = 1;

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip collectSound;


    private void OnTriggerEnter(Collider other)
    {
        // Solo el jugador puede recoger monedas
        if (!other.CompareTag("Player")) return;

        // Reproducir sonido si existe
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Añadir moneda al Game Manager (lo haremos después)
        GameManager.Instance?.AddCoins(coinValue);

        // Destruir la moneda
        Destroy(gameObject);
    }
}