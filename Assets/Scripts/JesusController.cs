using UnityEngine;
using UnityEngine.Events;

public class JesusController : MonoBehaviour
{
   [Header("Dialogues")]
   [SerializeField] private DialogueSystem dialogue;

   [Header("Events")]
   public UnityEvent onTalkStarted;
   public UnityEvent onTalkEnded;

   private bool _playerIsClose;
   private bool _isTalking;

   private void Start()
   {
      if (dialogue != null)
         dialogue.onDialogueEnd.AddListener(OnDialogueFinished);
   }

   private void Update()
   {
      if (_playerIsClose && !_isTalking && Input.GetKeyDown(KeyCode.E))
         StartDialogue();
   }

   private void StartDialogue()
   {
      _isTalking = true;
      onTalkStarted?.Invoke();

      if (dialogue)
         dialogue.StartDialogue();
   }

   private void OnDialogueFinished()
   {
      _isTalking = false;
      onTalkEnded?.Invoke();
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (collision.CompareTag("Player"))
         _playerIsClose = true;
   }

   private void OnTriggerExit2D(Collider2D collision)
   {
      if (collision.CompareTag("Player"))
         _playerIsClose = false;
   }

   private void OnDestroy()
   {
      if (dialogue != null)
         dialogue.onDialogueEnd.RemoveListener(OnDialogueFinished);
   }
}