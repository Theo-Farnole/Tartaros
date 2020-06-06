using Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PopulationManager : MonoBehaviour
{
    #region Fields
    private const string debugLogHeader = "Population Manager : ";

    public static event DoubleIntDelegate OnPopulationCountChanged;

    private int _populationCount = 0;
    private int _maxPopulation = 0;

    private int _startPopulation = 0;
    #endregion

    #region Properties
    public int PopulationCount
    {
        get => _populationCount;

        set
        {
            _populationCount = value;
            OnPopulationCountChanged?.Invoke(_populationCount, _maxPopulation);

            LookForErrorsInPopCounts();
        }
    }

    public int MaxPopulation
    {
        get => _maxPopulation;

        set
        {
            _maxPopulation = value;
            OnPopulationCountChanged?.Invoke(_populationCount, _maxPopulation);

            LookForErrorsInPopCounts();
        }
    }

    public int StartPopulation
    {
        get => _startPopulation;
        set
        {
            var difference = value - _startPopulation;

            _startPopulation = value;
            MaxPopulation += difference;
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        Entity.OnSpawn += Entity_OnSpawn;
        Entity.OnDeath += Entity_OnDeath;
        Entity.OnTeamSwap += Entity_OnTeamSwap;
    }

    void OnDisable()
    {
        Entity.OnSpawn -= Entity_OnSpawn;
        Entity.OnDeath -= Entity_OnDeath;
        Entity.OnTeamSwap -= Entity_OnTeamSwap;
    }
    #endregion

    #region Events Handlers
    private void Entity_OnSpawn(Entity entity)
    {
        if (entity.Team == Team.Player)
        {
            PopulationCount += entity.Data.PopulationUse;
            MaxPopulation += entity.Data.IncreaseMaxPopulationAmount;
        }
    }

    private void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
    {
        if (!entity.IsSpawned && !entity.IsInstanciate)
            return;

        // leave player team
        if (IsLeavingPlayerTeam(oldTeam, newTeam))
        {
            PopulationCount -= entity.Data.PopulationUse;
            MaxPopulation -= entity.Data.IncreaseMaxPopulationAmount;
        }
        else if (IsJoiningPlayerTeam(oldTeam, newTeam))
        {
            PopulationCount += entity.Data.PopulationUse;
            MaxPopulation += entity.Data.IncreaseMaxPopulationAmount;
        }
    }

    private void Entity_OnDeath(Entity entity)
    {
        if (entity.Team == Team.Player)
        {
            PopulationCount -= entity.Data.PopulationUse;
            MaxPopulation -= entity.Data.IncreaseMaxPopulationAmount;
        }
    }
    #endregion

    #region Public methods    
    public bool HasEnoughtPopulationToSpawn()
    {
        return _populationCount <= _maxPopulation;
    }

    public bool HasEnoughtPopulationToSpawn(EntityData unitData)
    {
        return (_populationCount + unitData.PopulationUse <= _maxPopulation);
    }
    #endregion

    #region Look for Errors Methods
    private void LookForErrorsInPopCounts()
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // On start, if there is already created Entity in scene assert fails.
        //
        // ex: 
        // There 3 entities in the scene.
        // The first entity send OnSpawn. The population equals 1.
        // Then the second. The population equals 2.
        // However because there is 2 entities, GetCurrentPopulation returns directly 2.            
        // So, at the first entity's OnSpawn 1 != 2. 
        // To avoid that, we add delay
        //
        // We could have used frameCount. But there is not 'Time.frameSinceLevelLoad'
        if (Time.timeSinceLevelLoad < 0.2f)
            return;

        if (_populationCount > _maxPopulation)
            Debug.LogWarningFormat(debugLogHeader + "Current population count is above max population.\nCurrent: {0} >= MaxPop: {1}", _populationCount, _maxPopulation);

        if (GetCurrentPopulation() != _populationCount)
            Debug.LogErrorFormat(debugLogHeader + "Current population isn't the same as calculated.\nCurrent: {0} != Calculated: {1}", _populationCount, GetCurrentPopulation());

        if (GetCurrentMaxPopulation() != _maxPopulation)
            Debug.LogErrorFormat(debugLogHeader + "Max population isn't the same as calculated.\nCurrent: {0} != Calculated: {1}", _maxPopulation, GetCurrentMaxPopulation());
#endif
    }
    #endregion

    #region Getter / Calculate methods
    private static bool IsLeavingPlayerTeam(Team oldTeam, Team currentTeam)
    {
        return currentTeam != Team.Player && oldTeam == Team.Player;
    }

    private static bool IsJoiningPlayerTeam(Team oldTeam, Team currentTeam)
    {
        return currentTeam == Team.Player && oldTeam != Team.Player;
    }

    int GetCurrentPopulation()
    {
        var entities = FindObjectsOfType<Entity>();
        int populationUsage = 0;

        foreach (var entity in entities)
        {
            if (entity.Team == Team.Player && entity.IsSpawned)
            {
                populationUsage += entity.Data.PopulationUse;
            }
        }

        foreach (var pendingEntity in GameManager.Instance.PendingCreation)
        {
            populationUsage += MainRegister.Instance.GetEntityData(pendingEntity).PopulationUse;         
        }

        return populationUsage;
    }

    int GetCurrentMaxPopulation()
    {
        var entities = FindObjectsOfType<Entity>();
        int maxPopulation = _startPopulation;

        foreach (var entity in entities)
        {
            if (entity.Team == Team.Player)
            {
                maxPopulation += entity.Data.IncreaseMaxPopulationAmount;
            }
        }

        return maxPopulation;
    }
    #endregion
    #endregion
}
