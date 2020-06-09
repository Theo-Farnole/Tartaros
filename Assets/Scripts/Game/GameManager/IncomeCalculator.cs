using Game.Entities;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Used in PanelGameResources to display current income.
    /// </summary>
    public class IncomeCalculator : MonoBehaviour
    {
        public event Action<ResourcesWrapper> OnIncomeChanged;

        [SerializeField] private float _incomeTick = 10f; // give the income made in 10 seconds
        [SerializeField] private Team _teamToListen = Team.Player;

        private ResourcesWrapper _currentIncome = new ResourcesWrapper();

        #region Methods
        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            Entity.OnSpawn += Entity_OnSpawn;
            Entity.OnTeamSwap += Entity_OnTeamSwap;
            Entity.OnDeath += Entity_OnDeath;
        }

        void OnDisable()
        {
            Entity.OnSpawn -= Entity_OnSpawn;
            Entity.OnTeamSwap -= Entity_OnTeamSwap;
            Entity.OnDeath -= Entity_OnDeath;
        }
        #endregion

        #region Events Handlers
        private void Entity_OnSpawn(Entity entity)
        {
            if (entity.Data.CanCreateResources && entity.Team == _teamToListen)
            {
                AddEntity(entity);
            }
        }

        private void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
        {
            if (!entity.Data.CanCreateResources)
                return;

            if (oldTeam == Team.Enemy && newTeam == Team.Player)
            {
                AddEntity(entity);
            }
            else
            {
                RemoveEntity(entity);
            }
        }

        private void Entity_OnDeath(Entity entity)
        {
            if (entity.Data.CanCreateResources && entity.Team == _teamToListen)
            {
                RemoveEntity(entity);
            }
        }
        #endregion

        #region Private Methods
        private void AddEntity(Entity entity)
        {
            if (!entity.Data.CanCreateResources)
                return;

            if (entity.Data.GenerationType == GenerationType.PerCell)
            {
                Debug.LogErrorFormat("Income Calculator : Can't calculate income of building '{0}' because its generation is 'PerCell'.", entity.EntityID);
                return;
            }

            _currentIncome += ResourcesWrapper.CrossProduct(entity.Data.ConstantResourcesGeneration, _incomeTick, entity.Data.GenerationTick);
            OnIncomeChanged?.Invoke(_currentIncome);
        }

        private void RemoveEntity(Entity entity)
        {
            if (!entity.Data.CanCreateResources)
                return;

            if (entity.Data.GenerationType == GenerationType.PerCell)
            {
                Debug.LogErrorFormat("Income Calculator : Can't calculate income of building '{0}' because its generation is 'PerCell'.", entity.EntityID);
                return;
            }

            _currentIncome -= ResourcesWrapper.CrossProduct(entity.Data.ConstantResourcesGeneration, _incomeTick, entity.Data.GenerationTick);
            OnIncomeChanged?.Invoke(_currentIncome);
        }
        #endregion
        #endregion
    }
}
