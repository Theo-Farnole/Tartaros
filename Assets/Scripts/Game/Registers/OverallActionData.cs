using System.Collections;
using System.Collections.Generic;
using UI.Game.HoverPopup;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/Overall Action")]
public class OverallActionData : ScriptableObject
{
    [SerializeField] private Sprite _portrait;
    [SerializeField] private KeyCode _hotkey;
    [Space]
    [SerializeField] private HoverPopupInfo _hoverPopupData;

    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public HoverPopupInfo HoverPopupData { get => _hoverPopupData; }
}
