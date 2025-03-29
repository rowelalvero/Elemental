using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private TMP_Text speakerText;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private Image portraitImage;

    [Header("Content")]
    [SerializeField]
    private List<DialogueEntry> dialogues = new List<DialogueEntry>();

    private bool dialogueActivated;
    private int step = 0;

    [System.Serializable]
    public class DialogueEntry
    {
        public string speaker;
        public Sprite portrait;
        [TextArea(2, 8)]
        public string dialogueWords;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            dialogueActivated = true;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && dialogueActivated)
        {
            if (step >= dialogues.Count)
            {
                dialogueCanvas.SetActive(false);
                step = 0;
            }
            else
            {
                dialogueCanvas.SetActive(true);
                speakerText.text = dialogues[step].speaker;
                dialogueText.text = dialogues[step].dialogueWords;
                portraitImage.sprite = dialogues[step].portrait;
                step += 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dialogueActivated = false;
        dialogueCanvas.SetActive(false);
    }
}
