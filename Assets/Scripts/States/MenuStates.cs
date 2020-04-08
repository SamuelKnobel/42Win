using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStates : States
{

    protected static  MenuStates Loading = new MenuStates(0, Enum.Menu_Loading);
    protected static  MenuStates InMainMenu = new MenuStates(0, Enum.Menu_InMenu);
    protected static  MenuStates Settings = new MenuStates(0, Enum.Menu_Settings);
    protected static  MenuStates Help = new MenuStates(0, Enum.Menu_Help);




    protected MenuStates(int value, Enum displayName) : base(value, displayName)
    {
        Dict_States.Add(displayName, this);
    }

    public static void Initialize()
    {
    }



}
