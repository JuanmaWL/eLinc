using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimacionVentanas : MonoBehaviour
{
    public Sprite manzanaverde, manzanaroja;
    public Button comer, dormir;
    public AudioClip platoEntra, platoSale;
    public void Animacion(GameObject panel)
    {
        Animator animator = panel.GetComponent<Animator>();
        if (animator != null)
        {
            bool abrir = animator.GetBool("abrir");
            animator.SetBool("abrir", !abrir);
            if (panel.name.Equals("Plato"))
            {
                AudioSource audioSource = panel.GetComponent<AudioSource>();

                if (!abrir)
                {
                    audioSource.clip = platoEntra;
                    audioSource.Play();
                    dormir.interactable = false;
                    comer.GetComponent<Image>().sprite = manzanaverde;
                }

                else
                { 
                    audioSource.clip = platoSale;
                    audioSource.Play();
                    dormir.interactable = true;
                    comer.GetComponent<Image>().sprite = manzanaroja;
                }
            }
        }

    }
}
