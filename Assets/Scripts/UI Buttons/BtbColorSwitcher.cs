using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtbColorSwitcher : MonoBehaviour
{
    private Button buttonComponent = null;
    private Text btnText = null;
    private ColorBlock colorBlock;

    private Color HighlightedColor
    {
        get
        {
            float darkness = 0.5f;
            return new Color(colorBlock.normalColor.r - darkness, colorBlock.normalColor.g - darkness, colorBlock.normalColor.b - darkness);
        }
    }

    void Start()
    {
        buttonComponent = gameObject.GetComponent<Button>();
        btnText = buttonComponent.GetComponentInChildren<Text>();
        colorBlock = buttonComponent.colors;

        UpdateButtonColor();
    }

    public void ChangeColor()
    {
        MapEditorManager.Instance.SwitchColor();
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        switch (MapEditorManager.Instance.CurrentColor)
        {
            case Note.ColorType.Red:
                colorBlock.normalColor = Color.red;
                colorBlock.highlightedColor = HighlightedColor;
                btnText.text = "Red";
                break;
            case Note.ColorType.Blue:
                colorBlock.normalColor = Color.blue;
                colorBlock.highlightedColor = HighlightedColor;
                btnText.text = "Blue";
                break;
            default:
                break;
        }

        buttonComponent.colors = colorBlock;
    }
}
