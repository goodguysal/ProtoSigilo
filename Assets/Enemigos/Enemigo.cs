using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Transform[] puntosPatrulla;
    public float velocidad = 2f;
    public float distanciaLlegada = 0.2f;
    public float tiempoEspera = 3f;

    private int puntoActual = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Patrullar();
    }

    void Patrullar()
    {
        if (puntosPatrulla.Length == 0)
            return;

        if (esperando)
        {
            temporizadorEspera -= Time.deltaTime;

            if (temporizadorEspera <= 0f)
            {
                esperando = false;
                puntoActual++;

                if (puntoActual >= puntosPatrulla.Length)
                {
                    puntoActual = 0;
                }
            }

            return;
        }

        Transform objetivo = puntosPatrulla[puntoActual];

        Vector3 direccion = objetivo.position - transform.position;
        direccion.y = 0f;

        if (direccion.magnitude > distanciaLlegada)
        {
            rb.MovePosition(
                transform.position +
                direccion.normalized * velocidad * Time.deltaTime
            );
        }
        else
        {
            esperando = true;
            temporizadorEspera = tiempoEspera;
        }
    }
}
