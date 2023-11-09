using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BattleStartEvent : UnityEvent<AbstractEntityController, AbstractEntityController>
{
}
