using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/GameManager")]
public class GameManagerData : ScriptableObject
{
    [SerializeField] private ResourcesWrapper _startingResources = new ResourcesWrapper(10, 5, 0);
    public ResourcesWrapper StartingResources { get => _startingResources; }
}
