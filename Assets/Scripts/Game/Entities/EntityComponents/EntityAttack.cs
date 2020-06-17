namespace Game.Entities
{
    using Game.Entities.Actions;
    using Lortedo.Utilities.Pattern;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public delegate void DelegateAttack(Entity attacker, Entity victim);

    /// <summary>
    /// This script manage the attack (smart classname isn't it ?).
    /// </summary>
    public partial class EntityAttack : AbstractEntityComponent
    {
        #region Fields
        [Tooltip("Used only if is attacker is an a distance attacker.")]
        [SerializeField] private Vector3 _throwProjectileLocalPosition = Vector3.zero;

        private float _attackTime = 0;
        #endregion

        #region Events
        public event DelegateAttack OnAttack;
        #endregion

        #region Properties
        public Vector3 ThrowProjectilePosition { get => transform.position + _throwProjectileLocalPosition; }
        #endregion

        #region Methods
        #region Mono Callbacks
        void Awake()
        {
            CalculateNewAttackTimer();
        }

        void OnEnable()
        {
            Entity.GetCharacterComponent<EntityDetection>().OnOpponentEnterViewRange += OnEnemyEnterViewRange;
            Entity.GetCharacterComponent<EntityHealth>().OnDamageReceived += OnDamageReceived;
        }

        void OnDisable()
        {
            Entity.GetCharacterComponent<EntityDetection>().OnOpponentEnterViewRange += OnEnemyEnterViewRange;
            Entity.GetCharacterComponent<EntityHealth>().OnDamageReceived -= OnDamageReceived;
        }
        #endregion

        #region Events Handlers
        private void OnEnemyEnterViewRange(Entity enemy)
        {
            if (Entity.IsIdle)
            {
                // auto attack the enemy
                ActionAttackEntity action = new ActionAttackEntity(Entity, enemy, true);
                Entity.SetAction(action);
            }
        }

        private void OnDamageReceived(Entity entity, Entity attacker, int currentHp, int damageAmount)
        {
            if (Entity.IsIdle)
            {
                // auto attack the enemy
                ActionAttackEntity action = new ActionAttackEntity(Entity, attacker, true);
                Entity.SetAction(action);
            }
        }
        #endregion

        #region Public methods
        public void DoAttack(Entity target)
        {
            if (!Entity.Data.CanAttack)
                return;

            Entity.GetCharacterComponent<EntityMovement>().SetAvoidance(Avoidance.Fight);

            if (Time.time < _attackTime)
                return;

            if (Entity.GetCharacterComponent<EntityDetection>().IsEntityInAttackRange(target))
            {
                CalculateNewAttackTimer();

                LookAtEntity(target);

                if (Entity.Data.IsMelee)
                {
                    target.GetCharacterComponent<EntityHealth>().GetDamage(Entity.Data.Damage, Entity);
                }
                else
                {
                    // prefab project not null
                    Assert.IsNotNull(Entity.Data.PrefabProjectile, string.Format("Entity Attack : Projectile set in EntityData is null for {0} of type {1}", name, Entity.EntityID));

                    GameObject gameObjectProjectile = ObjectPooler.Instance.SpawnFromPool(Entity.Data.PrefabProjectile, ThrowProjectilePosition, Quaternion.identity, true);

                    // projectile from pool not null
                    Assert.IsNotNull(gameObjectProjectile, string.Format("Entity Attack : Projectile '{0}' from object pooler is null.", Entity.Data.PrefabProjectile.name));

                    Projectile projectile = gameObjectProjectile.GetComponent<Projectile>();

                    // projectile from pool has Projectile component
                    Assert.IsNotNull(string.Format("Prefab projectile of {0} is missing Projectile component. Please, add one.", name));

                    projectile.Throw(target, Entity);

                }

                OnAttack?.Invoke(Entity, target);
            }
        }

        /// <summary>
        /// If an enemy is visible, set Entity action to ActionAttackEntity.
        /// </summary>
        /// <returns>Return true if an enemy has been founded</returns>
        public bool TryStartActionAttackNearestEnemy()
        {
            if (!Entity.Data.CanAttack)
                return false;

            var nearestEnemy = Entity.GetCharacterComponent<EntityDetection>().GetNearestOpponent();

            if (nearestEnemy == null)
                return false;

            if (nearestEnemy.Team == Entity.Team) Debug.LogWarningFormat("Entity Attack : Entity {0} tries to auto attack an ally.", name);

            if (Entity.GetCharacterComponent<EntityDetection>().IsEntityInViewRadius(nearestEnemy))
            {
                ActionAttackEntity action = new ActionAttackEntity(Entity, nearestEnemy, true);
                Entity.SetAction(action);

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Private methods
        void CalculateNewAttackTimer()
        {
            _attackTime = Time.time + 1 / Entity.Data.AttackPerSecond;
        }

        // REFACTOR NOTE:
        // Set this method into another EntityComponent (or create another)
        private void LookAtEntity(Entity target)
        {
            Vector3 lookPos = target.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }
        #endregion
        #endregion
    }

#if UNITY_EDITOR
    public partial class EntityAttack : AbstractEntityComponent
    {
        void OnDrawGizmos()
        {
            if (Entity.EntityID == string.Empty)
                return;

            if (Entity.Data.CanAttack && !Entity.Data.IsMelee)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(ThrowProjectilePosition, 0.1f);
            }
        }
    }
#endif
}
