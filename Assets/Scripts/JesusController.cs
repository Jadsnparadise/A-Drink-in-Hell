using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class JesusController : MonoBehaviour
{
   [Header("Dialogues")]
   [SerializeField] private DialogueSystem dialogue;
   [SerializeField] private float delayToReleasePlayer = 0.5f;

   [Header("Events")]
   public UnityEvent onTalkStarted;
   public UnityEvent onTalkEnded;

   private bool _alreadyTalked;

   private void Start()
   {
      if (dialogue != null)
         dialogue.onDialogueEnd.AddListener(OnDialogueFinished);
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (_alreadyTalked) return;

      if (collision.CompareTag("Player"))
      {
         StartDialogue();
      }
   }

   private void StartDialogue()
   {
      _alreadyTalked = true;

      PlayerController.Instance.isTalking = true;
      onTalkStarted?.Invoke();

      if (dialogue)
         dialogue.StartDialogue();
   }

   private void OnDialogueFinished()
   {
      StartCoroutine(nameof(EndTalk));
   }

   private IEnumerator EndTalk()
   {
      yield return new WaitForSeconds(delayToReleasePlayer);
      PlayerController.Instance.isTalking = false;
      onTalkEnded?.Invoke();
   }

   private void OnDestroy()
   {
      if (dialogue != null)
         dialogue.onDialogueEnd.RemoveListener(OnDialogueFinished);
   }
}