namespace Game.Entities
{
    using Game.Entities.Actions;
    using UnityEngine;

    public class EntityShift : AbstractEntityComponent
    {
        #region Fields
        [SerializeField] private EntityShiftData _shiftData;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            Entity.GetCharacterComponent<EntityDetection>().OnAllyEnterShiftRange += EntityShift_OnAllyEnterShiftRange;
        }

        private void OnDisable()
        {
            Entity.GetCharacterComponent<EntityDetection>().OnAllyEnterShiftRange -= EntityShift_OnAllyEnterShiftRange;
        }
        #endregion

        #region Events Handlers
        private void EntityShift_OnAllyEnterShiftRange(Entity ally)
        {
            // Make the entity shift to let the hitEntity walks throught the crowd
            if (Entity.IsIdle && !ally.IsIdle)
            {
                Shift(ally);
            }
        }
        #endregion

        #region Private MEthods
        private void Shift(Entity hitEntity)
        {
            Vector3 fleeHitEntityDirection = Quaternion.Euler(0, -90, 0) * -Lortedo.Utilities.Math.Direction(Entity.transform.position, hitEntity.transform.position);

            ActionMoveToPosition action = new ActionMoveToPosition(Entity, transform.position + fleeHitEntityDirection * _shiftData.ShiftLength);

            Entity.SetAction(action, false);
        }
        #endregion
        #endregion
    }
}
