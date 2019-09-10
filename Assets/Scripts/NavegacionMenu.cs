using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class NavegacionMenu : MonoBehaviour{

    public TextMeshProUGUI administracion, tareas, tienda, textoAtras;
    public GameObject panelAdmin, panelTareas, panelTienda;
    public Button anadirTarea, anadirArticulo;
    public CambiarDeEscena cambiarDeEscena;
    public GestionTareas gestionTareas;
    public GestionArticulos gestionArticulos;

    public void Portada()
    {
        if (panelAdmin.gameObject.activeSelf)
        {
            cambiarDeEscena.Portada();
            tareas.gameObject.SetActive(false);
            tienda.gameObject.SetActive(false);
        }
        else if (tareas.gameObject.activeSelf || tienda.gameObject.activeSelf)
        {            
            tareas.gameObject.SetActive(false);            
            tienda.gameObject.SetActive(false);
            panelTienda.gameObject.SetActive(false);
            panelTareas.gameObject.SetActive(false);
            anadirTarea.gameObject.SetActive(false);
            anadirArticulo.gameObject.SetActive(false);
            administracion.gameObject.SetActive(true);
            panelAdmin.gameObject.SetActive(true);
            textoAtras.text = "Menu Principal";

            gestionTareas.VaciarListaTareas();
            gestionArticulos.VaciarListaArticulos();

        }

    }

    public void BotonAtras()
    {
        textoAtras.text = "Atrás";
    }

 
}
