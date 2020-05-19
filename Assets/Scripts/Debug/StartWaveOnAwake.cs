using Game.Cheats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWaveOnAwake : MonoBehaviour
{
    [SerializeField] public float delay = 0.5f;

    void Awake()
    {
        this.ExecuteAfterTime(delay, StartWave);
    }

    private static void StartWave()
    {
        TartarosCheatsManager cheatManager = FindObjectOfType<TartarosCheatsManager>();
        cheatManager.StartWave();
    }
}
