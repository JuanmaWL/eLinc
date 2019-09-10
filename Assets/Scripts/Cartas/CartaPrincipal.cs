using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartaPrincipal : MonoBehaviour
{
    [SerializeField] private Controlador controlador;
    [SerializeField] private GameObject cartaDetras;
    [SerializeField] private AudioSource audio;

    public void OnMouseDown()
    {
        if (cartaDetras.activeSelf  && controlador.puedeRevelarse)
        {
            audio.Play();
            this.GetComponent<Animator>().Play("VoltearCarta", -1, 0f);
            cartaDetras.SetActive(false);
            controlador.CartaRevelada(this);
        }
    }

    private int _id;
    
    public int id
    {
        get { return _id; }
    }

    public void CambiarSprite(int id, Sprite image)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = image;
    }
    
    public void Revelar()
    {
        this.GetComponent<Animator>().Play("VoltearCarta", -1, 0f);
        cartaDetras.SetActive(true);
    }
}
