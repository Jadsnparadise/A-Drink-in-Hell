using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private int sceneIndexToLoad;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndexToLoad);
        }
    }
}
