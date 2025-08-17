using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMine : MonoBehaviour
{
    public float damage;
    public float knockback;
    public float stunDuration;
    public AnimationCurve curve;

    [Header("Pulsing")]
    private float pulseOffset = 0f;
    private float baseSize;
    public float pulseStrength;
    public float pulseSpeed;

    [Header("Dying")]
    public bool pausePulsing;
    public float lifetime;
    // amount of hits before dying
    public int hitsBeforeDeath = 1;
    private int hitsTaken;

    [Header("Growing")]
    public AnimationCurve growingCurve;
    float growingAnimationTime;

    [Header("-- Unlocks -- ")]
    [Header("Explosion")]
    public bool explode;
    public float explosionRadius = 8f;
    private void Start()
    {
        baseSize = transform.localScale.x;
        growingAnimationTime = growingCurve[growingCurve.length - 1].time;
        StartCoroutine(SpawnIn());
        StartCoroutine(DeathTimer());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !pausePulsing)
        {
            collision.GetComponent<Enemy>().TakeDamage(this.transform, damage, knockback, stunDuration, curve, true);
            // death
            hitsTaken++;
            if (hitsTaken >= hitsBeforeDeath)
            {
                StartCoroutine(StartDying());
            }
            // exploding circle
            if (explode)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthAndDamage>().ExplodeCircle(transform.position, damage, explosionRadius, false);
            }
        }
    }

    private void Update()
    {
        if (!pausePulsing)
            PulsingGrowth();
    }

    void PulsingGrowth()
    {
        float scale = baseSize + Mathf.Sin((Time.time - pulseOffset) * pulseSpeed) * pulseStrength;
        transform.localScale = new Vector3(scale, scale, scale);
    }


    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(lifetime);
        pausePulsing = true;
        StartCoroutine(StartDying());
    }

    public IEnumerator ScaleOverTime(Vector3 from, Vector3 to)
    {
        float time = 0f;
        float duration = growingAnimationTime;

        while (time < duration)
        {
            float t = time / duration;
            float curveValue = growingCurve.Evaluate(t);

            transform.localScale = Vector3.LerpUnclamped(from, to, curveValue);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = to; // snap at end
    }
    IEnumerator StartDying()
    {
        pausePulsing = true;
        StopCoroutine(DeathTimer());
        // scale to zero
        yield return StartCoroutine(ScaleOverTime(transform.localScale, Vector3.zero));
        Destroy(this.gameObject);
    }

    IEnumerator SpawnIn()
    {
        pausePulsing = true;

        // Run the spawn scale
        yield return StartCoroutine(ScaleOverTime(Vector3.zero, new Vector3(baseSize, baseSize, baseSize)));

        // Reset sine wave so it starts clean at baseSize
        pulseOffset = Time.time;

        pausePulsing = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
