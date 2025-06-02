using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    public DialogueDataSO myDialogue;
    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        if(dialogueManager == null)
        {
            Debug.LogError("다이얼로그 매니저가 없습니다.");
        }
    }

    private void OnMouseDown()
    {
        if (dialogueManager == null) return;
        if (dialogueManager.IsDialogueActive()) return;
        if(myDialogue == null) return;

        dialogueManager.StartDialogue(myDialogue);
    }
}
