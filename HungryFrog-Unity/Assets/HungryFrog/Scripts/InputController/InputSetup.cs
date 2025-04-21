using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetup : MonoBehaviour
{
    [SerializeField] private Frog player1;
    [SerializeField] private Frog player2;

    [SerializeField] private ScoreController scoreController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SelectingController());
    }

    IEnumerator SelectingController()
    {
        var usedGamepads = new HashSet<Gamepad>();

        while (player1.inputController == null || player2.inputController == null)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (!gamepad.buttonSouth.wasPressedThisFrame)
                {
                    continue;
                }

                if (usedGamepads.Contains(gamepad))
                {
                    continue;
                }

                if (player1.inputController == null)
                {
                    player1.inputController = gamepad;
                    usedGamepads.Add(gamepad);
                    Debug.Log("Player 1 selected controller.");
                }
                else if (player2.inputController == null)
                {
                    player2.inputController = gamepad;
                    usedGamepads.Add(gamepad);
                    Debug.Log("Player 2 selected controller.");
                }
            }

            yield return null;
        }

        Debug.Log("Both players selected controllers, initializing game");
        scoreController.StartGame();
    }
}
