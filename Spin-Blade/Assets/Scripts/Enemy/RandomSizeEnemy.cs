using UnityEngine;

public class RandomSizeEnemy : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float randomScaleX = Random.Range(minSize, maxSize);
        float randomScaleY = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomScaleX, randomScaleY, 1f);
    }
}
