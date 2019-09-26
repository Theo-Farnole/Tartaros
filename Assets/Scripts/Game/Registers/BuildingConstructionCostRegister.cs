using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingConstructionCostRegister : Register<ResourcesWrapperDatabase, Building>
{
    [EnumNamedArray(typeof(Building))]
    [SerializeField] private ResourcesWrapperDatabase[] _constructionCost;

    protected override ResourcesWrapperDatabase[] Prefabs { get => _constructionCost; set => _constructionCost = value; }
    protected override int DeltaIndex { get => EntitySystem.STARTING_INDEX_BUILDING; }
}
