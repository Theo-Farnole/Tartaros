using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    [CreateAssetMenu(menuName = "Leonidas Legacy/Register")]
    public class RegisterData : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private UnitData _entityData;
        [Space]
        [SerializeField] private Sprite _portrait;
        [SerializeField] private KeyCode _hotkey;

        public GameObject Prefab { get => _prefab; }
        public Sprite Portrait { get => _portrait; }
        public UnitData EntityData { get => _entityData; }
        public KeyCode Hotkey { get => _hotkey; }
    }
}
