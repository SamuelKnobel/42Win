using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // TODO: Stats Tracken for KI

    public static GameManager gameManager;

    public static float currentAI_ThinkTime;    
    public static int currentAI_ThinkDepth;
    public static float currentPlayer_ThinkTime;


    public static Player[] Players = new Player[2];
    public static Player currentPlayer;

    public static PlayerName currPlayer = PlayerName.Player2;

    public static States State;

    public static List<Coin> playedCoins = new List<Coin>();

    public string InfoText1 = "empty";
    public string InfoText2 = "empty";
    public string InfoText3 = "empty";
    public string InfoText4 = "empty";
    public string InfoText5 = "empty";
    [TextArea(5, 6)] public string InfoText6 = "empty";


    private void Awake()
    {
        if (gameManager == null)
        {
            DontDestroyOnLoad(this);
            gameManager = this;
        }
        else
            Destroy(this);
        Players[0] = new Player("Human1", Player.PlayerType.Human, 0);
        Players[1] = new Player("Human2", Player.PlayerType.Human, 1);
        
    }




    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        InfoText1 = "MenuState: " + States.currentMenuState;

        InfoText2 = "GameState: " + States.currentGameState;
        InfoText3 = "TurnState: " + States.currentTurnState;     
        InfoText4 = "GamePlayState: " + States.currentGamePlayState;

        InfoText5 = "played Coins:" + playedCoins.Count;

        if (States.compareState(States.currentGameState,States.Enum.Game_InGame))
        {
            InfoText6 = "Players: " + '\n' +
            Players[0].playerName + '\n' + Players[1].playerName + '\n' +
            "current Player: " + currentPlayer.playerName;
        }
 
    }
}
