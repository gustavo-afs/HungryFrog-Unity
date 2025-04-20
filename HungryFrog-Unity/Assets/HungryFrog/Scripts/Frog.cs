using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Frog : MonoBehaviour
{
    [Header("Input")]
    public Gamepad inputController;

    [Header("Frog References")]
    [SerializeField] private GameObject frogObject;
    [SerializeField] private GameObject tongueScope;
    [SerializeField] private LineRenderer tongueLineRenderer;

    [Header("Tongue Settings")]
    [SerializeField] private float tongueMaxSize = 4f;
    [SerializeField] private float tongueSpeed = 1f;
    [SerializeField] private float catchDuration = 0.5f;
    [SerializeField] private float catchStartOffset = 0.01f;

    private Vector3 tongueTargetPosition;
    private bool isCatching = false;
    private bool isTongueReleasing = false;

    void Update()
    {
        if (inputController == null)
        {
            Debug.LogWarning($"{nameof(Frog)}: No controller assigned");
            return;
        }

        if (isTongueReleasing) return;

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
        if (input.sqrMagnitude < 0.01f) return;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        frogObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveTongueScope(Vector2 input)
    {
        tongueTargetPosition = input.sqrMagnitude > 0.01f 
            ? Vector3.right * (input.magnitude * tongueMaxSize)
            : transform.position;

        tongueScope.transform.localPosition = tongueTargetPosition;
    }

    #endregion

    #region Tongue Logic

    private IEnumerator TongueAnimation()
    {
        Debug.Log($"{nameof(Frog)}: Tongue released");

        float position = 0f;
        float targetX = tongueTargetPosition.x;

        tongueLineRenderer.SetPosition(0, transform.localPosition);
        tongueLineRenderer.SetPosition(1, Vector3.zero);

        // Extend tongue
        while (position < targetX)
        {
            position += Time.deltaTime * tongueSpeed;
            tongueLineRenderer.SetPosition(1, new Vector3(position, 0f, 0f));

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
            yield return null;
        }

        isTongueReleasing = false;
    }

    private IEnumerator CatchFly()
    {
        Debug.Log($"{nameof(Frog)}: Catching fly...");

        float timer = catchDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isCatching = false;
        Debug.Log($"{nameof(Frog)}: Done catching");
    }

    #endregion
}
