using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] tilePrefabs;  // Array de tiles diferentes

    [Header("Configuración de Spawn")]
    [SerializeField] private int numberOfTilesOnScreen = 5;  // Tiles visibles a la vez
    [SerializeField] private float tileLength = 50f;          // Longitud de cada tile
    [SerializeField] private float safeZone = 100f;           // Distancia antes de spawnear nuevos tiles

    [Header("Tile Inicial")]
    [SerializeField] private GameObject startingTile;         // Tile sin obstáculos al inicio

    private List<GameObject> activeTiles = new List<GameObject>();
    private float nextSpawnZ = 0f;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("¡Falta asignar el Player en TileSpawner!");
            return;
        }

        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogError("¡Falta asignar Tile Prefabs en TileSpawner!");
            return;
        }

        // Spawnear tile inicial (sin obstáculos)
        if (startingTile != null)
        {
            SpawnTile(startingTile);
        }

        // Spawnear los primeros tiles
        for (int i = 0; i < numberOfTilesOnScreen; i++)
        {
            SpawnRandomTile();
        }
    }

    private void Update()
    {
        // Spawnear nuevo tile cuando el jugador se acerca
        if (player.position.z + safeZone > nextSpawnZ)
        {
            SpawnRandomTile();
        }

        // Eliminar tiles que el jugador ya pasó
        DeleteOldTiles();
    }

    private void SpawnRandomTile()
    {
        // Elegir un tile aleatorio
        GameObject tilePrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
        SpawnTile(tilePrefab);
    }

    private void SpawnTile(GameObject tilePrefab)
    {
        // Instanciar el tile en la siguiente posición Z
        GameObject newTile = Instantiate(tilePrefab, new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        newTile.transform.SetParent(transform);

        activeTiles.Add(newTile);

        // Calcular siguiente posición
        nextSpawnZ += tileLength;
    }

    private void DeleteOldTiles()
    {
        // Eliminar tiles que están muy atrás del jugador
        if (activeTiles.Count > 0)
        {
            GameObject oldestTile = activeTiles[0];

            if (oldestTile.transform.position.z + tileLength < player.position.z - safeZone)
            {
                activeTiles.RemoveAt(0);
                Destroy(oldestTile);
            }
        }
    }

    // Útil para debugging
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                new Vector3(-10, 0, player.position.z + safeZone),
                new Vector3(10, 0, player.position.z + safeZone)
            );
        }
    }
}