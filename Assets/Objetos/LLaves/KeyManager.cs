using UnityEngine;
using TMPro;

public class KeyManager : MonoBehaviour
{
    public static int llaves = 0;

    public TMP_Text textoLlaves;

    void Start()
    {
        llaves = 0;
        ActualizarTexto();
    }

    public void AgarrarLlave()
    {
        if (llaves < 2)
        {
            llaves++;
            ActualizarTexto();
        }
    }

    void ActualizarTexto()
    {
        textoLlaves.text = llaves + "/2";
    }
}
