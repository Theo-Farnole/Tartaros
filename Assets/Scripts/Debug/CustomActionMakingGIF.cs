using Game.Entities;
using Game.Entities.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomActionMakingGIF : MonoBehaviour
{
    [SerializeField] private float _destinationOffsetZ = 10;
    [SerializeField] private KeyCode _input = KeyCode.E;
    [SerializeField] private bool _playOnStart = true;

    private void Start()
    {
        if (_playOnStart)
        {
            MakeEntitiesGreatAgain();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(_input))
        {
            MakeEntitiesGreatAgain();
        }
    }

    private void MakeEntitiesGreatAgain()
    {
        //Debug.Log("MakeEntitiesGreatAgain, attack'll work at " + System.DateTime.Now.AddSeconds(ActionMoveToPositionAggressively.DELAY).ToLongTimeString());

        Entity[] entities = FindObjectsOfType<Entity>();

        int length = entities.Length;
        for (int i = 0; i < length; i++)
        {
            Entity entity = entities[i];

            // calculate position
            Vector3 position = entity.transform.position + GetOffset(entity.Team);

            // apply move to position method
            ActionMoveToPositionAggressively action = new ActionMoveToPositionAggressively(entity, position);
            entity.SetAction(action);
        }
    }

    Vector3 GetOffset(Team team)
    {
        switch (team)
        {
            case Team.Player:
                return _destinationOffsetZ * Vector3.forward;

            case Team.Enemy:
                return -_destinationOffsetZ * Vector3.forward;

            default:
                throw new System.NotImplementedException();
        }
    }
}