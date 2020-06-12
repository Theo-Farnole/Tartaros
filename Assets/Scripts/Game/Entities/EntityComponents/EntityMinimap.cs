namespace Game.Entities
{
    using UnityEngine;
    using UnityEngine.Assertions;

    public class EntityMinimap : EntityComponent
    {
        [SerializeField] private SpriteRenderer _minimapSprite;
        [SerializeField] private Color[] _teamColor;

        // Called from Entity.SetupTeamComponents()
        public void UpdatePointColor()
        {
            var material = GetMaterial(Entity.Team);

            _minimapSprite.color = material;
        }

        Color GetMaterial(Team team)
        {
            int index = (int)team;

            Assert.IsTrue(_teamColor.IsIndexInsideBounds(index));

            return _teamColor[index];
        }
    }
}
