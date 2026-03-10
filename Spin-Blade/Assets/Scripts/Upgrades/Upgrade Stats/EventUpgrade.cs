using UnityEngine;

public class EventUpgrade : MonoBehaviour, IUpgrade
{
	public bool enableEvents;

	public void ApplyUpgrade()
	{
		EventManager eventManager = EventManager.Instance;

		if (!eventManager.enableEvents)
			eventManager.enableEvents = enableEvents;
	}
}