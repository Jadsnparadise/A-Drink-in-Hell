using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SatanController : MonoBehaviour
{
    private bool playerAlreadyTalked;
    private GameManager gameManager;
    [SerializeField] private DialogueSystem firstTalk;
    [SerializeField] private DialogueSystem othersTalk;
    [SerializeField] private float delayToReleasePlayer = 0.5f;

    public UnityEvent onArriveOnSatan;

    private void Start()
    {
        gameManager = GameManager.Instance;      
        firstTalk.onDialogueEnd.AddListener(FreePlayer);
        othersTalk.onDialogueEnd.AddListener(FreePlayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerAlreadyTalked) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            onArriveOnSatan?.Invoke();
            PlayerController.Instance.isTalking= true;
            if (gameManager.firstTimeTalkingToSatan)
            {
                firstTalk.StartDialogue();

                PlayerPrefs.SetInt("first Time", 0); //0 false, 1 true
                Debug.Log("FALOU");
            }
            else
            {
                othersTalk.StartDialogue();
            }

            GameManager.Instance.SelectRandomDrink();
            playerAlreadyTalked = true;
        }
    }
    
    private void FreePlayer()
    {
        StartCoroutine(nameof(EndTalk));
    }

    private IEnumerator EndTalk()
    {
        yield return new WaitForSeconds(delayToReleasePlayer);
        PlayerController.Instance.isTalking = false;
    }

    private void OnDestroy()
    {
        firstTalk.onDialogueEnd.RemoveListener(FreePlayer);
        othersTalk.onDialogueEnd.RemoveListener(FreePlayer);
    }
}
