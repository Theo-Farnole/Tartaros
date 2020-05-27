using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TeamSetMeshMaterial : MonoBehaviour
{
    [SerializeField] private Entity _meshOwner;

    [SerializeField] private Material[] _materials;

    [SerializeField] private MeshRenderer meshRenderer;

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


    private void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
    {
        Assert.IsNotNull(_meshOwner, string.Format("_owner field is missing in {0} inspector", name));

        if (entity == _meshOwner)
            UpdateMeshColor();
    }

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
}
