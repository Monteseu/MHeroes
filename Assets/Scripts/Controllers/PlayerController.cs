using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerController : CombatCharacterController
{
    [SerializeField]
    private JoystickController joystickController;
    [SerializeField]
    private bool usePhysicsMovement = true;
    AsyncOperationHandle<GameObject> characterHandle;
    [SerializeField]
    GameObject enemyInteractionTrigger;
    HeroModelData heroModelData;
    HeroWeaponHandler weaponHandler;

    private HashSet<EnemyController> enemiesInRange = new HashSet<EnemyController>();
    private HashSet<ITargetable> targetsInArea = new HashSet<ITargetable>();

    float currentAttackDamage;
    float currentAttackRate;
    float currentAttackRange;
    string currentWeaponID;
    public float AttackDamage { get => currentAttackDamage; set => currentAttackDamage = value; }
    public float AttackRate { get => currentAttackRate; set => currentAttackRate = value; }
    public float AttackRange { get => currentAttackRange; set => currentAttackRange = value; }

    protected void Start() => joystickController.gameObject.SetActive(false);

    public void Initialize(string heroID, string weaponID)
    {
        currentWeaponID = weaponID;
        heroModelData = ServicesManager.Get().GetService<ModelDataService>().GetLibrary<HeroesModelLibrary>().GetHeroByID(heroID);
        AssetReference defaultHeroRef = heroModelData.prefabReference;
        characterHandle = Addressables.LoadAssetAsync<GameObject>(defaultHeroRef);
        characterHandle.Completed += OnCharacterLoaded;

        maxHealthPoints = heroModelData.MaxHealthPoints;
        currentHealthPoints = maxHealthPoints;
        currentMovementSpeed = heroModelData.BaseMoveSpeed;
        UpdateHealthBar();
    }

    private void OnCharacterLoaded(AsyncOperationHandle<GameObject> characterOperation)
    {
        if (characterOperation.Status == AsyncOperationStatus.Succeeded)
        {
            characterView = Instantiate(characterOperation.Result, transform).GetComponent<CharacterView>();
            if (characterView != null)
                joystickController.gameObject.SetActive(true);
            else
                Debug.LogError("CharacterView component not found on the instantiated character prefab");
        }
        else
            Debug.LogError("Failed to load and instantiate character prefab");

        healthBarView.gameObject.SetActive(true);
        enemyInteractionTrigger.SetActive(true);
        SetupCombatHandler(heroModelData);

        weaponHandler = new HeroWeaponHandler(this, characterView as HeroCharacterView);
        weaponHandler.EquipWeaponByID(currentWeaponID);
    }
    public override void SetupCombatHandler(CombatCharacterModelData data)
    {
        currentAttackDamage = data.BaseAttackDamage;
        currentAttackRate = data.BaseAttackRate;
        currentAttackRange = data.BaseAttackRange;

        Func<float> getAttackDamage = () => currentAttackDamage;
        Func<float> getAttackRate = () => currentAttackRate;
        Func<float> getAttackRange = () => currentAttackRange;

        combatHandler = new CombatHandler(
            owner: this,
            getAttackDamage: getAttackDamage,
            getAttackRate: getAttackRate,
            getAttackRange: getAttackRange,
            setState: s => characterView.SetState(s),
            applyDamageAction: ApplyDamageToTarget
        );
    }
    protected override void InitializeMovementHandler()
    {
        joystickController.OnMoveJoystick.AddListener(HandleMovement);

        if (usePhysicsMovement)
            movementHandler = new PhysicsMovementHandler(GetComponent<Rigidbody>());
        else
            movementHandler = new TransformMovementHandler(transform);
    }

    private void Update()
    {
        if (combatHandler == null)
            return;

        if (!isMoving)
        {
            if (combatHandler.GetTarget() == null)
            {
                ITargetable newTarget = SelectClosestTargetInRange();
                if (newTarget != null)
                    combatHandler.SetTarget(newTarget);
            }
            else
                movementHandler.SetRotation(combatHandler.GetTarget().GetTransform().position - transform.position);

            combatHandler.UpdateCombat();
        }
        else
            combatHandler.ClearTarget();
    }

    private void OnDestroy()
    {
        if (characterHandle.IsValid())
            Addressables.Release(characterHandle);
    }

    private ITargetable SelectClosestTargetInRange()
    {
        List<ITargetable> validTargets = targetsInArea.Where(t => t != null && t.IsAlive() &&
                        Vector3.Distance(transform.position, t.GetTransform().position) <= currentAttackRange).ToList();

        if (validTargets.Count == 0)
            return null;

        return validTargets.OrderBy(t => Vector3.Distance(transform.position, t.GetTransform().position))
                           .FirstOrDefault();
    }

    private void ApplyDamageToTarget(ITargetable target, float damage)
    {
        if (target is IDamageable dmg)
            dmg.TakeDamage(damage);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITargetable target) && (object)target != this)
        {
            targetsInArea.Add(target);
            if (target is EnemyController enemy)
            {
                enemy.OnEnemyDefeated += HandleEnemyDefeated;
                enemy.NotifyHeroEnteredArea(transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ITargetable target) && targetsInArea.Contains(target))
        {
            targetsInArea.Remove(target);
            if (target is EnemyController enemy)
            {
                enemy.OnEnemyDefeated -= HandleEnemyDefeated;
                enemy.NotifyHeroExitedArea();
            }

            if (combatHandler.GetTarget() == target)
                combatHandler.ClearTarget();
        }
    }

    private void HandleEnemyDefeated(EnemyController killedEnemy)
    {
        if (targetsInArea.Contains(killedEnemy))
            targetsInArea.Remove(killedEnemy);

        if ((object)combatHandler.GetTarget() == killedEnemy)
            combatHandler.ClearTarget();
    }

    public override void OnAttackAnimationEvent()
    {
        if (weaponHandler != null && weaponHandler.IsRangedWeapon())
        {
            var target = combatHandler.GetTarget();
            if (target != null && target.IsAlive())
            {
                weaponHandler.SpawnProjectile(target, AttackDamage);
            }
            combatHandler.ResetAttackTimer();
        }
        else
            combatHandler.OnAttackAnimationEvent();
    }

}
