using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/Cost/Units Creation")]
public class UnitCreationData : ScriptableObject
{
    [SerializeField] private Unit _type;
    public Unit Type { get => _type; }

    [SerializeField] private ResourcesWrapper _cost;
    public ResourcesWrapper Cost { get => _cost; }
}
