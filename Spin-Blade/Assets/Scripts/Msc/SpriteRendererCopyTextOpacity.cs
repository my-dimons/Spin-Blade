using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererCopyTextOpacity : MonoBehaviour
{
	public TMP_Text text;
	private SpriteRenderer spriteRenderer;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, text.color.a);
	}
}
