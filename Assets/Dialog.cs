using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Dialog : MonoBehaviour
{
    [SerializeField] private Text messageText;
    private Queue<string> messages;
    [SerializeField] private MoveController moveController;
    private void Start()
    {
        messages = new Queue<string>();
    }
    public void StartDialog(DialogMessages dialog)
    {
        messageText.gameObject.SetActive(true);
        messages.Clear();
        foreach (string message in dialog.messages)
        {
            messages.Enqueue(message);
        }
        ShowNextMessage();
    }
    public void ShowNextMessage()
    { 
        if(messages.Count == 0)
        {
            EndDialog();
            return;
        }
        string message = messages.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeMessage(message));
    }
    IEnumerator TypeMessage(string message)
    {
        messageText.text = "";
        foreach(char letter in message.ToCharArray())
        {
            messageText.text += letter;
            yield return null;
        }
    }
    private void EndDialog()
    {
        moveController.isDialogGoingOn = false;
        messageText.gameObject.SetActive(false);
    }
}
