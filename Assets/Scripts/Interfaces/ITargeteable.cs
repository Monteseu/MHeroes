using UnityEngine;

public interface ITargetable
{
    Transform GetTransform();
    bool IsAlive();
    float CurrentHealthPoints { get; }
}