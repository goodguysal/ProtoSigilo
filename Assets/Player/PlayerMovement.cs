
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 7f;

    [Header("Camara")]
    public Transform camara;
    public float sensibilidad = 200f;
    public float limiteVertical = 80f;

    private Rigidbody rb;
    private bool puedeSaltar;

    private float rotacionVertical = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // =========================
        // SALTO
        // =========================

        if (Keyboard.current.spaceKey.wasPressedThisFrame && puedeSaltar)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            puedeSaltar = false;
        }


        // =========================
        // CAMARA / MOUSE
        // =========================

        Vector2 mouse = Mouse.current.delta.ReadValue();

        // Girar personaje izquierda / derecha
        float rotacionHorizontal = mouse.x * sensibilidad * Time.deltaTime;

        transform.Rotate(Vector3.up * rotacionHorizontal);


        // Mirar arriba / abajo
        rotacionVertical -= mouse.y * sensibilidad * Time.deltaTime;

        rotacionVertical = Mathf.Clamp(
            rotacionVertical,
            -limiteVertical,
            limiteVertical
        );

        camara.localRotation = Quaternion.Euler(
            rotacionVertical,
            0f,
            0f
        );


        // =========================
        // ESC
        // =========================

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    void FixedUpdate()
    {
        float horizontal = 0f;
        float vertical = 0f;


        // =========================
        // WASD
        // =========================

        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        if (Keyboard.current.wKey.isPressed)
            vertical = 1f;

        if (Keyboard.current.sKey.isPressed)
            vertical = -1f;


        // =========================
        // MOVIMIENTO SEGUN LA CAMARA
        // =========================

        Vector3 adelante = camara.forward;
        Vector3 derecha = camara.right;

        // Ignorar la inclinacion vertical de la camara
        adelante.y = 0f;
        derecha.y = 0f;

        adelante.Normalize();
        derecha.Normalize();


        // W = hacia donde mira la camara
        // S = hacia atras
        // A = izquierda
        // D = derecha

        Vector3 movimiento =
            (adelante * vertical) +
            (derecha * horizontal);

        movimiento.Normalize();


        // Aplicar velocidad

        Vector3 velocidadMovimiento = movimiento * velocidad;

        rb.linearVelocity = new Vector3(
            velocidadMovimiento.x,
            rb.linearVelocity.y,
            velocidadMovimiento.z
        );
    }


    private void OnCollisionEnter(Collision collision)
    {
        puedeSaltar = true;
    }
}



