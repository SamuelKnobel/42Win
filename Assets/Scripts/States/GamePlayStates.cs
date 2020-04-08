using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayStates : States
{
    protected static readonly GamePlayStates SinglePlayer = new GamePlayStates(0, Enum.SinglePlayer);
    protected static readonly GamePlayStates MultiPlayer = new GamePlayStates(1, Enum.MultiPlayer);


    public GamePlayStates(int value, Enum displayName) : base(value, displayName)
    {
        Dict_States.Add(displayName, this);

    }
    public static void Initialize()
    {
    }
}
