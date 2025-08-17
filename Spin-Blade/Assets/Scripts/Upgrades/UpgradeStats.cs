using UnityEngine;
public class UpgradeStats : MonoBehaviour
{
    [Header("|---------- Effects ----------|")]
    [Space(20)]
    [Header("|----- Player -----|")]

    [Header("Health")]
    public float healthIncrease;
    public float regenIncrease;
    public float healthOnKillIncrease;
    public int reviveIncreases;


    [Header("Speed & Size")]
    public float speedIncrease;
    public float sizeIncrease;

    [Header("Damage")]
    public float damageIncrease;
    public float knockbackIncrease;
    public float knockbackDurationIncrease;
    [Header("|----- Player -----|")]
    [Space(20)]
    [Header("|----- Special Player Stats -----|")]
    [Header("-- Mini Saws")]

    [Header("Unlocks")]
    public bool spawnMiniSaw;

    [Header("Stats")]
    public float miniSawSpeedIncrease;
    public float miniSawDamageIncrease;

    [Space(20)]
    [Header("-- Triangle Projectiles")]

    [Header("Unlocks")]
    public bool unlockShootingTriangles;
    public bool rangedAutofire;
    public bool homingTriangles;
    public bool piercingTriangles;

    [Header("Stats")]
    public float triangleDamageIncrease;
    public float triangleSpeedIncrease;
    public float triangleFireRateIncrease;

    [Space(20)]
    [Header("-- Exploding Circle")]

    [Header("Stats")]
    public float explodingCircleCooldownIncrease;
    public float explodingCircleDamageMultiplierIncrease;

    [Header("Unlocks")]
    public bool unlockExplodingCircle;
    public bool unlockExplodingCircleKnockback;

    [Space(20)]
    [Header("-- Mines")]

    [Header("Stats")]
    public float mineExplosionRadiusIncrease;
    public float mineDamageMultiplierIncrease;
    public float mineKnockbackIncrease;
    public float mineLifetimeIncrease;
    public float mineCooldownIncrease;

    [Header("Unlocks")]
    public bool unlockMines;
    public bool explodingMines;
    
    [Header("|----- Special Player Stats -----|")]
    [Space(20)]
    [Header("|----- Enemies -----|")]

    [Header("Enemies")]
    public GameObject addEnemy; // leave null to not add any enemies to the spawning
    public float enemySpeedMultiplierIncrease;
    public float enemyDifficultyIncrease;
    public float enemySpawnRateIncrease;

    [Header("Bosses")]
    public float enemyBossHealthMultiplierIncrease;

    [Header("|----- Enemies -----|")]
    [Space(20)]
    [Header("|----- Other -----|")]

    [Header("Money")]
    public float moneyMultiplierIncrease;
    public float passiveIncomeIncrease;

    [Header("Win")]
    public bool win;

    [Header("|----- Other -----|")]
    [Space(20)]
    [Header("|---------- Effects ----------|")]
    [Space(20)]

    [SerializeField]
    private string sectionSeparator = "";

    GameObject player;
    MoneyManager moneyManager;
    EnemyManager enemyManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
    }
    public void ApplyEffects()
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        PlayerHealthAndDamage playerHealth = player.GetComponent<PlayerHealthAndDamage>();

        // --- PLAYER ---
        // -- damage
        // speed
        playerMovement.speed += speedIncrease;
        // size
        playerHealth.sizeMultiplier += sizeIncrease;
        // damage
        playerHealth.damage += damageIncrease;
        // kb & stun
        playerHealth.knockbackDistance += knockbackIncrease;
        playerHealth.knockbackDuration += knockbackDurationIncrease;

        // -- health
        // increase max health
        playerHealth.IncreaseMaxHealth(healthIncrease);
        // revive
        playerHealth.revives += reviveIncreases;
        // increase regen
        playerHealth.regenPerSecond += regenIncrease;
        // knockback & stun time

        // health on kill
        playerHealth.killRegenAmount += healthOnKillIncrease;
        // --- PLAYER ---

        // --- MONEY ---
        // multiplier
        moneyManager.moneyMultiplier += moneyMultiplierIncrease;
        // passive income
        moneyManager.passiveIncome += passiveIncomeIncrease;
        // --- MONEY ---

        // --- MINI SAWS ---
        // spawn
        if (spawnMiniSaw)
            playerHealth.SpawnSaw();
        // stats
        foreach (GameObject miniSaw in playerHealth.miniSaws)
        {
            miniSaw.GetComponent<PlayerMiniSaw>().IncreaseSpeed(miniSawSpeedIncrease);
            miniSaw.GetComponent<Projectile>().damage += miniSawDamageIncrease;
        }
        // --- MINI SAWS ---

        // --- SHOOTING TRIANGLES ---
        // unlocks
        if (unlockShootingTriangles)
            playerHealth.unlockedRangedTriangles = true;

        if (!playerHealth.autofireTriangles)
            playerHealth.autofireTriangles = rangedAutofire;
        if (!playerHealth.homingTriangles)
            playerHealth.homingTriangles = homingTriangles;
        if (!playerHealth.piercingTriangles)
            playerHealth.piercingTriangles = piercingTriangles;
        // stats
        playerHealth.triangleDamage += triangleDamageIncrease;
        playerHealth.triangleSpeed += triangleSpeedIncrease;
        playerHealth.triangleFireRate += triangleFireRateIncrease;
        // --- SHOOTING TRIANGLES ---

        // --- EXPLODING CIRCLE ---
        // unlocks
        if (!playerHealth.explodingCircle)
            playerHealth.explodingCircle = unlockExplodingCircle;
        if (!playerHealth.explodingCircleKnockback)
            playerHealth.explodingCircleKnockback = unlockExplodingCircleKnockback;
        // stats
        playerHealth.explodingCircleCooldown += explodingCircleCooldownIncrease;
        playerHealth.explodingCircleDamageMultiplier += explodingCircleDamageMultiplierIncrease;
        // --- EXPLODING CIRCLE ---

        // --- MINES ---
        // unlocks
        if (!playerHealth.mines)
            playerHealth.mines = unlockMines;
        if (!playerHealth.explodingMines)
            playerHealth.explodingMines = explodingMines;
        // stats
        playerHealth.mineExplosionRadius += mineExplosionRadiusIncrease;
        playerHealth.mineDamageMultiplier += mineDamageMultiplierIncrease;
        playerHealth.mineDamageMultiplier += mineKnockbackIncrease;
        playerHealth.minesLifetime += mineLifetimeIncrease;
        playerHealth.minesCooldown += mineCooldownIncrease;
        // --- MINES ---

        // --- ENEMIES ---
        // add enemy
        if (addEnemy != null)
        {
            EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
            enemyManager.enemies.Add(addEnemy);
        }
        // boss health mult
        enemyManager.bossHealthMultiplier += enemyBossHealthMultiplierIncrease;
        // spawnrate
        enemyManager.spawnRate += enemySpawnRateIncrease;
        // speed multiplier
        enemyManager.enemySpeedMultiplier += enemySpeedMultiplierIncrease;
        // --- ENEMIES ---

        // --- WIN ---
        if (win)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().Win();
        }
    }
}
