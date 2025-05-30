using System;
using System.Collections;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public event Action onTimerEnded;
    public event Action<float> onTimerUpdate;
    private float defaultTimerDuration;
    private float currentTimer = 0f;
    private bool isTimerRunning = false;
    private Coroutine timerCoroutine;
    
    public void StartTimer(float timerDuration)
    {
        defaultTimerDuration = timerDuration;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        currentTimer = defaultTimerDuration;
        isTimerRunning = true;
        while (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            onTimerUpdate?.Invoke(currentTimer);
            yield return null;
        }
        isTimerRunning = false;
        onTimerUpdate?.Invoke(0f);
        onTimerEnded?.Invoke();
    }
    
    //TODO: Implement stop and pause actions
}
