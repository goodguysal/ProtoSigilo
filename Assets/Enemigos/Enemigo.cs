using UnityEngine;

public enum EstadoEnemigo
{
    Patrullando,
    Alerta,
    Persiguiendo
}

public class Enemigo : MonoBehaviour
{
    public EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;

    // PATRULLA
    public Transform[] puntosPatrulla;
    public float velocidad = 2f;
    public float distanciaLlegada = 0.2f;
    public float tiempoEspera = 3f;

    // JUGADOR
    public Transform jugador;
    public LayerMask capaJugador;
    public LayerMask capasObstaculos;

    // VISION
    public float radioAlerta = 6f;
    public float distanciaDeteccionCercana = 1.2f;
    public float anguloVision = 45f;

    // PERSECUCION
    public float radioPersecucion = 8f;
    public float velocidadPersecucion = 4f;
    public float distanciaAtaque = 1f;

    // ALERTA
    public float tiempoAlerta = 1.5f;

    // GIRO
    public float velocidadGiro = 5f;

    // RESPAWN
    public Transform checkpointJugador;

    // SIMBOLOS
    public GameObject simboloPregunta;
    public GameObject simboloExclamacion;

    // VARIABLES INTERNAS
    private Rigidbody rb;

    private int puntoActual = 0;
    private float temporizadorEspera = 0f;
    private float temporizadorAlerta = 0f;

    private bool esperando = false;
    private bool jugadorDetectado = false;
    private bool jugadorEscondido = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Update()
    {
        DetectarJugador();

        if (estadoActual == EstadoEnemigo.Patrullando)
        {
            Patrullar();
        }
        else if (estadoActual == EstadoEnemigo.Alerta)
        {
            MirarAlJugador();
        }
        else if (estadoActual == EstadoEnemigo.Persiguiendo)
        {
            PerseguirJugador();
        }

        ActualizarSimbolos();
    }

    // ==========================================
    // DETECTAR JUGADOR
    // ==========================================
   
    void DetectarJugador()
    {
        // PATRULLANDO
        if (estadoActual == EstadoEnemigo.Patrullando)
        {
            if (JugadorEnRadioDeAlerta())
            {
                estadoActual = EstadoEnemigo.Alerta;
                temporizadorAlerta = 0f;
                jugadorDetectado = false;

                return;
            }
        }
        if (jugadorEscondido)
        {
            temporizadorAlerta += Time.deltaTime;

            if (temporizadorAlerta >= tiempoAlerta)
            {
                estadoActual = EstadoEnemigo.Patrullando;
                temporizadorAlerta = 0f;
                jugadorDetectado = false;
                esperando = false;
                temporizadorEspera = 0f;
                puntoActual = 0;
            }

            return;
        }

        // ALERTA
        if (estadoActual == EstadoEnemigo.Alerta)
        {
            temporizadorAlerta += Time.deltaTime;

            if (JugadorEnConoDeVision())
            {
                estadoActual = EstadoEnemigo.Persiguiendo;
                temporizadorAlerta = 0f;
                jugadorDetectado = true;

                return;
            }

            if (temporizadorAlerta >= tiempoAlerta)
            {
                estadoActual = EstadoEnemigo.Patrullando;

                temporizadorAlerta = 0f;
                jugadorDetectado = false;

                esperando = false;
                temporizadorEspera = 0f;

                puntoActual = 0;

                return;
            }
        }

        // PERSIGUIENDO
        if (estadoActual == EstadoEnemigo.Persiguiendo)
        {
            if (JugadorVisibleDurantePersecucion())
            {
                jugadorDetectado = true;
                return;
            }

            estadoActual = EstadoEnemigo.Alerta;

            temporizadorAlerta = 0f;
            jugadorDetectado = false;
        }
    }

    // ==========================================
    // DETECCION NORMAL
    // ==========================================

