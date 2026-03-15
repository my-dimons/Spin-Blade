using System;
using System.Collections;
using UnityEngine;
using UnityUtils.ScriptUtils.Audio;
using UnityUtils.ScriptUtils.Objects;
using UnityUtils.ScriptUtils.Particles;

public class Enemy : MonoBehaviour {
  [Header("Movement")]
  [HideInInspector] public GameObject target;
  public float speed = 5f;
  [Space(10)]
  public float rotateSpeed = 0;
  public float rotateMultiplier = 1f;

  [Header("Money")]
  public float value;
  public MoneyManager.Currency valueCurrencyType = MoneyManager.Currency.money;

  [Header("Damage")]
  public float damage = 1f;

  [Range(0, 1)]
  public float spawnRate = 1;

  [Header("Health")]
  public bool damageFromProjectiles = true;
  public float maxHealth = 1f;
  public float currentHealth;

  private Coroutine knockbackRoutine;

  [Header("On Hit")]
  public Color hitColor = Utils.ColorFromHex("#FF4E4E"); // when this enemy gets hit, particle & stuffs color

  public GameObject deathParticles;
  public GameObject hitParticles;
  public float deathParticleScale = 0.23f;

  private Color badMoneyColor = Utils.ColorFromHex("#8A3131");

  [Header("Audio and Effects")]
  public GameObject deathMoneyText;
  public GameObject takeDamageText;
  public GameObject dealDamageText;
  public AudioClip deathSound;
  public AudioClip hitSound;

  private bool isDead = false;

  public event Action OnDeath;
  public event Action OnCircleHit;
  public event Action OnHit;

  private void OnValidate() {
    currentHealth = maxHealth;
  }

  MoneyManager moneyManager;
  EnemyManager enemyManager;
  PlayerHealthAndDamage playerHealth;

  void Start() {
    enemyManager = EnemyManager.Instance;
    playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();
    moneyManager = MoneyManager.Instance;

    //speed *= enemyManager.difficulty;

    if (!TryGetComponent<BossEnemy>(out _)) {
      damage *= enemyManager.difficulty;
      maxHealth *= enemyManager.difficulty;

      currentHealth = maxHealth;
    }
  }

  private void FixedUpdate() {
    if (target != null)
      EnemyMovement();
  }
  private void Update() {
    transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
  }

  void RotateTowardsTarget(GameObject target) {
    Vector3 vectorToTarget = target.transform.position - transform.position;
    float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - rotateMultiplier;
    Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

    transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
  }

  void EnemyMovement() {
    if (rotateSpeed == 0)
      RotateTowardsTarget(target);

    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime * enemyManager.enemySpeedMultiplier);
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("PlayerProjectile") && damageFromProjectiles) {
      Projectile proj = other.GetComponent<Projectile>();

      if (other.GetComponent<TriangleProjectile>()) {
        other.GetComponent<TriangleProjectile>().homingTarget = null;
      }

      if (proj.destroyOnHit)
        Destroy(other.gameObject);

      Vector3 particlePos = other.ClosestPoint(transform.position);
      TakeDamage(other.transform, proj.damage, particlePos, proj.knockbackForce, proj.stunDuration, playerHealth.knockbackCurve, true);
    }

    if (other.CompareTag("Circle") && currentHealth > 0) {
      HitCircle();
    }
  }

  public void TakeDamage(Transform attacker, float damageAmount, Vector3 particlePos, float distance = 0, float duration = 0, AnimationCurve curve = null, bool knockback = false) {
    if (isDead)
      return;

    OnHit?.Invoke();

    currentHealth -= damageAmount;

    if (currentHealth <= 0) {
      Death();
      return;
    }

    SfxManager.PlaySfxAudioClip(hitSound);
    ParticleSpawner.SpawnBurstParticle(hitParticles, particlePos, color: hitColor);
    Utils.SpawnFloatingText(takeDamageText, transform.position, Math.Round(damageAmount, 2).ToString(), 6f, 0.3f, 40f, 0.45f, 0.15f, Color.white);

    GetComponent<ObjectColorFlash>().FlashWhite(0.08f);

    if (knockback) {
      KnockbackFrom(Vector2.zero, distance, duration, curve);
    }

  }
  /// <summary>
  /// Moves the enemy away from a point by a given distance, following an animation curve.
  /// </summary>
  public void KnockbackFrom(Vector3 centerPoint, float distance, float duration, AnimationCurve curve) {
    // Cancel any ongoing knockback
    if (knockbackRoutine != null)
      StopCoroutine(knockbackRoutine);

    knockbackRoutine = StartCoroutine(KnockbackRoutine(centerPoint, distance, duration, curve));
  }

  private IEnumerator KnockbackRoutine(Vector3 centerPoint, float distance, float knockbackDuration, AnimationCurve knockbackCurve) {
    Vector3 startPos = transform.position;

    // Direction away from the point
    Vector3 dir = (startPos - centerPoint).normalized;

    // Calculate knockback target once
    Vector3 endPos = startPos + dir * distance;

    float time = 0f;
    while (time < knockbackDuration) {
      float t = time / knockbackDuration;
      float curveValue = knockbackCurve.Evaluate(t); // Curve mapping 0 → 1

      // Smoothly move along the curve
      transform.position = Vector3.Lerp(startPos, endPos, curveValue);
      // If using physics:
      // rb.MovePosition(Vector3.Lerp(startPos, endPos, curveValue));

      time += Time.deltaTime;
      yield return null;
    }

    transform.position = endPos; // Snap to end
    knockbackRoutine = null;
  }

  [ContextMenu("Kill Enemy")]
  private void ContextMenuDeath() {
    Death(false);
  }

  public void Death(bool playerStatGain = true) {
    if (isDead)
      return;
    isDead = true;

    Debug.Log("Killed enemy");

    OnDeath?.Invoke();

    SfxManager.PlaySfxAudioClip(deathSound, 0.8f);

    ParticleSpawner.SpawnBurstParticle(deathParticles, transform.position, color: hitColor);

    Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.08f);

    // text
    Color color;
    if (value > 0)
      color = MoneyManager.GetCurrencyColor(valueCurrencyType);
    else {
      color = badMoneyColor;
    }

    if (playerStatGain) {
      Utils.SpawnFloatingText(deathMoneyText, transform.position, MoneyManager.GetMoneyString(moneyManager.CalculateCurrency(value, valueCurrencyType), valueCurrencyType), 6f, 0.3f, 40f, 0.45f, 0.15f, color);

      moneyManager.AddCurrency(value, valueCurrencyType);

      playerHealth.Heal(playerHealth.killRegenAmount);

      GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().kills++;
    }

    Destroy(gameObject);
  }

  public void HitCircle() {
    OnCircleHit?.Invoke();

    ParticleSpawner.SpawnBurstParticle(deathParticles, transform.position, color: hitColor);
    Camera.main.GetComponent<CameraScript>().ScreenshakeFunction(.5f);

    PlayerHealthAndDamage player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>();

    Color flashColor = Utils.ColorFromHex("#FF4E4E");
    if (TryGetComponent<CurrencyEnemy>(out _) || damage <= 0)
      flashColor = MoneyManager.GetCurrencyColor(valueCurrencyType);
    else
      Utils.SpawnFloatingText(dealDamageText, transform.position, Math.Round(damage, 2).ToString(), 6f, 0.3f, 40f, 0.45f, 0.15f, Color.white);

    player.TakeDamage(damage, flashColor);


    Destroy(gameObject);
  }
}