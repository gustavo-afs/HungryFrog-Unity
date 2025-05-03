using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    // UI Toolkit
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

    private Label[] scoreLabels;
    // Legacy UI
    [SerializeField] private TMP_Text mainPanelText;
    //[SerializeField] private TMP_Text timerText;
    //[SerializeField] private TMP_Text[] scoreLabelArray;
    
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
        
        scoreLabels = new []{player1Score, player2Score};
    }

    public void InitializeButtons(Action startButtonAction, Action quitButtonAction)
    {
        startButton.RegisterCallback<ClickEvent>(evt =>
        {
            startButtonAction();
            Debug.Log("Clicked StartButton");
        });
        startButton.RegisterCallback<NavigationSubmitEvent>(evt =>
        {
            startButtonAction();
            Debug.Log("Submitted StartButton");
        });

        exitButton.RegisterCallback<ClickEvent>(evt =>
        {
            quitButtonAction();
            Debug.Log("Clicked QuitButton");
        });
        exitButton.RegisterCallback<NavigationSubmitEvent>(evt =>
        {
            quitButtonAction();
            Debug.Log("Submitted QuitButton");
        });
    }

    public void SetEnabledMainMenu(bool enabled)
    {
        mainMenu.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;

        // Define the focus on the first focusable child element
        if (enabled && mainMenu.childCount > 0)
        {
            mainMenu[0]?.Focus();
        }
    }

    public void SetEnabledScorePanel(bool enabled)
    {
        scorePanel.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    public void ResetUI(float defaultTime)
    {
        mainPanelText.text = "";
        timer.text = Mathf.CeilToInt(defaultTime).ToString();

        for (int i = 0; i < scoreLabels.Length; i++)
        {
            scoreLabels[i].text = "0";
        }
    }
    
    public void UpdateMainPanelText(string text)
    {
        mainPanelText.text = text;
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
}
