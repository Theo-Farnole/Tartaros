using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/System/Game Manager")]
public class GameManagerData : ScriptableObject
{
    [SerializeField] private ResourcesWrapper _startingResources = new ResourcesWrapper(10, 5, 0);
    [SerializeField] private int _startMaxPopulationCount = 10;

    [Header("CONSTRUCTION")]
    [SerializeField] private string[] _IDInPanelConstruction = new string[0];

    [Header("VICTORY CONDITION")]
    [SerializeField] private int _wavesPassedToWin = 1;

    [Header("LOSE CONDITIONS")]
    [SerializeField] private string _entityIDToAttack = "building_temple";

    public ResourcesWrapper StartingResources { get => _startingResources; }
    public int StartMaxPopulationCount { get => _startMaxPopulationCount; }
    public int WavesPassedToWin { get => _wavesPassedToWin; }
    public string[] IDsInPanelConstruction { get => _IDInPanelConstruction; }
    public string EntityIDToAttack { get => _entityIDToAttack; }
}
