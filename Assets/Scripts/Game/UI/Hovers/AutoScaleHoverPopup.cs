using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScaleHoverPopup : MonoBehaviour
{
    [SerializeField] private RectTransform _verticalLayoutGroup;

    private RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, GetTotalHeight());
    }

    /// <summary>
    /// Get sum of each active child
    /// </summary>
    /// <returns></returns>
    float GetTotalHeight()
    {
        float totalHeight = 0;

        // sum the height of each active child
        _verticalLayoutGroup.transform.ActionForEachChildren((GameObject child) =>
        {
            if (child.activeInHierarchy)
                totalHeight += child.GetComponent<RectTransform>().rect.height;
        });

        // add offset of the layout
        float delta = -_verticalLayoutGroup.offsetMax.y + _verticalLayoutGroup.offsetMin.y;
        totalHeight += delta;

        Debug.LogFormat("delta is min: {0} | max {1} => delta is {2}", _verticalLayoutGroup.offsetMax.y, _verticalLayoutGroup.offsetMin.y, delta);

        return totalHeight;
    }
}
