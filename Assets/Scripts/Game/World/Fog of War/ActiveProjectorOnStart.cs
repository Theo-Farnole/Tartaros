using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FogOfWar
{
    public class ActiveProjectorOnStart : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Projector>().enabled = true;
        }
    }
}
