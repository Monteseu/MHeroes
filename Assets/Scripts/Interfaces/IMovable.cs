using UnityEngine;

public interface IMovable
{
    bool IsActive { get; set; } 
    void MoveTowards(Vector3 target, float speed);
    void SetRotation(Vector3 direction);
    void Stop();
}