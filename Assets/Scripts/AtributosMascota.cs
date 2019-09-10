using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtributosMascota : MonoBehaviour
{
    float FPS = 30.0f;

    public GameObject LINCE, CUERPO, ducha;

    AnimacionVentanas animacion;

    //Con esta variable sabremos si sumar o restar al atributo Sueño, en función de si el lince está durmiendo o despierto

    public CambiarDeEscena cambiarDeEscena;
    public static int estaDurmiendo;

    public float hambre, sueno, limpieza, diversion;
    public float hambreTiempo, suenoTiempo, limpiezaTiempo, diversionTiempo;

    public Slider barraHambre, barraSueno, barraLimpieza, barraDiversion; //Sliders de los atributos
    public Text valorHambre, valorSueno, valorLimpieza, valorDiversion;   //% de llenado de Sliders
    public Image colorHambre, colorSueno, colorLimpieza, colorDiversion; //Color de relleno de las barras

    public Sprite[] spritesPersonalizacion; //Sprites para las paredes
    public Sprite[] spriteDormir; //Sprites para cambiar el icono según si está dormido o no

    public GameObject panelAtributos, contenedorAcciones, pared, suelo, fondoDormir, botonDormir;

    public AudioSource on, off;

    public Color valorMaximo, valorMinimo;

    DateTime currentDate, oldDate;
    TimeSpan segundosDiferencia;

    private void Start()
    {
        //Situar el slide de las acciones en la izquierda al cargar el script
        contenedorAcciones.transform.parent.transform.Find("Scrollbar Horizontal").GetComponent<Scrollbar>().value = 0;

        barraHambre.maxValue = 100;
        barraSueno.maxValue = 100;
        barraLimpieza.maxValue = 100;
        barraDiversion.maxValue = 100;

        //PlayerPrefs.DeleteAll();

        //Guarda la hora al iniciar
        currentDate = System.DateTime.Now;
        print("Hora de apertura: " + System.DateTime.Now);

        //Coge la hora de cierre del PlayerPrefs
        string oldDateAUX = PlayerPrefs.GetString("sysString");
        if (oldDateAUX == "")
        {
            hambre = 100; sueno = 100; limpieza = 100; diversion = 100;
            estaDurmiendo = 0;
            print("(AtributosMascota) Primera vez que se inicia del juego");
        }

        else
        {
            print("(AtributosMascota) El juego ya se ha iniciado más de una vez");

            calcularValoresAtributos();

            int indexPared = PlayerPrefs.GetInt("ParedAplicada");
            int indexSombrero = PlayerPrefs.GetInt("SombreroAplicado");
            int indexPajarita = PlayerPrefs.GetInt("PajaritaAplicada");
            int indexCalcetines = PlayerPrefs.GetInt("CalcetinesAplicados");
            //Si es 0 significa que la variable está vacía, lo cual quiere decir que nunca antes se ha comprado ningún item, por lo
            //tanto se pone el fondo pretederminado de la posición 0
            if (indexPared == 0) pared.GetComponent<SpriteRenderer>().sprite = spritesPersonalizacion[0];
            else pared.GetComponent<SpriteRenderer>().sprite = spritesPersonalizacion[indexPared - 1];

            if (indexSombrero != 0) LINCE.transform.GetChild(indexSombrero).gameObject.SetActive(true);
            if (indexPajarita != 0) LINCE.transform.GetChild(indexPajarita).gameObject.SetActive(true);
            if (indexCalcetines != 0) LINCE.transform.GetChild(indexCalcetines).gameObject.SetActive(true);

            barraHambre.value = hambre;
            barraSueno.value = sueno;
            barraLimpieza.value = limpieza;
            barraDiversion.value = diversion;
        }

        ActualizarUI();

    }

    //Calcula los valores de los atributos cuando la aplicación no está en ejecución (pausa, minimizada, pantalla bloqueada, cerrada, etc)
    private void calcularValoresAtributos()
    {
        long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));

        //Convierte la hora de cierre de binario a DataTime
        DateTime oldDate = DateTime.FromBinary(temp);

        //Metodo Subtract para operar con 2 horas distintas
        segundosDiferencia = currentDate.Subtract(oldDate);

        //Calcula lo que deben bajar las stats en el tiempo que ha estado el juego cerrado

        //Baja X% en 30 segundos, según el valor del float de cada atributo (hambreTiempo, suenoTiempo...)

        hambre = PlayerPrefs.GetFloat("Hambre") - (float)segundosDiferencia.TotalSeconds / FPS * hambreTiempo;
        limpieza = PlayerPrefs.GetFloat("Limpieza") - (float)segundosDiferencia.TotalSeconds / FPS * limpiezaTiempo;
        diversion = PlayerPrefs.GetFloat("Diversion") - (float)segundosDiferencia.TotalSeconds / FPS * diversionTiempo;

        estaDurmiendo = PlayerPrefs.GetInt("EstaDurmiendo");

        Animator anima = LINCE.GetComponent<Animator>();

        if (estaDurmiendo == 0)
        {
            botonDormir.GetComponent<Image>().sprite = spriteDormir[0];
            Debug.Log("No está durmiendo");
            sueno = PlayerPrefs.GetFloat("Sueno") - (float)segundosDiferencia.TotalSeconds / FPS * suenoTiempo;
            anima.SetInteger("estaDurmiendo", -1);
            deshabilitarOtrosBotones(true, true, true, true, true);
        }
        else
        {
            botonDormir.GetComponent<Image>().sprite = spriteDormir[1];
            Animator anim = fondoDormir.GetComponent<Animator>();
            anim.SetBool("abrir", true);
            Debug.Log("Está durmiendo");
            sueno = PlayerPrefs.GetFloat("Sueno") + (float)segundosDiferencia.TotalSeconds / FPS * suenoTiempo;
            anima.Play("DormirEntero");
            deshabilitarOtrosBotones(true, false, false, false, true);

        }
    }

    private void Update()
    {
        hambre -= hambreTiempo / FPS * Time.deltaTime;
        subirSueno(estaDurmiendo);
        limpieza -= limpiezaTiempo / FPS * Time.deltaTime;
        diversion -= diversionTiempo / FPS * Time.deltaTime;

        if (LINCE.GetComponent<Animator>().GetInteger("estaDuchandose") == 1) limpieza += 0.25f;

        ActualizarUI();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject panelSalir = gameObject.transform.parent.transform.Find("PanelSalir").gameObject;
            Animator anima = panelSalir.GetComponent<Animator>();
            panelSalir.SetActive(true);
            anima.SetBool("abrir", true);
        }
        
        controlAnimaciones();
    }

    public void controlAnimaciones()
    {
        Animator anima = LINCE.GetComponent<Animator>();
        anima.SetFloat("Diversion", diversion);

        if (estaDurmiendo == 1)
        {
            LINCE.transform.Find("CejaIzquierda").gameObject.SetActive(false);
            LINCE.transform.Find("CejaDerecha").gameObject.SetActive(false);
            LINCE.transform.Find("CejaIzquierda_1").gameObject.SetActive(false);
            LINCE.transform.Find("CejaDerecha_1").gameObject.SetActive(false);
        }
        else
        {

            if (sueno < 50 || hambre < 50 || limpieza < 50)
            {
                LINCE.transform.Find("CejaIzquierda").gameObject.SetActive(true);
                LINCE.transform.Find("CejaDerecha").gameObject.SetActive(true);

                if (sueno < 25 || hambre < 25 || limpieza < 25)
                {
                    LINCE.transform.Find("CejaIzquierda").gameObject.SetActive(false);
                    LINCE.transform.Find("CejaDerecha").gameObject.SetActive(false);
                    LINCE.transform.Find("CejaIzquierda_1").gameObject.SetActive(true);
                    LINCE.transform.Find("CejaDerecha_1").gameObject.SetActive(true);
                }

                else
                {
                    LINCE.transform.Find("CejaIzquierda_1").gameObject.SetActive(false);
                    LINCE.transform.Find("CejaDerecha_1").gameObject.SetActive(false);
                }

            }
            else
            {
                LINCE.transform.Find("CejaIzquierda").gameObject.SetActive(false);
                LINCE.transform.Find("CejaDerecha").gameObject.SetActive(false);
            }
        }
    }
    public void salirAplicacion()
    {
        cambiarDeEscena.Portada();
    }
    IEnumerator QuitarDucha()
    {
        yield return new WaitForSeconds(5.5f);
        LINCE.GetComponent<Animator>().SetInteger("estaDuchandose", 0);
        deshabilitarOtrosBotones(true, true, true, true, true);
    }


    public void ducharse()
    {
        Animator anima = LINCE.GetComponent<Animator>();
        anima.SetInteger("estaDuchandose", 1);
        deshabilitarOtrosBotones(true, false, false, false, false);
        StartCoroutine(QuitarDucha());
    }

    //Funcion para subir o bajar el atributo sueño
    private void subirSueno(int i)
    {
        if (i == 0) sueno -= suenoTiempo / FPS * Time.deltaTime;
        else sueno += suenoTiempo / FPS * Time.deltaTime;
    }

    //Cambia el valor de la variable estaDurmiendo 
    public void dormir()
    {
        Animator anim = LINCE.GetComponent<Animator>();

        if (estaDurmiendo == 0)
        {
            botonDormir.GetComponent<Image>().sprite = spriteDormir[1];
            estaDurmiendo = 1;
            Debug.Log("Se duerme");
            anim.SetInteger("estaDurmiendo", 1);
            deshabilitarOtrosBotones(true, false, false, false, true);

        }
        else
        {
            botonDormir.GetComponent<Image>().sprite = spriteDormir[0];
            estaDurmiendo = 0;
            Debug.Log("Se despierta");
            anim.SetInteger("estaDurmiendo", -1);
            deshabilitarOtrosBotones(true, true, true, true, true);

        }
    }

    //Se le llama al darle al botón 
    public void OcultarTrasComer()
    {
        if (contenedorAcciones.transform.Find("Comer").GetComponent<Image>().sprite.name == "manzanaverde")
        {
            deshabilitarOtrosBotones(true, true, false, false, false);
        }
        else
        {
            Animator anim = LINCE.GetComponent<Animator>();
            anim.SetInteger("estaComiendo", 0);
            deshabilitarOtrosBotones(true, true, true, true, true);
        }
    }

    public void deshabilitarOtrosBotones(Boolean tienda, Boolean comer, Boolean bañar, Boolean jugar, Boolean dormir)
    {
        contenedorAcciones.transform.Find("Tienda").GetComponent<Button>().interactable = tienda;
        contenedorAcciones.transform.Find("Comer").GetComponent<Button>().interactable = comer;
        contenedorAcciones.transform.Find("Bañar").GetComponent<Button>().interactable = bañar;
        contenedorAcciones.transform.Find("Jugar").GetComponent<Button>().interactable = jugar;
        contenedorAcciones.transform.Find("Dormir").GetComponent<Button>().interactable = dormir;
    }

    private void ActualizarUI()
    {
        hambre = Mathf.Clamp(hambre, 0, 100f);
        sueno = Mathf.Clamp(sueno, 0, 100f);
        limpieza = Mathf.Clamp(limpieza, 0, 100f);
        diversion = Mathf.Clamp(diversion, 0, 100f);

        barraHambre.value = hambre;
        barraSueno.value = sueno;
        barraLimpieza.value = limpieza;
        barraDiversion.value = diversion;

        valorHambre.text = Mathf.Round(hambre).ToString() + "%";
        valorSueno.text = Mathf.Round(sueno).ToString() + "%";
        valorLimpieza.text = Mathf.Round(limpieza).ToString() + "%";
        valorDiversion.text = Mathf.Round(diversion).ToString() + "%";

        colorHambre.color = Color.Lerp(valorMinimo, valorMaximo, (float)hambre / 100);
        colorSueno.color = Color.Lerp(valorMinimo, valorMaximo, (float)sueno / 100);
        colorLimpieza.color = Color.Lerp(valorMinimo, valorMaximo, (float)limpieza / 100);
        colorDiversion.color = Color.Lerp(valorMinimo, valorMaximo, (float)diversion / 100);
    }

    public void abrirPanelAtributos()
    {

        if (!panelAtributos.activeSelf)
        {
            panelAtributos.SetActive(true);
            on.Play();
        }
        else
        {
            panelAtributos.SetActive(false);
            off.Play();
        }
    }
       
    public void subirHambre(float cantidad)
    {
        hambre += cantidad;
        ActualizarUI();
    }
    public void subirLimpieza(float cantidad)
    {
        limpieza += cantidad;
        ActualizarUI();
    }
    public void subirDiversion(float cantidad)
    {
        diversion += cantidad;
        ActualizarUI();
    }
    public void subirEnergia(float cantidad)
    {
        sueno += cantidad;
        ActualizarUI();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetFloat("Hambre", hambre);
            PlayerPrefs.SetFloat("Sueno", sueno);
            PlayerPrefs.SetInt("EstaDurmiendo", estaDurmiendo);
            PlayerPrefs.SetFloat("Limpieza", limpieza);
            PlayerPrefs.SetFloat("Diversion", diversion);
            PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
            PlayerPrefs.Save();

            salirAplicacion();
        }
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Hambre", hambre);
        PlayerPrefs.SetFloat("Sueno", sueno);
        PlayerPrefs.SetInt("EstaDurmiendo", estaDurmiendo);
        PlayerPrefs.SetFloat("Limpieza", limpieza);
        PlayerPrefs.SetFloat("Diversion", diversion);
        PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();

    }

}
