using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetup : MonoBehaviour
{
    private PlayerInput[] playerInputs;
    [SerializeField] private GameObject playerInputPrefab;

    private int playersCount;
    
    public event Action<List<GameObject>> OnAllPlayersReady;
    public event Action<int /*PlayerID*/,bool /*State*/> OnPlayersUpdated;

    public void OnToggleJoinPerformed(InputAction.CallbackContext callbackContext)
    {
        var device = callbackContext.control.device;

        if (IsDeviceAssigned(device, out PlayerInput playerInput))
        {
            UnpairDevice(playerInput);
        }
        else
        {
            PairDevice(device);
        }
    }

    private void PairDevice(InputDevice device)
    {
        var playerInput = PlayerInput.Instantiate(playerInputPrefab, pairWithDevice: device);
        if (playerInput.user.valid)
        {
            for (int i = 0; i < playersCount; i++)
            {
                if (playerInputs[i] == null)
                {
                    playerInputs[i] = playerInput;
                    OnPlayersUpdated?.Invoke(i,true);
                    ValidateInputs();
                    return;
                }
            }
        }
        else
        {
            Destroy(playerInput.gameObject);
        }
    }
    
    private void ValidateInputs()
    {
        if (AreControllersReady())
        {
            List<GameObject> allPlayers = new List<GameObject>();
            foreach (var player in playerInputs)
            {
                allPlayers.Add(player.gameObject);
            }
            OnAllPlayersReady?.Invoke(allPlayers);
        }
    }

    private void UnpairDevice(PlayerInput playerInput)
    {
        for (int i = 0; i < playersCount; i++)
        {
            if (playerInputs[i] == playerInput)
            {
                playerInputs[i] = null;
                Destroy(playerInput.gameObject);
                OnPlayersUpdated?.Invoke(i,false);
            }
        }
    }

    private bool IsDeviceAssigned(InputDevice device, out PlayerInput assignedPlayerInput)
    {
        assignedPlayerInput = null;

        if (playerInputs == null)
        {
            return false;
        }

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] != null)
            {
                foreach (var playerInputDevice in playerInputs[i].devices)
                {
                    if (playerInputDevice == device)
                    {
                        assignedPlayerInput = playerInputs[i];
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool AreControllersReady()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            var playerInput = playerInputs[i];

            if (playerInput == null)
            {
                return false;
            }

            if (!playerInput.user.valid)
            {
                return false;
            }
            
            if (playerInputs[i].devices.Count == 0)
            {
                return false;
            }
        }
        return true;
    }

    public void SetupPlayerInputs(int playerCountInput)
    {
        playersCount = playerCountInput;
        playerInputs = new PlayerInput[playersCount];
    }
}
