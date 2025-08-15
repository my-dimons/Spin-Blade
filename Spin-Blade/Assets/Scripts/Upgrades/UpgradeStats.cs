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

        // increase speed
        playerMovement.speed += speedIncrease;
        // increase size
        playerHealth.sizeMultiplier += sizeIncrease;
        // increase money multiplier
        moneyManager.moneyMultiplier += moneyMultiplierIncrease;
        // increase passive income
        moneyManager.passiveIncome += passiveIncomeIncrease;
        // increase max health
        playerHealth.IncreaseMaxHealth(healthIncrease);
        // revive
        playerHealth.revives += reviveIncreases;
        // increase damage
        playerHealth.damage += damageIncrease;
        // increase regen
        playerHealth.regenPerSecond += regenIncrease;
        // increase knockback & stun time
        playerHealth.knockbackDistance += knockbackIncrease;
        playerHealth.knockbackDuration += knockbackDurationIncrease;
        // increase enemy speed multiplier
        enemyManager.enemySpeedMultiplier += enemySpeedMultiplierIncrease;

        playerHealth.killRegenAmount += healthOnKillIncrease;

        // buy mini saws
        if (spawnMiniSaw)
            playerHealth.SpawnSaw();
        // mini saw stats
        foreach (GameObject miniSaw in playerHealth.miniSaws)
        {
            miniSaw.GetComponent<PlayerMiniSaw>().IncreaseSpeed(miniSawSpeedIncrease);
            miniSaw.GetComponent<Projectile>().damage += miniSawDamageIncrease;
        }

        // unlock shooting triangles
        if (unlockShootingTriangles)
            playerHealth.unlockedRangedTriangles = true;
        // shooting triangles stats
        playerHealth.triangleDamage += triangleDamageIncrease;
        playerHealth.triangleSpeed += triangleSpeedIncrease;
        playerHealth.triangleFireRate += triangleFireRateIncrease;
        if (!playerHealth.autofireTriangles)
            playerHealth.autofireTriangles = rangedAutofire;
        if (!playerHealth.homingTriangles)
            playerHealth.homingTriangles = homingTriangles;
        if (!playerHealth.piercingTriangles)
            playerHealth.piercingTriangles = piercingTriangles;



        // add enemy
        if (addEnemy != null)
        {
            EnemyManager enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
            enemyManager.enemies.Add(addEnemy);
        }
        // increase boss health mult
        enemyManager.bossHealthMultiplier += enemyBossHealthMultiplierIncrease;

        enemyManager.spawnRate += enemySpawnRateIncrease;

        if (win)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().Win();
        }
    }
}
