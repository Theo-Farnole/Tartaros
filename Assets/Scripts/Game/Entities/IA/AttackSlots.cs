using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackSlots
{
    #region Class
    public class Slot
    {
        public bool assigned = false;
        public Vector3 localPosition = Vector3.zero;
    }
    #endregion

    #region Fields
    private Slot[] _slots;
    private Transform _transform;
    #endregion

    #region Properties
    private Slot[] AvailableSlots { get => (from x in _slots where x.assigned == true select x).ToArray(); }
    private Slot[] UnvailableSlots { get => (from x in _slots where x.assigned == false select x).ToArray(); }
    private Vector3[] LocalPositions { get => (from x in _slots select x.localPosition).ToArray(); }
    #endregion

    #region Methods
    public AttackSlots(Transform transform, float slotRange, float distanceBetweenSlot)
    {
        _transform = transform;
        CreateSlots(slotRange, distanceBetweenSlot);
    }

    private void CreateSlots(float slotRange, float distanceBetweenSlot)
    {
        // find angleInterval & anglesCount
        float hypotenuse = Mathf.Sqrt(slotRange * slotRange + distanceBetweenSlot * distanceBetweenSlot);
        float angleInterval = Mathf.Acos(slotRange / hypotenuse) * Mathf.Rad2Deg;
        int anglesCount = Mathf.RoundToInt(360 / angleInterval);

        // create array
        _slots = new Slot[anglesCount];

        // calculate localPosition of slots
        for (int i = 0; i < anglesCount; i++)
        {
            float angle = angleInterval * i * Mathf.Deg2Rad;

            _slots[i] = new Slot
            {
                assigned = false,
                localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * slotRange
            };
        }
    }

    public Slot AssignNearestSlot(Vector3 worldPosition)
    {
        return AssignSlot(GetNearestAvailableSlotIndex(worldPosition));
    }

    private Slot AssignSlot(int index)
    {
        // avoid out of bounds errors
        if (index < 0 || index >= _slots.Length)
            return null;

        // check if slot isn't already taken
        if (_slots[index].assigned)
            return null;

        _slots[index].assigned = true;

        return _slots[index];
    }

    public void ReleaseSlot(Slot slot)
    {
        ReleaseSlot(Array.IndexOf(_slots, slot));
    }

    private void ReleaseSlot(int index)
    {
        // avoid out of bounds errors
        if (index < 0 || index >= _slots.Length)
            return;

        // check if slot isn't already realse
        if (_slots[index].assigned == false)
            return;

        _slots[index].assigned = false;
    }

    private int GetNearestAvailableSlotIndex(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - _transform.position;

        Vector3[] localPositionAvailableSlots = (from x in _slots
                                                 where x.assigned == false
                                                 select x.localPosition).ToArray();

        Vector3 closestVector = localPosition.GetClosestVector(localPositionAvailableSlots);
        Slot closestSlot = _slots.First(x => x.localPosition == closestVector);

        return Array.IndexOf(_slots, closestSlot);
    }

    public Slot GetNearestSlot(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - _transform.position;

        Vector3[] localPositionSlots = (from x in _slots
                                        select x.localPosition).ToArray();

        Vector3 closestVector = localPosition.GetClosestVector(localPositionSlots);
        Slot closestSlot = _slots.FirstOrDefault(x => x.localPosition == closestVector);

        return closestSlot;
    }

    private Vector3 GetLocalPosition(int index)
    {
        // avoid out of bounds errors
        if (index < 0 || index >= _slots.Length)
            return Vector3.zero;

        return _slots[index].localPosition;
    }

    #region Debug Method
    /// <summary>
    /// Must be called in Gizmos.
    /// </summary>
    public void GizmosDrawSlots()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].assigned == true) Gizmos.color = Color.red;
            else Gizmos.color = Color.blue;

            Gizmos.DrawSphere(_slots[i].localPosition + _transform.position, 0.1f);
        }
    }
    #endregion
    #endregion
}
