using Game.IA.Action;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnUnitCreated(Entity creator, Entity spawned);

public class EntityUnitSpawner : EntityComponent
{
    #region Fields
    public static event OnUnitCreated OnUnitCreated;

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
        if (_modelAnchorPoint != null)
            _modelAnchorPoint.transform.position = _anchorPosition;
    }
    #endregion

    public bool CanSpawnEntity(string entityID, bool logErrors)
    {
        if (!Entity.Data.CanSpawnUnit)
        {
            Debug.LogWarningFormat("You must tick 'can spawn unit' on database " + Entity.EntityID + ".");
        }

        if (!Entity.Data.AvailableUnitsForCreation.Contains(entityID))
        {
            Debug.LogWarningFormat(debugLogHeader + "Can't create {0} because it's not inside _creatableUnits of {1}.", entityID, name);
        }

        Assert.IsNotNull(GameManager.Instance, "GameManager is missing. Can't spawn unit");
        Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing. Can't spawn unit");

        var unitData = MainRegister.Instance.GetEntityData(entityID);
        Assert.IsNotNull(unitData, string.Format(debugLogHeader + "Entity Unit Spawn could find EntityData of {0}. Aborting method.", entityID));

        if (!GameManager.Instance.HasEnoughtPopulationToSpawn())
        {
            if (logErrors) UIMessagesLogger.Instance.AddErrorMessage(string.Format("Build more house to create {0} unit.", entityID));
            return false;
        }

        if (!GameManager.Instance.Resources.HasEnoughResources(unitData.SpawningCost))
        {
            if (logErrors) UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enought resources to create " + entityID + ".");
            return false;
        }

        return true;
    }

    public Entity SpawnUnit(string unitID)
    {
        Assert.IsNotNull(GameManager.Instance, "GameManager is missing. Can't spawn unit");
        Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing. Can't spawn unit");

        var unitData = MainRegister.Instance.GetEntityData(unitID);

        Assert.IsNotNull(unitData, string.Format(debugLogHeader + "Entity Unit Spawn could find EntityData of {0}. Aborting method.", unitID));

        // remove resources
        GameManager.Instance.Resources -= unitData.SpawningCost;

        var instantiatedObject = ObjectPooler.Instance.SpawnFromPool(unitData.Prefab, transform.position, Quaternion.identity, true);
        var instanciatedEntity = instantiatedObject.GetComponent<Entity>();

        Assert.IsNotNull(instanciatedEntity, string.Format("Prefab {0} miss an Entity component.", unitData.Prefab.name));

        instanciatedEntity.Team = Entity.Team;
        MoveGameObjectToAnchor(instanciatedEntity);

        OnUnitCreated?.Invoke(Entity, instanciatedEntity);

        return instanciatedEntity;
    }

    private void MoveGameObjectToAnchor(Entity entity)
    {
        Action moveToAnchorAction = new ActionMoveToPosition(entity, _anchorPosition);
        entity.SetAction(moveToAnchorAction);
    }
    #endregion
}
