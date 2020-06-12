using Game.UI.HoverPopup;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderContent
{
    #region Fields
    [SerializeField] private KeyCode _hotkey;
    [SerializeField] private Sprite _portrait;
    [SerializeField] private HoverPopupData _hoverPopupData;
    [SerializeField] private int _linePosition;

    private bool _enabled = true;
    private Action _onClick;
    #endregion

    #region Properties
    public Action OnClick { get => _onClick; set => _onClick = value; }
    public HoverPopupData HoverPopupData { get => _hoverPopupData; }
    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public int LinePosition { get => _linePosition; }
    public bool Enabled { get => _enabled; set => _enabled = value; }
    #endregion

    #region ctor
    public OrderContent(KeyCode hotkey, Sprite portrait, HoverPopupData hoverPopupData, Action onClick, int linePosition, bool enabled = true)
    {
        _hotkey = hotkey;
        _portrait = portrait;
        _hoverPopupData = hoverPopupData;
        _onClick = onClick;
        _linePosition = linePosition;
        _enabled = enabled;
    }
    #endregion
}
