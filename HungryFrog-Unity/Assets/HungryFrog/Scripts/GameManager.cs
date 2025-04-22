using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private ScoreManager scoreManager = new();
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private InputSetup inputSetup;
    [SerializeField] private FliesSpawner fliesSpawner;
    [SerializeField] private Frog[] frogsArray;
    
    [SerializeField] private float timerDuration = 60;
    
    bool allControllersReady = false;
    private int gameStateInt;

    private void Start()
    {
        scoreManager.InitializeScore(2);
        timerManager.onTimerEnded += () =>
        {
            gameStateInt = 4;
        };
        
        timerManager.onTimerUpdate += (timerValue) =>
        {
            uIManager.UpdateTimer(timerValue);
        };
        
        for (int i = 0; i < frogsArray.Length; i++)
        {
            frogsArray[i].iDNumber = i;
            frogsArray[i].catchAction += (id) =>
            {
                if (scoreManager.TryAddScore(id))
                {
                    int actualScore;
                    if (scoreManager.TryGetScore(id,out actualScore))
                    {
                        uIManager.TryUpdateScore(id, actualScore);
                    }
                }
            };
        }
        gameStateInt = 0;
    }

    private void Update()
    {
        GameStates();
    }

    void GameStates()
    {
        //TODO: Improve this state machine
        //TODO: Add Restart
        //TODO: Add main screen
        //TODO: Add Pause
        //TODO: Add Quit
        switch (gameStateInt)
        {
            //Initialize Input Search
            case 0:
                InitializeInputSearch();
                gameStateInt = 1;
                break;
            //Waiting InputSearch;
            case 1:
                if (allControllersReady)
                {
                    FinalizeInputSearch();
                    gameStateInt = 2;
                }
                break;
            //StartGame
            case 2:
                uIManager.ResetUI(timerDuration);
                timerManager.StartTimer(timerDuration);
                fliesSpawner.StartSpawning();
                gameStateInt = 3;
                break;
            //During Game
            case 3:
                //Nothing
                break;
            //Game Ended
            case 4:
                int frog;
                int score;
                (frog, score) = scoreManager.GetWinner();

                if (frog == -1 || score == -1)
                {
                    uIManager.UpdateMainPanelText("Draw");
                }
                else
                {
                    uIManager.UpdateMainPanelText($"The Winner is the Frog {frog} and Score {score}");
                }
                break;
        }
    }

    private void InitializeInputSearch()
    {
        inputSetup.StartInputSearching();
        inputSetup.OnPlayerJoined += AssignInputs;
        inputSetup.OnPlayerLeft += RemoveInputs;
    }

    private void FinalizeInputSearch()
    {
        inputSetup.StopInputSearching();
        inputSetup.OnPlayerJoined -= AssignInputs;
        inputSetup.OnPlayerLeft -= RemoveInputs;
    }

    private void AssignInputs(Gamepad gamepad)
    {
        for (int i = 0; i < frogsArray.Length; i++)
        {
            if (frogsArray[i].inputController == null)
            {
                frogsArray[i].inputController = gamepad;
                ValidateAllInputs();
                return;
            }
        }
    }
    
    private void RemoveInputs(Gamepad gamepad)
    {
        for (int i = 0; i < frogsArray.Length; i++)
        {
            if (frogsArray[i].inputController == gamepad)
            {
                frogsArray[i].inputController = null;
            }
        }
    }

    private void ValidateAllInputs()
    {
        for (int i = 0; i < frogsArray.Length; i++)
        {
            if (frogsArray[i].inputController == null)
            {
                return;
            }
        }
        allControllersReady = true;
    }
}
