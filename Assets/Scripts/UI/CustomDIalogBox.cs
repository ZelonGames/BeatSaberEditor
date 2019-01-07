using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomDIalogBox : MonoBehaviour
{
    private static CustomDIalogBox dialogBox = null;

    [SerializeField]
    private TextMeshProUGUI txtTitle;

    [SerializeField]
    private TextMeshProUGUI txtMessage;

    public enum AnswerState
    {
        Yes,
        No,
        Thinking,
    }

    public AnswerState Answer
    {
        get
        {
            if (!dialogBox.answer.HasValue)
                return AnswerState.Thinking;
            else if (dialogBox.answer.Value)
                return AnswerState.Yes;
            else
                return AnswerState.No;
        }
    }

    private bool? answer = null;

    public static CustomDIalogBox Show(string title, string message)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (dialogBox == null)
            dialogBox = ((GameObject)Instantiate(Resources.Load("DialogBoxView"), canvas.transform, false)).GetComponent<CustomDIalogBox>();
        else
            Instantiate(dialogBox);

        dialogBox.txtTitle.text = title;
        dialogBox.txtMessage.text = message;

        dialogBox.answer = null;

        return dialogBox;
    }

    public void OnAnswerYes()
    {
        AnswerAction(true);
    }

    public void OnAnswerNo()
    {
        AnswerAction(false);
    }

    private void AnswerAction(bool answer)
    {
        Destroy(dialogBox.gameObject);
        dialogBox.answer = answer;
    }
}
