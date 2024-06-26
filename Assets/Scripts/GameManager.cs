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
using UnityEngine.Serialization;
using Utility;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int wavesCount = 5;
    [SerializeField] private float initialComplexity = 20;
    [SerializeField] private float complexityMultiplier = 1.5f;
    [SerializeField] private int maxEnemies = 100;
    
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private Enemy finalBoss;
    [SerializeField] private Portal portalPrefab;
    [SerializeField] private List<ZoneTrigger> zones;
    [SerializeField] private Animator canvas;
    [SerializeField] private OptionsLayout optionsLayout;
    [SerializeField] private PowerUpsSet powerUps;
    [SerializeField] private int numberOfOptionsForPowerUp = 3;

    [FormerlySerializedAs("soundtrackController")] [SerializeField] private MusicController musicController;

    private PlayerInput _playerInput;

    private int _currentWave = -1;
    private Rect _spawnArea;
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
        _gameIsOn = true;
        _playerInput.SwitchCurrentActionMap("gameplay");
        canvas.SetTrigger(OnGameStartedTrigger);
        StartCoroutine(LaunchChallenge());
        musicController.PlayMainTheme();
    }

    public void StopGame(bool death)
    {
        if (!_gameIsOn) return;
        _gameIsOn = false;
        _uniquePowerUpsUsed.Clear();
        _playerInput.SwitchCurrentActionMap("menu");
        canvas.SetTrigger(death ? OnDeathTrigger : OnGameFinishedTrigger);
        StartCoroutine(WaitForMainMenuAndResetScene());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator WaitForMainMenuAndResetScene()
    {
        yield return new WaitUntil(() => canvas.GetCurrentAnimatorStateInfo(0).IsName("Screens_MainMenu"));
        musicController.PlayMenuTheme();
        ResetScene();
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
        var enemiesCounter = 0;
        while (targetComplexity > 0 && enemiesCounter < maxEnemies)
        {
            SpawnRandomEnemy(targetComplexity, out var complexity);
            targetComplexity -= complexity;
            enemiesCounter++;
        }
    }

    private void SpawnRandomEnemy(float targetComplexity, out float complexity)
    {
        if (targetComplexity < _minEnemiesComplexity) targetComplexity = _minEnemiesComplexity;
        var possibleEnemies = enemies.Where(enemy => enemy.Complexity <= targetComplexity).ToList();
        var sum = possibleEnemies.Sum(enemy => GetEnemyProbability(enemy, targetComplexity));
        var rand = Random.Range(0f, sum);
        sum = 0;
        complexity = 0;
        foreach (var candidate in possibleEnemies)
        {
            var prob = GetEnemyProbability(candidate, targetComplexity);
            sum += prob;
            if (rand > sum) continue;
            
            StartCoroutine(SpawnEnemy(candidate));
            complexity = candidate.Complexity;
            return;
        }
    }

    private static float GetEnemyProbability(Enemy enemy, float targetComplexity)
    {
        if (enemy.Complexity > targetComplexity) return 1f / Mathf.Pow(enemy.Complexity, 2); // low chance
        if (enemy.Complexity >= targetComplexity / 2f) return 10f / enemy.Complexity; // high chance
        return 1f / enemy.Complexity;
    }

    private IEnumerator LaunchChallenge()
    {
        _currentWave = -1;
        while (_currentWave + 1 <= wavesCount)
        {
            if (_currentWave >= 0) yield return StartCoroutine(LevelUp());
            CallNextWave();
            yield return new WaitWhile(() => _enemiesAlive > 0 && _gameIsOn);
            if (!_gameIsOn) yield break;
        }

        yield return new WaitWhile(() => _enemiesAlive > 0 && _gameIsOn);
        StopGame(false);
    }

    private IEnumerator LevelUp()
    {
        canvas.SetBool(LevelUpScreenBool, true);
        _playerInput.SwitchCurrentActionMap("menu");
        yield return new WaitUntil(() => optionsLayout.isActiveAndEnabled);
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
        if (_currentWave > 0) zones[_currentWave].Open();
        _spawnArea = zones[_currentWave].ZoneBounds;
        if (_currentWave == wavesCount) StartCoroutine(SpawnEnemy(finalBoss));
        else SpawnEnemies(initialComplexity * MathF.Pow(complexityMultiplier, _currentWave));
    }

    private IEnumerator SpawnEnemy(Enemy enemy)
    {
        _enemiesAlive += 1;
        var portal = Instantiate(
            portalPrefab, 
            CustomRandom.GetPosition(_spawnArea) + portalPrefab.transform.position,
            Quaternion.identity
            );
        portal.enemyToSpawn = enemy;
        yield return new WaitUntil(() => portal.SpawnedEnemy);
        var spawnedEnemy = portal.SpawnedEnemy;

        yield return new WaitUntil(() => spawnedEnemy.IsDestroyed());
        _enemiesAlive -= 1;
    }
}