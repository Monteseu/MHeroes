using UnityEngine;

// NOTE: I'll focus on the physics based movement so I'm leaving a very basic logic just as an abstraction showoff. 

public class TransformMovementHandler : IMovable
{
    private Transform transform;
    bool isActive = true;
    public bool IsActive { get => isActive; set => isActive = value; }
    public TransformMovementHandler(Transform transform)
    {
        this.transform = transform;
    }

    public void MoveTowards(Vector3 direction, float speed)
    {
        if (!isActive)
            return;

        Vector3 movement = direction.normalized * speed;
        transform.Translate(movement * Time.deltaTime, Space.World);
    }

    public void SetRotation(Vector3 direction)
    {
        if (!isActive)
            return;
        direction = direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void Stop()
    {
        // No need to add anything for instant stop. Useful to add deceleration logic if needed, but let's skip this for now!
    }
}