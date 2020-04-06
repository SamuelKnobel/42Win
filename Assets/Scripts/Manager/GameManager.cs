using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // TODO: Stats Tracken for KI

    public static GameManager gameManager;

    public static GameStates currentGamestate;
    public static float currentAI_ThinkTime;
    public static int currentAI_ThinkDepth;
    public static float currentPlayer_ThinkTime;
    public static Sprite SpritePlayer1;
    public static Sprite SpritePlayer2;

    public GameStates state;

    private void Awake()
    {
        SwitchStateTo(GameStates.LoadMenu);
        if (gameManager == null)
        {
            DontDestroyOnLoad(this);
            gameManager = this;
        }
        else
            Destroy(this);
        
    }
    //void LoadSettings() // Not sure if ever used, here just in case
    //{
    //    // Load Settings
    //    SwitchStateTo(GameStates.InMainMenu);
    //}

    void Start()
    {
        //LoadSettings();
    }

    // Update is called once per frame
    void Update()
    {
        state = currentGamestate;

        if (currentGamestate == GameStates.LoadMenu)
        {
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                SwitchStateTo(GameStates.InMainMenu);
            }
        }
        if (currentGamestate == GameStates.LoadGame)
        {
            if (SceneManager.GetActiveScene().name == "GamePlay")
            {
                SwitchStateTo(GameStates.InGame);
            }
        }


    }
    public static void SwitchStateTo(GameStates states)
    {
        switch (states) 
        {
            case GameStates.LoadMenu:
                currentGamestate = GameStates.LoadMenu;
                break;

            case GameStates.InMainMenu:
                currentGamestate = GameStates.InMainMenu;
                FindObjectOfType<HUD>().AllocateUIElementsInMenu();
                break;
            case GameStates.InSettings:
                currentGamestate = GameStates.InSettings;
                break;
            case GameStates.InHelp:
                currentGamestate = GameStates.InHelp;
                break;
            case GameStates.LoadGame:
                currentGamestate = GameStates.LoadGame;

                break;
            case GameStates.InGame:
                print(SceneManager.GetActiveScene().name);
                print(FindObjectOfType<HUD>());
                FindObjectOfType<HUD>().AllocateUIElementsInGame();
                currentGamestate = GameStates.InGame;
                FindObjectOfType<GamePlayHandler>().StartGame();


                break;

            case GameStates.GameEnd:
                currentGamestate = GameStates.GameEnd;
                FindObjectOfType<HUD>().ShowPlayerWin();

                break;
            default:
                break;
        }
    }
}
