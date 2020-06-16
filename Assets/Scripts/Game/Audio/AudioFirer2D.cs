using Game.Entities;
using Game.Selection;
using Game.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// This script listen to game events to firer audio clip to AudioManager2D.    
    /// </summary>
    public class AudioFirer2D : MonoBehaviour
    {
        private AudioManager2D _audioManager;

        void Awake()
        {
            // we use 'FindObjectOfType' to avoid use of a Singleton
            _audioManager = FindObjectOfType<AudioManager2D>();
        }

        void OnEnable()
        {
            WaveManager.OnWaveStart += WaveManager_OnWaveStart;
            WaveManager.OnWaveClear += WaveManager_OnWaveClear;
            GameManager.OnGameResourcesUpdate += GameManager_OnGameResourcesUpdate;
            EntityUnitSpawner.OnUnitCreated += EntityUnitSpawner_OnUnitCreated;
            GameManager.OnBuildSuccessful += GameManager_OnBuildSuccessful;
            SelectionManager.OnSelectionUpdated += SelectionManager_OnSelectionUpdated;           
            SelectedGroupsActionsCaller.OnOrderGiven += SelectedGroupsActionsCaller_OnOrderGiven;
            SelectedGroupsActionsCaller.OnOrder_SetAnchorPosition += SelectedGroupsActionsCaller_OnOrder_SetAnchorPosition;

            SelectedGroupsActionsCaller.OnOrder_AttackUnit += SelectedGroupsActionsCaller_OnOrder_AttackUnit;
            SelectedGroupsActionsCaller.OnOrder_MoveToPosition += SelectedGroupsActionsCaller_OnOrder_MoveToPosition;
            SelectedGroupsActionsCaller.OnOrder_MoveAggressively += SelectedGroupsActionsCaller_OnOrder_MoveAggressively;
            SelectedGroupsActionsCaller.OnOrder_Patrol += SelectedGroupsActionsCaller_OnOrder_Patrol;
        }

        private void SelectedGroupsActionsCaller_OnOrder_Patrol(Vector3 targetPosition) => _audioManager.PlayRandomClip(Sound2D.OrderPatrol);

        private void SelectedGroupsActionsCaller_OnOrder_MoveToPosition(Vector3 destination) => _audioManager.PlayRandomClip(Sound2D.OrderMove);

        private void SelectedGroupsActionsCaller_OnOrder_MoveAggressively(Vector3 destination) => _audioManager.PlayRandomClip(Sound2D.OrderMoveAggressively);

        private void SelectedGroupsActionsCaller_OnOrder_AttackUnit(Entity target) => _audioManager.PlayRandomClip(Sound2D.OrderAttack);

        private void GameManager_OnBuildSuccessful(GameManager gameManager) => _audioManager.PlayOneShotRandomClip(Sound2D.SuccessfulBuilding);

        private void SelectedGroupsActionsCaller_OnOrderGiven() => _audioManager.PlayOneShotRandomClip(Sound2D.OrderGiven);

        private void EntityUnitSpawner_OnUnitCreated(Entity creator, Entity spawned) => _audioManager.PlayOneShotRandomClip(Sound2D.UnitCreated);

        private void GameManager_OnGameResourcesUpdate(ResourcesWrapper resources) => _audioManager.PlayOneShotRandomClip(Sound2D.NotEnoughResources);

        private void WaveManager_OnWaveClear(int waveCountCleared) => _audioManager.PlayRandomClip(Sound2D.WaveEnd);

        private void WaveManager_OnWaveStart(int waveCount) => _audioManager.PlayRandomClip(Sound2D.WaveStart);

        private void SelectionManager_OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
        {
            if (selectedGroups.Length == 0)
                return;

            _audioManager.PlayOneShotRandomClip(Sound2D.OnSelection);
        }

        // NOTE:
        // Because SelectedGroupsAction also call OrderGiver while calling this event,
        // We could have 2 sounds playing simultaneously
        //
        // To avoid that, in 'OnOrderGiven', we could check if the selection is only building.
        // If true, we don't play sounds.
        //
        // We could do this now, but I'd prefer wait for sounds implementation to test it.
        private void SelectedGroupsActionsCaller_OnOrder_SetAnchorPosition(Vector3 destination) => _audioManager.PlayOneShotRandomClip(Sound2D.OnSetAnchorPosition);
    }
}
