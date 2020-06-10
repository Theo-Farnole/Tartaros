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
        private List<Entity> _incomeGeneratorEntities = new List<Entity>();

        #region Methods
        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            //Entity.OnSpawn += Entity_OnSpawn;
            Entity.OnTeamSwap += Entity_OnTeamSwap;
            //Entity.OnDeath += Entity_OnDeath;
            EntityResourcesGeneration.OnResourceGenerationEnable += EntityResourcesGeneration_OnResourceGenerationEnable;
            EntityResourcesGeneration.OnResourceGenerationDisable += EntityResourcesGeneration_OnResourceGenerationDisable;
        }

        void OnDisable()
        {
            //Entity.OnSpawn -= Entity_OnSpawn;
            Entity.OnTeamSwap -= Entity_OnTeamSwap;
            //Entity.OnDeath -= Entity_OnDeath;
            EntityResourcesGeneration.OnResourceGenerationEnable  -= EntityResourcesGeneration_OnResourceGenerationEnable;
            EntityResourcesGeneration.OnResourceGenerationDisable -= EntityResourcesGeneration_OnResourceGenerationDisable;
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
            if (!_incomeGeneratorEntities.Contains(entity))
                return;

            if (!entity.Data.CanCreateResources)
                return;

            if (oldTeam != _teamToListen && newTeam == _teamToListen)
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

        private void EntityResourcesGeneration_OnResourceGenerationEnable(Entity entity)
        {
            if (entity.Data.CanCreateResources && entity.Team == _teamToListen)
            {
                AddEntity(entity);
            }
        }

        private void EntityResourcesGeneration_OnResourceGenerationDisable(Entity entity)
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

            _incomeGeneratorEntities.Add(entity);

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

            _incomeGeneratorEntities.Remove(entity);

            _currentIncome -= ResourcesWrapper.CrossProduct(entity.Data.ConstantResourcesGeneration, _incomeTick, entity.Data.GenerationTick);
            OnIncomeChanged?.Invoke(_currentIncome);
        }
        #endregion
        #endregion
    }
}
