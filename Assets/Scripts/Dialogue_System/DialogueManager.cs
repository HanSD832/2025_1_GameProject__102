using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI ��� - Inspector���� ����")]
    public GameObject DialoguePanel;
    public Image characterImage;
    public TextMeshProUGUI characternameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    [Header("�⺻ ����")]
    public Sprite defaultCharacterImage;

    [Header("Ÿ���� ȿ�� ����")]
    public float typingSpeed = 0.05f;
    public bool skipTypingOnClick = true;

    private DialogueDataSO currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        DialoguePanel.SetActive(false);
        nextButton.onClick.AddListener(HandleNexInput);
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            HandleNexInput();
        }
    }
    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogueText.text = "";

        for(int i = 0; i < textToType.Length; i++)
        {
            dialogueText.text += textToType[i];
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void CompleteTyping()
    {
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        isTyping = false;

        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
        }
    }

    void ShowCurrentLine()
    {
        if(currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            if(typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            string currentText = currentDialogue.dialogueLines[currentLineIndex];
            typingCoroutine=StartCoroutine(TypeText(currentText));
        }
    }

    public void ShowNextLine()
    {
        currentLineIndex++;

        if(currentLineIndex>=currentDialogue.dialogueLines.Count)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine();
        }
    }

    void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;
        isTyping = false;
        DialoguePanel.SetActive(false);
        currentLineIndex = 0;
    }

    public void HandleNexInput()
    {
        if (isTyping && skipTypingOnClick)
        {
            CompleteTyping();
        }
        else if(!isTyping)
        {
            ShowNextLine();
        }
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    public void StartDialogue(DialogueDataSO dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Count == 0) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        DialoguePanel.SetActive(true);
        characternameText.text = dialogue.characterName;

        if(characterImage != null)
        {
            if(dialogue.characterImage != null)
            {
                characterImage.sprite = dialogue.characterImage;
            }
            else
            {
                characterImage.sprite = defaultCharacterImage;
            }
        }

        ShowCurrentLine();
    }
}

