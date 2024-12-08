using System;
using UnityEngine;
/// <summary>
/// We can use this Handler to encapsulate the battle logic for the player AND any NPC.
/// It gets the updated version of the relevant states of the entity every time it's needed.
/// To take this further, we could create different handlers (with different rules and logic) with a common interface
/// </summary>
public class CombatHandler
{
    private ITargetable owner;
    private Func<float> getAttackDamage;
    private Func<float> getAttackRate;
    private Func<float> getAttackRange;

    private Action<CharacterState> setState;
    private Action<ITargetable, float> applyDamageAction;

    private ITargetable currentTarget;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    public CombatHandler(
        ITargetable owner,
        Func<float> getAttackDamage,
        Func<float> getAttackRate,
        Func<float> getAttackRange,
        Action<CharacterState> setState,
        Action<ITargetable, float> applyDamageAction)
    {
        this.owner = owner;
        this.getAttackDamage = getAttackDamage;
        this.getAttackRate = getAttackRate;
        this.getAttackRange = getAttackRange;
        this.setState = setState;
        this.applyDamageAction = applyDamageAction;
    }

    public void SetTarget(ITargetable target) => currentTarget = target;

    public ITargetable GetTarget() => currentTarget;

    public void ClearTarget()
    {
        currentTarget = null;
        isAttacking = false;
    }

    public void UpdateCombat()
    {
        if (currentTarget == null || !currentTarget.IsAlive())
        {
            ClearTarget();
            setState(CharacterState.Idle);
            return;
        }

        float distance = Vector3.Distance(owner.GetTransform().position, currentTarget.GetTransform().position);
        if (distance > getAttackRange())
        {
            ClearTarget();
            setState(CharacterState.Idle);
            return;
        }

        if (isAttacking)
            return;

        if (Time.time >= nextAttackTime)
        {
            setState(CharacterState.Attack);
            isAttacking = true;
        }
        else
            setState(CharacterState.Idle);
    }
    public void OnAttackAnimationEvent()
    {
        if (isAttacking && currentTarget != null && currentTarget.IsAlive())
        {
            float damage = getAttackDamage();
            applyDamageAction(currentTarget, damage);
        }

        isAttacking = false;
        nextAttackTime = Time.time + (1f / getAttackRate());
    }
    public void ResetAttackTimer()
    {
        isAttacking = false;
        nextAttackTime = Time.time + (1f / getAttackRate());
    }
}
