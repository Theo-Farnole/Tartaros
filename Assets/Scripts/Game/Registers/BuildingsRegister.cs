using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Building
{
    House,
    Barracks
}

public class BuildingsRegister : Singleton<BuildingsRegister>
{
    [EnumNamedArray(typeof(Building))]
    [SerializeField] private GameObject[] _prefab;

    private Dictionary<Building, GameObject> _dictionnary = new Dictionary<Building, GameObject>();

    #region Methods
    void Awake()
    {
        for (int i = 0; i < _prefab.Length; i++)
        {
            Building index = (Building)i;
            _dictionnary.Add(index, _prefab[i]);
        }
    }

    public GameObject GetBuildingPrefab(Building buildingType)
    {
        if (_dictionnary.ContainsKey(buildingType) == false)
        {
            Debug.LogError(buildingType + " prefab isn't linked in BuildingRegister");
            return null;
        }

        return _dictionnary[buildingType];
    }
    #endregion
}
