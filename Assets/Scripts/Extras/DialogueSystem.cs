using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public struct DialogueLine
{
    [TextArea(3, 10)]
    public string sentence;

    public float displayDuration;
}

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI uiText;

    [Header("Dialogue")]
    public List<DialogueLine> dialogueLines;

    private float typingSpeed = 0.05f;

    [SerializeField] private float endEventDelay = 0f;

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private Coroutine dialogueCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false;

    void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            skipTyping = true;
        }
    }

    public void StartDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(PlayDialogue());
    }

    private IEnumerator PlayDialogue()
    {
        uiText.transform.parent.gameObject.SetActive(true);

        onDialogueStart?.Invoke();

        foreach (DialogueLine line in dialogueLines)
        {
            uiText.text = "";
            isTyping = true;
            skipTyping = false;

            foreach (char letter in line.sentence.ToCharArray())
            {
                if (skipTyping)
                {
                    uiText.text = line.sentence;
                    break;
                }

                uiText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;

            yield return new WaitForSeconds(line.displayDuration);
        }

        uiText.text = "";
        uiText.transform.parent.gameObject.SetActive(false);

        if (endEventDelay > 0f)
        {
            yield return new WaitForSeconds(endEventDelay);
        }

        
        onDialogueEnd?.Invoke();
        dialogueCoroutine = null;
    }
}