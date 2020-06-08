using Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Selection
{

    [CreateAssetMenu(menuName = "Tartaros/Selection/Circle Color", fileName = "DB_Selection_CircleColor")]
    public class SelectionCircleColorData : ScriptableObject
    {
        [SerializeField] private Color _teamPlayerColor;
        [SerializeField] private Color _teamEnemyColor;

        public Color GetColor(Team team)
        {
            switch (team)
            {
                case Team.Player:
                    return _teamPlayerColor;

                case Team.Enemy:
                    return _teamEnemyColor;

                default:
                    throw new System.NotSupportedException("Value '" + team + "' not supported.");
            }
        }
    }
}