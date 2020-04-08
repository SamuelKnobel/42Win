using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class States : Enumeration
{
    public enum Enum
    {
        Menu_Loading, Menu_InMenu, Menu_Settings, Menu_Help,
        HumanPlayer1Turn, HumanPlayer2Turn, ComputerPlayerTurn,
        Game_Loading, Game_InGame, Game_End,
        MultiPlayer, SinglePlayer
    }

    public static GameStates currentGameState;
    public static MenuStates currentMenuState;
    public static TurnState currentTurnState;
    public static GamePlayStates currentGamePlayState;

    public static Dictionary<Enum, States> Dict_States = new Dictionary<Enum, States>();


    protected States(int value, Enum displayName) : base(value, displayName.ToString()){  }

    public static void Init()
    {
        MenuStates.Initialize();
        TurnState.Initialize();
        GamePlayStates.Initialize();
    }

    public static GameStates SetGameState(Enum newState)
    {
        currentGameState = null;
        if (Dict_States.TryGetValue(newState, out States result))
        {
            currentGameState = (GameStates)result;
            //EventManager.CallStateSwitchEvent(currentGameState);
        }
        else
        {
            currentGameState = null;
            Debug.Log("State Cannot be set: " + newState);
        }
        return currentGameState;
    }
    public static void ResetState(States stateToReset)
    {
        if (stateToReset != null)
        {
            if (stateToReset.GetType() == typeof(MenuStates))
            {
                currentMenuState = null;
            }
            else if ((stateToReset.GetType() == typeof(GameStates)))
            {
                currentGameState = null;
            }
            else if ((stateToReset.GetType() == typeof(GamePlayStates)))
            {
                currentGamePlayState = null;
            }
            else if ((stateToReset.GetType() == typeof(TurnState)))
            {
                currentTurnState = null;
            }
        }
    }


    public static MenuStates SetMenuState(Enum newState)
    {
        if (Dict_States.TryGetValue(newState, out States result))
        {
            currentMenuState = (MenuStates)result;
            //EventManager.CallStateSwitchEvent(currentMenuState);
        }
        else
        {
            currentMenuState = null;
            Debug.Log("State Cannot be set: " + newState);
        }
        return currentMenuState;
    }
    public static TurnState SetTurnState(Enum newState)
    {
        currentTurnState = null;
        if (Dict_States.TryGetValue(newState, out States result))
        {
            currentTurnState = (TurnState)result;
            //EventManager.CallStateSwitchEvent(currentTurnState);
        }
        else
        {
            currentTurnState = null;
            Debug.Log("State Cannot be set: " + newState);
        }
        return currentTurnState;
    }
    public static GamePlayStates SetGamePlayState(Enum newState)
    {
        currentGamePlayState = null;
        if (Dict_States.TryGetValue(newState, out States result))
        {
            currentGamePlayState = (GamePlayStates)result;
            //EventManager.CallStateSwitchEvent(currentTurnState);
        }
        else
        {
            currentGamePlayState = null;
            Debug.Log("State Cannot be set: " + newState);
        }
        return currentGamePlayState;
    }
    public static bool compareState(States states, Enum enumToCompare)
    {
        bool result = false;
        if (states!= null)
        {
            if (states.ToString() == enumToCompare.ToString())
            {
                result = true;
            }
        }
        return result;
    }

}
