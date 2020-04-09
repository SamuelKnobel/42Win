using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;
using System.Threading;
using System.Reflection;
using UnityEngine.XR;
public class HUD : MonoBehaviour
{
    // UI Elements
    [SerializeField]
    GameObject MainMenuContainer;
    [SerializeField]
    GameObject SettingsContainer;
    [SerializeField]
    GameObject HelpMenuContainer;   
    [SerializeField]
    GameObject PauseMenuContainer; 
    [SerializeField]
    GameObject EndGameMenuContainer;
    [SerializeField]
    GameObject MenuCanvas;
    [SerializeField]
    GameObject GameCanvas;   
    [SerializeField]
    GameObject Environment;



    [SerializeField]    Toggle T_Ki;   
    [SerializeField]    Toggle T_Human;
    [SerializeField]    Toggle T_MoveBack;
    [SerializeField]    Slider S_Width;
    [SerializeField]    Slider S_Height;    
    [SerializeField]    Slider S_Difficulty;

    List<Dropdown.OptionData> CoinSpriteDD = new List<Dropdown.OptionData>();
    [SerializeField] Dropdown ChooseSpritePlayer1;
    [SerializeField] Dropdown ChooseSpritePlayer2;


    [SerializeField] bool b_showMoveBackButton;

    public List<Sprite> CoinSprites;

    [SerializeField]    Text currentPlayerText;
    string PreTextPlayerInfo = "Current Player: ";
    [SerializeField]    Text WinText;
    string PreTextPlayerWin = "Winner: ";
    [SerializeField] public Text TimeText;
    public string PreTextTime = "Remaining Time: ";


    [SerializeField]
    Button[] allButtons;


    private void OnEnable()
    {
        EventManager.MenuLoadingCompleteEvent += MenuLoadingComplete;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventManager.UpdateUIOnTurnEndEvent += UpdateUIOnEndTurn;
        EventManager.GameEndEvent += ShowPlayerWin;

    }
    private void OnDisable()
    {
        EventManager.MenuLoadingCompleteEvent -= MenuLoadingComplete;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventManager.UpdateUIOnTurnEndEvent -= UpdateUIOnEndTurn;
        EventManager.GameEndEvent -= ShowPlayerWin;

    }

    void MenuLoadingComplete()
    {
        States.SetMenuState(States.Enum.Menu_InMenu);
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        States.SetGamePlayState(States.Enum.MultiPlayer);
        allocateButtonsFunctions();
        AllocateUIElementsInMenu();
        AllocateUIElementsInGame();
        GameCanvas.SetActive(false);
        Environment.SetActive(false);

        EventManager.CallMenuLoadingCompleteEvent();
    }
    void UpdateUIOnEndTurn()
    {
        if (currentPlayerText != null)
        {
            currentPlayerText.text = PreTextPlayerInfo + GameManager.currentPlayer.playerName;
        }
    }

    public void ShowPlayerWin()
    {
        EndGameMenuContainer.SetActive(true);

        WinText.gameObject.SetActive(true);
        currentPlayerText.gameObject.SetActive(false);
        WinText.text = PreTextPlayerWin + GameManager.currentPlayer.playerName;

        if (States.compareState(States.currentGamePlayState, States.Enum.SinglePlayer))
        {
            if (GameManager.currentPlayer.playerType == Player.PlayerType.Human)
                Audiomanager.PlaySound(Audiomanager.Sounds.Win);
            else
                Audiomanager.PlaySound(Audiomanager.Sounds.Loose);
        }


    }


    private void Awake()
    {
        if (CoinSprites.Count > 0)
        {
            GameManager.Players[0].playerSprite =  CoinSprites[0];
            GameManager.Players[1].playerSprite = CoinSprites[CoinSprites.Count - 1];
        }
        foreach (var item in CoinSprites)
        {
            Dropdown.OptionData newSprite = new Dropdown.OptionData("", item);
            CoinSpriteDD.Add(newSprite);
        }
    }

