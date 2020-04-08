using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnState : GameStates
{

    protected static readonly TurnState HumanPlayer1 = new TurnState(0, Enum.HumanPlayer1Turn);
    protected static readonly TurnState HumanPlayer2 = new TurnState(0, Enum.HumanPlayer2Turn);
    protected static readonly TurnState ComputerPlayer = new TurnState(0, Enum.ComputerPlayerTurn);

    protected TurnState(int value, Enum displayName) : base(value, displayName)
    {
        if (!Dict_States.ContainsKey(displayName))
        {
            Dict_States.Add(displayName, this);
        }
    }

    public static void Initialize()
    {

    }
}
