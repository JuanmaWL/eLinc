using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubirNivelyMonedas : MonoBehaviour
{
    public Image barraNivel;
    public Text nivel, nivelTienda, monedas, monedasTienda;
    int nivelAux, monedasAux;
    public GameObject estrella;    

    void Awake()
    {
        //PlayerPrefs.DeleteAll();

        //Si no hay datos en referencia al nivel sabemos al 100% que el juego no se ha iniciado nunca.

        string nivelAntiguo = PlayerPrefs.GetString("Nivel");  
        if (nivelAntiguo == null)
        {
            print("(SubirNivelyMonedas) Primera vez de inicio del juego");
            barraNivel.fillAmount = 0;
            nivelAux = 1;
            nivel.text = nivelAux.ToString();
            nivelTienda.text = nivelAux.ToString();
            monedas.text = 0.ToString();
            monedasTienda.text = 0.ToString();

            PlayerPrefs.SetInt("Nivel", nivelAux);
            PlayerPrefs.SetFloat("Experiencia", barraNivel.fillAmount);
            PlayerPrefs.SetInt("Monedas", int.Parse(monedas.text));
        }

        //Cargamos el nivel, la experiencia y las monedas guardadas en PlayerPrefs y lo reflejamos en los elementos del UI.

        else
        {
            print("(SubirNivelyMonedas) El juego se ha iniciado más de una vez");            
            nivelAux = PlayerPrefs.GetInt("Nivel");
            Debug.Log("Nivel al abrir " + nivelAux);
            nivel.text = nivelAux.ToString();
            nivelTienda.text = nivelAux.ToString();
            barraNivel.fillAmount = PlayerPrefs.GetFloat("Experiencia");
            monedas.text = PlayerPrefs.GetInt("Monedas").ToString();
            monedasTienda.text = PlayerPrefs.GetInt("Monedas").ToString();
        }
    }


    //void Update()
    //{
    //    barraNivel.fillAmount += 5.0f / waitTime * Time.deltaTime;
    //    if (barraNivel.fillAmount == 1)
    //    {
    //        nivelAux++;
    //        nivel.text = nivelAux.ToString();
    //        barraNivel.fillAmount = 0;
    //    }
    //}

    public void subirExperiencia(float cantidadExperiencia)
    {
        if (barraNivel.fillAmount + cantidadExperiencia >= 1)
        {
            float expAux = barraNivel.fillAmount;

            estrella.GetComponent<Animator>().Play("SubirNivel", -1, 0f);
            estrella.GetComponent<AudioSource>().Play();
            nivelAux++;
            nivel.text = nivelAux.ToString();
            nivelTienda.text = nivelAux.ToString();
            barraNivel.fillAmount = 0;
            barraNivel.fillAmount += (expAux + cantidadExperiencia - 1);
        }
        else
        {
            barraNivel.fillAmount += cantidadExperiencia;
        }

        PlayerPrefs.SetInt("Nivel", nivelAux);
        PlayerPrefs.Save();


    }

    public void subirMonedas(int cantidadMonedas)
    {
        monedasAux = int.Parse(monedas.text);
        monedasAux += cantidadMonedas;
        monedas.text = monedasAux.ToString();
        monedasTienda.text = monedasAux.ToString();

        PlayerPrefs.SetInt("Monedas", monedasAux);

    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetInt("Nivel", nivelAux);
            PlayerPrefs.SetFloat("Experiencia", barraNivel.fillAmount);
            PlayerPrefs.SetInt("Monedas", int.Parse(monedas.text));
            PlayerPrefs.Save();
        }
    }
    public void OnApplicationQuit()
    {
        Debug.Log(gameObject);
        Debug.Log("Nivel al cerrar " + nivelAux);
        PlayerPrefs.SetInt("Nivel", nivelAux);
        PlayerPrefs.SetFloat("Experiencia", barraNivel.fillAmount);
        Debug.Log("Monedas al cerrar " + monedas.text);
        PlayerPrefs.SetInt("Monedas", int.Parse(monedas.text));
        PlayerPrefs.Save();

    }
}
