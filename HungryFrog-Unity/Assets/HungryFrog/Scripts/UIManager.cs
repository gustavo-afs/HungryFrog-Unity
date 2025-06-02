using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject UIToolkitObject;
    private VisualElement root;
    
    private VisualElement mainMenu;
    private const string mainMenuName = "MainMenu";

    private VisualElement scorePanel;
    private const string scorePanelName = "ScorePanel";

    private Button startButton;
    private const string startButtonName = "StartButton";

    private Button exitButton;
    private const string exitButtonName = "ExitButton";

    private Label player1Score;
    private const string player1ScoreName = "Player1Score";

    private Label player2Score;
    private const string player2ScoreName = "Player2Score";

    private Label timer;
    private const string timerName = "Timer";

    private VisualElement resultPanel;
    private const string resultPanelName = "GameResult";

    private Label resultText;
    private const string resultTextName = "ResultText";

    private Button restartRoundButton;
    private const string restartRoundButtonName = "RestartRound";

    private Button quitToMenuButton;
    private const string quitToMenuButtonName = "QuitToMenu";

    private VisualElement inputSelectionPanel;
    private const string inputSelectionPanelName = "InputSelectionMenu";

    private Label inputSelectionMenuText;
    private const string inputSelectionMenuTextName = "InputSelectionMenuText";
    private Toggle frog1Toggle;
    private const string frog1ToggleName = "Frog1Toggle";

    private Toggle frog2Toggle;
    private const string frog2ToggleName = "Frog2Toggle";
    
    private List<Toggle> frogToggles;

    private Label[] scoreLabels;

    private void Awake()
    {
        root = UIToolkitObject.GetComponent<UIDocument>().rootVisualElement;
        
        mainMenu = root.Q(mainMenuName);
        scorePanel = root.Q(scorePanelName);
        startButton = (Button)root.Q(startButtonName);
        exitButton = (Button)root.Q(exitButtonName);
        player1Score = (Label)root.Q(player1ScoreName);
        player2Score = (Label)root.Q(player2ScoreName);
        timer = (Label)scorePanel.Q(timerName);
        resultPanel = root.Q(resultPanelName);
        resultText = (Label)root.Q(resultTextName);
        restartRoundButton = (Button)root.Q(restartRoundButtonName);
        quitToMenuButton = (Button)root.Q(quitToMenuButtonName);
        inputSelectionPanel = root.Q(inputSelectionPanelName);
        inputSelectionMenuText = (Label)root.Q(inputSelectionMenuTextName);
        frog1Toggle = (Toggle)root.Q(frog1ToggleName);
        frog2Toggle = (Toggle)root.Q(frog2ToggleName);
        
        scoreLabels = new[] { player1Score, player2Score };
        frogToggles = new List<Toggle> { frog1Toggle, frog2Toggle };
    }

    public void InitializeButtons(Action startButtonAction, Action quitButtonAction, Action restartRoundAction, Action quitToMenuAction)
    {
        startButton.RegisterCallback<ClickEvent>(evt => startButtonAction());
        startButton.RegisterCallback<NavigationSubmitEvent>(evt => startButtonAction());

        exitButton.RegisterCallback<ClickEvent>(evt => quitButtonAction());
        exitButton.RegisterCallback<NavigationSubmitEvent>(evt => quitButtonAction());

        restartRoundButton.RegisterCallback<ClickEvent>(evt => restartRoundAction());
        restartRoundButton.RegisterCallback<NavigationSubmitEvent>(evt => restartRoundAction());

        quitToMenuButton.RegisterCallback<ClickEvent>(evt => quitToMenuAction());
        quitToMenuButton.RegisterCallback<NavigationSubmitEvent>(evt => quitToMenuAction());
    }

    public void SetInputSelectionMenuText(string text)
    {
        inputSelectionMenuText.text = text;
    }

    public void SetEnabledMainMenu(bool enabled)
    {
        mainMenu.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;

        if (enabled && mainMenu.childCount > 0)
        {
            mainMenu[0]?.Focus();
        }
    }

    public void SetEnabledScorePanel(bool enabled)
    {
        scorePanel.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetEnabledInputSelectionPanel(bool enabled)
    {
        inputSelectionPanel.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void ResetUI(float defaultTime)
    {
        timer.text = Mathf.CeilToInt(defaultTime).ToString();

        foreach (var label in scoreLabels)
        {
            label.text = "0";
        }
        
        for (int i = 0; i < frogToggles.Count; i++)
        {
            frogToggles[i].value = false;
        }
    }

    public void SetEnabledResultPanel(string text, bool enabled)
    {
        resultPanel.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
        resultText.text = text;
        if (enabled)
        {
            restartRoundButton.Focus();
        }
    }

    public void UpdateTimer(float timeRemaining)
    {
        timer.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    public bool TryUpdateScore(int playerID, int score)
    {
        if (playerID >= 0 && playerID < scoreLabels.Length)
        {
            scoreLabels[playerID].text = score.ToString();
            return true;
        }

        Debug.LogWarning($"Invalid player ID: {playerID}");
        return false;
    }
    
    public void SetFrogToggle(int playerID, bool state)
    {
        if (playerID >= 0 && playerID < frogToggles.Count)
        {
            frogToggles[playerID].value = state;
        }
    }
}
