using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SatanController : MonoBehaviour
{
    private bool playerAlreadyTalked;
    private GameManager gameManager;
    [SerializeField] private DialogueSystem firstTalk;
    [SerializeField] private DialogueSystem othersTalk;

    public UnityEvent onArriveOnSatan;

    private void Start()
    {
        gameManager = GameManager.Instance;       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerAlreadyTalked) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            onArriveOnSatan?.Invoke();
            collision.gameObject.GetComponent<PlayerController>().enabled = false;
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
    

}
