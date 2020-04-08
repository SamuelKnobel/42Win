using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameStates : States
{

    protected static readonly GameStates Loading = new GameStates(0, Enum.Game_Loading);
    protected static readonly GameStates InGame = new GameStates(1, Enum.Game_InGame);
    protected static readonly GameStates End = new GameStates(2, Enum.Game_End);

    protected GameStates(int value, Enum displayName) : base(value, displayName)
    {
        Dict_States.Add(displayName, this);
    }


}
