using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private WaveManagerData _data;

    private int _waveCount = 1;
    private float _timer = 0;
    #endregion

    #region Properties
    private float RemainingTime
    {
        get
        {
            return _data.TimerBetweenWavesInSeconds - _timer;
        }
    }
    #endregion

    #region Methods
    void Update()
    {
        _timer += Time.deltaTime;

        UIManager.Instance.SetWaveText(_waveCount, RemainingTime);

        if (RemainingTime <= 0)
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
