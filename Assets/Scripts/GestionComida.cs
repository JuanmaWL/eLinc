using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GestionComida : MonoBehaviour
{
    SubirNivelyMonedas subir;
    GestionTienda gestionTienda;
    public GameObject prefabComida;
    public static ListaComida listaComida;
    static int posicionEnLista = 0;
    public Sprite[] spritesComida;
    static string filePath, jsonString, jsonToSave, folderPath;

    public void Start()
    {
        CrearFichero();
        CargaDeFichero();
    }


    //Se pulsa desde el prefab Plato, así que .gameObject es el propio plato
    public void LlenarPlato()
    {
        GameObject item = gameObject.transform.GetChild(0).gameObject;
        GameObject cantidad = gameObject.transform.GetChild(1).gameObject;
        TMP_Text textoCantidad = cantidad.gameObject.GetComponent<TMP_Text>();
        TMP_Text porcentaje = gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
        Button btnAnterior = gameObject.transform.GetChild(3).gameObject.GetComponent<Button>();
        Button btnSiguiente = gameObject.transform.GetChild(4).gameObject.GetComponent<Button>();
        btnAnterior.interactable = true;
        btnSiguiente.interactable = true;

        Image imagen = item.GetComponent<Image>();

        if (listaComida.listaComida.Count() == 1)
        {
            btnAnterior.interactable = false;
            btnSiguiente.interactable = false;
        }

        if (posicionEnLista + 1 >= listaComida.listaComida.Count())
        {
            btnAnterior.interactable = false;
        }

        if (posicionEnLista - 1 < 0)
        {
            btnSiguiente.interactable = false;
        }

        Comida comida = new Comida();
        comida = listaComida.listaComida[posicionEnLista];
        Debug.Log(comida.nombre);
        if (comida.infinito == true)
        {
            cantidad.SetActive(false);
            porcentaje.text = "5";
        }
        else
        {
            cantidad.SetActive(true);
            textoCantidad.text = comida.cantidad.ToString();
            porcentaje.text = comida.subidaComida.ToString();
        }

        for (int i = 0; i < spritesComida.Length; i++)
        {
            if (comida.imagen == spritesComida[i].name)
            {
                imagen.sprite = spritesComida[i];
            }
        }
    }


    public void ItemAnterior()
    {
        posicionEnLista += 1;
        Debug.Log("Posicion en la lista" + posicionEnLista);
        LlenarPlato();
    }
    public void ItemSiguiente()
    {
        posicionEnLista -= 1;
        Debug.Log("Posicion en la lista" + posicionEnLista);
        LlenarPlato();
    }

    public void AnadirComidaInventario(GameObject prefab)
    {

       subir = gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.Find("MANAGER CANVAS").GetComponent<SubirNivelyMonedas>();

        gestionTienda = gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.Find("MANAGER CANVAS").GetComponent<GestionTienda>();

        Comida comida = new Comida();
        Comida comidaAux = new Comida();

        string nombre = gameObject.transform.Find("FondoNombre")
                        .gameObject.transform.Find("InputNombre")
                        .gameObject.GetComponent<TMP_Text>().text.ToString();

        string imagen = gameObject.transform.Find("FondoItem")
                        .gameObject.transform.Find("Item")
                        .gameObject.GetComponent<Image>().sprite.name;

        int cantidad = int.Parse(gameObject.transform.Find("FondoNumeroItems")
                                .gameObject.transform.Find("NumeroItems")
                                .gameObject.GetComponent<TMP_Text>().text);


        int precio = int.Parse(gameObject.transform.Find("FondoPrecio")
                                .gameObject.transform.Find("InputPrecio")
                                .gameObject.GetComponent<TMP_Text>().text);


        string subidaComida = gameObject.transform.Find("FondoPropiedad")
                             .gameObject.transform.Find("InputPropiedad")
                             .gameObject.GetComponent<TMP_Text>().text.ToString();

        int subida = int.Parse(subidaComida.Substring(1, subidaComida.Length - 2));

        Debug.Log(gameObject);

        int precioTotal = cantidad * precio;

        subir.subirMonedas(-precioTotal);
        gestionTienda.comprobarDisponibilidadComida();


        //Busca en la lista el item con el mismo nombre, para que, si se añade cualquier cantidad, se vaya incrementando y 
        //no se creen entradas nuevas en el fichero JSON
        comidaAux = listaComida.listaComida.Find(x => x.nombre == nombre);
        if (comidaAux != null)
        {
            comidaAux.cantidad += cantidad;
            EscribirEnFichero();
        }
        else
        {
            comida.cantidad = cantidad;
            comida.imagen = imagen;
            comida.nombre = nombre;
            comida.subidaComida = subida;
            comida.precio = 0;
            comida.infinito = false;
            listaComida.listaComida.Add(comida);
            EscribirEnFichero();
        }
    }

    //Se usa en DragAndDropCell para eliminar los items del plato (y de la lista) cuando el número sea 0.

    public void EliminarItem(GameObject item)
    {
        GameObject plato = item.transform.parent.gameObject.transform.parent.gameObject.transform.Find("Plato").gameObject;
        GameObject comida = plato.transform.GetChild(0).gameObject;
        GameObject cantidad = plato.transform.GetChild(1).gameObject;

        TMP_Text textCantidad = cantidad.gameObject.GetComponent<TMP_Text>();
        TMP_Text porcentaje = plato.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();

        Button btnAnterior = plato.transform.GetChild(3).gameObject.GetComponent<Button>();
        Button btnSiguiente = plato.transform.GetChild(4).gameObject.GetComponent<Button>();

        Comida itemComida = new Comida();

        itemComida = listaComida.listaComida[posicionEnLista];
        int cantidadItems = int.Parse(textCantidad.text);

        //Si al eliminar el item hay una cantidad inferior a 1
        if (itemComida.cantidad - 1 <= 0)
        {
            //Controla que no se elimine nunca el item infinito (manzana roja) y sí el resto   
            if (!itemComida.infinito)
            {
                listaComida.listaComida.Remove(itemComida);

                //Si hay otros items en la lista de comida
                if (listaComida.listaComida.Count() != 0)
                {
                    posicionEnLista = 0;
                    comida.SetActive(true);
                    cantidad.SetActive(true);
                }
                else
                {
                    posicionEnLista = 1;
                    comida.SetActive(false);
                    cantidad.SetActive(false);
                }
            }
        }

        else
        {
            comida.SetActive(true);
            cantidad.SetActive(true);

            cantidadItems -= 1;
            itemComida.cantidad = cantidadItems;
            textCantidad.text = itemComida.cantidad.ToString();
        }

        EscribirEnFichero();
        LlenarPlato();

    }



    public void CargaDeFichero()
    {
        jsonString = File.ReadAllText(filePath);
        listaComida = JsonUtility.FromJson<ListaComida>(jsonString);

    }

    public void EscribirEnFichero()
    {
        listaComida.listaComida.Sort((a, b) => a.cantidad.CompareTo(b.cantidad));
        jsonToSave = JsonUtility.ToJson(listaComida);
        File.WriteAllText(filePath, jsonToSave);
    }

    public void CrearFichero()
    {
        folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/json/";
        filePath = folderPath + "Comida.json";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            File.WriteAllText(filePath, "{\"listaComida\": [{\"precio\":0,\"nombre\":\"Manzana Roja\",\"cantidad\":1,\"subidaComida\":3,\"infinito\":true,\"imagen\":\"manzanaroja\"}]}");

        }
    }
}

[System.Serializable]
public class Comida : Articulo
{
    public int cantidad;
    public string imagen;
    public int subidaComida;
    public bool infinito;

    public override string ToString()
    {
        return string.Format("Nombre: {0} | Cantidad: {1}", nombre, cantidad);
    }
}

[System.Serializable]
public class ListaComida
{
    public List<Comida> listaComida;

    public void Listar()
    {
        foreach (Comida comida in listaComida)
        {
            Debug.Log(comida);
        }
    }
}