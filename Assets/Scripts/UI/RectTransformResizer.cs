using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectTransformResizer : MonoBehaviour
{
    private Rect rect;

    private void Start()
    {
        rect = gameObject.GetComponent<RectTransform>().rect;
    }

    private void Update()
    {
        rect.height = rect.width * (3f / 4f);
    }
}
