using UnityEngine;

// Note: I called it "Debug.." because it is assumed that if we play from the pc we are in the editor. 
public class DebugInputProvider : IInputProvider
{
    public Vector2 GetInputPosition() => Input.mousePosition;
    public bool IsHoldingDown() => Input.GetMouseButton(0);
    public bool IsPressingDown() => Input.GetMouseButtonDown(0);
}
