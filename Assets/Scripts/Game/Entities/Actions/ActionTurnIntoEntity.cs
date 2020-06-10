using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Actions
{
    /// <summary>
    /// Replace the entity by another entity.
    /// </summary>
    public class ActionTurnIntoEntity : Action
    {
        public readonly string _entityIDToTurnInto;

        private bool _successfulSpawnUnit = false; // allow returning true of CanExecuteAction()
        private bool _isInToPendingCreation = false;
        private bool _doPendingCreationFailed = false;

        public ActionTurnIntoEntity(Entity owner, string entityID) : base(owner)
        {
            _entityIDToTurnInto = entityID;

            if (CanExecuteAction())
            {
                _doPendingCreationFailed = false;
                SetInPendingCreation();
            }
            else
            {
                _doPendingCreationFailed = true;
            }
        }

        ~ActionTurnIntoEntity()
        {
            RemoveFromPendingCreation();
        }

        public override bool CanExecuteAction()
        {
            return !_doPendingCreationFailed && 
                (_isInToPendingCreation || _successfulSpawnUnit || _owner.GetCharacterComponent<EntityUnitSpawner>().CanSpawnEntity(_entityIDToTurnInto, true));
        }

        public override void Tick()
        {
            Debug.Log("Tick action turn into");
            TurnIntoEntity();
        }

        public override string ToString()
        {
            return string.Format("{0} turns into {1}.", _owner.name, _entityIDToTurnInto);
        }

        void TurnIntoEntity()
        {
            _successfulSpawnUnit = true;

            // spawn new entity
            var entity = _owner.GetCharacterComponent<EntityUnitSpawner>().SpawnUnit(_entityIDToTurnInto);

            entity.StopEveryActions(); // stop move to anchor action
            entity.transform.position = _owner.transform.position;
            entity.transform.rotation = _owner.transform.rotation;

            // then kill the current entity
            _owner.Death();
            _owner.StopEveryActions();

            if (entity.Data.EntityType == EntityType.Building)
            {
                TileSystem.Instance.SetTile(entity.gameObject, entity.Data.TileSize);
            }
        }

        private void SetInPendingCreation()
        {
            if (_isInToPendingCreation)
                return;

            GameManager.Instance.AddPendingCreationEntity(_entityIDToTurnInto);
            _isInToPendingCreation = true;
        }

        private void RemoveFromPendingCreation()
        {
            if (!_isInToPendingCreation)
                return;

            GameManager.Instance.RemovePendingCreationEntity(_entityIDToTurnInto);
            _isInToPendingCreation = false;
        }
    }
}
