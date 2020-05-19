using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FogOfWar
{
    public class ActiveProjectorOnAwake : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Projector>().enabled = true;
        }
    }
}
