using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;

public class Frog : MonoBehaviour
{
    public Gamepad InputController;
    private Vector3 tonguePosition;
    bool isCatching = false;
    bool isTongueReleasing = false;
    
    [SerializeField]
    private GameObject frogObject;
    [SerializeField]
    private GameObject tongueScope;
    [SerializeField]
    private float tongueMaxSize = 4f;
    [SerializeField]
    private LineRenderer tongueLineRenderer;
    [SerializeField]
    private float tongueSpeed = 1f;
    [SerializeField]
    private float catchTimeCounter = 0.5f;
    [SerializeField]
    private float catchOffsetStart = 0.01f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputController != null)
        {
            if (!isTongueReleasing)
            {
                Vector2 leftStickValues = InputController.leftStick.ReadValue();
                RotateFrog(leftStickValues);
                MoveFrogScope(leftStickValues);
                if (InputController.crossButton.wasPressedThisFrame)
                {
                    isTongueReleasing = true;
                    StartCoroutine(TongueAnimation());
                }
            }
        }
        else
        {
            Debug.Log("No controller assigned");
        }
    }

    void RotateFrog(Vector2 inputValues)
    {
        if (inputValues.sqrMagnitude < 0.01f)
        {
            return;
        }
        
        float angle = Mathf.Atan2(inputValues.y, inputValues.x) * Mathf.Rad2Deg;
        frogObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void MoveFrogScope(Vector2 inputValues)
    {
        if (inputValues.sqrMagnitude > 0.01f)
        {
            float distance = inputValues.magnitude * tongueMaxSize;
            tonguePosition = Vector3.right * distance;
        }
        else
        {
            tonguePosition = transform.position;
        }

        tongueScope.transform.localPosition = tonguePosition;
    }

    IEnumerator TongueAnimation()
    {
        Debug.Log("ReleaseTongue");
        tongueLineRenderer.SetPosition(0, transform.localPosition);
        float position = 0;
        tongueLineRenderer.SetPosition(1,Vector3.zero);

        var targetTonguePosition = tonguePosition.x;

        
        
        while (position < targetTonguePosition) //growing tongue
        {
            //Debug.Log(position);
            Vector3 linePosition = new Vector3(position, 0, 0);
            tongueLineRenderer.SetPosition(1,linePosition);
            position += Time.deltaTime * tongueSpeed;
            if (targetTonguePosition - position < catchOffsetStart && !isCatching) //start tongue catching
            {
                isCatching = true;
                StartCoroutine(CatchFly());
            }
            yield return null;
        }

        while (position > 0) // ungrowing tongue
        {
            //Debug.Log(position);
            position -= Time.deltaTime * tongueSpeed;
            Vector3 linePosition = new Vector3(position, 0, 0);
            tongueLineRenderer.SetPosition(1,linePosition);
            yield return null;
        }
        isTongueReleasing = false;
        yield return null;
    }

    IEnumerator CatchFly()
    {
        Debug.Log("CatchFly");
        float counter = catchTimeCounter;
        
        while(counter > 0)
        {
            counter -= Time.deltaTime;
            Debug.Log(counter + "is catching");
            yield return null;
        }
        isCatching = false;
        Debug.Log("Not catching");
        yield return null;
    }
}