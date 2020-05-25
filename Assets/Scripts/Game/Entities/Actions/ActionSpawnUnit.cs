using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.IA.Action
{
    public class ActionSpawnUnit : Action
    {
        public readonly float startTime;
        public readonly float spawnTime;

        public readonly string entityIDToSpawn;

        public ActionSpawnUnit(Entity owner, float creationDuration, string entityIDToSpawn) : base(owner)
        {
            startTime = Time.time;
            spawnTime = Time.time + creationDuration;

            this.entityIDToSpawn = entityIDToSpawn;
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
    }
}
