using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    public static readonly List<Train> ActiveTrains = new List<Train>();

    [Header("Carril del tren")]
    public int lane; // -1 izquierda, 0 centro, 1 derecha

    [Header("Offsets desde el pivot")]
    public float frontOffset = 7.1f; // punta delantera
    public float backOffset = 7.1f;  // punta trasera

    public float FrontZ => transform.position.z + frontOffset;
    public float BackZ => transform.position.z - backOffset;

    private void OnEnable()
    {
        ActiveTrains.Add(this);
    }

    private void OnDisable()
    {
        ActiveTrains.Remove(this);
    }
}