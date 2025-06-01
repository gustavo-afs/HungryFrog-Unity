using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetup : MonoBehaviour
{
    private GameInputs input;
    private List<PlayerInput> playerInputs = new List<PlayerInput>();
    [SerializeField] private GameObject playerInputPrefab;

    private int playersCount;
    
    public event Action<List<GameObject>> OnAllPlayersReady;
    public event Action<int> OnPlayersUpdated;

    public void Awake()
    {
        input = new GameInputs();
        input.Lobby.ToggleJoin.performed += OnToggleJoinPerformed;
    }

    private void OnDisable()
    {
        input.Lobby.ToggleJoin.performed -= OnToggleJoinPerformed;
        input.Dispose();
    }
    
    private void OnToggleJoinPerformed(InputAction.CallbackContext callbackContext)
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
            playerInputs.Add(playerInput);
            OnPlayersUpdated?.Invoke(playerInputs.Count);
            ValidateInputs();
        }
        else
        {
            Destroy(playerInput.gameObject);
        }
    }

    public bool AreControllersReady()
    {
        return playerInputs.Count == playersCount;
    }
    
    private void ValidateInputs()
    {
        if (AreControllersReady())
        {
            Debug.Log($"PlayersInput Count: {playerInputs.Count} playersCount {playersCount}");
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
        Destroy(playerInput.gameObject);
        playerInputs.Remove(playerInput);
        OnPlayersUpdated?.Invoke(playerInputs.Count);
    }

    private bool IsDeviceAssigned(InputDevice device, out PlayerInput assignedPlayerInput)
    {
        assignedPlayerInput = null;

        foreach (var playerInput in playerInputs)
        {
            foreach (var playerInputDevice in playerInput.devices)
            {
                if (playerInputDevice == device)
                {
                    assignedPlayerInput = playerInput;
                    return true;
                }
            }
        }
        return false;
    }

    private void ValidateInputs()
    {
        if (AreControllersReady())
        {
            List<GameObject> allPlayers = new();
            foreach (var player in playerInputs)
            {
                allPlayers.Add(player.gameObject);
            }
            OnAllPlayersReady?.Invoke(allPlayers);
        }
    }

    public bool AreControllersReady()
    {
        return playerInputs.Count == playersCount;
    }

    public void SetupPlayerInputs(int playerCountInput)
    {
        playersCount = playerCountInput;
    }

    public void StartInputSearching()
    {
        input.Lobby.Enable();
    }

    public void StopInputSearching()
    {
        input.Lobby.Disable();
    }
}
