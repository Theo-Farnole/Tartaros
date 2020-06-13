namespace Game.Entities
{
    using Sirenix.OdinInspector;
    using System;
    using System.Linq;
    using UnityEngine;

    [Serializable]
    public class UnitSpawnCondition
    {
        [SerializeField] private string _entityIDToSpawn = string.Empty;

        [ToggleGroup(nameof(_hasSpawnCondition))]
        [SerializeField] private bool _hasSpawnCondition = false;

        [ToggleGroup(nameof(_hasSpawnCondition))]
        [SerializeField] private int _maxAlliesOfSameIDAlive = -1;

        public string EntityIDToSpawn { get => _entityIDToSpawn; }

        /// <summary>
        /// Warning, this method call 'FindObjectOfTypes'. It can be performance heavy.
        /// </summary>
        /// <returns></returns>
        public bool DoConditionsAreMet()
        {
            if (!_hasSpawnCondition)
                return true;

            return IsCondition_MaxAlliesOfSameIDAliveMet();
        }

        private bool IsCondition_MaxAlliesOfSameIDAliveMet()
        {
            if (_maxAlliesOfSameIDAlive == -1)
                return true;

            // PERFORMANCE NOTE:
            // Create an EntityManager where it store every Entity[]
            Entity[] entities = UnityEngine.Object.FindObjectsOfType<Entity>();


            int alliesOfSameIDAlive =
                entities.Where(x => x.EntityID == _entityIDToSpawn).Count()
                + GameManager.Instance.PendingCreation.Where(x => x == _entityIDToSpawn).Count();

            return alliesOfSameIDAlive < _maxAlliesOfSameIDAlive;
        }
    }
}
