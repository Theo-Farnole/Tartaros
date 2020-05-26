using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnWaveTimerUpdate(int waveCount, float remainingTime);
public delegate void OnWaveStart(int waveCount);
public delegate void OnWaveClear(int waveCountCleared);

/// <summary>
/// Spawn enemies waves frequently.
/// </summary>
public class WaveManager : MonoBehaviour
{
    #region Fields
    private const string debugLogHeader = "WaveManager : ";

    public static event OnWaveTimerUpdate OnWaveTimerUpdate;
    public static event OnWaveStart OnWaveStart;
    public static event OnWaveClear OnWaveClear;

    [SerializeField] private WaveManagerData _data;

    private int _waveCount = 0;
    private float _timer = 0;
    private int _lastFrame_TimerInSeconds;

    private bool _isInWaveSpawning = false;
    #endregion

    #region Properties
    public int WaveCount { get => _waveCount; }
    #endregion

    #region Methods
    void Start()
    {
        Assert.AreEqual(FindObjectsOfType<WaveManager>().Length, 1, "There is more or less than only 1 WaveManager on Scene.");
    }

    void Update()
    {
        IncreaseTimer(Time.deltaTime);

        float remainingTime = CalculateRemainingTime();

        TryTrigger_OnWaveTimerUpdate(remainingTime);
        TryTrigger_Wave(remainingTime);

        SetLastFrame_TimerInSeconds();
    }

    private void IncreaseTimer(float deltaTime)
    {
        _timer += deltaTime;
    }

    private void SetLastFrame_TimerInSeconds()
    {
        _lastFrame_TimerInSeconds = (int)_timer;
    }

    #region Trigger events
    private void TryTrigger_Wave(float remainingTime)
    {
        if (_isInWaveSpawning)
            return;

        if (remainingTime <= 0)
        {
            StartWave();
        }
    }

    private void TryTrigger_OnWaveTimerUpdate(float remainingTime)
    {
        if (_isInWaveSpawning)
            return;

        // trigger each seconds
        if (_lastFrame_TimerInSeconds < (int)_timer)
        {
            OnWaveTimerUpdate?.Invoke(_waveCount, remainingTime);
        }
    }
    #endregion

    void StartWave()
    {
        _isInWaveSpawning = true;
        _waveCount++;

        OnWaveStart?.Invoke(_waveCount);

        Debug.LogFormat(debugLogHeader + "Wave {0} starts.", _waveCount);

        if (_data.TimeBeforeStartNewWave > 0) this.ExecuteAfterTime(_data.TimeBeforeStartNewWave, EndWave);
        else EndWave();
    }

    void EndWave()
    {
        _isInWaveSpawning = false;

        // reset timer
        _timer = 0;

        // TODO:
        // we should check if all waves has been spawned
        OnWaveClear?.Invoke(_waveCount);
    }

    private float CalculateRemainingTime()
    {
        return _data.TimerBetweenWavesInSeconds - _timer;
    }
    #endregion
}
