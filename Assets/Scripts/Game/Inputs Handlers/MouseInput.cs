using Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inputs
{
    [System.Flags]
    public enum MouseLayer
    {
        Entity,
        Terrain
    }

    /// <summary>
    /// Manage raycast from mouse.
    /// We regroup mouse raycast here, if we find a way to detect click on entities without raycast.
    /// That'll make the changement easier.
    /// </summary>
    public static class MouseInput
    {
        private readonly static int layerMaskEntity = LayerMask.GetMask("Entity");
        private readonly static int layerMaskTerrain = LayerMask.GetMask("Terrain");

        public static bool IsMouseOver(out RaycastHit hit, MouseLayer layerFlags, float distance = Mathf.Infinity)
        {
            // PERFORMANCE NOTE:
            // Avoid usage of Camera.main
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            return Physics.Raycast(ray, out hit, distance, GetLayerMask(layerFlags));
        }

        public static bool GetEntityUnderMouse(out Entity entity, float distance = Mathf.Infinity)
        {
            // PERFORMANCE NOTE:
            // Avoid usage of Camera.main
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMaskEntity))
            {
                entity = hit.collider.GetComponent<Entity>();
                return true;
            }
            else
            {
                entity = null;
                return false;
            }
        }

        private static int GetLayerMask(MouseLayer layer)
        {
            int o = 0;

            switch (layer)
            {
                case MouseLayer.Entity:
                    o |= layerMaskEntity;
                    break;

                case MouseLayer.Terrain:
                    o |= layerMaskTerrain;
                    break;
            }

            return o;
        }
    }
}