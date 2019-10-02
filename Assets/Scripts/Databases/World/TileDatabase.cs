using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/World/Tile")]
public class TileDatabase : ScriptableObject
{
    [SerializeField, Range(1, 101)] private int _cellCount = 51;
    public int CellCount { get => _cellCount; }

    [SerializeField] private float _cellSize = 1f;
    public float CellSize { get => _cellSize; }

    public void OnValidate()
    {
        // is _cellCount even ?
        if (_cellCount % 2 == 0)
        {
            _cellCount++;
        }
    }
}
