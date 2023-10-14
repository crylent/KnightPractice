using System;
using System.Collections;
using Enemies;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Challenge challenge;
    [SerializeField] private Portal portalPrefab;
    [SerializeField] private Rect spawnArea;
    [SerializeField] private Animator canvas;

    private PlayerInput _playerInput;

    private int _currentWave = -1;
    private long _enemiesAlive;
    private bool _gameIsOn;
    private static readonly int OnGameStartedTrigger = Animator.StringToHash("onGameStarted");
    private static readonly int OnGameFinishedTrigger = Animator.StringToHash("onGameFinished");
    private static readonly int OnDeathTrigger = Animator.StringToHash("onDeath");

    private void Start()
    {
        _playerInput = PlayerComponents.Object.GetComponent<PlayerInput>();
    }

    public void StartGame()
    {
        ResetScene();
        _gameIsOn = true;
        _playerInput.SwitchCurrentActionMap("gameplay");
        canvas.SetTrigger(OnGameStartedTrigger);
        StartCoroutine(LaunchChallenge());
    }

    public void StopGame(bool death)
    {
        if (!_gameIsOn) return;
        _gameIsOn = false;
        _playerInput.SwitchCurrentActionMap("menu");
        canvas.SetTrigger(death ? OnDeathTrigger : OnGameFinishedTrigger);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private static void ResetScene()
    {
        PlayerComponents.Controller.ResetPlayer();
        foreach (var entity in FindObjectsOfType<GameEntity>())
        {
            if (entity is not PlayerController) Destroy(entity.gameObject);
        }
    }

    private IEnumerator LaunchChallenge()
    {
        _currentWave = -1;
        while (_currentWave + 1 < challenge.WavesCount)
        {
            CallNextWave();
            yield return new WaitWhile(() => _enemiesAlive > 0 && _gameIsOn);
            if (!_gameIsOn) yield break;
        }
        StopGame(false);
    }

    private void CallNextWave()
    {
        _currentWave += 1;
        var enemiesToSpawn = challenge.Waves[_currentWave].enemies;
        _enemiesAlive += enemiesToSpawn.Count;
        foreach (var enemy in enemiesToSpawn)
        {
            StartCoroutine(SpawnEnemy(enemy));
        }
    }

    private IEnumerator SpawnEnemy(Enemy enemy)
    {
        var portal = Instantiate(
            portalPrefab, 
            CustomRandom.GetPosition(spawnArea) + portalPrefab.transform.position,
            Quaternion.identity
            );
        portal.enemyToSpawn = enemy;
        yield return new WaitUntil(() => portal.SpawnedEnemy);
        var spawnedEnemy = portal.SpawnedEnemy;

        yield return new WaitUntil(() => spawnedEnemy.IsDestroyed());
        _enemiesAlive -= 1;
    }
}