namespace Game.Appearance
{
    using Lortedo.Utilities.Pattern;
    using Sirenix.OdinInspector;
    using UnityEngine;

    /// <summary>
    /// Gives the user a feedback about if the current building is on a free tile or not.
    /// Use to apply transparency and changes color of mesh when the building is being builded.
    /// </summary>
    public class BuildingMesh : MonoBehaviour, IPooledObject
    {
        public enum State
        {
            CanBuild,
            CannotBuild,
            NotInBuildState
        }

        #region Fields
        private static readonly int SHADERID_ISBUILDING = Shader.PropertyToID("_IsBuilding");
        private static readonly int SHADERID_CANBUILD = Shader.PropertyToID("_CanBuild");

        [SerializeField] private MeshRenderer[] _meshes = new MeshRenderer[0];
        #endregion

        #region Properties
        public string ObjectTag { get; set; }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            OnObjectSpawn();
        }
        #endregion

        #region Public Methods
        public void OnObjectSpawn()
        {
            CheckForColorsErrors();
        }

        public void SetState(State state)
        {
            float isBuilding = IsBuilding(state) ? 1 : 0;
            float canBuild = CanBuild(state) ? 1 : 0;

            foreach (var mesh in _meshes)
            {
                if (mesh == null)
                {
                    Debug.LogWarningFormat("BuildingMesh: '{0}' has empty mesh. Consider using button to refresh the MeshRenderer list.", name);
                    continue;
                }
                mesh.material.SetFloat(SHADERID_ISBUILDING, isBuilding);
                mesh.material.SetFloat(SHADERID_CANBUILD, canBuild);
            }
        }
        #endregion

        #region Private Methods
        private bool IsBuilding(State state)
        {
            switch (state)
            {
                case State.CanBuild:
                case State.CannotBuild:
                    return true;

                case State.NotInBuildState:
                    return false;

                default:
                    throw new System.NotImplementedException();
            }
        }

        [Button("Get MeshRenderer in children")]
        public void GetMeshRenderersInChild()
        {
            _meshes = GetComponentsInChildren<MeshRenderer>();
        }

        private bool CanBuild(State state)
        {
            switch (state)
            {
                case State.CanBuild:
                    return true;

                case State.CannotBuild:
                case State.NotInBuildState:
                    return false;

                default:
                    throw new System.NotImplementedException();
            }
        }

        private void CheckForColorsErrors()
        {
            if (_meshes.Length == 0)
            {
                Debug.LogWarningFormat("Building Mesh : " + "There is no mesh assigned to BuildingMesh {0}.", name);
            }
        }
        #endregion
        #endregion
    }
}