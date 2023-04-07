using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private Health player;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private List<Health> enemies = new List<Health>();

    public int EnemiesToBeat { get; set; }
    public int TotalEnemiesToBeat { get; set; }

    public Action<string> OnScoreChange { get; set; }
    public Action OnGameLose { get; set; }
    public Action OnGameWin { get; set; }

    private void Awake()
    {
        TotalEnemiesToBeat = enemies.Count;
        EnemiesToBeat = 0;

        foreach (var item in enemies)
        {
            item.OnDead += () => { EnemiesToBeat++; UpdateObjective(); };
        }

        Time.timeScale = 0;
        inputManager.AllowInputs = false;

        player.OnDead += () =>
        {
            OnGameLose?.Invoke();
            Time.timeScale = 0;
            inputManager.AllowInputs = false;
        };
    }

    public void UpdateObjective()
    {
        OnScoreChange?.Invoke($"{EnemiesToBeat}/{TotalEnemiesToBeat}");
        if (EnemiesToBeat >= TotalEnemiesToBeat)
        {
            OnGameWin?.Invoke();
            Time.timeScale = 0;
            inputManager.AllowInputs = false;
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        StartCoroutine(C_DelayInputActivation());
    }

    private IEnumerator C_DelayInputActivation()
    {
        yield return new WaitForSeconds(1);
        inputManager.AllowInputs = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
