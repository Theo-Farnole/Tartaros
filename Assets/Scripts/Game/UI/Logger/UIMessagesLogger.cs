using Lortedo.Utilities;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Display messages in UI.
/// </summary>
public class UIMessagesLogger : Singleton<UIMessagesLogger>
{
    [SerializeField] private Transform _parentErrors;
    [Space]
    [SerializeField] private Color _color = Color.red;
    [SerializeField] private float _lifetime = 3;
    [SerializeField] private float _fadeDuration = 1;

    void Start()
    {
        // sometime developer disable UI by deactivating the canvas
        // however, for performance reason, we just disable the Canvas component
        // so, we assert that our gameobject isn't disabled
        if (_parentErrors.gameObject.activeInHierarchy)
        {
            _parentErrors.parent.gameObject.SetActive(true);
        }
    }

    public void LogErrorFormat(string text, string arg)
    {
        LogError(string.Format(text, arg));
    }

    public void LogErrorFormat(string text, params string[] args)
    {
        LogError(string.Format(text, args));
    }

    public void LogError(string text)
    {
        // we use instantiate method to avoid need to reset scale
        Transform errorTransform = new GameObject().transform;
        errorTransform.SetParent(_parentErrors, false);
        errorTransform.SetSiblingIndex(0);

        TextMeshProUGUI errorText = errorTransform.gameObject.AddComponent<TextMeshProUGUI>();
        errorText.text = text;
        errorText.color = _color;
        errorText.fontSize = 30;
        errorText.enableWordWrapping = false;

        float timeBeforeFade = _lifetime - _fadeDuration;

        this.ExecuteAfterTime(timeBeforeFade, () => StartFade(errorText));
    }

    void StartFade(TextMeshProUGUI errorText)
    {
        new Timer(this, _fadeDuration,

        (float completion) =>
        {
            errorText.alpha = 1 - completion;
        },

        () =>
        {
            Destroy(errorText.gameObject);
        });
    }
}
