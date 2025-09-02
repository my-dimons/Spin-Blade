using UnityEngine;

public class UiHoverSounds : MonoBehaviour
{
    public AudioClip hoverAudio;
    
    public void HoverSfx()
    {
        Utils.PlayAudioClip(hoverAudio, 0.2f, 0.07f);
    }
}
