using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    ThreadQueuer threadQueuer;
    // TODO: Stats Tracken for KI

    public static GameManager gameManager;

    public static float currentAI_ThinkTime;    
    public static int currentAI_ThinkDepth;
    public static float currentPlayer_ThinkTime;

    public static GameGrid GameGrid;
    public static Player[] Players = new Player[2];
    public static Player currentPlayer;

    public static States State;
    public bool[] _ThreadsFinished;
    public static List<Coin> playedCoins = new List<Coin>();

    [TextArea(5, 6)] public string InfoText0 = "empty";
    public string InfoText1 = "empty";
    public string InfoText2 = "empty";
    public string InfoText3 = "empty";
    public string InfoText4 = "empty";
    public string InfoText5 = "empty";
    [TextArea(5, 6)] public string InfoText6 = "empty";

    public static bool FirstMove;


    private void Awake()
    {
        FirstMove = true;
        _ThreadsFinished = new bool[0];

        threadQueuer = GetComponent<ThreadQueuer>();
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
    private void OnEnable()
    {
        EventManager.ThreadEvent += printConfig;
        EventManager.SingleThreadEndEvent += OnThreadsFinished;
    }
    private void OnDisable()
    {
        EventManager.ThreadEvent -= printConfig;
        EventManager.SingleThreadEndEvent -= OnThreadsFinished;
    }

    void printConfig(Configuration toPrint)
    {
        print(toPrint.ToString());
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

 

        if (Input.GetKeyDown(KeyCode.P))
        {
            //print(new Configuration(GameGrid).ToString());

            //foreach (Configuration item in currentPlayer.GetNextConfigurations(new Configuration(GameGrid)))
            //{
            //    print(item.ToString());
            //}
            //ThreadQueuer.StartThreadedFunction(currentPlayer.TreeBuilder);
            // ThreadQueuer.StartThreadedFunction(() => { Testfunction(1, new Configuration(GameGrid)); });

            //InfoText0 = "Tree: " + currentPlayer.tree.ToString();
            //InfoText0 = "Tree: " + currentPlayer.tree.Count;
            StartTreeBuilding();
            currentPlayer.StartBuildTree(currentPlayer.PlayerIndex);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < currentPlayer.tree.TempBranches.Length; i++)
            {
                currentPlayer.tree.AddBranch(currentPlayer.tree.TempBranches[i], currentPlayer.tree);
            }
            print(currentPlayer.tree);
            print(currentPlayer.FindBestMove());

        }

        if (States.compareState(GameStates.currentGameState, States.Enum.Game_InGame))
        {
            if (currentPlayer.playerType == Player.PlayerType.Computer)
            {
                if (!FindObjectOfType<ThreadQueuer>().ThredsRunning)
                {
                    if (currentPlayer.tree.Count == 1)
                    {
                        Debug.Log("AllThreads Finished");
                        for (int i = 0; i < currentPlayer.tree.TempBranches.Length; i++)
                        {
                            currentPlayer.tree.AddBranch(currentPlayer.tree.TempBranches[i], currentPlayer.tree);
                        }
                        //FindObjectOfType<GamePlayHandler>().NextMoveCol = currentPlayer.FindBestMove();
                        EventManager.CallAllThreadEndEvent(-1);
                    }
                }
            }
        }

    }
    public void StartTreeBuilding()
    {
        Debug.Log("StartBuilding Tree");
        _ThreadsFinished = new bool[GameGrid.Width];
        for (int i = 0; i < _ThreadsFinished.Length; i++)
        {
            _ThreadsFinished[i] = false;
        }
    }



    void OnThreadsFinished(float threadNumber)
    {
        _ThreadsFinished[(int)threadNumber] = true;
    }

    //int Testfunction(int test1, Configuration test2)
    //{
    //    print(test2.ToString());

    //    return test1;
    //}

}
