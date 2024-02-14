using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Player;
using PowerUps;
using UI;
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
    [SerializeField] private OptionsLayout optionsLayout;
    [SerializeField] private PowerUpsSet powerUps;
    [SerializeField] private int numberOfOptionsForPowerUp = 3;

    private PlayerInput _playerInput;

    private int _currentWave = -1;
    private long _enemiesAlive;
    private bool _gameIsOn;
    private static readonly int OnGameStartedTrigger = Animator.StringToHash("onGameStarted");
    private static readonly int OnGameFinishedTrigger = Animator.StringToHash("onGameFinished");
    private static readonly int OnDeathTrigger = Animator.StringToHash("onDeath");
    private static readonly int LevelUpScreenBool = Animator.StringToHash("levelUpScreen");

    private float _minEnemiesComplexity;

    private void Start()
    {
        _playerInput = PlayerComponents.Object.GetComponent<PlayerInput>();
        _minEnemiesComplexity = enemies.Min(enemy => enemy.Complexity);
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
        _uniquePowerUpsUsed.Clear();
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

    private void SpawnEnemies(float targetComplexity)
    {
        var i = 0;
        while (targetComplexity > 0 && i < 20)
        {
            SpawnRandomEnemy(targetComplexity, out var complexity);
            targetComplexity -= complexity;
        }
    }

    private void SpawnRandomEnemy(float targetComplexity, out float complexity)
    {
        if (targetComplexity < _minEnemiesComplexity) targetComplexity = _minEnemiesComplexity;
        var possibleEnemies = enemies.Where(enemy => enemy.Complexity <= targetComplexity).ToList();
        var sum = possibleEnemies.Sum(GetEnemyProbability);
        var rand = Random.Range(0f, sum);
        sum = 0;
        complexity = 0;
        foreach (var candidate in possibleEnemies)
        {
            var prob = GetEnemyProbability(candidate);
            sum += prob;
            if (rand > sum) continue;
            
            StartCoroutine(SpawnEnemy(candidate));
            complexity = candidate.Complexity;
            return;
        }
    }

    private static float GetEnemyProbability(Enemy enemy) => 1f / enemy.Complexity;

    private IEnumerator LaunchChallenge()
    {
        _currentWave = -1;
        while (_currentWave + 1 < wavesCount)
        {
            if (_currentWave >= 0) yield return StartCoroutine(LevelUp());
            CallNextWave();
            yield return new WaitWhile(() => _enemiesAlive > 0 && _gameIsOn);
            if (!_gameIsOn) yield break;
        }
        StopGame(false);
    }

    private IEnumerator LevelUp()
    {
        canvas.SetBool(LevelUpScreenBool, true);
        _playerInput.SwitchCurrentActionMap("menu");
        optionsLayout.Show(CreateRandomPowerUpsSet());
        yield return new WaitUntil(() => optionsLayout.OptionSelected);
        canvas.SetBool(LevelUpScreenBool, false);
        _playerInput.SwitchCurrentActionMap("gameplay");
    }

    private readonly HashSet<PowerUp> _uniquePowerUpsUsed = new();

    private HashSet<PowerUp> CreateRandomPowerUpsSet()
    {
        HashSet<PowerUp> options = new();
        for (var i = 0; i < numberOfOptionsForPowerUp; i++)
        {
            var forbidden = new HashSet<PowerUp>(options);
            forbidden.UnionWith(_uniquePowerUpsUsed);
            options.Add(SelectRandomPowerUp(forbidden));
        }
        return options;
    }

    private PowerUp SelectRandomPowerUp(ICollection<PowerUp> forbidden)
    {
        var availablePowerUps = powerUps.powerUps.Where(powerUp => !forbidden.Contains(powerUp.powerUp)).ToList();
        var sum = availablePowerUps.Sum(powerUp => powerUp.chance);
        var rand = Random.Range(0f, sum);
        sum = 0;
        foreach (var powerUp in availablePowerUps)
        {
            sum += powerUp.chance;
            if (rand > sum) continue;
            
            if (powerUp.powerUp.isUnique) _uniquePowerUpsUsed.Add(powerUp.powerUp);
            return powerUp.powerUp;
        }
        throw new ApplicationException();
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