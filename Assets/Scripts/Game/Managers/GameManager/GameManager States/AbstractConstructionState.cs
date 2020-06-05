using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Game.ConstructionSystem
{
    public abstract class AbstractConstructionState : OwnedState<GameManager>
    {
        #region Fields
        public static readonly string debugLogHeader = "Abstract Building State";
        public static readonly int terrainLayerMask = LayerMask.GetMask("Terrain");

        private string _entityID;
        private EntityData _entityData;

        private bool _sucessfulBuild = false;
        private bool _firstFrame = true;
        #endregion

        #region Properties
        private ResourcesWrapper CurrentBuildingCost
        {
            get
            {
                if (_entityData == null)
                    Debug.LogFormat("Can't get CurrentBuildingCost because _buildingData is null!");

                return _entityData.SpawningCost;
            }
        }

        protected bool SucessfulBuild { get => _sucessfulBuild; set => _sucessfulBuild = value; }
        protected EntityData EntityData { get => _entityData; }
        protected string EntityID { get => _entityID; }
        #endregion

        #region Methods
        public AbstractConstructionState(GameManager owner, string buildingID) : base(owner)
        {
            SetCurrentBuilding(buildingID);
        }

        #region Public override Methods
        public override void OnStateEnter()
        {
            _owner.Invoke_OnStartBuild();
        }

        public override void OnStateExit()
        {
            if (!_sucessfulBuild)
            {
                DestroyAndRefundBuilding();
            }
            else
            {
                _owner.Invoke_OnBuildSuccessful();
            }

            _owner.Invoke_OnStopBuild();
        }

        public override void Tick()
        {
            ProcessInputs();
            _firstFrame = false;
        }

        public virtual void OnGUI() { }
        #endregion

        #region Abstract methods
        protected abstract void ConstructBuilding();
        #endregion

        #region Protected Methods
        protected virtual void OnCurrentBuildingSet(string buildingID, EntityData buildingData) { }
        protected virtual void OnMouseDown() { }
        protected virtual void OnMouseUp() { }
        protected abstract void DestroyAllConstructionBuildings();
        protected abstract ResourcesWrapper GetConstructionCost();
        #endregion

        #region Private methods
        private void ProcessInputs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopConstructionAndRefund();
            }

            if (!_firstFrame && Input.GetMouseButtonDown(0))
            {

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // clicked on UI
                    _sucessfulBuild = false;
                    LeaveState();
                }
                else
                {
                    OnMouseDown();
                }
            }

            if (!_firstFrame && Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // clicked on UI
                    _sucessfulBuild = false;
                    LeaveState();
                }
                else
                {
                    OnMouseUp();
                }
            }
        }

        public void LeaveState()
        {
            _owner.State = null;
        }

        private void StopConstructionAndRefund()
        {
            DestroyAndRefundBuilding();
            _owner.State = null;
        }

        private void SetCurrentBuilding(string buildingID)
        {
            // try to get prefab for instantiation
            var buildingData = MainRegister.Instance.GetEntityData(buildingID);

            Assert.IsNotNull(buildingData,
                string.Format("Building State : can't SetCurrentBuilding because cannot get building data from MainRegister of {0}.", buildingID));

            _entityData = buildingData;
            _entityID = buildingID;
            _owner.Resources -= CurrentBuildingCost;

            OnCurrentBuildingSet(buildingID, buildingData);
        }

        private void DestroyAndRefundBuilding()
        {
            DestroyAllConstructionBuildings();
            _owner.Resources += GetConstructionCost();
        }
        #endregion
        #endregion
    }
}

