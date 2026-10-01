using UnityEngine;
using TMPro;
using System.Collections;

public class Door : MonoBehaviour
{
    public GameObject pantallaFinal;
    public TMP_Text textoMensaje;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = pantallaFinal.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = pantallaFinal.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        textoMensaje.alpha = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (KeyManager.llaves >= 2)
            {
                StartCoroutine(NivelCompletado());
            }
            else
            {
                textoMensaje.text = "Llaves insuficientes\n" + KeyManager.llaves + "/2";

                StopAllCoroutines();
                StartCoroutine(MostrarMensajeInsuficiente());
            }
        }
    }

    IEnumerator NivelCompletado()
    {
        textoMensaje.text = "¡Nivel completado!";

        float tiempo = 2f;
        float transcurrido = 0f;

        while (transcurrido < tiempo)
        {
            transcurrido += Time.deltaTime;

            float progreso = transcurrido / tiempo;

            canvasGroup.alpha = progreso;
            textoMensaje.alpha = progreso;

            yield return null;
        }

        canvasGroup.alpha = 1f;
        textoMensaje.alpha = 1f;
    }

    IEnumerator MostrarMensajeInsuficiente()
    {
        textoMensaje.alpha = 1f;

        yield return new WaitForSeconds(2f);

        float tiempo = 1f;
        float transcurrido = 0f;

        while (transcurrido < tiempo)
        {
            transcurrido += Time.deltaTime;

            textoMensaje.alpha = 1f - (transcurrido / tiempo);

            yield return null;
        }

        textoMensaje.alpha = 0f;
    }
}