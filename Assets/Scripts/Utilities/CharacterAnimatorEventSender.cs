using UnityEngine;
/// <summary>
/// Use this component to send the animations events to the upper classes implementing ICombatAnimationEventsReceiver;
/// </summary>
public class CharacterAnimatorEventSender : MonoBehaviour
{
    private ICombatAnimationEventsReceiver eventReceiver;
    void Awake() => eventReceiver = GetComponentInParent<ICombatAnimationEventsReceiver>();
    public void OnAttackEvent() => eventReceiver?.OnAttackAnimationEvent();
}
