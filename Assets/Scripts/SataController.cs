using UnityEngine;

public class SataController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //TODO: Start fala de satã
            //Aqui vai ser definido qual drink ele vai querer
        }
    }
}
