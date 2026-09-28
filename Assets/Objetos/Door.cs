using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (KeyManager.llaves >= 2)
            {
                Debug.Log("¡Nivel completado!");

                // cargar nivel
        }
    }
}
}
