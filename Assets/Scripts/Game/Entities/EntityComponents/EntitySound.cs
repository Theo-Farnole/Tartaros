namespace Game.Entities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EntitySound : AbstractEntityComponent
    {
        void OnEnable()
        {
            GetCharacterComponent<EntityAttack>().OnAttack += EntitySound_OnAttack;   
        }

        void OnDisable()
        {
            GetCharacterComponent<EntityAttack>().OnAttack -= EntitySound_OnAttack;
        }

        private void EntitySound_OnAttack(Entity attacker, Entity victim) => AudioSource.PlayClipAtPoint(Entity.Data.GetRandomAttackSound(), transform.position);
    }
}
