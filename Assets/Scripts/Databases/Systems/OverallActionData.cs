using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/Overall Action")]
public class OverallActionData : ScriptableObject
{
    [SerializeField] private Sprite _portrait;
    [SerializeField] private KeyCode _hotkey;

    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
}
