using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GestionTienda : MonoBehaviour
{
    public GameObject contenedorComida, contenedorPersonalizacion, contenedorEspecial;
    public GameObject prefabComida, prefabPersonalizacion;
    static int cantidadMonedas;
    public Sprite itemBloqueado, itemDesbloqueado;

    public void subirCantidad()
    {
        GameObject panelTienda = gameObject.transform.parent.gameObject
                                        .transform.parent.gameObject
                                        .transform.parent.gameObject
                                        .transform.parent.gameObject;

        cantidadMonedas = int.Parse(panelTienda.transform.GetChild(0).gameObject
                                        .transform.GetChild(0).gameObject
                                        .transform.GetChild(1).gameObject.GetComponent<Text>().text);

        int precio = int.Parse(gameObject.transform.Find("FondoPrecio").gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text);

        GameObject item = gameObject.transform.Find("FondoNumeroItems").gameObject;
        TMP_Text numeroItems = item.transform.Find("NumeroItems").gameObject.GetComponent<TMP_Text>();
        int cantidad = int.Parse(item.transform.Find("NumeroItems").gameObject.GetComponent<TMP_Text>().text);

        int precioFinal = precio * (cantidad + 1);

        if (precioFinal <= cantidadMonedas)
        {
            if (cantidad == 100) cantidad = 100;
            else cantidad += 1;
            numeroItems.text = cantidad.ToString();
        }
        else
        {
            numeroItems.color = Color.red;
        }
    }

    public void bajarCantidad()
    {
        GameObject panelTienda = gameObject.transform.parent.gameObject
                                                .transform.parent.gameObject
                                                .transform.parent.gameObject
                                                .transform.parent.gameObject;

        cantidadMonedas = int.Parse(panelTienda.transform.GetChild(0).gameObject
                                        .transform.GetChild(0).gameObject
                                        .transform.GetChild(1).gameObject.GetComponent<Text>().text);

        int precio = int.Parse(gameObject.transform.Find("FondoPrecio").gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text);

        GameObject item = gameObject.transform.Find("FondoNumeroItems").gameObject;
        TMP_Text numeroItems = item.transform.Find("NumeroItems").gameObject.GetComponent<TMP_Text>();
        int cantidad = int.Parse(item.transform.Find("NumeroItems").gameObject.GetComponent<TMP_Text>().text);

        int i = cantidad;

        if (numeroItems.color == Color.red)
        {
            i--;
            numeroItems.color = Color.white;
            numeroItems.text = i.ToString();
        }

        else
        {
            if (i <= 1) i = 1;
            else i--;
            numeroItems.text = i.ToString();
        }

    }

    public void colocarListasArriba(GameObject contenedor)
    {
        //Cada vez que se pinche el botón tienda, el scroll bar se situará en el primer item
        contenedor.transform.parent.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1;
    }

    //Métodos para comprobar que se tiene el nivel y las monedas suficientes para comprar los artículos
    public void comprobarDisponibilidadComida()
    {
        foreach (Transform child in contenedorComida.transform)
        {
            Button botonComprar = child.transform.Find("btnComprar").GetComponent<Button>();
            GameObject fondoItem = child.transform.Find("FondoItem").gameObject;
            GameObject gameobjectNivel = fondoItem.transform.Find("Nivel").gameObject;
            int inputNivel = int.Parse(gameobjectNivel.transform.Find("InputNivel").gameObject.GetComponent<TMP_Text>().text);

            GameObject fondoNumeroItems = child.transform.Find("FondoNumeroItems").gameObject;
            fondoNumeroItems.transform.Find("NumeroItems").gameObject.GetComponent<TMP_Text>().text = "1";
            fondoNumeroItems.transform.Find("NumeroItems").gameObject.GetComponent<TMP_Text>().color = Color.white;
            Button botonDisminuir = fondoNumeroItems.transform.Find("btnDisminuir").GetComponent<Button>();
            Button botonAumentar = fondoNumeroItems.transform.Find("btnAumentar").GetComponent<Button>();

            GameObject fondoPrecio = child.transform.Find("FondoPrecio").gameObject;
            int inputPrecio = int.Parse(fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().text);

            Image item = fondoItem.transform.Find("Item").GetComponent<Image>();
            Image candado = fondoItem.transform.Find("Candado").GetComponent<Image>();

            //Se reinicia cada campo del Item antes de comprobar
            fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.white;
            botonComprar.interactable = true;
            botonDisminuir.interactable = true;
            botonAumentar.interactable = true;

            int nivel = PlayerPrefs.GetInt("Nivel");
            int monedas = PlayerPrefs.GetInt("Monedas");

            //Si el nivel no es suficiente

            if (nivel < inputNivel)
            {
                candado.sprite = itemBloqueado;
                botonComprar.interactable = false;
                botonDisminuir.interactable = false;
                botonAumentar.interactable = false;

                if (monedas < inputPrecio)
                {
                    fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.red;
                }
            }

            else
            {
                candado.sprite = itemDesbloqueado;
                if (monedas < inputPrecio)
                {
                    fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.red;
                    botonComprar.interactable = false;
                    botonDisminuir.interactable = false;
                    botonAumentar.interactable = false;
                }
            }

        }
    }

    public void comprobarDisponibilidadPersonalizacion(GameObject contenedor)
    {
        foreach (Transform child in contenedor.transform)
        {
            //Controlamos que en el contenedorComplementos no se comprueben las cabeceras con texto
            //Con "continue" pasamos a la siguiente iteracion

            if (child.name == "Sombreros" || child.name == "Pajaritas" || child.name == "Calcetines") continue;

            Button botonComprar = child.transform.Find("btnComprar").GetComponent<Button>();
            Button botonAplicar = child.transform.Find("btnAplicar").GetComponent<Button>();
            TMP_Text comprado = child.transform.Find("COMPRADO").GetComponent<TMP_Text>();
            TMP_Text aplicado = child.transform.Find("APLICADO").GetComponent<TMP_Text>();
            TMP_Text aplicar = child.transform.Find("Aplicar").GetComponent<TMP_Text>();

            GameObject fondoItem = child.transform.Find("FondoItem").gameObject;
            GameObject gameobjectNivel = fondoItem.transform.Find("Nivel").gameObject;
            int inputNivel = int.Parse(gameobjectNivel.transform.Find("InputNivel").gameObject.GetComponent<TMP_Text>().text);

            GameObject fondoPrecio = child.transform.Find("FondoPrecio").gameObject;
            int inputPrecio = int.Parse(fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().text);

            Image item = fondoItem.transform.Find("Item").GetComponent<Image>();
            Image candado = fondoItem.transform.Find("Candado").GetComponent<Image>();

            //Se reinicia cada campo del Item antes de comprobar
            fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.white;
            botonComprar.interactable = true;

            int nivel = PlayerPrefs.GetInt("Nivel");
            int monedas = PlayerPrefs.GetInt("Monedas");

            //Si el item no está comprado aún
            if (int.Parse(comprado.text) == 0)
            {
                //botonAplicar.gameObject.SetActive(false);
                //botonAplicar.interactable = false;

                //Si el nivel no es suficiente
                if (nivel < inputNivel)
                {
                    candado.sprite = itemBloqueado;
                    botonComprar.interactable = false;
                    //Si las monedas no son suficientes
                    if (monedas < inputPrecio)
                    {
                        fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.red;
                    }
                }

                //Si el nivel es suficiente
                else
                {
                    candado.sprite = itemDesbloqueado;
                    //Si las monedas no son suficientes
                    if (monedas < inputPrecio)
                    {
                        fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.red;
                        botonComprar.interactable = false;
                    }
                }
            }

            //Si está comprado
            else
            {
                botonComprar.interactable = false;
                botonComprar.gameObject.SetActive(false);
                fondoPrecio.gameObject.SetActive(false);
                botonAplicar.interactable = true;
                botonAplicar.gameObject.SetActive(true);
                aplicar.gameObject.SetActive(true);

                if (int.Parse(aplicado.text) == 1) botonAplicar.interactable = false;

            }


        }

    }

    public void comprobarDisponibilidadEspecial()
    {
        foreach (Transform child in contenedorEspecial.transform)
        {
            Button botonComprar = child.transform.Find("btnComprar").GetComponent<Button>();
            TMP_Text comprado = child.transform.Find("ArticuloComprado").GetComponent<TMP_Text>();
            GameObject fondoItem = child.transform.Find("FondoItem").gameObject;
            GameObject gameobjectNivel = fondoItem.transform.Find("Nivel").gameObject;
            int inputNivel = int.Parse(gameobjectNivel.transform.Find("InputNivel").gameObject.GetComponent<TMP_Text>().text);

            GameObject fondoPrecio = child.transform.Find("FondoPrecio").gameObject;
            int inputPrecio = int.Parse(fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().text);

            Image item = fondoItem.transform.Find("Item").GetComponent<Image>();
            Image candado = fondoItem.transform.Find("Candado").GetComponent<Image>();

            //Se reinicia cada campo del Item antes de comprobar
            fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.white;
            botonComprar.interactable = true;

            int nivel = PlayerPrefs.GetInt("Nivel");
            int monedas = PlayerPrefs.GetInt("Monedas");

            //Si el articulo no está comprado
            if (int.Parse(comprado.text) == 0)
            {
                child.transform.Find("Comprado").gameObject.SetActive(false);

                //Si el nivel no es suficiente
                if (nivel < inputNivel)
                {
                    candado.sprite = itemBloqueado;
                    botonComprar.interactable = false;

                    if (monedas < inputPrecio)
                    {
                        fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.red;
                    }
                }

                else
                {
                    candado.sprite = itemDesbloqueado;
                    if (monedas < inputPrecio)
                    {
                        fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().color = Color.red;
                        botonComprar.interactable = false;
                    }
                }
            }

            //Si está comprado
            else
            {
                Debug.Log("ESTÁ COMPRADO");
                botonComprar.gameObject.SetActive(false);
                child.transform.Find("Comprado").gameObject.SetActive(true);
                child.GetComponent<Image>().color = Color.cyan;
                candado.sprite = itemDesbloqueado;
            }

        }

    }

}