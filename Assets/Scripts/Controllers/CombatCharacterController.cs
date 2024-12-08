using UnityEngine;

public abstract class CombatCharacterController : MonoBehaviour, IDamageable, ITargetable, ICombatAnimationEventsReceiver
{
    protected IMovable movementHandler;
    protected CharacterView characterView;
    [SerializeField]
    protected HealthBarView healthBarView;
    protected float currentMovementSpeed = 5f;

    protected float maxHealthPoints = 100f;
    protected float currentHealthPoints;

    public float MaxHealthPoints => maxHealthPoints;
    public float CurrentHealthPoints => currentHealthPoints;
    protected CombatHandler combatHandler;
    protected bool isMoving;
    public Transform GetTransform() => transform;
    public bool IsAlive() => currentHealthPoints > 0f;

    protected virtual void Awake() => InitializeMovementHandler();
    protected abstract void InitializeMovementHandler();
    protected virtual void HandleMovement(Vector2 direction)
    {
        isMoving = direction.magnitude != 0;
        if (!isMoving)
        {
            movementHandler.Stop();
            characterView.SetState(CharacterState.Idle);
        }
        else
        {
            Vector3 movementDirection = new Vector3(-direction.x, 0, -direction.y);
            movementHandler.MoveTowards(movementDirection, GetMovementSpeed());
            movementHandler.SetRotation(movementDirection);
            characterView.SetState(CharacterState.Move);
        }
    }

    protected virtual float GetMovementSpeed() => currentMovementSpeed;

    public virtual void TakeDamage(float amount)
    {
        currentHealthPoints = Mathf.Max(currentHealthPoints - amount, 0f);

        UpdateHealthBar();

        if (currentHealthPoints <= 0f)
            Die();
        else
            characterView.SetState(CharacterState.Impact);
    }
    protected void UpdateHealthBar()
    {
        if (healthBarView != null)
        {
            float normalizedHealth = currentHealthPoints / maxHealthPoints;
            healthBarView.SetHealthFill(normalizedHealth);
        }
    }

    public abstract void SetupCombatHandler(CombatCharacterModelData data);

    public virtual void Die()
    {
        healthBarView.gameObject.SetActive(false);
        characterView.SetState(CharacterState.Die);
        movementHandler.Stop();
        movementHandler.IsActive = false;
        enabled = false;
    }
    public virtual void OnAttackAnimationEvent() => combatHandler.OnAttackAnimationEvent();
}
