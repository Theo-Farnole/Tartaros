using Game.UI.HoverPopup;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderContent
{
    private KeyCode _hotkey;
    private Sprite _portrait;
    private HoverPopupData _hoverPopupData;

    private Action _onClick;

    private int _linePosition;

    public Action OnClick { get => _onClick; }
    public HoverPopupData HoverPopupData { get => _hoverPopupData; }
    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public int LinePosition { get => _linePosition; }

    public OrderContent(KeyCode hotkey, Sprite portrait, HoverPopupData hoverPopupData, Action onClick, int linePosition)
    {
        _hotkey = hotkey;
        _portrait = portrait;
        _hoverPopupData = hoverPopupData;
        _onClick = onClick;
        _linePosition = linePosition;
    }
}
