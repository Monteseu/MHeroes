using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileView : MonoBehaviour
{
    private float damage;
    private ITargetable target;
    [SerializeField]
    private float speed = 10f;
    private Rigidbody rigidBody;
    [SerializeField]
    private ParticleSystem trailVFX;

    private Action<ProjectileView> releaseToPool;
    // caché the aiming offset so we don't shoot at its feet.
    Vector3 targetOffset = Vector3.up;
    public void Initialize(float damage, ITargetable target)
    {
        this.damage = damage;
        this.target = target;
        // May not be null if it was in the pool.
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        Vector3 direction = (target.GetTransform().position + targetOffset - transform.position).normalized;
        rigidBody.velocity = direction * speed;

        if (trailVFX)
            trailVFX.Play();
    }

    public void SetReleaseCallback(Action<ProjectileView> releaseCallback) => releaseToPool = releaseCallback;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITargetable hitTarget))
        {
            if (hitTarget == target && hitTarget.IsAlive())
            {
                if (hitTarget is IDamageable dmg)
                    dmg.TakeDamage(damage);
            }
        }

        if (trailVFX)
            trailVFX.Stop();

        ReleaseProjectile();
    }

    private void ReleaseProjectile()
    {
        if (rigidBody != null)
            rigidBody.velocity = Vector3.zero;
        target = null;
        releaseToPool?.Invoke(this);
    }
}
