using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HeroWeaponHandler
{
    private PlayerController playerController;
    private HeroCharacterView heroView;
    private WeaponModelData currentWeaponData;
    private GameObject currentWeaponInstance;

    private DynamicPool<ProjectileView, ProjectileSpawnContext> projectilePool;

    public HeroWeaponHandler(PlayerController playerController, HeroCharacterView heroView)
    {
        this.playerController = playerController;
        this.heroView = heroView;
    }

    public void EquipWeaponByID(string id)
    {
        var weapon = ServicesManager.Get()
            .GetService<ModelDataService>()
            .GetLibrary<WeaponsModelLibrary>()
            .GetWeaponByID(id);

        if (weapon == null)
        {
            Debug.LogWarning("No weapons found in library.");
            return;
        }

        currentWeaponData = weapon;

        var handle = currentWeaponData.prefabReference.InstantiateAsync();
        handle.Completed += OnWeaponInstantiated;
    }

    private void OnWeaponInstantiated(AsyncOperationHandle<GameObject> weaponOperation)
    {
        if (weaponOperation.Status == AsyncOperationStatus.Succeeded)
        {
            currentWeaponInstance = weaponOperation.Result;

            if (heroView.weaponMountPoint != null)
            {
                currentWeaponInstance.transform.SetParent(heroView.weaponMountPoint, false);
                currentWeaponInstance.transform.localPosition = Vector3.zero;
                currentWeaponInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("No weapon mount point found on HeroCharacterView");
            }

            ApplyWeaponStats();

            if (IsRangedWeapon())
            {
                var rangedData = (RangedWeaponModelData)currentWeaponData;
                projectilePool = new DynamicPool<ProjectileView, ProjectileSpawnContext>(
                    (context) => rangedData.projectilePrefabReference.InstantiateAsync(),
                    OnProjectileGet,
                    OnProjectileRelease
                );
                // Pre-pool some projectiles so they're instanced at time for the first time.
                playerController.StartCoroutine(projectilePool.Preload(3, new ProjectileSpawnContext()));
            }
        }
        else
            Debug.LogError("Failed to instantiate weapon prefab");
    }

    private void ApplyWeaponStats()
    {
        if (currentWeaponData == null) return;

        playerController.AttackRate *= currentWeaponData.AttackSpeedBonus;
        playerController.AttackRange *= currentWeaponData.AttackRangeBonus;

        if (heroView.TryGetComponent(out Animator animator))
            animator.speed *= currentWeaponData.AttackSpeedBonus;
    }

    public bool IsRangedWeapon() => currentWeaponData != null && currentWeaponData is RangedWeaponModelData;

    public void SpawnProjectile(ITargetable target, float damage)
    {
        if (!IsRangedWeapon())
        {
            Debug.LogWarning("Attempting to spawn projectile with a non-ranged weapon.");
            return;
        }

        var context = new ProjectileSpawnContext(target, damage);
        playerController.StartCoroutine(projectilePool.GetOrCreate(context));
    }

    private void OnProjectileGet(ProjectileView projectile, ProjectileSpawnContext context)
    {
        if (projectile == null) return;
        projectile.SetReleaseCallback((p) => projectilePool.Release(p));
        projectile.gameObject.SetActive(true);

        // Posicionamos el proyectil en el punto de montaje del arma
        projectile.transform.position = heroView.weaponMountPoint.position;
        Vector3 dir = (context.target.GetTransform().position - projectile.transform.position).normalized;
        projectile.transform.rotation = Quaternion.LookRotation(dir);

        projectile.Initialize(context.damage, context.target);
    }

    private void OnProjectileRelease(ProjectileView projectile)
    {
        if (projectile == null) return;
        projectile.gameObject.SetActive(false);
    }
}

public struct ProjectileSpawnContext
{
    public ITargetable target;
    public float damage;

    public ProjectileSpawnContext(ITargetable target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }
}
