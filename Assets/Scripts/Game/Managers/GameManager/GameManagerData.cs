using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/System/Game Manager")]
public class GameManagerData : ScriptableObject
{
    [SerializeField] private ResourcesWrapper _startingResources = new ResourcesWrapper(10, 5, 0);
    [SerializeField] private int _startMaxPopulationCount = 10;
    [Header("Victory")]
    [SerializeField] private int _wavesPassedToWin = 1;

    public ResourcesWrapper StartingResources { get => _startingResources; }
    public int StartMaxPopulationCount { get => _startMaxPopulationCount; }
    public int WavesPassedToWin { get => _wavesPassedToWin; }
}
