using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BattleEndEvent : UnityEvent<AbstractEntityController, AbstractEntityController>
{ 
}
