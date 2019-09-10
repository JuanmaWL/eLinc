using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controlador : MonoBehaviour
{
    public const int gridRows = 4;
    public const int gridCols = 5;
    public const float offsetX = 2f;
    public const float offsetY = 2f;

    private static int cartasSolucionadas;

    [SerializeField] private CartaPrincipal originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private GameObject panelCartas, panelMensaje;
    [SerializeField] private AtributosMascota subirAtributo;
    [SerializeField] private SubirNivelyMonedas subir;
    
    
    public void RellenarPanelCartas()
    {
        originalCard = panelCartas.transform.GetChild(0).gameObject.GetComponent<CartaPrincipal>();

        cartasSolucionadas = 0;
        Vector3 startPos = originalCard.transform.position; //The position of the first card. All other cards are offset from here.

        CartaPrincipal cartaCopia = Instantiate(originalCard, panelCartas.transform) as CartaPrincipal;
        cartaCopia.name = "CARTA_ORIGINAL";
        cartaCopia.gameObject.SetActive(false);

        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9 };
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                CartaPrincipal card;
                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard, panelCartas.transform) as CartaPrincipal;
                }

                int index = j * gridCols + i;
                int id = numbers[index];
                card.CambiarSprite(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY = (offsetY * j) + startPos.y;
                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    //---------------------------------------------------------------------------//

    private CartaPrincipal _primeraRevelada;
    private CartaPrincipal _segundaRevelada;

    private int _puntuacion = 0;

    public bool puedeRevelarse
    {
        get { return _segundaRevelada == null; }
    }

    public void CartaRevelada(CartaPrincipal carta)
    {
        if (_primeraRevelada == null)
        {
            _primeraRevelada = carta;
        }
        else
        {
            _segundaRevelada = carta;
            StartCoroutine(ComprobarPareja());
        }
    }

    private IEnumerator ComprobarPareja()
    {
        if (_primeraRevelada.id == _segundaRevelada.id)
        {
            cartasSolucionadas++;

            _puntuacion++;
            subir.subirMonedas(_puntuacion);
            subir.subirExperiencia(0.05f);
            subirAtributo.subirDiversion(10);
            ParticleSystem carta1 = _primeraRevelada.gameObject.transform.Find("Particle System").gameObject.GetComponent<ParticleSystem>();
            ParticleSystem carta2 = _segundaRevelada.gameObject.transform.Find("Particle System").gameObject.GetComponent<ParticleSystem>();
            carta1.Play();
            carta2.Play();
            AudioSource audio = _primeraRevelada.gameObject.GetComponent<AudioSource>();
            audio.Play();

            if (cartasSolucionadas == 10)
            {
                Debug.Log("Terminado");
                panelMensaje.SetActive(true);
            }
        }

        else
        {
            yield return new WaitForSeconds(0.5f);
            _primeraRevelada.Revelar();
            _segundaRevelada.Revelar();
        }

        _primeraRevelada = null;
        _segundaRevelada = null;
    }

    public void ReiniciarJuego()
    {
        StartCoroutine(EliminarCartasPanel());
    }
    public IEnumerator EliminarCartasPanel()
    {
        

        yield return new WaitForSeconds(0.8f);

        foreach (Transform child in panelCartas.transform)
        {
            if (child.name != "CARTA_ORIGINAL") Destroy(child.gameObject);
            else
            {
                child.gameObject.SetActive(true);
                child.name = "Carta";
            }
        }

        
    }
}
