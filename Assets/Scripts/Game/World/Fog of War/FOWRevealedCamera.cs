using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    public class FOWRevealedCamera : MonoBehaviour
    {
        [SerializeField] Material _projectorMaterial;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderTexture r = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            Graphics.Blit(source, r);
            Graphics.Blit(r, destination);

            //_projectorMaterial.SetTexture("_ReavaledRenderTexture", r);
            RenderTexture.ReleaseTemporary(r);
        }
    }
}
