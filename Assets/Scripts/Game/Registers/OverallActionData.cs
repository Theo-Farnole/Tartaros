using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Game.UI.HoverPopup;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/System/Overall Action")]
public class OverallActionData : ScriptableObject
{
    [SerializeField] private Sprite _portrait;
    [SerializeField] private KeyCode _hotkey;
    [Space]
    [InlineEditor]
    [SerializeField] private HoverPopupData _hoverPopupData;

    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public HoverPopupData HoverPopupData { get => _hoverPopupData; }
}
