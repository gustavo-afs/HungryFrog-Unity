using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text mainPanelText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text[] scoreLabelArray;
    
    public void ResetUI(float defaultTime)
    {
        mainPanelText.text = "";
        timerText.text = Mathf.CeilToInt(defaultTime).ToString();

        for (int i = 0; i < scoreLabelArray.Length; i++)
        {
            scoreLabelArray[i].text = "0";
        }
    }
    
    public void UpdateMainPanelText(string text)
    {
        mainPanelText.text = text;
    }
    
    public void UpdateTimer(float timeRemaining)
    {
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }
    
    public bool TryUpdateScore(int playerID, int score)
    {
        if (playerID >= 0 && playerID < scoreLabelArray.Length)
        {
            scoreLabelArray[playerID].text = score.ToString();
            return true;
        }
        Debug.LogWarning($"Invalid player ID: {playerID}");
        return false;
    }
}