    bool JugadorEnRadioDeAlerta()
    {
        Vector3 origen = transform.position + Vector3.up;

        Vector3 direccionJugador = jugador.position - origen;

        float distancia = direccionJugador.magnitude;

        if (distancia > radioAlerta)
        {
            return false;
        }

        // DETECCION 360 GRADOS DE CERCA
        if (distancia <= distanciaDeteccionCercana)
        {
            return PuedeDetectarJugadorDeCerca();
        }

        // CONO DE VISION
        float angulo = Vector3.Angle(
            transform.forward,
            direccionJugador.normalized
        );

        if (angulo > anguloVision)
        {
            return false;
        }

        // RAYCAST
        RaycastHit golpe;

        if (Physics.Raycast(
            origen,
            direccionJugador.normalized,
            out golpe,
            distancia,
            capaJugador | capasObstaculos))
        {
            if (EsElJugador(golpe.transform))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    // ==========================================
    // DETECCION CERCANA
    // ==========================================

    bool PuedeDetectarJugadorDeCerca()
    {
        Vector3 origen = transform.position + Vector3.up;

        Vector3 direccion = jugador.position - origen;

        float distancia = direccion.magnitude;

        RaycastHit golpe;

        if (Physics.Raycast(
            origen,
            direccion.normalized,
            out golpe,
            distancia,
            capaJugador | capasObstaculos))
        {
            if (EsElJugador(golpe.transform))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    // ==========================================
    // CONO DE VISION
    // ==========================================

    bool JugadorEnConoDeVision()
    {
        Vector3 origen = transform.position + Vector3.up;

        Vector3 direccionJugador = jugador.position - origen;

        float distanciaJugador = direccionJugador.magnitude;

        if (distanciaJugador > radioAlerta)
        {
            return false;
        }

        float angulo = Vector3.Angle(
            transform.forward,
            direccionJugador.normalized
        );

        if (angulo > anguloVision)
        {
            return false;
        }

        RaycastHit golpe;

        if (Physics.Raycast(
            origen,
            direccionJugador.normalized,
            out golpe,
            distanciaJugador,
            capaJugador | capasObstaculos))
        {
            if (EsElJugador(golpe.transform))
            {
                return true;
            }
        }

        return false;
    }

    // ==========================================
    // VISION DURANTE PERSECUCION
    // ==========================================

    bool JugadorVisibleDurantePersecucion()
    {
        Vector3 origen = transform.position + Vector3.up;

        Vector3 direccionJugador = jugador.position - origen;

        float distancia = direccionJugador.magnitude;

        if (distancia > radioPersecucion)
        {
            return false;
        }

        // MUY CERCA
        if (distancia <= distanciaDeteccionCercana)
        {
            return PuedeDetectarJugadorDeCerca();
        }

        // CONO
        float angulo = Vector3.Angle(
            transform.forward,
            direccionJugador.normalized
        );

        if (angulo > anguloVision)
        {
            return false;
        }

        // RAYCAST
        RaycastHit golpe;

        if (Physics.Raycast(
            origen,
            direccionJugador.normalized,
            out golpe,
            distancia,
            capaJugador | capasObstaculos))
        {
            if (EsElJugador(golpe.transform))
            {
                return true;
            }

            return false;
        }

        return false;
    }

    // ==========================================
    // COMPROBAR JUGADOR
    // ==========================================

    bool EsElJugador(Transform objetoGolpeado)
    {
        if (objetoGolpeado == jugador)
        {
            return true;
        }

        if (objetoGolpeado.IsChildOf(jugador))
        {
            return true;
        }

        return false;
    }

    // ==========================================
    // MIRAR AL JUGADOR
    // ==========================================

    void MirarAlJugador()
    {
        Vector3 direccion = jugador.position - transform.position;

        direccion.y = 0f;

        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(direccion);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadGiro * Time.deltaTime
            );
        }
    }

    // ==========================================
    // PERSEGUIR
    // ==========================================

    void PerseguirJugador()
    {
        Vector3 direccion = jugador.position - transform.position;

        direccion.y = 0f;

        float distancia = direccion.magnitude;

        // GIRAR
        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(direccion);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadGiro * Time.deltaTime
            );
        }

        // ATAQUE
        if (distancia <= distanciaAtaque)
        {
            MatarJugador();
            return;
        }

        // MOVIMIENTO
        if (distancia > 0.1f && rb != null)
        {
            Vector3 movimiento =
                direccion.normalized *
                velocidadPersecucion *
                Time.deltaTime;

            rb.MovePosition(
                rb.position + movimiento
            );
        }
    }

    // ==========================================
    // MATAR JUGADOR
    // ==========================================

    void MatarJugador()
    {
        jugador.position = checkpointJugador.position;

        estadoActual = EstadoEnemigo.Patrullando;

        jugadorDetectado = false;

        puntoActual = 0;

        temporizadorAlerta = 0f;

        esperando = false;

        temporizadorEspera = 0f;
    }

    // ==========================================
    // PATRULLAR
    // ==========================================

    void Patrullar()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
        {
            return;
        }

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

        if (objetivo == null)
        {
            return;
        }

        Vector3 direccion =
            objetivo.position - transform.position;

        direccion.y = 0f;

        if (direccion.magnitude > distanciaLlegada)
        {
            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(direccion.normalized);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadGiro * Time.deltaTime
            );

            if (rb != null)
            {
                Vector3 movimiento =
                    direccion.normalized *
                    velocidad *
                    Time.deltaTime;

                rb.MovePosition(
                    rb.position + movimiento
                );
            }
        }
        else
        {
            esperando = true;

            temporizadorEspera = tiempoEspera;
        }
    }

    // ==========================================
    // SIMBOLOS
    // ==========================================

    void ActualizarSimbolos()
    {
        if (simboloPregunta != null)
        {
            simboloPregunta.SetActive(
                estadoActual == EstadoEnemigo.Alerta
            );
        }

        if (simboloExclamacion != null)
        {
            simboloExclamacion.SetActive(
                estadoActual == EstadoEnemigo.Persiguiendo
            );
        }
    }
    public void EsconderJugador()
    {
        jugadorEscondido = true;

        estadoActual = EstadoEnemigo.Alerta;
        temporizadorAlerta = 0f;
        jugadorDetectado = false;
    }

    public void JugadorSalioDelCasillero()
    {
        jugadorEscondido = false;

        temporizadorAlerta = 0f;
        jugadorDetectado = false;
    }
}