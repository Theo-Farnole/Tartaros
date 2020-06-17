namespace Game.Entities
{
    using Lortedo.Utilities;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Shorthand to get entities with specific conditions (eg. viewport)
    /// </summary>
    public static class EntitiesGetter
    {
        public static Entity[] GetUnitsInCameraViewport(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            EntitySelectable[] unitsSelectable = Object.FindObjectsOfType<EntitySelectable>();
            Camera camera = Camera.main;
            Bounds viewportBounds = GUIRectDrawer.GetViewportBounds(camera, screenPosition1, Input.mousePosition);

            Entity[] entitiesInViewport = unitsSelectable
                .Where(x => IsWithinSelectionBounds(camera, viewportBounds, x.gameObject)) // is in rectangle
                .Select(x => x.GetComponent<Entity>())
                .Where(x => x != null && x.Data.EntityType == EntityType.Unit)
                .ToArray();

            return entitiesInViewport;
        }

        public static bool IsWithinSelectionBounds(Camera camera, Bounds viewportBounds, GameObject gameObject)
        {
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }
    }
}
