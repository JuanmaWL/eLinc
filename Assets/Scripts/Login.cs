using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public InputField inputPassword, inputNueva;
    public CambiarDeEscena cambiarDeEscena;
    public GameObject panelLogin;
    public Color error, exito;
    static string pass;
    bool menuPass;

    public void Start()
    {
        //PlayerPrefs.DeleteAll();

        menuPass = false;        
        pass = PlayerPrefs.GetString("Password");
        Debug.Log(pass);
        if (pass == "")
        {
            inputPassword.text = "";
            inputNueva.text = "1234";
            pass = inputPassword.text;
            cambiarPass();
        }
    }

    public void Update()
    {
        if (inputPassword.text != "" && inputNueva.text != "") panelLogin.transform.Find("btnCambiarPass").gameObject.GetComponent<Button>().interactable = true;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
    public void login()
    {
        pass = PlayerPrefs.GetString("Password");

        if (inputPassword.text == pass)
        {
            inputPassword.text = "";
            ColorBlock cb = inputPassword.colors;
            cb.normalColor = exito;
            inputPassword.colors = cb;
            cambiarDeEscena.PanelAdministracion();
        }
        else
        {
            inputPassword.text = "";

            ColorBlock cb = inputPassword.colors;
            cb.normalColor = error;
            inputPassword.colors = cb;
        }
    }

    public void menuCambiarPass()
    {
        inputPassword.text = "";
        inputPassword.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Antigua Contraseña";
        ColorBlock cb;
        cb = inputPassword.colors;
        cb.normalColor = Color.white;
        inputPassword.colors = cb;
        inputPassword.placeholder.color = Color.grey;
        inputNueva.placeholder.color = Color.grey;

        menuPass = true;
        panelLogin.transform.Find("btnEntrar").gameObject.SetActive(false);
        panelLogin.transform.Find("btnAtras").gameObject.SetActive(false);
        panelLogin.transform.Find("CambiarPass").gameObject.SetActive(false);
        panelLogin.transform.Find("btnCambiarPass").gameObject.SetActive(true);
        panelLogin.transform.Find("btnCambiarPass").gameObject.GetComponent<Button>().interactable = false;
        panelLogin.transform.Find("btnCancelar").gameObject.SetActive(true);
        panelLogin.transform.Find("MensajeCambioContraseña").gameObject.SetActive(false);
        panelLogin.transform.Find("PassInputFieldNueva").gameObject.SetActive(true);

        panelLogin.transform.Find("AccesoAPadres").gameObject.GetComponent<Text>().text = "- Cambio de Contraseña -";

    }

    public void cambiarPass()
    {
        ColorBlock cb;

        if (inputPassword.text != PlayerPrefs.GetString("Password"))
        {
            cb = inputPassword.colors;
            cb.normalColor = error;
            inputPassword.colors = cb;

            if (inputNueva.text == "")
            {
                cb = inputNueva.colors;
                cb.normalColor = error;
                inputNueva.colors = cb;
            }
        }

        else
        {


            if (inputNueva.text == "")
            {
                cb = inputNueva.colors;
                cb.normalColor = error;
                inputNueva.colors = cb;
            }

            else
            {
                string password = inputNueva.text;
                panelLogin.transform.Find("MensajeCambioContraseña").gameObject.SetActive(true);
                panelLogin.transform.Find("MensajeCambioContraseña").gameObject.GetComponent<Animator>().Play("ContraseñaCambiada");
                PlayerPrefs.SetString("Password", password);
                pass = PlayerPrefs.GetString("Password");
                PlayerPrefs.Save();
                panelLogin.transform.Find("btnEntrar").gameObject.SetActive(true);
                panelLogin.transform.Find("btnAtras").gameObject.SetActive(true);
                panelLogin.transform.Find("CambiarPass").gameObject.SetActive(true);
                panelLogin.transform.Find("btnCambiarPass").gameObject.SetActive(false);
                panelLogin.transform.Find("btnCancelar").gameObject.SetActive(false);
                panelLogin.transform.Find("AccesoAPadres").gameObject.GetComponent<Text>().text = "- Acceso a Padres -";
                inputPassword.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Contraseña";
                inputPassword.placeholder.color = Color.grey;
                inputPassword.text = "";
                panelLogin.transform.Find("PassInputFieldNueva").gameObject.SetActive(false);
                cb = inputPassword.colors;
                cb.normalColor = Color.white;
                inputPassword.colors = cb;
                inputNueva.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Contraseña";
                inputNueva.placeholder.color = Color.grey;
                cb = inputNueva.colors;
                cb.normalColor = Color.white;
                inputNueva.colors = cb;
                Debug.Log(pass);
            }
        }


    }

    public void btnAtras()
    {       
        inputPassword.text = "";
        inputPassword.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Contraseña";
        inputPassword.placeholder.color = Color.grey;
        panelLogin.transform.Find("PassInputFieldNueva").gameObject.SetActive(false);
        ColorBlock cb = inputPassword.colors;
        cb.normalColor = Color.white;
        inputPassword.colors = cb;

    }
}
