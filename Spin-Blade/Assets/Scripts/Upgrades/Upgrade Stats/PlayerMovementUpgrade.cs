using UnityEngine;

public class PlayerMovementUpgrade : MonoBehaviour, IUpgrade
{
    public float speedIncrease;

    public void ApplyUpgrade()
    {
        PlayerMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        playerMovement.speed += speedIncrease;
    }
}
