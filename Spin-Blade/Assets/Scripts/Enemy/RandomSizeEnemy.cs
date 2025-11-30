using UnityEngine;

public class RandomSizeEnemy : MonoBehaviour
{
    public float minSize = 0.5f;
    public float maxSize = 1.5f;
    [Space(10)]
    public bool uniformScale;

    void Start()
    {
        float uniformScaleValue = Random.Range(minSize, maxSize);

        float randomScaleX = uniformScale ? uniformScaleValue : Random.Range(minSize, maxSize);
        float randomScaleY = uniformScale ? uniformScaleValue : Random.Range(minSize, maxSize);

        transform.localScale = new Vector3(randomScaleX, randomScaleY, 1f);
    }
}
