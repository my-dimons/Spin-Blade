using UnityEngine;

public interface IEvent
{
    public void ApplyEvent();
    public bool IsEnabled();
    public string GetEventName();
    public string GetEventPopupTextString();

}
