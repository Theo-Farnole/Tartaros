using Game.IA.Action;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EntityUnitSpawner : EntityComponent
{
    #region Fields
    private static readonly string debugLogHeader = "Entity Unit Spawn : ";

    private Vector3 _anchorPosition;
    private GameObject _modelAnchorPoint;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _anchorPosition = transform.position + transform.forward * 1f;
    }

    void OnEnable()
    {
        if (Entity.Data.CanSpawnUnit)
        {
            Entity.GetCharacterComponent<EntitySelectable>().OnSelected += EntityUnitSpawner_OnSelected;
            Entity.GetCharacterComponent<EntitySelectable>().OnUnselected += EntityUnitSpawner_OnUnselected; ;
        }
    }

    void OnDisable()
    {
        if (Entity.Data.CanSpawnUnit)
        {
            Entity.GetCharacterComponent<EntitySelectable>().OnSelected -= EntityUnitSpawner_OnSelected;
            Entity.GetCharacterComponent<EntitySelectable>().OnUnselected -= EntityUnitSpawner_OnUnselected; ;
        }
    }
    #endregion

    #region Events Handlers    
    private void EntityUnitSpawner_OnSelected(Entity entity)
    {
        DisplayAnchorPoint();
    }

    private void EntityUnitSpawner_OnUnselected(Entity entity)
    {
        HideAnchorPoint();
    }
    #endregion

    public void SetAnchorPosition(Vector3 anchorPosition)
    {
        _anchorPosition = anchorPosition;
        UpdateAnchorPosition();
    }

    #region Anchor Point Model
    void DisplayAnchorPoint()
    {
        Assert.IsNull(_modelAnchorPoint, "Model anchor is already displayed.");
        Assert.IsTrue(Entity.Data.CanSpawnUnit, "Can't display anchor point of a unit that can't spawn unit.");

        _modelAnchorPoint = ObjectPooler.Instance.SpawnFromPool(ObjectPoolingTags.keyAnchorPoint, _anchorPosition, Quaternion.identity);
    }

    void HideAnchorPoint()
    {
        Assert.IsNotNull(_modelAnchorPoint, "Can't enqueue null _modelAnchorPoint. Call your coder please.");
        Assert.IsTrue(Entity.Data.CanSpawnUnit, "Can't hide anchor point of a unit that can't spawn unit.");

        ObjectPooler.Instance.EnqueueGameObject(ObjectPoolingTags.keyAnchorPoint, _modelAnchorPoint);
        _modelAnchorPoint = null;
    }

    void UpdateAnchorPosition()
    {
        _modelAnchorPoint.transform.position = _anchorPosition;
    }
    #endregion

    public void SpawnUnit(string unitID)
    {
        Assert.IsTrue(Entity.Data.CanSpawnUnit, "You must tick 'can spawn unit' on database " + Entity.EntityID + ".");
        Assert.IsTrue(Entity.Data.AvailableUnitsForCreation.Contains(unitID), string.Format(debugLogHeader + "Can't create {0} because it's not inside _creatableUnits of {1}.", unitID, name));
        Assert.IsNotNull(GameManager.Instance, "GameManager is missing. Can't spawn unit");

        var unitData = MainRegister.Instance.GetEntityData(unitID);

        Assert.IsNotNull(unitData, string.Format(debugLogHeader + "Entity Unit Spawn could find EntityData of {0}. Aborting method.", unitID));

        if (!GameManager.Instance.HasEnoughtPopulationToSpawn(unitData))
        {
            UIMessagesLogger.Instance.AddErrorMessage(string.Format("Build more house to create {0} unit.", unitID));
            return;
        }

        if (!GameManager.Instance.Resources.HasEnoughResources(unitData.SpawningCost))
        {
            UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enought resources to create " + unitID + ".");
            return;
        }

        // remove resources
        GameManager.Instance.Resources -= unitData.SpawningCost;

        var instantiatedObject = Instantiate(unitData.Prefab, transform.position, Quaternion.identity);
        MoveGameObjectToAnchor(instantiatedObject);
    }

    private void MoveGameObjectToAnchor(GameObject instantiatedObject)
    {
        if (instantiatedObject.TryGetComponent(out Entity entity))
        {
            Action moveToAnchorAction = new ActionMoveToPosition(entity, _anchorPosition);
            entity.SetAction(moveToAnchorAction);
        }
        else
        {
            Debug.LogErrorFormat(debugLogHeader + "Can't make {0} go to anchor point, because it has not Entity component.", name);
        }
    }
    #endregion
}
