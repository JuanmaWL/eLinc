using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CambiarDeEscena : MonoBehaviour
{

    public GameObject fondo;

    public void Portada()
    {
        Initiate.Fade("Portada", fondo.GetComponent<Image>().color, 4f);
    }
    public void Jugar()
    {
        Initiate.Fade("Juego", Color.black, 2.5f);
    }

    public void Cartas()
    {
        Initiate.Fade("Cartas", Color.black, 2.5f);
    }

    public void PanelAdministracion()
    {
        Initiate.Fade("Administracion", fondo.GetComponent<Image>().color, 3.5f);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
