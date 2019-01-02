using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonDownEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Color normalColor;
    private Color pressedColor = new Color(0.8f, 0.8f, 0.8f);
    private Text txtButton = null;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (txtButton == null)
            txtButton = gameObject.GetComponentInChildren<Text>();

        txtButton.color = pressedColor;

        RawImage[] childImages = gameObject.GetComponentsInChildren<RawImage>();
        normalColor = childImages.First().color;
        foreach (var childImage in childImages)
            childImage.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        txtButton.color = normalColor;

        foreach (var childObject in gameObject.GetComponentsInChildren<RawImage>())
            childObject.color = normalColor;
    }
}
