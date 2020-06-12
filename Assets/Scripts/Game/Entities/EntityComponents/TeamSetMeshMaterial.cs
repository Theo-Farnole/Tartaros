namespace Game.Entities
{
    using UnityEngine;
    using UnityEngine.Assertions;

    // we don't set 'TeamSetMeshMaterial ' as EntityComponent.
    // If we do, we should put this component in the same object of Entity component.
    // But we want to set this script on every MeshRenderer we have in our object.
    public class TeamSetMeshMaterial : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Entity _meshOwner;
        [SerializeField] private Material[] _materials;
        [SerializeField] private MeshRenderer meshRenderer;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            UpdateMeshColor();
        }

        void OnEnable()
        {
            Entity.OnTeamSwap += Entity_OnTeamSwap;
        }

        void OnDisable()
        {
            Entity.OnTeamSwap -= Entity_OnTeamSwap;
        }
        #endregion

        #region Events Handlers
        private void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
        {
            Assert.IsNotNull(_meshOwner, string.Format("_owner field is missing in {0} inspector", name));

            if (entity == _meshOwner)
                UpdateMeshColor();
        }
        #endregion

        #region Private Methods
        void UpdateMeshColor()
        {
            Assert.IsNotNull(_meshOwner, string.Format("_owner field is missing in {0} inspector", name));
            Assert.IsNotNull(meshRenderer);

            var team = _meshOwner.Team;

            var materialIndex = (int)team;
            Assert.IsTrue(_materials.IsIndexInsideBounds(materialIndex), string.Format("Material of team '{0}' is missing in {1}'s inspector.", team, name));

            var material = _materials[materialIndex];
            meshRenderer.material = material;
        }
        #endregion
        #endregion
    }
}
