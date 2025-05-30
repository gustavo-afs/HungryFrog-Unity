using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private ScoreManager scoreManager = new();
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private InputSetup inputSetup;
    [SerializeField] private FliesSpawner fliesSpawner;
    [SerializeField] private Frog[] frogsArray;
    [SerializeField] private float timerDuration = 60;
    [SerializeField] private InputActionAsset inputActionAsset;
    
    private int gameStateInt;
    
    private void Start()
    {
        uIManager.InitializeButtons(() => gameStateInt = -1, () => Application.Quit());

        scoreManager.InitializeScore(frogsArray.Length);
        timerManager.onTimerEnded += () => gameStateInt = 4;

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
                    if (scoreManager.TryGetScore(id, out int actualScore))
                    {
                        uIManager.TryUpdateScore(id, actualScore);
                    }
                }
            };
        }
        gameStateInt = -3;
    }

    private void Update()
    {
        GameStates();
    }

    void GameStates()
    {
        //TODO: Improve this state machine
        //TODO: Add Restart
        //TODO: Add Pause
        //TODO: Add Quit
        switch (gameStateInt)
        {
            case -3:
                uIManager.SetEnabledMainMenu(true);
                gameStateInt = -2;
                break;
            case -2:
                // Waiting for menu interactions
                break;
            case -1:
                uIManager.SetEnabledMainMenu(false);
                uIManager.SetEnabledScorePanel(true);
                inputSetup.OnAllPlayersReady += AssignPlayerInputsToFrogs;
                inputSetup.StartInputSearching(frogsArray.Length);
                gameStateInt = 1;
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
                    uIManager.UpdateMainPanelText($"The Winner is the Frog {frog} with Score {score}");
                }
                break;
        }
    }

    private void AssignPlayerInputsToFrogs(List<GameObject> playersInput)
    {
        for (int i = 0; i < frogsArray.Length; i++)
        {
            playersInput[i].transform.SetParent(frogsArray[i].transform);
            frogsArray[i].SetupFrog(playersInput[i].GetComponent<PlayerInput>());
        }
        inputSetup.StopInputSearching();
        inputSetup.OnAllPlayersReady -= AssignPlayerInputsToFrogs;
        gameStateInt = 2;
    }
}
