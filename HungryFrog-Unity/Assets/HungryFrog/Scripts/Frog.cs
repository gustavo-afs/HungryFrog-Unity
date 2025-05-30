using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Frog : MonoBehaviour
{
    //TODO: Extract the tongue from this class
    [Header("Systems")]
    private InputAction moveAction;
    private InputAction fireTongueAction;
    public PlayerInput playerInput;
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

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        RotateFrog(moveInput);
        MoveTongueScope(moveInput);
    }

    private void OnMoveStop(InputAction.CallbackContext context)
    {
        MoveTongueScope(Vector2.zero); // Reseta a posição
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!isTongueReleasing)
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
            position -= Time.deltaTime * tongueSpeed;
            tongueLineRenderer.SetPosition(1, new Vector3(position, 0f, 0f));
            tongueTip.transform.localPosition = new Vector3(position, 0f, 0f);
            yield return null;
        }

        isTongueReleasing = false;
    }

    private IEnumerator CatchFly()
    {
        float timer = catchDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isCatching = false;
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

    public void SetupFrog(PlayerInput playerInputComponent)
    {
        playerInput = playerInputComponent;
        moveAction = playerInput.actions["Move"];
        fireTongueAction = playerInput.actions["FireTongue"];

        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveStop;
        fireTongueAction.performed += OnFire;

        moveAction.Enable();
        fireTongueAction.Enable();
    }
}
