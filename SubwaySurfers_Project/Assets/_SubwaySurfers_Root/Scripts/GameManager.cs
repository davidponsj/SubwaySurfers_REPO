using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Puntuación")]
    private int totalCoins = 0;
    private float distanceTraveled = 0f;

    [Header("Referencias")]
    [SerializeField] private Transform player;

    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    private void Update()
    {
        // Calcular distancia recorrida
        if (player != null)
        {
            float distance = player.position.z - lastPlayerPosition.z;
            if (distance > 0)
            {
                distanceTraveled += distance;
            }
            lastPlayerPosition = player.position;
        }
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log($"Monedas: {totalCoins}");
        // Aquí puedes actualizar UI si tienes
    }

    public int GetTotalCoins() => totalCoins;
    public float GetDistanceTraveled() => distanceTraveled;
}