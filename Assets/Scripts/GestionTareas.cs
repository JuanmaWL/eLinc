using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class GestionTareas : MonoBehaviour
{
    public GameObject prefabTarea, contenedorTareas;
    public Sprite tareaCompleta, tareaNoCompleta;
    public Color colorFondoTarea, colorFondoInputError;
    GameObject[] arrayPrefabTarea;
    ListaTareas listaTareas = null;
    SubirNivelyMonedas subir;
    static string filePath, jsonString, jsonToSave, folderPath;

    //COMENTAR CTRL K, C. CTRL K, U


    //Dependiendo del sitio donde estemos (Panel Administración/Juego), se cargarán las Tareas de forma distinta
    public void ExtraerTareas(bool administracion)
    {
        //Gestión del fichero

        CrearFichero();
        CargaDeFichero();
        VaciarListaTareas();

        if (listaTareas.tareas.Count() == 0)
        {
            contenedorTareas.transform.parent.
                  gameObject.transform.parent.
                  gameObject.transform.parent.Find("PanelMensaje").gameObject.SetActive(true);
        }


        
        //Se crea un array con el número de tareas que hay en la lista que previamente se ha leido de fichero
        arrayPrefabTarea = new GameObject[listaTareas.tareas.Count()];

        for (int i = 0; i < arrayPrefabTarea.Length; i++)
        {
            //Se mete el prefab de Tarea dentro del Contenedor correspondiente
            arrayPrefabTarea[i] = GameObject.Instantiate(prefabTarea, Vector3.forward, Quaternion.identity, contenedorTareas.transform);
            arrayPrefabTarea[i].GetComponent<RectTransform>().localPosition = Vector3.forward;

            GameObject idTarea = arrayPrefabTarea[i].transform.Find("IDTarea").gameObject;
            GameObject inputTarea = arrayPrefabTarea[i].transform.Find("InputTarea").gameObject;
            GameObject inputRecompensa = arrayPrefabTarea[i].transform.Find("InputRecompensa").gameObject;


            //Si estamos en el panel de Administración
            if (administracion)
            {
                GameObject inputRecompensaOpcional = arrayPrefabTarea[i].transform.Find("InputRecompensaOpcional").gameObject;

                TMP_InputField textoTarea = inputTarea.GetComponent<TMP_InputField>();
                textoTarea.text = listaTareas.tareas[i].nombre;

                TMP_InputField textoRecompensa = inputRecompensa.GetComponent<TMP_InputField>();
                textoRecompensa.text = listaTareas.tareas[i].cantidad.ToString();

                TMP_InputField textoRecompensaOpcional = inputRecompensaOpcional.GetComponent<TMP_InputField>();
                textoRecompensaOpcional.text = listaTareas.tareas[i].opcional;

                Image fondoReclamado = arrayPrefabTarea[i].transform.Find("FondoReclamado").GetComponent<Image>();
                TMP_Text textoReclamado = fondoReclamado.transform.Find("TextoReclamado").GetComponent<TMP_Text>();

                TMP_Text idTareaTexto = idTarea.transform.GetComponent<TMP_Text>();
                idTareaTexto.text = "#" + listaTareas.tareas[i].id.ToString();

                if (listaTareas.tareas[i].reclamada)
                {
                    Button botonEditar = arrayPrefabTarea[i].transform.Find("btnEditar").GetComponent<Button>();
                    botonEditar.interactable = false;
                    fondoReclamado.color = Color.green;
                    textoReclamado.text = "Reclamado";

                }
                else
                {
                    fondoReclamado.color = Color.yellow;
                    textoReclamado.text = "Sin Reclamar";
                }

                GameObject completado = arrayPrefabTarea[i].transform.Find("btnCompletado").gameObject;
                TMP_Text textoCompletado = completado.transform.Find("TextoCompletado").gameObject.GetComponent<TMP_Text>();

                //Si la tarea concreta está completada se pone el sprite y el texto correspondiente
                if (listaTareas.tareas[i].completada)
                {
                    completado.GetComponent<Image>().sprite = tareaCompleta;
                    textoCompletado.text = "Completado";
                    fondoReclamado.gameObject.SetActive(true);

                }
                else
                {
                    completado.GetComponent<Image>().sprite = tareaNoCompleta;
                    textoCompletado.text = "Pendiente";
                    fondoReclamado.gameObject.SetActive(false);
                }
            }

            //Si estamos en el juego
            else if (administracion == false)
            {
                TMP_Text idTareaTexto = idTarea.transform.GetComponent<TMP_Text>();
                idTareaTexto.text = "#" + listaTareas.tareas[i].id.ToString();

                TMP_Text textoTarea = inputTarea.GetComponent<TMP_Text>();
                textoTarea.text = "- " + listaTareas.tareas[i].nombre + " -";

                TMP_Text textoRecompensa = inputRecompensa.GetComponent<TMP_Text>();
                textoRecompensa.text = listaTareas.tareas[i].cantidad.ToString();

                GameObject fondoRecompensaAdicional = arrayPrefabTarea[i].transform.Find("FondoRecompensaOpcional").gameObject;
                GameObject inputRecompensaOpcional = arrayPrefabTarea[i].transform.Find("InputRecompensaOpcional").gameObject;
                TMP_Text textoRecompensaOpcional = inputRecompensaOpcional.GetComponent<TMP_Text>();

                Button botonReclamar = arrayPrefabTarea[i].transform.Find("btnReclamar").GetComponent<Button>();
                TMP_Text textoReclamar = botonReclamar.transform.Find("TextoReclamar").GetComponent<TMP_Text>();

                //Si no está reclamada, si se mostrará.                
                if (listaTareas.tareas[i].reclamada)
                {
                    Destroy(contenedorTareas.transform.GetChild(i).gameObject);
                    Destroy(arrayPrefabTarea[i].gameObject);

                }
                else
                {
                    //Si la tarea ya está completada (cambiado por el padre en menu admin)
                    if (listaTareas.tareas[i].completada)
                    {
                        botonReclamar.interactable = true;
                        textoReclamar.color = Color.white;
                    }

                    //Si no hay ninguna tarea opcional.
                    if (listaTareas.tareas[i].opcional != null)
                    {
                        fondoRecompensaAdicional.SetActive(true);
                        textoRecompensaOpcional.text = listaTareas.tareas[i].opcional;

                    }
                }

            }
        }

        //Cambia el pivote del contenedor de tareas para que se vea arriba del todo, estando la #1 la primera.
        contenedorTareas.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
    }

    public void comprobarContenedorVacioJuego(GameObject contenedorTareas)
    {
        if (listaTareas.tareas.All(x => x.completada && x.reclamada == true))
        {
            contenedorTareas.transform.parent.gameObject.
                             transform.parent.gameObject.
                             transform.parent.gameObject.
                             transform.Find("PanelMensaje").gameObject.SetActive(true);
        }

        else
        {
            contenedorTareas.transform.parent.gameObject.
                             transform.parent.gameObject.
                             transform.parent.gameObject.
                             transform.Find("PanelMensaje").gameObject.SetActive(false);
        }
    }

    //Se añade una nueva prefab al contenedor se marcan los campos como "interactables"
    public void AnadirNuevaTarea()
    {
        GameObject nuevaTarea;
        nuevaTarea = GameObject.Instantiate(prefabTarea, Vector3.forward, Quaternion.identity, contenedorTareas.transform);

        nuevaTarea.GetComponent<RectTransform>().localPosition = Vector3.forward;
        nuevaTarea.GetComponent<Image>().color = Color.cyan;
        nuevaTarea.transform.Find("IDTarea").GetComponent<TMP_Text>().text = "";
        nuevaTarea.transform.Find("InputTarea").GetComponent<TMP_InputField>().interactable = true;
        nuevaTarea.transform.Find("InputRecompensa").GetComponent<TMP_InputField>().interactable = true;
        nuevaTarea.transform.Find("InputRecompensaOpcional").GetComponent<TMP_InputField>().interactable = true;
        nuevaTarea.transform.Find("btnCompletado").GetComponent<Button>().interactable = false;
        nuevaTarea.transform.Find("btnGuardar").gameObject.SetActive(true);
        nuevaTarea.transform.Find("btnEditar").GetComponent<Button>().interactable = false;

        Image fondoReclamado = nuevaTarea.transform.Find("FondoReclamado").GetComponent<Image>();
        fondoReclamado.gameObject.SetActive(false);
        TMP_Text textoReclamado = fondoReclamado.transform.Find("TextoReclamado").GetComponent<TMP_Text>();
        textoReclamado.gameObject.SetActive(false);

        nuevaTarea.GetComponent<FadeOutScript>().FadeIn();
        nuevaTarea.GetComponent<Animator>().Play("AnadirTarea");

        if (contenedorTareas.transform.childCount < 4) contenedorTareas.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        else contenedorTareas.GetComponent<RectTransform>().pivot = new Vector2(0, -1);

        //Quita el mensaje de "No hay tareas"
        if (contenedorTareas.transform.childCount >= 1)
        {
            contenedorTareas.transform.parent.
                      gameObject.transform.parent.
                      gameObject.transform.parent.Find("PanelMensaje").gameObject.SetActive(false);
        }

    }

    public void EditarTarea(GameObject prefab)
    {
        //Se ponen editables los campos de Nombre, Monedas y el Botón de Completado.

        prefab.transform.Find("btnCompletado").GetComponent<Button>().interactable = true;
        prefab.transform.Find("InputTarea").GetComponent<TMP_InputField>().interactable = true;
        prefab.transform.Find("InputRecompensa").GetComponent<TMP_InputField>().interactable = true;
        prefab.transform.Find("InputRecompensaOpcional").GetComponent<TMP_InputField>().interactable = true;
        prefab.transform.Find("btnEditar").GetComponent<Button>().interactable = false;
        prefab.transform.Find("btnGuardar").gameObject.SetActive(true);

    }

    //Guardar una tarea, tanto si se crea una nueva como si se edita una existente
    public void GuardarTarea(GameObject prefab)
    {
        CargaDeFichero();

        //Si la tarea es Nueva (por el fondo verde)
        if (prefab.GetComponent<Image>().color == Color.cyan)
        {

            //Si no hay ninguna tarea en listaTareas
            if (listaTareas.tareas.Count() == 0)
            {
                Tarea tarea = new Tarea();

                if (cogerDatosYResetPrefab(tarea, prefab))
                {
                    prefab.transform.Find("IDTarea").GetComponent<TMP_Text>().text = "#1"; tarea.id = 1;
                    listaTareas.tareas.Add(tarea);
                    EscribirEnFichero();
                    CompletarDescompletar(prefab);
                }

            }

            //Si ya hay tareas en listaTareas
            else
            {
                Tarea ultimaTarea = new Tarea();
                ultimaTarea = listaTareas.tareas[listaTareas.tareas.Count() - 1];
                Tarea tarea = new Tarea();

                // Para no asignar un ID repetido, se recorre la lista tantas veces como ID tiene el último elemento (Ej: ID=12).
                // hasta que se encuentra el primer ID en la lista que no existe. 
                // Si existe el ID 1, 2, 3 y 5, se asignará el 4 a la nueva Tarea y parará el bucle.
                // Si existiesen 1, 2, 3, 4, 5... 12, la nueva tarea tendría el ID 13.
                for (int i = 1; i <= ultimaTarea.id + 1; i++)
                {
                    if (!listaTareas.tareas.Exists(x => x.id == i))
                    {
                        tarea.id = i;
                        break;
                    }
                }

                if (cogerDatosYResetPrefab(tarea, prefab))
                {
                    prefab.transform.Find("IDTarea").GetComponent<TMP_Text>().text = "#" + tarea.id.ToString();
                    listaTareas.tareas.Add(tarea);
                    EscribirEnFichero();
                    CompletarDescompletar(prefab);
                }
            }
        }

        //Si la tarea ya existe y se está editando
        else
        {
            Button boton = prefab.transform.Find("btnCompletado").GetComponent<Button>();
            Tarea tareaEditada = new Tarea();

            //Se comprueba el ID de la tarea que se está guardando.
            string idTarea = prefab.transform.Find("IDTarea").gameObject.GetComponent<TMP_Text>().text;
            idTarea = idTarea.Substring(1, idTarea.Length - 1);

            //Se localiza qué tarea tiene ese ID y se le modifica el campo "completada"
            tareaEditada = listaTareas.tareas.Find(x => x.id == int.Parse(idTarea));

            if (cogerDatosYResetPrefab(tareaEditada, prefab))
            {
                if (boton.GetComponent<Image>().sprite == tareaNoCompleta) tareaEditada.completada = false;
                else tareaEditada.completada = true;

                EscribirEnFichero();
            }
        }
    }

    public void EliminarTarea(GameObject prefab)
    {
        CargaDeFichero();

        contenedorTareas = prefab.transform.parent.gameObject;
        contenedorTareas.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);

        if (prefab.GetComponent<Image>().color == Color.cyan)
        {
            Destroy(prefab, 0.5f);
        }
        else
        {
            Tarea tareaEliminada = new Tarea();
            //Se comprueba qué ID tiene la tarea que estamos eliminando, para localizarla en la lista de objetos tarea en base a su atributo ID.
            string idTarea = prefab.transform.Find("IDTarea").gameObject.GetComponent<TMP_Text>().text;
            idTarea = idTarea.Substring(1, idTarea.Length - 1);
            tareaEliminada = listaTareas.tareas.Find(x => x.id == int.Parse(idTarea));

            listaTareas.tareas.Remove(tareaEliminada);
            Destroy(prefab, 0.5f);
            EscribirEnFichero();
        }
        //Muestra el mensaje de "No hay tareas" si no queda ninguna en el contenedor
        if (contenedorTareas.transform.childCount <= 1)
        {
            contenedorTareas.
            gameObject.transform.parent.
            gameObject.transform.parent.
            gameObject.transform.parent.
            Find("PanelMensaje").gameObject.SetActive(true);
        }
    }

    //Metodo para comprobar que no hay campos vitales vacios, para guardarlos cuando estén correctos y para resetear la interfaz
    public bool cogerDatosYResetPrefab(Tarea tarea, GameObject prefab)
    {
        bool sePuedeGuardar = false;

        TMP_InputField inputTarea = prefab.transform.Find("InputTarea").GetComponent<TMP_InputField>();
        TMP_InputField inputRecompensa = prefab.transform.Find("InputRecompensa").GetComponent<TMP_InputField>();
        TMP_InputField inputRecompensaOpcional = prefab.transform.Find("InputRecompensaOpcional").GetComponent<TMP_InputField>();
        Button botonCompletado = prefab.transform.Find("btnCompletado").GetComponent<Button>();
        Button botonEditar = prefab.transform.Find("btnEditar").GetComponent<Button>();
        Button botonGuardar = prefab.transform.Find("btnGuardar").GetComponent<Button>();

        if (inputTarea.text == "" || inputRecompensa.text == "")
        {
            if (inputTarea.text == "")
            {
                inputTarea.image.color = colorFondoInputError;
                if (!(inputRecompensa.text == "")) inputRecompensa.image.color = Color.white;
            }
            if (inputRecompensa.text == "")
            {
                inputRecompensa.image.color = colorFondoInputError;
                if (!(inputTarea.text == "")) inputTarea.image.color = Color.white;
            }
            sePuedeGuardar = false;
        }

        else
        {
            inputTarea.image.color = Color.white;
            inputRecompensa.image.color = Color.white;

            tarea.nombre = inputTarea.text;
            tarea.cantidad = int.Parse(inputRecompensa.text);
            tarea.opcional = inputRecompensaOpcional.text;
            tarea.completada = false;
            tarea.reclamada = false;
            prefab.GetComponent<Image>().color = colorFondoTarea;
            inputTarea.interactable = false;
            inputRecompensa.interactable = false;
            inputRecompensaOpcional.interactable = false;
            prefab.transform.Find("btnEditar").gameObject.SetActive(true);
            botonCompletado.interactable = false;
            botonEditar.interactable = true;
            botonGuardar.gameObject.SetActive(false);

            sePuedeGuardar = true;
        }

        return sePuedeGuardar;
    }

    public void VaciarListaTareas()
    {
        foreach (Transform child in contenedorTareas.transform) Destroy(child.gameObject);
    }

    //Método para cambiar las tareas de Completo a Pendiente y viceversa.    
    public void CompletarDescompletar(GameObject prefab)
    {
        CargaDeFichero();

        Button boton = prefab.transform.Find("btnCompletado").GetComponent<Button>();
        GameObject inputRecompensa = prefab.transform.Find("InputRecompensa").gameObject;
        TMP_Text textoCompletado = boton.transform.Find("TextoCompletado").gameObject.GetComponent<TMP_Text>();
        Image fondoReclamado = prefab.transform.Find("FondoReclamado").GetComponent<Image>();
        TMP_Text textoReclamado = fondoReclamado.transform.Find("TextoReclamado").gameObject.GetComponent<TMP_Text>();
        textoReclamado.gameObject.SetActive(true);

        if (textoCompletado.text.Equals("Pendiente"))
        {
            boton.GetComponent<Image>().sprite = tareaCompleta;
            textoCompletado.text = "Completado";
            fondoReclamado.gameObject.SetActive(true);
        }

        else
        {
            boton.GetComponent<Image>().sprite = tareaNoCompleta;
            textoCompletado.text = "Pendiente";
            fondoReclamado.gameObject.SetActive(false);
        }
    }

    public void ReclamarRecompensa(GameObject prefab)
    {
        CargaDeFichero();
        Tarea tareaReclamada = new Tarea();

        //Se comprueba qué ID tiene la tarea que estamos reclamando, para localizarla en la lista de objetos tarea en base a su atributo ID.
        string idTarea = prefab.transform.Find("IDTarea").gameObject.GetComponent<TMP_Text>().text;
        idTarea = idTarea.Substring(1, idTarea.Length - 1);

        tareaReclamada = listaTareas.tareas.Find(x => x.id == int.Parse(idTarea));

        int recompensa = tareaReclamada.cantidad;
        float experiencia = recompensa * 0.001f;

        tareaReclamada.reclamada = true;

        Button botonReclamar = prefab.transform.Find("btnReclamar").gameObject.GetComponent<Button>();
        botonReclamar.interactable = false;

        subir = gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.parent.
                gameObject.transform.Find("MANAGER CANVAS").GetComponent<SubirNivelyMonedas>();

        subir.subirExperiencia(experiencia);
        subir.subirMonedas(recompensa);

        prefab.GetComponent<Animator>().Play("EliminarTarea");
        Destroy(prefab, 0.5f);

        contenedorTareas = prefab.transform.parent.gameObject;
        if (contenedorTareas.transform.childCount <= 4) contenedorTareas.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);

        comprobarContenedorVacioJuego(contenedorTareas);

        EscribirEnFichero();
    }

    public void CargaDeFichero()
    {
        jsonString = File.ReadAllText(filePath);
        Debug.Log(jsonString);
        listaTareas = JsonUtility.FromJson<ListaTareas>(jsonString);
    }

    public void EscribirEnFichero()
    {
        listaTareas.tareas.Sort((a, b) => a.id.CompareTo(b.id));
        jsonToSave = JsonUtility.ToJson(listaTareas);
        File.WriteAllText(filePath, jsonToSave);
    }

    public void CrearFichero()
    {
        folderPath = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ? Application.persistentDataPath : Application.dataPath) + "/json/";
        filePath = folderPath + "Tareas.json";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            File.WriteAllText(filePath, "{\"tareas\": [{\"id\":1,\"nombre\":\"Tarea de Ejemplo\",\"cantidad\":10,\"opcional\":\"Helado\",\"completada\":true,\"reclamada\":false}]}");
        }

    }
}

[System.Serializable]
public class Tarea
{
    public int id;
    public string nombre;
    public int cantidad;
    public string opcional;
    public bool completada;
    public bool reclamada;

    public override string ToString()
    {
        return string.Format("ID: {0} | Tarea: {1}, {2} monedas (y) {3} | completada: {4}, reclamada: {5}", id, nombre, cantidad, opcional, completada, reclamada);
    }
}

[System.Serializable]
public class ListaTareas
{
    public List<Tarea> tareas;

    public void Listar()
    {
        foreach (Tarea tarea in tareas)
        {
            Debug.Log(tarea);
        }
    }
}