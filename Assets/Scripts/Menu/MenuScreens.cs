using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuScreens : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager manager;

    [SerializeField]
    private Button playButton, continueButton;

    [SerializeField]
    private GameObject mainScreen, endGameScreen, inGameScreenPC, inGameScreenMobile;

    [SerializeField]
    private TextMeshProUGUI endGamePrompt;

    private void Start()
    {
        playButton.onClick.AddListener(() => { manager.StartGame(); GoToGameScreen(); });
        continueButton.onClick.AddListener(manager.RestartGame);

        mainScreen.SetActive(true);
        endGameScreen.SetActive(false);
        inGameScreenPC.SetActive(false);
        inGameScreenMobile.SetActive(false);

        manager.OnGameLose += GameLose;
        manager.OnGameWin += GameWon;
    }

    private void GoToGameScreen()
    {
        mainScreen.SetActive(false);
        endGameScreen.SetActive(false);
#if UNITY_STANDALONE
        inGameScreenPC.SetActive(true);
#else
        inGameScreenMobile.SetActive(true);
#endif
    }

    private void GameWon()
    {
        endGamePrompt.text = "You Won!";

        mainScreen.SetActive(false);
        endGameScreen.SetActive(true);
        inGameScreenPC.SetActive(false);
        inGameScreenMobile.SetActive(false);
    }

    private void GameLose()
    {
        endGamePrompt.text = "You Lose :C";

        mainScreen.SetActive(false);
        endGameScreen.SetActive(true);
        inGameScreenPC.SetActive(false);
        inGameScreenMobile.SetActive(false);
    }


}
