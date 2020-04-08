using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{

    #region Events without Input
    public delegate void EventAction();

    public static event EventAction MenuLoadingCompleteEvent;

    public static void CallMenuLoadingCompleteEvent()
    {
        if (MenuLoadingCompleteEvent != null)
        {
            MenuLoadingCompleteEvent.Invoke();
        }
        else
            Debug.LogWarning("No Listener for CallMenuLoadingCompleteEvent");
    }



    #endregion



    #region Events with Float  Input
    public delegate void FloatEventAction(float input);



    #endregion



    #region Events with State  Input
    public delegate void StateEventAction(States input);

    public static event StateEventAction StateSwitchEvent;

    public static void CallStateSwitchEvent(States states)
    {
        if (StateSwitchEvent!= null)
        {
            StateSwitchEvent.Invoke(states);
        }
        else
            Debug.LogWarning("No Listener for CallStateSwitchEvent");
    }



    #endregion


}
