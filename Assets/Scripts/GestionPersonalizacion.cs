using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class GestionPersonalizacion : MonoBehaviour
{
    SubirNivelyMonedas subir;
    GestionTienda gestionTienda;
    public GameObject prefabPersonalizacion;
    public GameObject contenedorPersonalizacion, contenedorComplementos;
    public Sprite[] spritesPersonalizacion;
    public static ListaPersonalizacion listaPersonalizacion;
    static string filePath, jsonString, jsonToSave, folderPath;

    private static int paredCopia, sombreroCopia, pajaritaCopia, calcetinesCopia;


    //Aplica el efecto de usar cada item
    public void AplicarItemPersonalizable(GameObject prefab)
    {
        CargaDeFichero();

        Button botonAplicar = prefab.transform.Find("btnAplicar").GetComponent<Button>();
        TMP_Text aplicar = prefab.transform.Find("Aplicar").GetComponent<TMP_Text>();
        GameObject fondoItem = prefab.transform.Find("FondoItem").gameObject;
        Image imagenArticulo = fondoItem.transform.Find("Item").GetComponent<Image>();
        int tipo = int.Parse(prefab.transform.Find("TIPO").GetComponent<TMP_Text>().text);

        GameObject Pared = gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.Find("SueloPared").transform.Find("Pared").gameObject;

        GameObject LINCE = gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.Find("Linces").gameObject;

        Personalizacion articuloPersonalizable = new Personalizacion();

        //SWITCH PARA QUITAR EL OBJETO ANTIGUO (APLICADO = FALSE) y gestionar con el UI, deshabilitando el botón Aplicar.

        switch (tipo)
        {
            case 1:                   //Paredes
                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 1 && x.aplicado == true))
                {
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 1 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;

            case 2:                   //Sombreros

                //Comprueba si hay algún sombrero (tipo 2) que está aplicado ya.
                //Si hay uno, localiza su posición dentro de Lince (Index del hijo) gracias a PlayerPreffs y deshabilita el objeto
                //También deshabilita el botón Aplicar del Prefab y cambia en la listaDeArticulos el atributo "aplicado" del item a false

                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 2 && x.aplicado == true))
                {
                    int posicionSombreroAntiguo = PlayerPrefs.GetInt("SombreroAplicado");
                    LINCE.transform.GetChild(posicionSombreroAntiguo).gameObject.SetActive(false);
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 2 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;

            case 3:                   //Pajaritas
                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 3 && x.aplicado == true))
                {
                    int posicionPajaritaAntigua = PlayerPrefs.GetInt("PajaritaAplicada");
                    LINCE.transform.GetChild(posicionPajaritaAntigua).gameObject.SetActive(false);
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 3 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;

            case 4:                   //Calcetines
                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 4 && x.aplicado == true))
                {
                    int posicionCalcetinesAntiguos = PlayerPrefs.GetInt("CalcetinesAplicados");
                    LINCE.transform.GetChild(posicionCalcetinesAntiguos).gameObject.SetActive(false);
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 4 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;
        }

        //Comprueba qué item es el que está aplicado para poder modificar su estado en el archivo JSON y volver a habilitar el botón        

        articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.imagen == imagenArticulo.sprite.name);
        articuloPersonalizable.aplicado = true;
        botonAplicar.interactable = false;
        aplicar.text = "Aplicado";
        
        //SWITCH PARA APLICAR EL OBJETO ACTUAL y guardar su posición como hijo de Lince 

        switch (tipo)
        {
            case 1:                   //Paredes

                //Localiza la posición del objeto aplicado en el array de sprites
                int posicionEnArray = Array.FindIndex(spritesPersonalizacion, x => x.name == articuloPersonalizable.imagen);
                Debug.Log("Posicion en Array " + posicionEnArray);
                Pared.GetComponent<SpriteRenderer>().sprite = imagenArticulo.sprite;
                PlayerPrefs.SetInt("ParedAplicada", posicionEnArray + 1);
                paredCopia = posicionEnArray + 1;
                break;

            case 2:                   //Sombreros
                string nombreSombrero = prefab.name;
                GameObject sombrero = LINCE.transform.Find(nombreSombrero).gameObject;
                int posicionSombreroEnLince = LINCE.transform.Find(nombreSombrero).GetSiblingIndex(); //Localiza la posición gracias al nombre del prefab y de los hijos de Lince
                sombrero.SetActive(true);
                PlayerPrefs.SetInt("SombreroAplicado", posicionSombreroEnLince);
                sombreroCopia = posicionSombreroEnLince;
                break;

            case 3:                   //Pajaritas
                string nombrePajarita = prefab.name;
                GameObject pajarita = LINCE.transform.Find(nombrePajarita).gameObject;
                int posicionPajaritaEnLince = LINCE.transform.Find(nombrePajarita).GetSiblingIndex();
                pajarita.SetActive(true);
                PlayerPrefs.SetInt("PajaritaAplicada", posicionPajaritaEnLince);
                pajaritaCopia = posicionPajaritaEnLince;
                break;

            case 4:                   //Calcetines
                string nombreCalcetines = prefab.name;
                GameObject calcetines = LINCE.transform.Find(nombreCalcetines).gameObject;
                int posicionCalcetinesEnLince = LINCE.transform.Find(nombreCalcetines).GetSiblingIndex();
                calcetines.SetActive(true);
                PlayerPrefs.SetInt("CalcetinesAplicados", posicionCalcetinesEnLince);
                calcetinesCopia = posicionCalcetinesEnLince;
                break;
        }

        EscribirEnFichero();
        ExtraerDatosFicheroPersonalizacion(gameObject.transform.parent.gameObject);
    }

    public void QuitarItemPersonalizable(GameObject prefab)
    {
        CargaDeFichero();

        Button botonAplicar = prefab.transform.Find("btnAplicar").GetComponent<Button>();
        TMP_Text aplicar = prefab.transform.Find("Aplicar").GetComponent<TMP_Text>();
        GameObject fondoItem = prefab.transform.Find("FondoItem").gameObject;
        Image imagenArticulo = fondoItem.transform.Find("Item").GetComponent<Image>();
        int tipo = int.Parse(prefab.transform.Find("TIPO").GetComponent<TMP_Text>().text);

        GameObject Pared = gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.Find("SueloPared").transform.Find("Pared").gameObject;

        GameObject LINCE = gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.parent.
                           gameObject.transform.Find("Linces").gameObject;

        Personalizacion articuloPersonalizable = new Personalizacion();

        //SWITCH PARA QUITAR EL OBJETO ANTIGUO (APLICADO = FALSE) y gestionar con el UI, deshabilitando el botón Aplicar.

        switch (tipo)
        {
            case 2:                   //Sombreros

                //Comprueba si hay algún sombrero (tipo 2) que está aplicado ya.
                //Si hay uno, localiza su posición dentro de Lince (Index del hijo) gracias a PlayerPreffs y deshabilita el objeto
                //También deshabilita el botón Aplicar del Prefab y cambia en la listaDeArticulos el atributo "aplicado" del item a false

                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 2 && x.aplicado == true))
                {
                    int posicionSombreroAntiguo = PlayerPrefs.GetInt("SombreroAplicado");
                    LINCE.transform.GetChild(posicionSombreroAntiguo).gameObject.SetActive(false);
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 2 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;

            case 3:                   //Pajaritas
                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 3 && x.aplicado == true))
                {
                    int posicionPajaritaAntigua = PlayerPrefs.GetInt("PajaritaAplicada");
                    LINCE.transform.GetChild(posicionPajaritaAntigua).gameObject.SetActive(false);
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 3 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;

            case 4:                   //Calcetines
                if (listaPersonalizacion.listaPersonalizacion.Exists(x => x.tipo == 4 && x.aplicado == true))
                {
                    int posicionCalcetinesAntiguos = PlayerPrefs.GetInt("CalcetinesAplicados");
                    LINCE.transform.GetChild(posicionCalcetinesAntiguos).gameObject.SetActive(false);
                    articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.tipo == 4 && x.aplicado == true);
                    Debug.Log("AplicarItem " + articuloPersonalizable);
                    articuloPersonalizable.aplicado = false;
                    botonAplicar.interactable = true;
                    aplicar.text = "Aplicar";
                }
                break;
        }

        EscribirEnFichero();
        ExtraerDatosFicheroPersonalizacion(gameObject.transform.parent.gameObject);
    }
    //Cambia el atributo "comprado" del item correspondiente en la lista y lo guarda en el archivo JSON
    public void AdquirirItemPersonalizable(GameObject prefab)
    {
        CargaDeFichero();

        Button botonComprar = prefab.transform.Find("btnComprar").GetComponent<Button>();
        Button botonAplicar = prefab.transform.Find("btnAplicar").GetComponent<Button>();

        GameObject fondoPrecio = prefab.transform.Find("FondoPrecio").gameObject;
        int precio = int.Parse(fondoPrecio.transform.Find("InputPrecio").GetComponent<TMP_Text>().text);
        GameObject fondoItem = prefab.transform.Find("FondoItem").gameObject;
        Image imagenArticulo = fondoItem.transform.Find("Item").GetComponent<Image>();

        TMP_Text aplicar = prefab.transform.Find("Aplicar").GetComponent<TMP_Text>();

        Personalizacion articuloPersonalizable = new Personalizacion();

        articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.imagen == imagenArticulo.sprite.name);
        //Se comprueba que existe para que no haya NullReferenceException
        if (articuloPersonalizable != null && !articuloPersonalizable.Equals(null))
        {
            articuloPersonalizable.comprado = true;
            botonComprar.interactable = false;
            botonComprar.gameObject.SetActive(false);
            fondoPrecio.gameObject.SetActive(false);
            botonAplicar.interactable = true;
            botonAplicar.gameObject.SetActive(true);
            aplicar.gameObject.SetActive(true);
            aplicar.text = "Aplicar";
            aplicar.color = Color.white;

            GameObject managerCanvas = gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.Find("MANAGER CANVAS").gameObject;

            subir = managerCanvas.GetComponent<SubirNivelyMonedas>();

            subir.subirMonedas(-precio);

            gestionTienda = managerCanvas.GetComponent<GestionTienda>();
            //gestionTienda.comprobarDisponibilidadPersonalizacion(prefab.transform.parent.gameObject);

            EscribirEnFichero();
        }
    }

    //Actualiza los datos en base a la información contenida en el fichero JSON
    public void ExtraerDatosFicheroPersonalizacion(GameObject contenedor)
    {
        CrearFichero();
        CargaDeFichero();

        //Debug.Log("Items en contenedor " + contenedor.transform.childCount);
        //Debug.Log("Items en el fichero " + listaPersonalizacion.listaPersonalizacion.Count());

        foreach (Transform child in contenedor.transform)
        {
            if (child.name == "Sombreros" || child.name == "Pajaritas" || child.name == "Calcetines") continue;

            Button botonComprar = child.transform.Find("btnComprar").GetComponent<Button>();
            Button botonAplicar = child.transform.Find("btnAplicar").GetComponent<Button>();
            Button botonDesaplicar = child.transform.Find("btnDesaplicar").GetComponent<Button>();
            TMP_Text aplicar = child.transform.Find("Aplicar").GetComponent<TMP_Text>();
            TMP_Text comprado = child.transform.Find("COMPRADO").GetComponent<TMP_Text>();
            TMP_Text aplicado = child.transform.Find("APLICADO").GetComponent<TMP_Text>();
            int tipo = int.Parse(child.transform.Find("TIPO").GetComponent<TMP_Text>().text);

            GameObject fondoNombre = child.transform.Find("FondoNombre").gameObject;
            string inputNombre = fondoNombre.transform.Find("InputNombre").GetComponent<TMP_Text>().text;

            GameObject fondoPropiedad = child.transform.Find("FondoPropiedad").gameObject;
            string inputPropiedad = fondoPropiedad.transform.Find("InputPropiedad").GetComponent<TMP_Text>().text;

            GameObject fondoPrecio = child.transform.Find("FondoPrecio").gameObject;
            int inputPrecio = int.Parse(fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().text);

            GameObject fondoItem = child.transform.Find("FondoItem").gameObject;
            Image imagenArticulo = fondoItem.transform.Find("Item").GetComponent<Image>();

            Personalizacion articuloPersonalizable = new Personalizacion();

            //Si el item está comprado se habilitan los botones y textos correspondientes
            articuloPersonalizable = listaPersonalizacion.listaPersonalizacion.Find(x => x.imagen == imagenArticulo.sprite.name);

            //Se comprueba que existe para que no haya NullReferenceException
            if (articuloPersonalizable != null && !articuloPersonalizable.Equals(null))
            {
                if (articuloPersonalizable.comprado)
                {
                    comprado.text = 1.ToString();
                    botonComprar.gameObject.SetActive(false);
                    botonAplicar.gameObject.SetActive(true);
                    botonAplicar.interactable = true;
                    fondoPrecio.gameObject.SetActive(false);
                    aplicar.gameObject.SetActive(true);

                    //Si el item en concreto está aplicado actualmente
                    if (articuloPersonalizable.aplicado)
                    {
                        aplicado.text = 1.ToString();
                        botonComprar.gameObject.SetActive(false);                        
                        botonAplicar.interactable = false;
                        fondoPrecio.gameObject.SetActive(false);                        
                        aplicar.color = Color.white;
                        aplicar.text = "Aplicado";

                        if (contenedor.name == "ContenedorComplementos")
                        {
                            botonAplicar.gameObject.SetActive(false);
                            botonDesaplicar.gameObject.SetActive(true);
                            aplicar.text = "Quitar";
                        }
                    }

                    else
                    {
                        aplicado.text = 0.ToString();
                        botonComprar.gameObject.SetActive(false);
                        botonAplicar.gameObject.SetActive(true);
                        botonAplicar.interactable = true;
                        fondoPrecio.gameObject.SetActive(false);
                        aplicar.text = "Aplicar";
                        aplicar.color = Color.white;
                    }
                }
            }

        }

    }

    public void CargaDeFichero()
    {
        jsonString = File.ReadAllText(filePath);
        listaPersonalizacion = JsonUtility.FromJson<ListaPersonalizacion>(jsonString);
    }

    public void EscribirEnFichero()
    {
        listaPersonalizacion.listaPersonalizacion.Sort((a, b) => a.comprado.CompareTo(b.comprado));
        jsonToSave = JsonUtility.ToJson(listaPersonalizacion);
        File.WriteAllText(filePath, jsonToSave);
    }

    public void CrearFichero()
    {
        folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/json/";
        filePath = folderPath + "Personalizacion.json";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            //Crea el fichero con 1 item, el fondo predeterminado
            File.WriteAllText(filePath, "{\"listaPersonalizacion\": [{\"tipo\":1,\"precio\":100,\"nombre\":\"Pared\",\"descripcion\":\"Azul\",\"imagen\":\"Predeterminada\",\"comprado\":true,\"aplicado\":true}]}");
            LlenarFicheroItems(contenedorPersonalizacion);
            LlenarFicheroItems(contenedorComplementos);

        }
    }

    //Mete todos los items personalizables posibles en un fichero JSON para operar con ellos
    public void LlenarFicheroItems(GameObject contenedor)
    {
        foreach (Transform child in contenedor.transform)
        {
            //Controlamos que en el contenedorComplementos no se comprueben las cabeceras con texto
            //Con "continue" pasamos a la siguiente iteracion

            if (child.name == "Sombreros" || child.name == "Pajaritas" || child.name == "Calcetines") continue;

            CargaDeFichero();
            Button botonComprar = child.transform.Find("btnComprar").GetComponent<Button>();
            Button botonAplicar = child.transform.Find("btnAplicar").GetComponent<Button>();
            int comprado = int.Parse(child.transform.Find("COMPRADO").GetComponent<TMP_Text>().text);
            int aplicado = int.Parse(child.transform.Find("APLICADO").GetComponent<TMP_Text>().text);
            int tipo = int.Parse(child.transform.Find("TIPO").GetComponent<TMP_Text>().text);

            GameObject fondoNombre = child.transform.Find("FondoNombre").gameObject;
            string inputNombre = fondoNombre.transform.Find("InputNombre").GetComponent<TMP_Text>().text;

            GameObject fondoPropiedad = child.transform.Find("FondoPropiedad").gameObject;
            string inputPropiedad = fondoPropiedad.transform.Find("InputPropiedad").GetComponent<TMP_Text>().text;

            GameObject fondoPrecio = child.transform.Find("FondoPrecio").gameObject;
            int inputPrecio = int.Parse(fondoPrecio.transform.Find("InputPrecio").gameObject.GetComponent<TMP_Text>().text);

            GameObject fondoItem = child.transform.Find("FondoItem").gameObject;
            Image imagenArticulo = fondoItem.transform.Find("Item").GetComponent<Image>();

            Personalizacion articuloPersonalizable = new Personalizacion();

            //Marca si el fichero está recien creado, es decir, si la aplicación no se ha iniciado anteriormente
            //Se añaden todos los objetos personalizados al fichero JSON

            switch (tipo)
            {
                case 1:
                    articuloPersonalizable.tipo = 1;
                    break;
                case 2:
                    articuloPersonalizable.tipo = 2;
                    break;
                case 3:
                    articuloPersonalizable.tipo = 3;
                    break;
                case 4:
                    articuloPersonalizable.tipo = 4;
                    break;
            }
            articuloPersonalizable.nombre = inputNombre;
            articuloPersonalizable.descripcion = inputPropiedad;
            articuloPersonalizable.precio = inputPrecio;
            articuloPersonalizable.imagen = imagenArticulo.sprite.name;

            if (comprado == 0)
            {
                articuloPersonalizable.comprado = false; 
}
            else {
                articuloPersonalizable.comprado = true;
                if (aplicado == 0)
                {
                    articuloPersonalizable.aplicado = false;
                }
                else articuloPersonalizable.aplicado = true;
            }

            listaPersonalizacion.listaPersonalizacion.Add(articuloPersonalizable);

            listaPersonalizacion.listaPersonalizacion.Remove(listaPersonalizacion.listaPersonalizacion
                 .Find(x => x.nombre == "Predeterminado" && x.aplicado == false));

            EscribirEnFichero();
        }

    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetInt("ParedAplicada", paredCopia);
            PlayerPrefs.Save();
        }
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("ParedAplicada", paredCopia);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class Personalizacion : Articulo
{
    public int tipo;
    public string descripcion;
    public string imagen;
    public bool comprado;
    public bool aplicado;

    public override string ToString()
    {
        return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", tipo, nombre, precio, descripcion, imagen, comprado, aplicado);
    }
}

[System.Serializable]
public class ListaPersonalizacion
{
    public List<Personalizacion> listaPersonalizacion;

    public void Listar()
    {
        foreach (Personalizacion personalizacion in listaPersonalizacion)
        {
            Debug.Log(personalizacion);
        }
    }
}
