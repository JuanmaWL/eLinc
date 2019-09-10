using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GestionArticulos : MonoBehaviour
{
    public GameObject prefabEspecial, contenedorArticuloEspecial;
    public Color colorFondoArticulo, colorFondoInputError;
    GameObject[] arrayPrefab;
    SubirNivelyMonedas subir;
    ListaArticulosEspeciales listaArticulos = null;

    static string filePath, jsonString, jsonToSave, folderPath;

    public void ExtraerArticulosEspeciales(bool administracion)
    {
        //Gestión del fichero

        CrearFichero();
        CargaDeFichero();

        if (listaArticulos.articulos.Count() == 0)
        {
            contenedorArticuloEspecial.transform.parent.
                  gameObject.transform.parent.
                  gameObject.transform.parent.Find("PanelMensaje").gameObject.SetActive(true);
        }

        //Se crea un array con el número de articulos que hay en la lista que previamente se ha leido de fichero
        arrayPrefab = new GameObject[listaArticulos.articulos.Count()];

        for (int i = 0; i < arrayPrefab.Length; i++)
        {
            //Se mete el prefab de Articulo dentro del Contenedor correspondiente
            arrayPrefab[i] = GameObject.Instantiate(prefabEspecial, Vector3.forward, Quaternion.identity, contenedorArticuloEspecial.transform);
            arrayPrefab[i].GetComponent<RectTransform>().localPosition = Vector3.forward;

            GameObject idEspecial = arrayPrefab[i].transform.Find("IDEspecial").gameObject;
            TMP_Text ID = idEspecial.GetComponent<TMP_Text>();
            ID.text = "#" + listaArticulos.articulos[i].id.ToString();

            GameObject inputNombre;
            GameObject inputPrecio;
            GameObject inputNivel;

            //Si estamos en el panel de Administración
            if (administracion)
            {
                inputNombre = arrayPrefab[i].transform.Find("InputArticulo").gameObject;
                TMP_InputField nombre = inputNombre.GetComponent<TMP_InputField>();
                nombre.text = listaArticulos.articulos[i].nombre;

                inputPrecio = arrayPrefab[i].transform.Find("InputPrecio").gameObject;
                TMP_InputField precio = inputPrecio.GetComponent<TMP_InputField>();
                precio.text = listaArticulos.articulos[i].precio.ToString();

                inputNivel = arrayPrefab[i].transform.Find("InputNivel").gameObject;
                TMP_InputField nivel = inputNivel.GetComponent<TMP_InputField>();
                nivel.text = listaArticulos.articulos[i].nivel.ToString();

                GameObject fondoComprado = arrayPrefab[i].transform.Find("FondoComprado").gameObject;
                TMP_Text textoComprado = fondoComprado.transform.Find("TextoComprado").gameObject.GetComponent<TMP_Text>();
                if (listaArticulos.articulos[i].comprado == false) textoComprado.text = "Sin comprar";
                else textoComprado.text = "Comprado";
            }

            //Si estamos en el juego
            else if (administracion == false)
            {
                inputNombre = arrayPrefab[i].transform.Find("FondoNombre").gameObject;
                TMP_Text nombre = inputNombre.transform.GetChild(0).GetComponent<TMP_Text>();
                nombre.text = listaArticulos.articulos[i].nombre;

                inputPrecio = arrayPrefab[i].transform.Find("FondoPrecio").gameObject;
                TMP_Text precio = inputPrecio.transform.GetChild(1).GetComponent<TMP_Text>();
                precio.text = listaArticulos.articulos[i].precio.ToString();

                inputNivel = arrayPrefab[i].transform.Find("FondoItem").gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject;
                TMP_Text nivel = inputNivel.GetComponent<TMP_Text>();
                nivel.text = listaArticulos.articulos[i].nivel.ToString();

                GameObject articuloComprado = arrayPrefab[i].transform.Find("ArticuloComprado").gameObject;
                TMP_Text comprado = articuloComprado.GetComponent<TMP_Text>();
                if (listaArticulos.articulos[i].comprado == false) comprado.text = "0";
                else comprado.text = "1";

                if (listaArticulos.articulos[i].comprado == false)
                {
                    arrayPrefab[i].transform.Find("btnComprar").gameObject.SetActive(true);
                    arrayPrefab[i].transform.Find("Comprado").gameObject.SetActive(false);
                }
                else
                {
                    arrayPrefab[i].transform.Find("btnComprar").gameObject.SetActive(false);
                    arrayPrefab[i].transform.Find("Comprado").gameObject.SetActive(true);
                }
                

            }
        }

        //Cambia el pivote del contenedor de articulos especiales para que se vea arriba del todo, estando la #1 la primera.
        contenedorArticuloEspecial.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
    }

    public void AnadirNuevoArticuloEspecial()
    {
        GameObject nuevoArticulo;
        nuevoArticulo = GameObject.Instantiate(prefabEspecial, Vector3.forward, Quaternion.identity, contenedorArticuloEspecial.transform);

        nuevoArticulo.GetComponent<RectTransform>().localPosition = Vector3.forward;
        nuevoArticulo.GetComponent<Image>().color = Color.cyan;
        nuevoArticulo.transform.Find("IDEspecial").GetComponent<TMP_Text>().text = "";
        nuevoArticulo.transform.Find("InputArticulo").GetComponent<TMP_InputField>().interactable = true;
        nuevoArticulo.transform.Find("InputPrecio").GetComponent<TMP_InputField>().interactable = true;
        nuevoArticulo.transform.Find("InputNivel").GetComponent<TMP_InputField>().interactable = true;
        nuevoArticulo.transform.Find("btnGuardar").GetComponent<Button>().interactable = true;

        GameObject fondoComprado = nuevoArticulo.transform.Find("FondoComprado").gameObject;
        TMP_Text textoComprado = fondoComprado.transform.Find("TextoComprado").gameObject.GetComponent<TMP_Text>();
        textoComprado.text = "Sin comprar";

        nuevoArticulo.GetComponent<FadeOutScript>().FadeIn();
        nuevoArticulo.GetComponent<Animator>().Play("AnadirTarea");

        if (contenedorArticuloEspecial.transform.childCount < 4) contenedorArticuloEspecial.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        else contenedorArticuloEspecial.GetComponent<RectTransform>().pivot = new Vector2(0, -1);

        //Quita el mensaje de "No hay articulos"
        if (contenedorArticuloEspecial.transform.childCount >= 1)
        {
            contenedorArticuloEspecial.
                      gameObject.transform.parent.
                      gameObject.transform.parent.
                      gameObject.transform.parent.Find("PanelMensaje").gameObject.SetActive(false);
        }
    }

    public void EliminarArticulo(GameObject prefab)
    {
        CargaDeFichero();

        contenedorArticuloEspecial = prefab.transform.parent.gameObject;
        contenedorArticuloEspecial.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);

        if (prefab.GetComponent<Image>().color == Color.cyan)
        {
            Destroy(prefab, 0.5f);
        }
        else
        {
            ArticuloEspecial articuloEliminado = new ArticuloEspecial();
            //Se comprueba qué ID tiene el articulo que estamos eliminando, para localizarla en la lista de objetos articulo en base a su atributo ID.
            string idArticulo = prefab.transform.Find("IDEspecial").gameObject.GetComponent<TMP_Text>().text;
            idArticulo = idArticulo.Substring(1, idArticulo.Length - 1);
            articuloEliminado = listaArticulos.articulos.Find(x => x.id == int.Parse(idArticulo));

            listaArticulos.articulos.Remove(articuloEliminado);

            Destroy(prefab, 0.5f);
            EscribirEnFichero();

        }

        if (contenedorArticuloEspecial.transform.childCount <= 1)
        {
             contenedorArticuloEspecial.
             gameObject.transform.parent.
             gameObject.transform.parent.
             gameObject.transform.parent.
             Find("PanelMensaje").gameObject.SetActive(true);
        }
    }

    public void GuardarArticulo(GameObject prefab)
    {
        CargaDeFichero();

        //Si el Articulo es nuevo (por el fondo verde)
        if (prefab.GetComponent<Image>().color == Color.cyan)
        {

            //Si no hay ningun Articulo en listaArticulos
            if (listaArticulos.articulos.Count() == 0)
            {
                ArticuloEspecial articulo = new ArticuloEspecial();

                if (cogerDatosYResetPrefab(articulo, prefab))
                {
                    prefab.transform.Find("IDEspecial").GetComponent<TMP_Text>().text = "#1"; articulo.id = 1;
                    listaArticulos.articulos.Add(articulo);
                    EscribirEnFichero();
                }

            }

            //Si ya hay Articulos en listaArticulos
            else
            {
                ArticuloEspecial ultimoArticulo = new ArticuloEspecial();
                ultimoArticulo = listaArticulos.articulos[listaArticulos.articulos.Count() - 1];
                ArticuloEspecial articulo = new ArticuloEspecial();

                // Para no asignar un ID repetido, se recorre la lista tantas veces como ID tiene el último elemento (Ej: ID=12).
                // hasta que se encuentra el primer ID en la lista que no existe. 
                // Si existe el ID 1, 2, 3 y 5, se asignará el 4 al nuevo Articulo y parará el bucle.
                // Si existiesen 1, 2, 3, 4, 5... 12, el nuevo Articulo tendría el ID 13.
                for (int i = 1; i <= ultimoArticulo.id + 1; i++)
                {
                    if (!listaArticulos.articulos.Exists(x => x.id == i))
                    {
                        articulo.id = i;
                        break;
                    }
                }

                if (cogerDatosYResetPrefab(articulo, prefab))
                {
                    prefab.transform.Find("IDEspecial").GetComponent<TMP_Text>().text = "#" + articulo.id.ToString();
                    listaArticulos.articulos.Add(articulo);
                    EscribirEnFichero();
                }
            }
        }

    }

    public bool cogerDatosYResetPrefab(ArticuloEspecial articulo, GameObject prefab)
    {
        bool sePuedeGuardar = false;

        TMP_InputField inputArticulo = prefab.transform.Find("InputArticulo").GetComponent<TMP_InputField>();
        TMP_InputField inputPrecio = prefab.transform.Find("InputPrecio").GetComponent<TMP_InputField>();
        TMP_InputField inputNivel = prefab.transform.Find("InputNivel").GetComponent<TMP_InputField>();
        Button botonGuardar = prefab.transform.Find("btnGuardar").GetComponent<Button>();

        if (inputArticulo.text == "" || inputPrecio.text == "" || inputNivel.text == "")
        {
            if (inputArticulo.text == "")
            {
                inputArticulo.image.color = colorFondoInputError;
                if (!(inputPrecio.text == "")) inputPrecio.image.color = Color.white;
                if (!(inputNivel.text == "")) inputNivel.image.color = Color.white;
            }
            if (inputPrecio.text == "")
            {
                inputPrecio.image.color = colorFondoInputError;
                if (!(inputArticulo.text == "")) inputArticulo.image.color = Color.white;
                if (!(inputNivel.text == "")) inputNivel.image.color = Color.white;
            }
            if (inputNivel.text == "")
            {
                inputNivel.image.color = colorFondoInputError;
                if (!(inputArticulo.text == "")) inputArticulo.image.color = Color.white;
                if (!(inputPrecio.text == "")) inputPrecio.image.color = Color.white;
            }

            sePuedeGuardar = false;
        }

        else
        {
            inputArticulo.image.color = Color.white;
            inputPrecio.image.color = Color.white;
            inputNivel.image.color = Color.white;
            articulo.comprado = false;
            articulo.precio = int.Parse(inputPrecio.text);
            articulo.nivel = int.Parse(inputNivel.text);
            articulo.nombre = inputArticulo.text;
            prefab.GetComponent<Image>().color = colorFondoArticulo;
            inputArticulo.interactable = false;
            inputPrecio.interactable = false;
            inputNivel.interactable = false;
            botonGuardar.interactable = false;

            sePuedeGuardar = true;
        }

        return sePuedeGuardar;
    }

    public void ComprarArticuloPersonalizado(GameObject prefab)
    {
        CargaDeFichero();
        ArticuloEspecial articuloComprado = new ArticuloEspecial();

        prefab.GetComponent<Image>().color = Color.cyan;

        //Se comprueba qué ID tiene la tarea que estamos reclamando, para localizarla en la lista de objetos tarea en base a su atributo ID.
        string idTarea = prefab.transform.Find("IDEspecial").gameObject.GetComponent<TMP_Text>().text;
        idTarea = idTarea.Substring(1, idTarea.Length - 1);

        TMP_Text comprado = prefab.transform.Find("ArticuloComprado").GetComponent<TMP_Text>();
        comprado.text = "1";

        articuloComprado = listaArticulos.articulos.Find(x => x.id == int.Parse(idTarea));
        articuloComprado.comprado = true;

        
        prefab.transform.Find("Comprado").gameObject.SetActive(true);
        Button botonComprar = prefab.transform.Find("btnComprar").GetComponent<Button>();
        botonComprar.interactable = false;        

        GameObject fondoPrecio = prefab.transform.Find("FondoPrecio").gameObject;
        int inputPrecio = int.Parse(fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().text);

        subir = prefab.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.Find("MANAGER CANVAS").GetComponent<SubirNivelyMonedas>();

        subir.subirMonedas(-inputPrecio);

  

        EscribirEnFichero();
    }

    public void VaciarListaArticulos()
    {
        foreach (Transform child in contenedorArticuloEspecial.transform) Destroy(child.gameObject);
    }

    public void CargaDeFichero()
    {
        jsonString = File.ReadAllText(filePath);
        listaArticulos = JsonUtility.FromJson<ListaArticulosEspeciales>(jsonString);
    }

    public void EscribirEnFichero()
    {
        listaArticulos.articulos.Sort((a, b) => a.id.CompareTo(b.id));
        jsonToSave = JsonUtility.ToJson(listaArticulos);
        File.WriteAllText(filePath, jsonToSave);
    }

    public void CrearFichero()
    {
        folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/json/";
        filePath = folderPath + "Articulos.json";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            File.WriteAllText(filePath, "{\"articulos\": [{\"id\":1,\"nombre\":\"Ejemplo\",\"precio\":10,\"comprado\":false,\"nivel\":5}]}");

        }
    }
}

[System.Serializable]
public class Articulo
{
    public string nombre;
    public int precio;
}


[System.Serializable]
public class ArticuloEspecial : Articulo
{
    public int id;
    public bool comprado;
    public int nivel;
}

[System.Serializable]
public class ListaArticulosEspeciales
{
    public List<ArticuloEspecial> articulos;

    public void Listar()
    {
        foreach (ArticuloEspecial articulo in articulos)
        {
            Debug.Log(articulo);
        }
    }
}

