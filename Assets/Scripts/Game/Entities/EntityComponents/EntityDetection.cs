using Lortedo.Utilities.Pattern;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
    public delegate void OnEntityDetected(Entity entity);

    /// <summary>
    /// This script manage the detection of nearby entities.
    /// </summary>
    public class EntityDetection : EntityComponent
    {
        #region Fields
        private const string debugLogHeader = "Entity Detection : ";
        public readonly static float DISTANCE_THRESHOLD = 0.3f;
        public readonly static int frameIntervalToCheckNearestEntities = 5;

        public event OnEntityDetected OnAllyEnterShiftRange;
        public event OnEntityDetected OnOpponentEnterAttackRange;

        [SerializeField] private EntityShiftData _shiftData;

        private Entity _nearestOpponentTeamEntity = null;
        private Entity _nearestAllyTeamEntity = null;

        private int _frameOffset = -1;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks   
        void Awake()
        {
            _frameOffset = UnityEngine.Random.Range(0, frameIntervalToCheckNearestEntities);
            EntitiesNeightboorManager.Initialize();
        }

        void Start()
        {
            if (!Entity.Data.CanDetectEntities)
            {
                enabled = false;
            }
        }

        void Update()
        {
            // avoid calculation every frame
            if ((Time.frameCount - _frameOffset) % frameIntervalToCheckNearestEntities == 0)
            {
                CalculateNearestAllyTeamEntity();
                CalculateNearestOpponentTeamEntity();
            }
        }

        void OnEnable()
        {
            Entity.OnDeath += Entity_OnDeath;
        }

        void OnDisable()
        {
            Entity.OnDeath -= Entity_OnDeath;
        }

        void OnDrawGizmosSelected()
        {
            Debug_DrawAttackRange();
        }
        #endregion

        #region Events Handlers
        private void Entity_OnDeath(Entity entity)
        {
            if (_nearestOpponentTeamEntity == entity)
            {
                CalculateNearestOpponentTeamEntity();

                if (_nearestOpponentTeamEntity == entity) Debug.LogWarningFormat("Nearest opponent {0} is dead. It's shouldn't be the _nearestOpponentTeamEntity!", entity.name);
            }

            if (_nearestAllyTeamEntity == entity)
            {
                CalculateNearestOpponentTeamEntity();

                if (_nearestAllyTeamEntity == entity) Debug.LogWarningFormat("Nearest ally {0} is dead. It's shouldn't be the _nearestAllyTeamEntity!", entity.name);
            }
        }
        #endregion

        #region Public methods
        public bool IsEntityInAttackRange(Entity target)
        {
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Data);
            Assert.IsNotNull(Entity);
            Assert.IsNotNull(Entity.Data);

            return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.AttackRadius + (target.Data.GetBiggerTileSize() + Entity.Data.GetBiggerTileSize()) / 2;
        }

        public bool IsEntityInShiftRange(Entity target)
        {
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.Data);
            Assert.IsNotNull(Entity);
            Assert.IsNotNull(Entity.Data);

            return Vector3.Distance(transform.position, target.transform.position) <= _shiftData.ShiftCollisionRadius + (target.Data.GetBiggerTileSize() + Entity.Data.GetBiggerTileSize()) / 2;
        }

        public bool IsNearFromEntity(Entity target)
        {
            return Vector3.Distance(transform.position, target.transform.position) <= DISTANCE_THRESHOLD + (target.Data.GetBiggerTileSize() + Entity.Data.GetBiggerTileSize()) / 2;
        }

        public bool IsNearFromPosition(Vector3 position)
        {
            return Vector3.Distance(transform.position, position) <= DISTANCE_THRESHOLD;
        }

        public Entity GetNearestOpponent()
        {
            TF.Assertations.Assert.IsTrue(Entity.Data.CanDetectEntities, "To get nearest opponent, you must tick 'Can detect entities' on ID '{0}'.", Entity.EntityID);

            if (_nearestOpponentTeamEntity != null && !_nearestOpponentTeamEntity.IsInstanciate)
                _nearestOpponentTeamEntity = null;

            return _nearestOpponentTeamEntity;
        }

        public Entity GetNearestAlly()
        {
            TF.Assertations.Assert.IsTrue(Entity.Data.CanDetectEntities, "To get nearest opponent, you must tick 'Can detect entities' on ID '{0}'.", Entity.EntityID);

            if (_nearestAllyTeamEntity != null && !_nearestAllyTeamEntity.IsInstanciate)
                _nearestAllyTeamEntity = null;

            return _nearestAllyTeamEntity;
        }

        public bool IsEntityInViewRadius(Entity target)
        {
            return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.ViewRadius;
        }

        #region Obsolete
        [Obsolete("Use GetNearestOpponent with 'IsInViewRadius' please.")]
        public Entity GetNearestOpponentInViewRadius()
        {
            if (_nearestOpponentTeamEntity.Team == Entity.Team) Debug.LogErrorFormat("Entity Detection : Nearest opponent has the same team of EntityDetection.");

            return _nearestOpponentTeamEntity;
        }

        [Obsolete("Use GetNearestOpponent with 'IsInViewRadius' please.")]
        public Entity GetNearestAllyInViewRadius()
        {
            return _nearestAllyTeamEntity;
        }
        #endregion
        #endregion

        #region Private Methods
        private void CalculateNearestOpponentTeamEntity()
        {
            _nearestOpponentTeamEntity = EntitiesNeightboorManager.GetClosestOpponentEntity(transform.position, Entity.Team);

            if (_nearestOpponentTeamEntity != null && IsEntityInAttackRange(_nearestOpponentTeamEntity))
            {
                OnOpponentEnterAttackRange?.Invoke(_nearestOpponentTeamEntity);
            }
        }

        private void CalculateNearestAllyTeamEntity()
        {
            _nearestAllyTeamEntity = EntitiesNeightboorManager.GetClosestAllyEntity(transform.position, Entity.Team);

            if (_nearestAllyTeamEntity != null && IsEntityInShiftRange(_nearestAllyTeamEntity))
            {
                OnAllyEnterShiftRange?.Invoke(_nearestAllyTeamEntity);
            }
        }

        [Obsolete("Main behaviour in this method has been commented.")]
        private void Debug_DrawAttackRange()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;

            if (Entity.EntityID == string.Empty)
                return;

            Assert.IsNotNull(Entity);
            Assert.IsNotNull(Entity.Data);

            //UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, Entity.Data.AttackRadius + Entity.Data.GetBiggerTileSize());
#endif
        }
        #endregion
        #endregion
    }
}
