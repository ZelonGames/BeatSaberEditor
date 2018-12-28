using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class PrecisionSlider : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI precisionText;

    private Slider sliderComponent = null;

    private int SliderValue
    {
        get
        {
            return (int)sliderComponent.value + 1;
        }
    }

    private void Start()
    {
        sliderComponent = gameObject.GetComponent<Slider>();
        UpdatePrecisionText(SliderValue);
    }

    public void OnChangePrecision()
    {
        int value = SliderValue;
        MapEditorManager.Instance.ChangePrecision(value);
        UpdatePrecisionText(value);
    }

    private void UpdatePrecisionText(int value)
    {
        precisionText.text = value.ToString();
    }
}
