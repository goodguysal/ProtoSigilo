using UnityEngine;
using TMPro;
using System.Collections;

public class Door : MonoBehaviour
{
    public GameObject pantallaFinal;
    public TMP_Text textoMensaje;
    public TMP_Text textoLlavesInsuficientes;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        // Pantalla final
        canvasGroup = pantallaFinal.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = pantallaFinal.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;

        // Ocultar textos al comenzar
        textoMensaje.gameObject.SetActive(false);
        textoLlavesInsuficientes.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Jugador tocó la puerta. Llaves: " + KeyManager.llaves);

        if (KeyManager.llaves >= 2)
        {
            StartCoroutine(NivelCompletado());
        }
        else
        {
            StartCoroutine(MostrarLlavesInsuficientes());
        }
    }
    IEnumerator MostrarLlavesInsuficientes()
    {
        textoLlavesInsuficientes.text =
            "Llaves insuficientes...\n" +
            KeyManager.llaves + "/2";

        textoLlavesInsuficientes.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        textoLlavesInsuficientes.gameObject.SetActive(false);
    }

    IEnumerator NivelCompletado()
    {
        textoMensaje.text = "¡Nivel completado!";
        textoMensaje.gameObject.SetActive(true);

        float tiempo = 2f;
        float transcurrido = 0f;

        while (transcurrido < tiempo)
        {
            transcurrido += Time.deltaTime;

            canvasGroup.alpha = transcurrido / tiempo;

            yield return null;
        }

        canvasGroup.alpha = 1f;  }
}