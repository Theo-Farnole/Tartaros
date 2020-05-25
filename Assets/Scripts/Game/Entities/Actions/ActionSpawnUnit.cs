using Game.Cheats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.IA.Action
{
    public class ActionSpawnUnit : Action
    {
        public float startTime;
        public float spawnTime;
        public readonly float creationDuration;

        public readonly string entityIDToSpawn;

        public ActionSpawnUnit(Entity owner, float creationDuration, string entityIDToSpawn) : base(owner)
        {
            this.entityIDToSpawn = entityIDToSpawn;

            float finalCreationDuration = 
                TartarosCheatsManager.IsCreationTimeToZeroActive() 
                ? 0 
                : creationDuration;

            this.creationDuration = finalCreationDuration;
        }

        public override void OnStateEnter()
        {
            startTime = Time.time;
            spawnTime = Time.time + creationDuration;
        }

        public override void Tick()
        {
            if (Time.time >= spawnTime)
            {
                SpawnUnit();
            }
        }

        private void SpawnUnit()
        {
            _owner.GetCharacterComponent<EntityUnitSpawner>().SpawnUnit(entityIDToSpawn);

            // leave action
            _owner.StopCurrentAction();
        }

        public float GetCompletion()
        {
            return (Time.time - startTime) / (spawnTime - startTime);
        }

        public float GetRemainingTime()
        {
            return spawnTime - Time.time;
        }

        public string RemainingTimeToString()
        {
            float remainingTime = GetRemainingTime();

            if (remainingTime >= 0)
            {
                return string.Format("{0:N0}", GetRemainingTime());
            }
            else
            {
                return creationDuration.ToString();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} spawns {1} in {2}.", _owner.name, entityIDToSpawn, RemainingTimeToString());
        }
    }
}
