using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool destroyOnHit = false;
    public float speed = 10f;
    public float damage = 0.3f;
    public float knockbackForce = 4f;
    public float stunDuration = 0.3f;
}