    public void AllocateUIElementsInMenu()
    {
        if (MainMenuContainer == null)
            MainMenuContainer = GameObject.FindGameObjectWithTag("MainMenuContainer");
        if (SettingsContainer == null)
            SettingsContainer = GameObject.FindGameObjectWithTag("SettingsContainer");
        if (HelpMenuContainer == null)
            HelpMenuContainer = GameObject.FindGameObjectWithTag("HelpMenuContainer");

        allButtons = FindObjectsOfType<Button>();
        allocateGameGUIElements();

        if (HelpMenuContainer != null)
            HelpMenuContainer.SetActive(false);
        if (SettingsContainer != null)
            SettingsContainer.SetActive(false);
    }
    public void AllocateUIElementsInGame()
    {
        allButtons = FindObjectsOfType<Button>();
        allocateGameGUIElements();

        if (PauseMenuContainer == null)
            PauseMenuContainer = GameObject.FindGameObjectWithTag("PauseMenuContainer");
        if (PauseMenuContainer != null)
            PauseMenuContainer.SetActive(false);

        if (EndGameMenuContainer == null)
            EndGameMenuContainer = GameObject.FindGameObjectWithTag("EndGameMenuContainer");
        if (EndGameMenuContainer != null)
            EndGameMenuContainer.SetActive(false);
        FindButtonByName("TakeMoveBack").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (States.compareState(States.currentGameState, States.Enum.Game_InGame))
        {
            GamePlayHandler handler = FindObjectOfType<GamePlayHandler>();
            if (handler != null)
            {
                if (TimeText != null)
                {
                    TimeText.text = PreTextTime + Mathf.Round(handler.RemainingTime.RemainTime);
                }
            }
        }
        if (States.compareState(States.currentGameState,States.Enum.Game_InGame) || States.compareState(States.currentGameState, States.Enum.Game_End))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseMenuContainer.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    public void ButtonFunctions()
    {
        Audiomanager.PlaySound(Audiomanager.Sounds.Button);
        string currentname = EventSystem.current.currentSelectedGameObject.name;
        switch (currentname.ToLower())
        {
            case "start":
                States.SetGameState(States.Enum.Game_Loading);
                Environment.SetActive(true);
                SettingsContainer.SetActive(false);
                MainMenuContainer.SetActive(true);
                States.ResetState(States.currentMenuState);
                MenuCanvas.SetActive(false);
                GameCanvas.SetActive(true);

                FindObjectOfType<GamePlayHandler>().StartGame();

                break;
            case "exit":
                if (States.compareState(States.currentGameState, States.Enum.Game_InGame) || States.compareState(States.currentMenuState, States.Enum.Menu_InMenu))
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }
                else if (States.compareState(States.currentMenuState, States.Enum.Menu_Help) || States.compareState(States.currentMenuState, States.Enum.Menu_Settings))
                {
                    HelpMenuContainer.SetActive(false);
                    SettingsContainer.SetActive(false);
                    MainMenuContainer.SetActive(true);
                    States.SetMenuState(States.Enum.Menu_InMenu);
                }
                break;
            case "settings":
                States.SetMenuState(States.Enum.Menu_Settings);
                HelpMenuContainer.SetActive(false);
                SettingsContainer.SetActive(true);
                MainMenuContainer.SetActive(false);
                break;
            case "help":
                States.SetMenuState(States.Enum.Menu_Help);
                HelpMenuContainer.SetActive(true);
                SettingsContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                break;
            case "resume":
                PauseMenuContainer.SetActive(false);
                Time.timeScale = 1;
                break;           
            case "backtomenu":
                Time.timeScale = 1;
                WinText.gameObject.SetActive(false);
                currentPlayerText.gameObject.SetActive(true);
                States.SetMenuState(States.Enum.Menu_Loading);
                States.ResetState(States.currentGameState);
                States.ResetState(States.currentTurnState);
                EndGameMenuContainer.SetActive(false);
                PauseMenuContainer.SetActive(false);
                MenuCanvas.SetActive(true);
                GameCanvas.SetActive(false);
                Environment.SetActive(false);
                States.SetMenuState(States.Enum.Menu_InMenu);
                break;
            case "restart":
                States.SetGameState(States.Enum.Game_Loading);
                ResetScene();
                break;   
            case "takemoveback":

                Debug.Log("one Move back");
                break;

            default:
                Debug.LogWarning("Button: " + currentname + " not defined");
                break;
        }
    }

