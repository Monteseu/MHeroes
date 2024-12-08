public interface IDamageable
{
    float MaxHealthPoints { get; }
    float CurrentHealthPoints { get; }
    void TakeDamage(float amount);
    void Die();
}
