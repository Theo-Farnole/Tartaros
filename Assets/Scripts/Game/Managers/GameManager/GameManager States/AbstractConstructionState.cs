using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.ConstructionSystem
{
    public abstract class AbstractConstructionState : OwnedState<GameManager>
    {
        #region Fields
        public static readonly string debugLogHeader = "Abstract Building State";
        public static readonly int terrainLayerMask = LayerMask.GetMask("Terrain");

        private BuildingType _buildingType;
        private EntityData _buildingData;

        private bool _sucessfulBuild = false;
        private bool _firstFrame = true;
        #endregion

        #region Properties
        private ResourcesWrapper CurrentBuildingCost
        {
            get
            {
                if (_buildingData == null)
                    Debug.LogFormat("Can't get CurrentBuildingCost because _buildingData is null!");

                return _buildingData.SpawningCost;
            }
        }

        protected bool SucessfulBuild { get => _sucessfulBuild; set => _sucessfulBuild = value; }
        #endregion

        #region Methods
        public AbstractConstructionState(GameManager owner, BuildingType buildingType) : base(owner)
        {
            SetCurrentBuilding(buildingType);
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

            _owner.Invoke_OnStopBuild();
        }

        public override void Tick()
        {
            ProcessInputs();
            _firstFrame = false;
        }
        #endregion

        #region Abstract methods
        protected abstract void ConstructBuilding();
        #endregion

        #region Protected Methods
        protected virtual void OnCurrentBuildingSet(BuildingType buildingType, EntityData buildingData) { }
        protected virtual void OnMouseDown() { }
        protected virtual void OnMouseUp() { }
        protected abstract void DestroyConstructionBuilding();
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
                OnMouseDown();
            }

            if (!_firstFrame && Input.GetMouseButtonUp(0))
            {
                OnMouseUp();                
            }
        }

        private void StopConstructionAndRefund()
        {
            DestroyAndRefundBuilding();
            _owner.State = null;
        }        

        private void SetCurrentBuilding(BuildingType buildingType)
        {
            // try to get prefab for instantiation
            if (MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData buildingData))
            {
                _buildingData = buildingData;
                _buildingType = buildingType;
                _owner.Resources -= CurrentBuildingCost;

                OnCurrentBuildingSet(buildingType, buildingData);                
            }
            else
            {
                Debug.LogErrorFormat("Building State : can't SetCurrentBuilding because cannot get building data from MainRegister of {0}.", buildingType);
            }
        }

        private void DestroyAndRefundBuilding()
        {
            DestroyConstructionBuilding();            
            _owner.Resources += CurrentBuildingCost;
        }
        #endregion
        #endregion
    }
}

