using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetup : MonoBehaviour
{
    public event Action<Gamepad> OnPlayerJoined;
    public event Action<Gamepad> OnPlayerLeft;
    private HashSet<Gamepad> assignedGamepads = new HashSet<Gamepad>();
    private Coroutine searchingInput;

    public void StartInputSearching()
    {
        StopInputSearching();
        searchingInput= StartCoroutine(SearchingInput());
    }

    public void StopInputSearching()
    {
        if (searchingInput != null)
        {
            StopCoroutine(searchingInput);
        }
    }

    IEnumerator SearchingInput()
    {
        while (true)
        {
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.buttonSouth.wasPressedThisFrame && !assignedGamepads.Contains(gamepad))
                {
                    assignedGamepads.Add(gamepad);
                    OnPlayerJoined?.Invoke(gamepad);
                }
                else if(gamepad.buttonEast.wasPressedThisFrame && assignedGamepads.Contains(gamepad))
                {
                    assignedGamepads.Remove(gamepad);
                    OnPlayerLeft?.Invoke(gamepad);
                }
            }
            yield return null;
        }
    }
}
