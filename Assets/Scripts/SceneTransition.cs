using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
   [SerializeField] private string nextSceneName = "FinalMap";

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (collision.CompareTag("Player"))
         SceneManager.LoadScene(nextSceneName);
   }
}