namespace Game.Entities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EntitySound : AbstractEntityComponent
    {
        #region Methods
        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            GetCharacterComponent<EntityAttack>().OnAttack += EntitySound_OnAttack;
            Entity.OnDeath += Entity_OnDeath;
        }

        void OnDisable()
        {
            GetCharacterComponent<EntityAttack>().OnAttack -= EntitySound_OnAttack;
            Entity.OnDeath -= Entity_OnDeath;
        }
        #endregion

        #region Events Handlers
        private void EntitySound_OnAttack(Entity attacker, Entity victim) => PlayAudioClipAtPosition(Entity.Data.GetRandomAttackSound());

        private void Entity_OnDeath(Entity entity)
        {
            if (entity == Entity)
            {
                PlayAudioClipAtPosition(Entity.Data.GetRandomDeathSound());
            }
        }
        #endregion

        #region Private Methods
        private void PlayAudioClipAtPosition(AudioClip audioClip) => AudioSource.PlayClipAtPoint(audioClip, transform.position);
        #endregion
        #endregion
    }
}
