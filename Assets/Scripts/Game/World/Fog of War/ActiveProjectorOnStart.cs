using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    public class ActiveProjectorOnStart : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Projector>().enabled = true;
        }
    }
}
