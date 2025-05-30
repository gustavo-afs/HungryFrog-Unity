using System.Collections.Generic;
using UnityEngine;
public class ScoreManager
{
    private int[] scoreArray;

    public void InitializeScore(int playersQuantity)
    {
        scoreArray = new int[playersQuantity];
    }
    
    public void ResetScore()
    {
        for (int i = 0; i < scoreArray.Length; i++)
        {
            scoreArray[i] = 0;
        }
    }

    public bool TryAddScore(int playerID)
    {
        if (IsValidPlayer(playerID))
        {
            scoreArray[playerID]++;
            return true;
        }
        else
        {
            Debug.LogWarning($"Invalid player ID: {playerID}");
            return false;
        }
    }
    
    public bool TryGetScore(int playerID, out int score)
    {

        if (IsValidPlayer(playerID))
        {
            score = scoreArray[playerID];
            return true;
        }
        else
        {
            Debug.LogWarning($"Invalid player ID: {playerID}");
            score = -1;
            return false;
        }
    }

    private bool IsValidPlayer(int playerID)
    {
        if (playerID >= 0 && playerID < scoreArray.Length)
        {
            return true;
        }

        return false;
    }

    public (int playerID, int score)  GetWinner()
    {
        int highestScore = -1;
        int winnerID = -1;
        bool isDraw = false;
        
        for (int i = 0; i < scoreArray.Length; i++)
        {
            if (scoreArray[i] > highestScore)
            {
                highestScore = scoreArray[i];
                winnerID = i;
                isDraw = false;
            } else if (scoreArray[i] == highestScore)
            {
                isDraw = true;
            }
        }

        if (isDraw)
        {
            return (-1, -1);
        }
        else
        {
            return (winnerID+1, highestScore);
        }
    }
}
