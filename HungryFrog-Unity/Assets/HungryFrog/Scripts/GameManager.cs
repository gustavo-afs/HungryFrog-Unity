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
        uIManager.InitializeButtons(
            () => gameStateInt = -1,
            () => Application.Quit(),
            () => gameStateInt = 2,
            () => gameStateInt = -3
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
        
        gameStateInt = -3;
    }

    private void Update()
    {
        GameStates();
    }
    
    void GameStates()
    {
        //TODO: Improve this state machine
        //TODO: Add Pause
        switch (gameStateInt)
        {
            case -3:
                uIManager.SetEnabledResultPanel(text: "", enabled: false);
                uIManager.SetEnabledMainMenu(true);
                inputSetup.SetupPlayerInputs(frogsArray.Length);
                gameStateInt = -2;
                break;
            case -2:
                break;
            case -1:
                uIManager.SetEnabledMainMenu(false);
                if (inputSetup.AreControllersReady())
                {
                    gameStateInt = 2;
                    break;
                }
                inputSetup.OnAllPlayersReady += AssignPlayerInputsToFrogs;
                inputSetup.StartInputSearching();
                uIManager.SetEnabledInputSelectionPanel(true);
                gameStateInt = 0;
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
                    frog.playerInput.ActivateInput();
                }

                gameStateInt = 3;
                break;
            case 3:
                break;
            case 4:
                foreach (var frogss in frogsArray)
                {
                    frogss.playerInput.DeactivateInput();
                }

                fliesSpawner.StopSpawning();

                (int froggy, int score) = scoreManager.GetWinner();

                string result;
                if (froggy == -1 || score == -1)
                {
                    result = "Draw!";
                }
                else
                {
                    result = $"The Winner is: Frog {froggy} \n\n Score: {score}";                    
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

        inputSetup.StopInputSearching();
        inputSetup.OnAllPlayersReady -= AssignPlayerInputsToFrogs;
    }
    
    private void UpdateToggles(int joinedPlayerCount)
    {
        for (int i = 0; i < frogsArray.Length; i++)
        {
            uIManager.SetFrogToggle(i, i < joinedPlayerCount);
        }
    }
}
