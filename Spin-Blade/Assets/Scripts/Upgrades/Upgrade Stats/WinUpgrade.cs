using UnityEngine;

public class WinUpgrade : MonoBehaviour, IUpgrade
{
	public int winFragments;
	public void ApplyUpgrade()
	{
		if (winFragments < WinManager.WIN_FRAGMENTS_NEEDED)
		{
			WinManager.AddWinFragment(winFragments);
		}
	}
}
