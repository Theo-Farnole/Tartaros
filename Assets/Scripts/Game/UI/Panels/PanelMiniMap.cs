using Game.WaveSystem;
using Lortedo.Utilities.Managers;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class PanelMiniMap : Panel
    {
        private List<GameObject> _wavesIndicators = new List<GameObject>();

        //public override void Initialize<T>(T uiManager)
        //{
        //    base.Initialize(uiManager);

        //    UpdateWavesIndicators();
        //}

        //public override void SubscribeToEvents<T>(T uiManager)
        //{
        //    base.SubscribeToEvents(uiManager);

        //    WaveManager.OnWaveClear += WaveManager_OnWaveClear;
        //}

        //public override void UnsubscribeToEvents<T>(T uiManager)
        //{
        //    base.UnsubscribeToEvents(uiManager);
        //    WaveManager.OnWaveClear -= WaveManager_OnWaveClear;
        //}

        //private void WaveManager_OnWaveClear(int waveCountCleared)
        //{
        //    UpdateWavesIndicators();
        //}

        //void UpdateWavesIndicators()
        //{
        //    foreach (var arrows in _wavesIndicators)
        //    {

        //    }
        //}

        //void CreateNeedWavesIndicators()
        //{
        //    // make sure to destroy old waves indicator
        //    DestroyWavesIndicators();

        //    var waveManager = Object.FindObjectOfType<WaveManager>();

        //    Assert.IsNotNull(waveManager, "WaveManager missing.");

        //    var nonEmptyWaveSpawnPoints = WaveSpawnPoint.GetAllNonEmptyWaveSpawnPoints(waveManager.WaveCount);

        //    foreach (var waveSpawnPoint in nonEmptyWaveSpawnPoints)
        //    {
        //        CreateWaveIndicator(waveSpawnPoint);
        //    }
        //}

        //void CreateWaveIndicator(WaveSpawnPoint waveSpawnPoint)
        //{
        //    ObjectPooler.Instance.SpawnFromPool(ObjectPoolingTags.keyUIWavesIndicators)
        //}

        //void DestroyWavesIndicators()
        //{
        //    foreach (var arrows in _wavesIndicators)
        //    {
        //        ObjectPooler.Instance.EnqueueGameObject(ObjectPoolingTags.keyUIWavesIndicators, arrows);
        //    }

        //    _wavesIndicators.Clear();
        //}        
    }
}
