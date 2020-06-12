namespace Game.Entities
{
    using Game.Entities.Actions;
    using Lortedo.Utilities.Pattern;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Assertions;

    public delegate void OnUnitCreated(Entity creator, Entity spawned);

    public partial class EntityUnitSpawner : EntityComponent
    {
        #region Fields

        private static readonly string debugLogHeader = "Entity Unit Spawn : ";

        [SerializeField] private Vector3 _spawnPointLocal;

        /// <summary>
        /// The anchor is the little flag that said to player "my spawned units'll go there".
        /// </summary>
        private GameObject _modelAnchorPoint;
        private Vector3 _anchorPosition;
        #endregion

        #region Events
        public static event OnUnitCreated OnUnitCreated;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            _anchorPosition = transform.position + transform.forward * 1f;
        }

        void OnEnable()
        {
            if (Entity.Data.CanSpawnUnit)
            {
                Entity.GetCharacterComponent<EntitySelectable>().OnSelected += EntityUnitSpawner_OnSelected;
                Entity.GetCharacterComponent<EntitySelectable>().OnUnselected += EntityUnitSpawner_OnUnselected; ;
            }
        }

        void OnDisable()
        {
            if (Entity.Data.CanSpawnUnit)
            {
                Entity.GetCharacterComponent<EntitySelectable>().OnSelected -= EntityUnitSpawner_OnSelected;
                Entity.GetCharacterComponent<EntitySelectable>().OnUnselected -= EntityUnitSpawner_OnUnselected; ;
            }
        }
        #endregion

        #region Events Handlers    
        private void EntityUnitSpawner_OnSelected(Entity entity)
        {
            DisplayAnchorPoint();
        }

        private void EntityUnitSpawner_OnUnselected(Entity entity)
        {
            HideAnchorPoint();
        }
        #endregion

        #region Public Methods
        public void SetAnchorPosition(Vector3 anchorPosition)
        {
            _anchorPosition = anchorPosition;
            UpdateAnchorPosition();
        }

        public bool CanSpawnEntity(string entityID, bool logErrors)
        {
            if (!Entity.Data.CanSpawnUnit)
            {
                Debug.LogErrorFormat("You must tick 'can spawn unit' on database " + Entity.EntityID + ".");
                return false;
            }

            if (!DoSpawnConditionAreMet(entityID))
            {
                return false;
            }

            Assert.IsNotNull(GameManager.Instance, "GameManager is missing. Can't spawn unit");

            return GameManager.Instance.CanSpawnEntity(entityID, logErrors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        public Entity SpawnUnit(string unitID)
        {
            Assert.IsNotNull(GameManager.Instance, "GameManager is missing. Can't spawn unit");

            Entity spawnedEntity = GameManager.Instance.SpawnEntity(unitID, GetSpawnPoint(), Quaternion.identity, Entity.Team);

            MoveEntityToAnchor(spawnedEntity);

            OnUnitCreated?.Invoke(Entity, spawnedEntity);

            return spawnedEntity;
        }
        #endregion

        #region Private Methods
        private bool DoSpawnConditionAreMet(string entityID)
        {
            UnitSpawnCondition spawnCondition = Entity.Data.GetSpawnCondition(entityID);

            if (spawnCondition == null)
            {
                Debug.LogErrorFormat(debugLogHeader + "Can't create {0} because it's not inside _creatableUnits of {1}.", entityID, name);
                return false;
            }

            if (spawnCondition != null && !spawnCondition.DoConditionsAreMet())
                return false;

            return true;
        }

        void DisplayAnchorPoint()
        {
            Assert.IsTrue(Entity.Data.CanSpawnUnit, "Can't display anchor point of a unit that can't spawn unit.");

            // Model anchor is already displayed.
            if (_modelAnchorPoint != null)
                return;

            _modelAnchorPoint = ObjectPooler.Instance.SpawnFromPool(ObjectPoolingTags.keyAnchorPoint, _anchorPosition, Quaternion.identity);
        }

        void HideAnchorPoint()
        {
            Assert.IsTrue(Entity.Data.CanSpawnUnit, "Can't hide anchor point of a unit that can't spawn unit.");

            // Model is already hided.
            if (_modelAnchorPoint == null)
                return;

            ObjectPooler.Instance.EnqueueGameObject(ObjectPoolingTags.keyAnchorPoint, _modelAnchorPoint);
            _modelAnchorPoint = null;
        }

        void UpdateAnchorPosition()
        {
            if (_modelAnchorPoint == null)
                return;

            _modelAnchorPoint.transform.position = _anchorPosition;
        }

        private void MoveEntityToAnchor(Entity entity)
        {
            Action moveToAnchorAction = new ActionMoveToPosition(entity, _anchorPosition);
            entity.SetAction(moveToAnchorAction);
        }

        private Vector3 GetSpawnPoint()
        {
            return transform.position + _spawnPointLocal;
        }
        #endregion
        #endregion
    }

#if UNITY_EDITOR
    public partial class EntityUnitSpawner : EntityComponent
    {
        [SerializeField] private Color _spawnPointColor = Color.cyan;

        void OnDrawGizmosSelected()
        {
            Vector3 spawnPoint = GetSpawnPoint();

            Gizmos.color = _spawnPointColor;
            Gizmos.DrawWireSphere(spawnPoint, 0.1f);
        }
    }
#endif
}
