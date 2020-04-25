using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager 
{

    #region Events without Input
    public delegate void EventAction();

    public static event EventAction MenuLoadingCompleteEvent;
    public static event EventAction UpdateUIOnTurnEndEvent;
    public static event EventAction GameEndEvent;

    public static void CallMenuLoadingCompleteEvent()
    {
        if (MenuLoadingCompleteEvent != null)
        {
            MenuLoadingCompleteEvent.Invoke();
        }
        else
            Debug.LogWarning("No Listener for CallMenuLoadingCompleteEvent");
    }
    public static void CallUpdateUIOnTurnEndEvent()
    {
        if (UpdateUIOnTurnEndEvent != null)
        {
            UpdateUIOnTurnEndEvent.Invoke();
        }
        else
            Debug.LogWarning("No Listener for CallUpdateUIOnTurnEndEvent");
    }
    public static void CallGameEndEvent()
    {
        if (GameEndEvent != null)
        {
            GameEndEvent.Invoke();
        }
        else
            Debug.LogWarning("No Listener for CallGameEndEvent");
    }
      





    #endregion



    #region Events with Float  Input
    public delegate void FloatEventAction(float input);

    public static event FloatEventAction SingleThreadEndEvent;
    public static event FloatEventAction AllThreadsEndedEvent;

    //public static event FloatEventAction ThreadEvent;

    //public static void CallThreadEvent(float input)
    //{
    //    if (ThreadEvent != null)
    //    {
    //        ThreadEvent.Invoke(input);
    //    }
    //    else
    //        Debug.LogWarning("No Listener for CallGameEndEvent");
    //}

    public static void CallThreadEndEvent(float input)
    {
        if (SingleThreadEndEvent != null)
        {
            SingleThreadEndEvent.Invoke(input);
        }
        else
            Debug.LogWarning("No Listener for CallThreadEndEvent");
    }    
    public static void CallAllThreadEndEvent(float input)
    {
        if (AllThreadsEndedEvent != null)
        {
            AllThreadsEndedEvent.Invoke(input);
        }
        else
            Debug.LogWarning("No Listener for CallAllThreadEndEvent");
    }

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

    #region Events with Config  Input
    public delegate void ConfigEventAction(Configuration input);
    public static event ConfigEventAction ThreadEvent;

    public static void CallThreadEvent(Configuration input)
    {
        if (ThreadEvent != null)
        {
            ThreadEvent.Invoke(input);
        }
        else
            Debug.LogWarning("No Listener for CallThreadEvent");
    }
    #endregion
}
