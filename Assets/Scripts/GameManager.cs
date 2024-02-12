using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int wavesCount = 5;
    [SerializeField] private float initialComplexity = 20;
    [SerializeField] private float complexityMultiplier = 1.5f;
    
    [SerializeField] private List<Enemy> enemies;
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
    
    private struct Range
    {
        public float Min;
        public float Max;
    }

    private void SpawnEnemies(float targetComplexity)
    {
        while (targetComplexity > 0)
        {
            SpawnRandomEnemy(targetComplexity * 2, out var complexity);
            targetComplexity -= complexity;
        }
    }

    private void SpawnRandomEnemy(float maxComplexity, out float complexity)
    {
        var probabilities = new Dictionary<Enemy, Range>();
        var sum = 0f;
        foreach (var candidate in enemies)
        {
            if (candidate.Complexity > maxComplexity) continue;
            
            var prob = GetEnemyProbability(candidate);
            probabilities.Add(candidate, new Range { Min = sum, Max = sum + prob });
            sum += prob;
        }

        complexity = 0;
        var rand = Random.Range(0f, sum);
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var candidate in probabilities)
        {
            if (rand < candidate.Value.Min || rand > candidate.Value.Max) continue;
            StartCoroutine(SpawnEnemy(candidate.Key));
            complexity = candidate.Key.Complexity;
            return;
        }
    }

    private static float GetEnemyProbability(Enemy enemy) => 1f / enemy.Complexity;

    private IEnumerator LaunchChallenge()
    {
        _currentWave = -1;
        while (_currentWave + 1 < wavesCount)
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
        SpawnEnemies(initialComplexity * MathF.Pow(complexityMultiplier, _currentWave));
    }

    private IEnumerator SpawnEnemy(Enemy enemy)
    {
        _enemiesAlive += 1;
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