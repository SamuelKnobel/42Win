using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // TODO: Stats Tracken for KI

    public static GameManager gameManager;

    public static InGameStates currentGamestate;
    public static float currentAI_ThinkTime;
    public static int currentAI_ThinkDepth;
    public static float currentPlayer_ThinkTime;
    public static Sprite SpritePlayer1;
    public static Sprite SpritePlayer2;


    public static PlayerName currentPlayer = PlayerName.Player2;
    public static EnemyType enemyPlayer = EnemyType.Human;

    public static States State;


    [SerializeField]
    InGameStates state;
    public string InfoText1 = "empty";
    public string InfoText2 = "empty";
    public string InfoText3 = "empty";
    public string InfoText4 = "empty";
    public string InfoText5 = "empty";



    private void Awake()
    {
        //SwitchStateTo(InGameStates.LoadMenu);
        if (gameManager == null)
        {
            DontDestroyOnLoad(this);
            gameManager = this;
        }
        else
            Destroy(this);
    }




    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
            InfoText1 = "MenuState: " + States.currentMenuState;

        //if (States.currentGameState != null)
            InfoText2 = "GameState: " + States.currentGameState;
        //if (States.currentTurnState != null)
            InfoText3 = "TurnState: " + States.currentTurnState;     
        //if (States.currentGamePlayState != null)
            InfoText4 = "GamePlayState: " + States.currentGamePlayState;

        InfoText5 = "DictLenght:" + States.Dict_States.Count;

        state = currentGamestate;
    }
    public static void SwitchStateTo(InGameStates states)
    {
        switch (states) 
        {
            case InGameStates.LoadMenu:
                currentGamestate = InGameStates.LoadMenu;
                break;

            case InGameStates.InMainMenu:
                currentGamestate = InGameStates.InMainMenu;
                //FindObjectOfType<HUD>().AllocateUIElementsInMenu();
                break;
            case InGameStates.InSettings:
                currentGamestate = InGameStates.InSettings;
                break;
            case InGameStates.InHelp:
                currentGamestate = InGameStates.InHelp;
                break;
            case InGameStates.LoadGame:
                currentGamestate = InGameStates.LoadGame;

                break;
            case InGameStates.InGame:
                //FindObjectOfType<HUD>().AllocateUIElementsInGame();
                currentGamestate = InGameStates.InGame;
               
                break;

            case InGameStates.GameEnd:
                currentGamestate = InGameStates.GameEnd;
               

                break;
            default:
                break;
        }
    }
}
