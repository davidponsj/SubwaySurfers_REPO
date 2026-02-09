using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("ESCENA1");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salir del juego"); // Para que funcione en el editor
    }
}
