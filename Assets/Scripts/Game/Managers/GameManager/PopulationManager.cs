using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PopulationManager : MonoBehaviour
{
    #region Fields
    public static event DoubleIntDelegate OnPopulationCountChanged;

    private int _populationCount = 0;
    private int _maxPopulation = 0;

    private int _startPopulation = 0;
    #endregion

    #region Properties
    public int PopulationCount
    {
        get => _populationCount;

        private set
        {
            _populationCount = value;
            OnPopulationCountChanged?.Invoke(_populationCount, _maxPopulation);
        }
    }

    public int MaxPopulation
    {
        get => _maxPopulation;

        private set
        {
            _maxPopulation = value;
            OnPopulationCountChanged?.Invoke(_populationCount, _maxPopulation);
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
            if (Time.timeSinceLevelLoad > 0.2f)
            {
                Assert.AreEqual(_populationCount, GetCurrentPopulation(), "Game Manager : Current population isn't the same as calculated.");
                Assert.AreEqual(_maxPopulation, GetCurrentMaxPopulation(), "Game Manager : Max population isn't the same as calculated.");
            }
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

        Assert.AreEqual(_populationCount, GetCurrentPopulation(), "Game Manager : Current population isn't the same as calculated.");
        Assert.AreEqual(_maxPopulation, GetCurrentMaxPopulation(), "Game Manager : Max population isn't the same as calculated.");
    }

    private void Entity_OnDeath(Entity entity)
    {
        if (entity.Team == Team.Player)
        {
            PopulationCount -= entity.Data.PopulationUse;
            MaxPopulation -= entity.Data.IncreaseMaxPopulationAmount;

            Assert.AreEqual(_populationCount, GetCurrentPopulation(), "Game Manager : Current population isn't the same as calculated.");
            Assert.AreEqual(_maxPopulation, GetCurrentMaxPopulation(), "Game Manager : Max population isn't the same as calculated.");
        }
    }
    #endregion

    #region Public methods
    public bool HasEnoughtPopulationToSpawn(EntityData unitData)
    {
        return (_populationCount + unitData.PopulationUse <= _maxPopulation);
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
