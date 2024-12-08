
public class EnemyCharacterView : CharacterView
{
    protected override void OnIdleState() => animator.SetBool("IsMoving", false);
    protected override void OnMoveState() => animator.SetBool("IsMoving", true);

    // Note: The hero animator works with an "Attack" bool, thile Enemy animator works with an attack trigger.
    // I coul've used the same in the animators but wanted to show the flexibility of the pattern.
    protected override void OnAttackState() => animator.SetTrigger("Attack");
    protected override void OnDieState() => animator.SetTrigger("Die");
    protected override void ClearAttackState() => animator.ResetTrigger("Attack");
}
