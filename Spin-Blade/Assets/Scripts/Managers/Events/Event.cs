using UnityEngine;

public abstract class Event : ScriptableObject
{
	public abstract void ApplyEvent();
	public abstract bool IsEnabled();
	public abstract string GetEventName();
	public abstract string GetEventPopupTextString();

}
