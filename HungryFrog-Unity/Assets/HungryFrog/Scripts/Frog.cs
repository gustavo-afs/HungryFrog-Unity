using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Frog : MonoBehaviour
{
    //TODO: Extract the tongue from this class
    [Header("Systems")]
    public Gamepad inputController;
    public int iDNumber;
    [SerializeField] private AudioSource audioSource;

    [Header("Frog References")]
    [SerializeField] private GameObject frogObject;
    [SerializeField] private GameObject tongueScope;
    [SerializeField] private LineRenderer tongueLineRenderer;
    [SerializeField] private GameObject tongueTip;
    [SerializeField] private AudioClip tongueAudioClip;

    [Header("Tongue Settings")]
    [SerializeField] private float tongueMaxSize = 4f;
    [SerializeField] private float tongueSpeed = 1f;
    [SerializeField] private float catchDuration = 0.5f;
    [SerializeField] private float catchStartOffset = 0.01f;

    private Vector3 tongueTargetPosition;
    private bool isCatching = false;
    private bool isTongueReleasing = false;
    public event Action<int> catchAction;

    void Update()
    {
        if (inputController == null)
        {
            //Debug.LogWarning($"{nameof(this.gameObject.name)}: No controller assigned");
            return;
        }

        if (isTongueReleasing)
        {
            return;
        }

        Vector2 leftStick = inputController.leftStick.ReadValue();

        RotateFrog(leftStick);
        MoveTongueScope(leftStick);

        if (inputController.crossButton.wasPressedThisFrame)
        {
            isTongueReleasing = true;
            StartCoroutine(TongueAnimation());
        }
    }

    #region Frog Movement

    private void RotateFrog(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
        {
            return;
        }
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        frogObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveTongueScope(Vector2 input)
    {
        if (input.sqrMagnitude > 0.01f)
        {
            float distance = input.magnitude * tongueMaxSize;
            tongueTargetPosition = Vector3.right * distance;
        }
        else
        {
            tongueTargetPosition = Vector3.zero;
        }

        tongueScope.transform.localPosition = tongueTargetPosition;
    }

    #endregion

    #region Tongue Logic

    private IEnumerator TongueAnimation()
    {
        float position = 0f;
        float targetX = tongueTargetPosition.x;

        tongueLineRenderer.SetPosition(0, Vector3.zero);
        tongueLineRenderer.SetPosition(1, Vector3.zero);

        audioSource.clip = tongueAudioClip;
        audioSource.Play();

        // Extend tongue
        while (position < targetX)
        {
            //Debug.Log($"{nameof(Frog)}: Tongue extending");
            position += Time.deltaTime * tongueSpeed;
            tongueLineRenderer.SetPosition(1, new Vector3(position, 0f, 0f));
            tongueTip.transform.localPosition = new Vector3(position, 0f, 0f);

            if (!isCatching && targetX - position < catchStartOffset)
            {
                isCatching = true;
                StartCoroutine(CatchFly());
            }

            yield return null;
        }

        // Retract tongue
        while (position > 0f)
        {
            //Debug.Log($"{nameof(Frog)}: Tongue retracting");
            position -= Time.deltaTime * tongueSpeed;
            tongueLineRenderer.SetPosition(1, new Vector3(position, 0f, 0f));
            tongueTip.transform.localPosition = new Vector3(position, 0f, 0f);
            yield return null;
        }

        isTongueReleasing = false;
    }

    private IEnumerator CatchFly()
    {
        //Debug.Log($"{nameof(Frog)}: Catching fly...");

        float timer = catchDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isCatching = false;
        //Debug.Log($"{nameof(Frog)}: Done catching");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Fly") && isCatching)
        {
            catchAction(iDNumber);
            collision.GetComponent<Fly>().Catched();
        }
    }

    #endregion
}
