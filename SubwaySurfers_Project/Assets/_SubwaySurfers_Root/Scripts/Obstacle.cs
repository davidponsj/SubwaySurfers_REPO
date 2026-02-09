using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum Type { Train, Jump, Slide }
    public Type type;
    public int lane; // -1, 0, 1
}