    void ResetScene()
    {
        Time.timeScale = 1;
        WinText.gameObject.SetActive(false);
        currentPlayerText.gameObject.SetActive(true);
        EndGameMenuContainer.SetActive(false);
        PauseMenuContainer.SetActive(false);
        currentPlayerText.gameObject.SetActive(true);
        FindObjectOfType<GamePlayHandler>().StartGame();
    }
    public Button FindButtonByName(string buttonName)
    {
        Button result = null;
        if (allButtons == null)
        {
            allButtons = FindObjectsOfType<Button>();
        }
        else
        {
            foreach (Button b in allButtons)
            {
                if (b!= null)
                {
                    if (b.name.Equals(buttonName))
                    {
                        result = b;
                        break;
                    }
                }
            }
        }

        if ( result == null)
        {
            Debug.LogWarning("Button: " + buttonName + " not Found");
        }
        return result;
    }

    public void ChangeButtonState(string buttonName, bool state)
    {
        Button b = FindButtonByName(buttonName);
        if (b != null)
        {
            b.gameObject.SetActive(state);
        }
        else
        {
            Debug.LogWarning("Button: " + buttonName + " not Found");

        }
    }

    void allocateGameGUIElements()
    {
        AllocateToggels();
        AllocateText();
        AllocateSlider();
        AllocateDropdown();

    }
    void allocateButtonsFunctions()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button b in buttons)
        {
            b.onClick.AddListener(() => ButtonFunctions());
        }
    }

    void AllocateToggels()
    {
        Toggle[] ts = FindObjectsOfType<Toggle>();
        foreach (Toggle t in ts)
        {
            if (t.name == "KI")
            {
                T_Ki = t;
                T_Ki.onValueChanged.AddListener(delegate {KIvsHumanToggle();});
            }
            if (t.name == "Human")
            {
                T_Human = t;
            }
            if (t.name == "ShowMoveBackButton")
            {
                T_MoveBack = t;
                T_MoveBack.onValueChanged.AddListener(delegate { ShowMoveBackButtonToggle(); });
            }
            if (T_Ki != null && T_Human != null && T_MoveBack != null)
            {
                break;
            }
        }
        if (T_Ki == null)
        {
            Debug.LogWarning("KI Toggle could not be allocated");
        }
        if (T_Human == null)
        {
            Debug.LogWarning("Human Toggle could not be allocated");
        }    
        if (T_MoveBack == null)
        {
            Debug.LogWarning("MoveBack Toggle could not be allocated");
        }
    }
    void AllocateSlider()
    {
        Slider[] S = FindObjectsOfType<Slider>();
        foreach (Slider s in S)
        {
            if (s.name == "Width")
            {
                S_Width = s;
                S_Width.onValueChanged.AddListener(delegate { changeWidth(S_Width); });
                S_Width.value = ConfigurationUtils.Width;

            }
            if (s.name == "Height")
            {
                S_Height = s;
                S_Height.onValueChanged.AddListener(delegate { changeHeight(S_Height); });
                S_Height.value = ConfigurationUtils.Height;


            }
            if (s.name == "Difficulty")
            {
                S_Difficulty = s;
                S_Difficulty.onValueChanged.AddListener(delegate { changeDiff(S_Difficulty); });
                S_Difficulty.value = 0;
            }
            if (S_Width != null && S_Height != null && S_Difficulty != null)
            {
                break;
            }
        }
        if (S_Width == null)
        {
            Debug.LogWarning("Width could not be allocated");
        }
        if (S_Height == null)
        {
            Debug.LogWarning("Heigth could not be allocated");
        }
        if (S_Difficulty == null)
        {
            Debug.LogWarning("Difficulty could not be allocated");
        }
    }
    void AllocateDropdown()
    {
        Dropdown[] D = FindObjectsOfType<Dropdown>();
        foreach (Dropdown d in D)
        {
            if (d.name == "DDPlayer1Sprite")
            {
                ChooseSpritePlayer1 = d;
                ChooseSpritePlayer1.ClearOptions();
                ChooseSpritePlayer1.AddOptions(CoinSpriteDD);


                ChooseSpritePlayer1.onValueChanged.AddListener(delegate{
                    changeSprite(GameManager.Players[0], ChooseSpritePlayer1.value); });

                int i1 = CoinSprites.FindIndex(x => x == GameManager.Players[0].playerSprite);
                ChooseSpritePlayer1.value = i1;
            }         
            if (d.name == "DDPlayer2Sprite")
            {
                ChooseSpritePlayer2 = d;
                ChooseSpritePlayer2.ClearOptions();
                ChooseSpritePlayer2.AddOptions(CoinSpriteDD);
                ChooseSpritePlayer2.onValueChanged.AddListener(delegate {
                    changeSprite(GameManager.Players[1], ChooseSpritePlayer2.value);
                });
                int i2 = CoinSprites.FindIndex(x => x == GameManager.Players[1].playerSprite);
                ChooseSpritePlayer2.value = i2;
            }
        }

        if (ChooseSpritePlayer1 == null)
        {
            Debug.LogWarning("ChooseSpritePlayer1 could not be allocated");
        }
        if (ChooseSpritePlayer2 == null)
        {
            Debug.LogWarning("ChooseSpritePlayer2 could not be allocated");
        }
    }
    void AllocateText()
    {
        Text[] texts = FindObjectsOfType<Text>();
        foreach (Text t in texts)
        {
            if (t.name == "PlayerInfoText")
            {
                currentPlayerText = t;
            }
            if (t.name == "Wintext")
            {
                WinText = t;
                WinText.gameObject.SetActive(false);
            }
            if (t.name == "TimeInfoText")
            {
                TimeText = t;
            }
        }

        if (currentPlayerText == null)
            Debug.LogWarning("Player TextField could not be allocated");
        if (WinText == null)
            Debug.LogWarning("Win TextField could not be allocated");
        if (TimeText == null)
            Debug.LogWarning("Time TextField could not be allocated");
    }
    void KIvsHumanToggle()
    {
        if (T_Ki.isOn)
        {
            States.SetGamePlayState(States.Enum.SinglePlayer);
        }
        else 
        {
            States.SetGamePlayState(States.Enum.MultiPlayer);
        }
    }
    void ShowMoveBackButtonToggle()
    {
        if (T_MoveBack.isOn)
            b_showMoveBackButton = true;
        else
            b_showMoveBackButton = false;
        FindButtonByName("TakeMoveBack").gameObject.SetActive(b_showMoveBackButton);
    }
    void changeWidth(Slider slider)
    {
        ConfigurationUtils.changeSettingValue(ConfigurationDataValueName.Width, (int)slider.value);
    }    
    void changeHeight(Slider slider)
    {
        ConfigurationUtils.changeSettingValue(ConfigurationDataValueName.Height, (int)slider.value);
    }
    void changeDiff(Slider slider)
    {
        ConfigurationUtils.changeSettingValue(ConfigurationDataValueName.Difficulty, (int)slider.value);    
    }
    void changeSprite(Player player, int spriteIndex)
    {
        player.playerSprite = CoinSprites[spriteIndex];
    }
  

}
