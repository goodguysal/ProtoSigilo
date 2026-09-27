using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // Busca la cámara principal automáticamente al iniciar
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // Hace que el texto mire en la misma dirección que la cámara,
            // evitando el efecto "espejo" o que se vea al revés.
            transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
        }
    }
}

