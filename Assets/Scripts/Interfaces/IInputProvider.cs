using UnityEngine;

public interface IInputProvider
{
    Vector2 GetInputPosition();
    bool IsHoldingDown();
    bool IsPressingDown();
}