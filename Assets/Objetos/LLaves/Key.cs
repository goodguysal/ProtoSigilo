using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<KeyManager>().AgarrarLlave();

            Destroy(gameObject);
        }
    }
}
