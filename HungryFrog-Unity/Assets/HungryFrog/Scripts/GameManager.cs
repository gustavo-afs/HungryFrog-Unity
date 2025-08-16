using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private GameInputs input;
    private ScoreManager scoreManager = new();
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private InputSetup inputSetup;
    [SerializeField] private FliesSpawner fliesSpawner;
    [SerializeField] private Frog[] frogsArray;
    [SerializeField] private float timerDuration = 60;

    private int gameStateInt;


    private string awaitingInputText = "Press EAST Button to Join or Leave Game";
    private string confirmStart = "Press SOUTH button to start the game";

    private void Awake()
    {
        input = new GameInputs();
    }

    private void Start()
    {
        input.Lobby.ConfirmInputs.performed += ctx =>
        {
            if (gameStateInt == 0)
            {
                gameStateInt = 1;
            }
        };
        
        uIManager.InitializeButtons(
            startButtonAction: () => gameStateInt = -2,
            quitButtonAction: () => Application.Quit(),
            restartRoundAction: () => gameStateInt = 2,
            quitToMenuAction: () => gameStateInt = -4
        );
        scoreManager.InitializeScore(frogsArray.Length);
        timerManager.onTimerEnded += () => gameStateInt = 4;
        timerManager.onTimerUpdate += (timerValue) => uIManager.UpdateTimer(timerValue);
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
        
        inputSetup.OnPlayersUpdated += UpdateToggles;
        
        gameStateInt = -4;
    }

    private void Update()
    {
        GameStates();
    }
    
    void GameStates()
    {
        //TODO: Improve this state machine
        //TODO: Add Pause
        Debug.Log(gameStateInt);
        switch (gameStateInt)
        {
            case -4:
                uIManager.SetEnabledScorePanel(false);
                uIManager.SetEnabledResultPanel(text: "", enabled: false);
                uIManager.SetEnabledMainMenu(true);
                inputSetup.SetupPlayerInputs(frogsArray.Length);
                gameStateInt = -3;
                break;
            case -3:
                break;
            case -2:
                uIManager.SetEnabledMainMenu(false);
                if (inputSetup.AreControllersReady())
                {
                    gameStateInt = 2;
                    break;
                }
                input.Lobby.Enable();
                input.Lobby.ToggleJoin.performed += inputSetup.OnToggleJoinPerformed;
                inputSetup.OnAllPlayersReady -= AssignPlayerInputsToFrogs; // previne múltiplos binds
                inputSetup.OnAllPlayersReady += AssignPlayerInputsToFrogs;
                uIManager.SetEnabledInputSelectionPanel(true);

                gameStateInt = -1;
                break;
            case -1:
                inputSetup.OnAllPlayersReady -= AssignPlayerInputsToFrogs;
                inputSetup.OnAllPlayersReady += AssignPlayerInputsToFrogs;

                if (inputSetup.AreControllersReady())
                {
                    gameStateInt = 0;
                }
                uIManager.SetInputSelectionMenuText(awaitingInputText);
                break;  
            case 0:
                if (!inputSetup.AreControllersReady())
                {
                    gameStateInt = -1;
                }
                uIManager.SetInputSelectionMenuText(confirmStart);
                break;
            case 1:
                input.Lobby.ToggleJoin.performed -= inputSetup.OnToggleJoinPerformed;
                input.Lobby.Disable();
                gameStateInt = 2;
                break;
            //StartGame
            case 2:
                uIManager.SetEnabledInputSelectionPanel(false);
                uIManager.SetEnabledScorePanel(true);
                uIManager.SetEnabledResultPanel("", false);
                uIManager.ResetUI(timerDuration);
                timerManager.StartTimer(timerDuration);
                scoreManager.ResetScore();
                fliesSpawner.StartSpawning();
                foreach (var frog in frogsArray)
                {
                    // frog.playerInput.ActivateInput();
                    frog.playerInput.actions.Enable();
                }
                gameStateInt = 3;
                break;
            case 3:
                break;
            case 4:
                foreach (var frog in frogsArray)
                {
                    // frog.playerInput.DeactivateInput();
                    
                    frog.playerInput.actions.Disable();
                }
                fliesSpawner.StopSpawning();
                (int frogID, int score) = scoreManager.GetWinner();
                string result;
                if (frogID == -1 || score == -1)
                {
                    result = "Draw!";
                }
                else
                {
                    result = $"The Winner is: Frog {frogID} \n\n Score: {score}";                    
                }
                uIManager.SetEnabledResultPanel(result, true);
                gameStateInt = 5;
                break;
            case 5:
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
    }
    
    private void UpdateToggles(int joinedPlayerCount)
    {
        for (int i = 0; i < frogsArray.Length; i++)
        {
            uIManager.SetFrogToggle(i, i < joinedPlayerCount);
        }
    }
}
