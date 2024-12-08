using UnityEngine;

// We don't need multitouch for this project so we'll use a basic logic:
public class TouchInputProvider : IInputProvider
{
    public Vector2 GetInputPosition() => Input.GetTouch(0).position;
    public bool IsHoldingDown() => Input.touchCount > 0;
    public bool IsPressingDown() => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
}