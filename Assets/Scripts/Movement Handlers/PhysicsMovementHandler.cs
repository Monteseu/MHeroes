using System;
using UnityEngine;

public class PhysicsMovementHandler : IMovable
{
    private Rigidbody rigidbody;
    private const float ROTATION_SPEED = 10f;
    bool isActive = true;
    public bool IsActive { get => isActive; set => isActive = value; }
    public PhysicsMovementHandler(Rigidbody rigidbody)
    {
        if (rigidbody == null)
            throw new NullReferenceException("Physics Movement Handler can't find a rigidbody");
        this.rigidbody = rigidbody;
    }

    public void MoveTowards(Vector3 direction, float speed)
    {
        if (!isActive)
            return;

        Vector3 movement = direction.normalized * speed;
        rigidbody.velocity = new Vector3(movement.x, 0, movement.z);
    }
    public void SetRotation(Vector3 direction)
    {
        if (direction.magnitude == 0 || !isActive)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        Quaternion smoothRotation = Quaternion.Slerp(
            rigidbody.rotation,
            targetRotation,
            ROTATION_SPEED * Time.fixedDeltaTime
        );

        rigidbody.MoveRotation(smoothRotation);
    }

    public void Stop()
    {
        // Note:  We could skip this and control deceleration by Rigidbody's Drag, but let's stick to the archero feeling.
        rigidbody.velocity = new Vector3(0, 0, 0);
    }
}
