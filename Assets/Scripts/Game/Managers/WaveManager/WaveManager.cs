using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnWaveTimerUpdate(int waveCount, float remainingTime);

/// <summary>
/// Spawn enemies waves frequently.
/// </summary>
public class WaveManager : MonoBehaviour
{
    #region Fields
    public static event OnWaveTimerUpdate OnWaveTimerUpdate;

    [SerializeField] private WaveManagerData _data;

    private int _waveCount = 1;
    private float _timer = 0;
    #endregion

    #region Methods
    void Update()
    {
        int oldTimerInSeconds = (int)_timer;
        _timer += Time.deltaTime;

        float remainingTime = _data.TimerBetweenWavesInSeconds - _timer;

        // Manage OnWaveTimerUpdate trigger:
        // trigger each seconds
        if (oldTimerInSeconds < (int)_timer)
        {
            OnWaveTimerUpdate?.Invoke(_waveCount, remainingTime);
        }

        // manage wave triggering
        if (remainingTime <= 0)
        {
            TriggerWave();
        }
    }

    void TriggerWave()
    {
        Debug.Log("Triggering wave " + _waveCount);

        _timer = 0;
        _waveCount++;
    }
    #endregion
}
