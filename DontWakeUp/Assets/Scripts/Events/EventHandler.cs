using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{

    private static EventHandler _instance = null;

    public static EventHandler instance { get 
        { 
            if (_instance == null)
            {
                _instance = new EventHandler();
            }
            return _instance;
        } 
    }

    #region EXAMPLE CODE: create events

    private MyCustomEvent _myCustomEventExample = null;
    public MyCustomEvent MyCustomEvent
    {
        get
        {
            if (_myCustomEventExample == null)
            {
                _myCustomEventExample = new MyCustomEvent();
            }
            return _myCustomEventExample;
        }
    }


    private MyCustomStringEvent _myCustomStringEventExample = null;
    public MyCustomStringEvent MyCustomStringEvent
    {
        get
        {
            if ( _myCustomStringEventExample == null)
            {
                _myCustomStringEventExample= new MyCustomStringEvent();
            }
            return _myCustomStringEventExample;
        }
    }

    #endregion

    #region EXAMPLE CODE: trigger events

    private void HowToTriggerAnEventExample()
    {
        //for event with no parameters:
        EventHandler.instance.MyCustomEvent.Invoke();

        //for events with parameters:
        EventHandler.instance.MyCustomStringEvent.Invoke("The value for the parameter(s) separated by a comma if multiple");
    }

    #endregion

    #region EXAMPLE CODE: use events

    //first you need to create a function that will be run when an event is triggered
    private void OnMyCustomEventTrigger()
    {
        //the code you want to run when "MyCustomEvent" is triggered
    }

    private void OnMyCustomStringEventTrigger(string myString)
    {
        //the code you want to run when MyCustomStringEvent is triggered, where "myString" will have the value passed when Invoke is called
    }


    //Then you will want to add the following code to your start function, to subscribe to the event
    private void CodeToAddToStart()
    {
        //we add a listener to the event we want, linking the function we created above:
        EventHandler.instance.MyCustomEvent.AddListener(OnMyCustomEventTrigger);

        //if the function has the correct parameters it will let you add the function as a listener
        EventHandler.instance.MyCustomStringEvent.AddListener(OnMyCustomStringEventTrigger);
    }
    #endregion


    private BattleStartEvent _battleStartEvent = null;
    public BattleStartEvent BattleStartEvent
    {
        get
        {
            if (_battleStartEvent == null)
            {
                _battleStartEvent = new BattleStartEvent();
            }
            return _battleStartEvent;
        }
    }

    private BattleEndEvent _battleEndEvent = null;
    public BattleEndEvent BattleEndEvent
    {
        get
        {
            if (_battleEndEvent == null)
            {
                _battleEndEvent = new BattleEndEvent();
            }
            return _battleEndEvent;
        }
    }


    //add your events below this line


    //add your events above this line
}
