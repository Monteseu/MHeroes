using UnityEngine;
using UnityEngine.Events;

//  Assignment Notes:
//  - Trying to get the Archero feel. Teleport on press point and drag from there. 
//  - Joystick doesn't need to know about the rest of the game. Useful to drag and drop to kickstart other prototypes.
//  - I'm Leaving OnMoveJoystick event public to subscribe from the outside OR reference from the inspector.
public class JoystickController : MonoBehaviour
{
    public RectTransform joystickBG;
    public RectTransform innerStick;

    private Vector3 originalPosition;
    private Vector2 directionOutput;
    private bool isDragging = false;

    private IInputProvider inputProvider;
    Vector2 touchPosition;

    public UnityEvent<Vector2> OnMoveJoystick;

    void Start()
    {
#if UNITY_EDITOR
        inputProvider = new DebugInputProvider();
#else
        inputProvider = new TouchInputProvider();
#endif
        originalPosition = joystickBG.transform.localPosition;
    }

    void Update() => HandleInput();

    void HandleInput()
    {
        touchPosition = inputProvider.GetInputPosition();

        if (inputProvider.IsHoldingDown())
        {
            if (!isDragging)
                OnBeginDrag(touchPosition);
            else
                OnDrag();
        }
        else if (isDragging)
            OnEndDrag();
    }
    private void OnBeginDrag(Vector2 touchPosition)
    {
        isDragging = true;
        joystickBG.gameObject.SetActive(true);
        joystickBG.position = touchPosition;
        innerStick.localPosition = Vector3.zero;
    }
    private void OnDrag()
    {
        Vector2 direction = touchPosition - (Vector2)joystickBG.position;
        float radius = joystickBG.sizeDelta.x / 2f;
        direction = Vector2.ClampMagnitude(direction, radius);
        innerStick.localPosition = direction;
        directionOutput = direction / radius;
        OnMoveJoystick?.Invoke(directionOutput);
    }
    private void OnEndDrag()
    {
        isDragging = false;
        directionOutput = Vector2.zero;
        innerStick.localPosition = Vector3.zero;
        joystickBG.localPosition = originalPosition;
        // Invoke one more time to send an stop order.
        OnMoveJoystick?.Invoke(directionOutput);
    }
}