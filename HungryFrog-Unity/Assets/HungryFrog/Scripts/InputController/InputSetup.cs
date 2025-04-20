using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetup : MonoBehaviour
{
    public Frog player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start");
        if (Gamepad.all.Count == 0)
        {
            Debug.Log("Gamepad count is 0");
            return;
        }
        player.InputController = Gamepad.all[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
