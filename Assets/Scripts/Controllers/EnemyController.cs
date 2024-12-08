using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CombatCharacterController
{
    public event Action<EnemyController> OnEnemyDefeated;
    [SerializeField]
    NavMeshAgent agent;

    private Transform heroTransform;
    private bool heroInLargeArea = false;
    private bool isChassingPlayer = false;
    private EnemyModelData modelData;

    protected override void Awake()
    {
        base.Awake();
        characterView = GetComponentInChildren<CharacterView>();
    }
    protected override void InitializeMovementHandler() => movementHandler = new NavMeshMovementHandler(agent);
    public void SetupData(EnemyModelData data)
    {
        modelData = data;
        maxHealthPoints = data.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        currentMovementSpeed = data.BaseMoveSpeed;

        SetupCombatHandler(data);
        UpdateHealthBar();
    }
    // Note: Enemies stats may not change during its lifetime, but maybe we implement some buffs or debuffs for them in the future, so it's still usefull
    public override void SetupCombatHandler(CombatCharacterModelData data)
    {
        Func<float> getAttackDamage = () => data.BaseAttackDamage;
        Func<float> getAttackRate = () => data.BaseAttackRate;
        Func<float> getAttackRange = () => data.BaseAttackRange;

        combatHandler = new CombatHandler(
            owner: this,
            getAttackDamage: getAttackDamage,
            getAttackRate: getAttackRate,
            getAttackRange: getAttackRange,
            setState: newState => characterView.SetState(newState),
            applyDamageAction: ApplyDamageToTarget
        );
    }

    private void Update()
    {
        if (heroInLargeArea && heroTransform != null && combatHandler != null)
        {
            float distToHero = Vector3.Distance(transform.position, heroTransform.position);
            if (distToHero <= modelData.detectionRange && !isChassingPlayer)
                StartChassing();
            // Do we want enemies to stop chassing the player if he's out of range? Just uncomment.
            //else if (distToHero > modelData.detectionRange && isAggro) 
            //StopAggro();

            if (isChassingPlayer)
                ChaseAndAttackTarget(distToHero);
            else
                characterView.SetState(CharacterState.Idle);
        }
        else
            characterView.SetState(CharacterState.Idle);
    }
    private void StartChassing()
    {
        isChassingPlayer = true;
        ITargetable heroTarget = heroTransform.GetComponentInParent<ITargetable>();
        combatHandler.SetTarget(heroTarget);
    }

    private void StopChassing()
    {
        isChassingPlayer = false;
        movementHandler.Stop();
        combatHandler.ClearTarget();
        characterView.SetState(CharacterState.Idle);
    }

    private void ChaseAndAttackTarget(float distToHero)
    {
        if (distToHero > modelData.BaseAttackRange)
        {
            movementHandler.MoveTowards(heroTransform.position, GetMovementSpeed());
            characterView.SetState(CharacterState.Move);
        }
        else
        {
            movementHandler.Stop();
            combatHandler.UpdateCombat();
        }
    }

    private void ApplyDamageToTarget(ITargetable target, float damage)
    {
        if (target is IDamageable dmg)
            dmg.TakeDamage(damage);
    }

    public override void TakeDamage(float amount)
    {
        if (!isChassingPlayer)
            StartChassing();
        base.TakeDamage(amount);
    }
    public override void Die()
    {
        base.Die();
        StartCoroutine(PoolRespawnDelay());
    }
    IEnumerator PoolRespawnDelay()
    {
        yield return new WaitForSeconds(1f);
        OnEnemyDefeated?.Invoke(this);
    }

    public void ResetEnemy()
    {
        currentHealthPoints = maxHealthPoints;
        healthBarView.gameObject.SetActive(true);
        UpdateHealthBar();
        characterView.SetState(CharacterState.Idle);
        enabled = true;
        movementHandler.IsActive = true;
        heroTransform = null;
        heroInLargeArea = false;
        StopChassing();
    }

    public void NotifyHeroEnteredArea(Transform hero)
    {
        heroTransform = hero;
        heroInLargeArea = true;
    }

    public void NotifyHeroExitedArea()
    {
        heroInLargeArea = false;
        heroTransform = null;
        StopChassing();
    }
}