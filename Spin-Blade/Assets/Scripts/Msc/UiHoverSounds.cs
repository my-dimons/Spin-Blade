using UnityEngine;
using UnityUtils.ScriptUtils.Audio;

public class UiHoverSounds : MonoBehaviour {
	public AudioClip hoverAudio;

	public void HoverSfx() {
		SfxManager.PlaySfxAudioClip(hoverAudio, 0.2f, 0.07f);
	}
}
