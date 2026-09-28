using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Casillero : MonoBehaviour
{
    [Header("JUGADOR")]
    public Transform jugador;

    [Header("PUNTOS")]
    public Transform puntoInterior;
    public Transform puntoSalida;

    [Header("MOVIMIENTO")]
    public PlayerMovement movimientoJugador;

    [Header("MODELO")]
    public Renderer[] renderersJugador;

    [Header("ENEMIGO")]
    public Enemigo enemigo;

    [Header("TEXTO")]
    public TextMeshProUGUI textoAccion;

    [Header("AJUSTES")]
    public float tiempoBloqueoE = 0.5f;

    private bool jugadorCerca = false;
    private bool jugadorEscondido = false;

    private float bloqueoE = 0f;

    void Start()
    {
        if (textoAccion != null)
        {
            textoAccion.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // ==========================================
        // BLOQUEO DE E
        // ==========================================

        if (bloqueoE > 0f)
        {
            bloqueoE -= Time.deltaTime;
        }

        if (Keyboard.current == null)
            return;

        if (bloqueoE > 0f)
            return;

        // ==========================================
        // E
        // ==========================================

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // SI ESTÁ ADENTRO
            if (jugadorEscondido)
            {
                SalirDelCasillero();
                return;
            }

            // SI ESTÁ AFUERA
            if (jugadorCerca)
            {
                EntrarAlCasillero();
            }
        }
    }

    // =================================================
    // ENTRAR
    // =================================================

    void EntrarAlCasillero()
    {
        if (jugador == null)
        {
            Debug.LogError("CASILLERO: No asignaste el Jugador.");
            return;
        }

        if (puntoInterior == null)
        {
            Debug.LogError("CASILLERO: No asignaste Punto Interior.");
            return;
        }

        jugadorEscondido = true;
        bloqueoE = tiempoBloqueoE;

        // ==========================================
        // DETENER FISICA
        // ==========================================

        Rigidbody rb = jugador.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
        }

        // ==========================================
        // MOVER AL INTERIOR
        // ==========================================

        jugador.SetPositionAndRotation(
            puntoInterior.position,
            jugador.rotation
        );

        // ==========================================
        // BLOQUEAR MOVIMIENTO
        // ==========================================

        if (movimientoJugador != null)
        {
            movimientoJugador.movimientoBloqueado = true;
        }

        // ==========================================
        // OCULTAR MODELO
        // ==========================================

        MostrarModelo(false);

        // ==========================================
        // ENEMIGO
        // ==========================================

        if (enemigo != null)
        {
            enemigo.EsconderJugador();
        }

        // ==========================================
        // TEXTO
        // ==========================================

        if (textoAccion != null)
        {
            textoAccion.text = "E - Salir";
            textoAccion.gameObject.SetActive(true);
        }

        Debug.Log("ENTRASTE AL CASILLERO");
    }

    // =================================================
    // SALIR
    // =================================================

    void SalirDelCasillero()
    {
        if (jugador == null)
        {
            Debug.LogError("CASILLERO: No asignaste el Jugador.");
            return;
        }

        if (puntoSalida == null)
        {
            Debug.LogError("CASILLERO: No asignaste Punto Salida.");
            return;
        }

        bloqueoE = tiempoBloqueoE;

        // ==========================================
        // MOVER AFUERA PRIMERO
        // ==========================================

        jugador.SetPositionAndRotation(
            puntoSalida.position,
            jugador.rotation
        );

        // ==========================================
        // REACTIVAR FISICA
        // ==========================================

        Rigidbody rb = jugador.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = false;
        }

        // ==========================================
        // MOSTRAR MODELO
        // ==========================================

        MostrarModelo(true);

        // ==========================================
        // ACTIVAR MOVIMIENTO
        // ==========================================

        if (movimientoJugador != null)
        {
            movimientoJugador.movimientoBloqueado = false;
        }

        // ==========================================
        // ESTADO
        // ==========================================

        jugadorEscondido = false;

        // ==========================================
        // ENEMIGO
        // ==========================================

        if (enemigo != null)
        {
            enemigo.JugadorSalioDelCasillero();
        }

        // ==========================================
        // TEXTO
        // ==========================================

        if (textoAccion != null)
        {
            textoAccion.gameObject.SetActive(false);
        }

        Debug.Log("SALISTE DEL CASILLERO");
    }

    // =================================================
    // MODELO
    // =================================================

    void MostrarModelo(bool mostrar)
    {
        if (renderersJugador == null)
            return;

        foreach (Renderer r in renderersJugador)
        {
            if (r != null)
            {
                r.enabled = mostrar;
            }
        }
    }

    // =================================================
    // TRIGGER
    // =================================================

    void OnTriggerEnter(Collider other)
    {
        if (jugador == null)
            return;

        if (other.transform == jugador ||
            other.transform.IsChildOf(jugador))
        {
            jugadorCerca = true;

            if (!jugadorEscondido && textoAccion != null)
            {
                textoAccion.text = "E - Esconderse";
                textoAccion.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (jugador == null)
            return;

        if (other.transform == jugador ||
            other.transform.IsChildOf(jugador))
        {
            jugadorCerca = false;

            // IMPORTANTE:
            // Si está escondido NO ocultamos el texto,
            // porque E sigue siendo necesario para salir.

            if (!jugadorEscondido && textoAccion != null)
            {
                textoAccion.gameObject.SetActive(false);
            }
        }
    }
}