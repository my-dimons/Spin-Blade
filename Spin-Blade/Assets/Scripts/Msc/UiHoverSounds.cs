using UnityEngine;

public class UiHoverSounds : MonoBehaviour
{
    public AudioClip hoverAudio;
    
    public void HoverSfx()
    {
        Utils.PlayClip(hoverAudio, 0.2f);
    }
}
