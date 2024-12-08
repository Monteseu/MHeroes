using UnityEngine;

public abstract class CharacterView : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected CharacterState currentState = CharacterState.Idle;

    private void Start() => OnStateChange(currentState); // Trigger the default state (Idle).

    public virtual void SetState(CharacterState newState)
    {
        currentState = newState;
        OnStateChange(newState);
    }

    public void OnStateChange(CharacterState newState)
    {
        switch (newState)
        {
            case CharacterState.Idle:
                ClearAttackState();
                OnIdleState();
                break;
            case CharacterState.Move:
                ClearAttackState();
                OnMoveState();
                break;
            case CharacterState.Attack:
                OnAttackState();
                break;
            case CharacterState.Die:
                ClearAttackState();
                OnDieState();
                break;
        }
    }

    // Override state behaviours on children if needed
    protected virtual void OnIdleState() => animator.SetBool("IsMoving", false);
    protected virtual void OnMoveState() => animator.SetBool("IsMoving", true);
    protected virtual void OnAttackState() => animator.SetBool("IsAttacking", true);
    protected virtual void OnDieState() => animator.SetTrigger("Die");
    protected virtual void OnImpact() => animator.SetTrigger("Impact");
    protected virtual void ClearAttackState() => animator.SetBool("IsAttacking", false);
}