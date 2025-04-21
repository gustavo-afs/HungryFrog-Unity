using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    
    private float frog1Score = 0;
    private float frog2Score = 0;
    [SerializeField] private TMP_Text frog1ScoreText;
    [SerializeField] private TMP_Text frog2ScoreText;
    [SerializeField] private float defaultTimer = 60f;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private FliesSpawner fliesSpawner;
    private float currentTimer;
    private bool canReset = false;
    
    void Reset()
    {
        StopAllCoroutines();
        frog1ScoreText.text = "0";
        frog2ScoreText.text = "0";
        frog1Score = 0;
        frog2Score = 0;
        resultText.text = "";
        currentTimer = defaultTimer;
    }

    public void ResetRequest()
    {
        if (canReset)
        {
            Reset();
            StartGame();
        }
    }
    public void IncreaseScore(int player)
    {
        if (player == 1)
        {
            frog1Score++;
            frog1ScoreText.text = frog1Score.ToString();
        }

        if (player == 2)
        {
            frog2Score++;
            frog2ScoreText.text = frog2Score.ToString();
        }
    }

    public void StartGame()
    {
        canReset = false;
        Reset();
        StartCoroutine(StartCounter(defaultTimer));
        fliesSpawner.StartSpawning();
    }

    IEnumerator StartCounter(float timer)
    {
        currentTimer = timer;
        while(currentTimer > 0 )
        {
            timerText.text = ((int)currentTimer).ToString();
            currentTimer -= Time.deltaTime;
            yield return null;
        }

        if (frog1Score > frog2Score)
        {
            resultText.text = "Frog 1 Win!";
        }
        else if(frog1Score < frog2Score)
        {
            resultText.text = "Frog 2 Win!";
        } else
        {
            resultText.text = "Draw!";
        }
        canReset = true;
    }
}
