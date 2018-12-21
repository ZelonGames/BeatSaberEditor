using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtbColorSwitcher : MonoBehaviour
{
    private Button buttonComponent = null;
    private Text btnText = null;
    private ColorBlock colorBlock;

    private Color PressedColor
    {
        get
        {
            float darkness = 0.5f;
            return new Color(colorBlock.normalColor.r - darkness, colorBlock.normalColor.g - darkness, colorBlock.normalColor.b - darkness);
        }
    }

    private void Start()
    {
        buttonComponent = gameObject.GetComponent<Button>();
        btnText = buttonComponent.GetComponentInChildren<Text>();
        
        UpdateButtonColor();
    }

    public void ChangeColor()
    {
        MapEditorManager.Instance.SwitchColor();
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        colorBlock = buttonComponent.colors;

        switch (MapEditorManager.Instance.CurrentColor)
        {
            case Note.ColorType.Red:
                colorBlock.normalColor = Color.red;
                colorBlock.pressedColor = colorBlock.highlightedColor = PressedColor;
                btnText.text = "Red";
                break;
            case Note.ColorType.Blue:
                colorBlock.normalColor = Color.blue;
                colorBlock.pressedColor = colorBlock.highlightedColor = PressedColor;
                btnText.text = "Blue";
                break;
            default:
                break;
        }

        buttonComponent.colors = colorBlock;
    }
}
