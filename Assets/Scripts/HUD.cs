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

    [SerializeField]    Toggle T_KiToggle;   
    [SerializeField]    Toggle T_HumanToggle;
    [SerializeField]    Slider S_Width;
    [SerializeField]    Slider S_Height;    
    [SerializeField]    Slider S_Difficulty;

    List<Dropdown.OptionData> CoinSpriteDD = new List<Dropdown.OptionData>();
    [SerializeField] Dropdown ChooseSpritePlayer1;
    [SerializeField] Dropdown ChooseSpritePlayer2;



    public List<Sprite> CoinSprites;

    [SerializeField]    Text currentPlayerText;
    string PreTextPlayerInfo = "Current Player: ";
    [SerializeField]    Text WinText;
    string PreTextPlayerWin = "Winner: ";
    [SerializeField] public Text TimeText;
    public string PreTextTime = "Remaining Time: ";


    [SerializeField]
    Button[] allButtons;

    private void Awake()
    {
        if (CoinSprites.Count > 0)
        {
            GameManager.SpritePlayer1 = CoinSprites[0];
            GameManager.SpritePlayer2 = CoinSprites[CoinSprites.Count - 1];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
        allocateGameGUIElements();

            PauseMenuContainer = GameObject.FindGameObjectWithTag("PauseMenuContainer");
        if (PauseMenuContainer != null)
            PauseMenuContainer.SetActive(false);

            EndGameMenuContainer = GameObject.FindGameObjectWithTag("EndGameMenuContainer");
        if (EndGameMenuContainer != null)
            EndGameMenuContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.currentGamestate == GameStates.InGame)
        {
            GamePlayHandler handler = FindObjectOfType<GamePlayHandler>();
            if (handler != null)
            {
                if (TimeText != null)
                {
                    if (GamePlayHandler.currentPlayer == PlayerName.Player2 && GamePlayHandler.enemyPlayer == EnemyType.Computer)
                    {
                        TimeText.text = PreTextTime + Mathf.Round(handler.RemainingTimeAIMove.RemainTime);
                    }
                    else
                        TimeText.text = PreTextTime + Mathf.Round(handler.RemainingTime.RemainTime);
                }
            }
        }
        if (GameManager.currentGamestate == GameStates.InGame|| GameManager.currentGamestate == GameStates.GameEnd)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseMenuContainer.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    // TODO: Solve it with events!
    public void ShowPlayerInfo()
    {
        if (currentPlayerText!= null)
        {
            currentPlayerText.text = PreTextPlayerInfo + GamePlayHandler.currentPlayer;
        }
    }

    public void ShowPlayerWin()
    {
        WinText.gameObject.SetActive(true);
        currentPlayerText.gameObject.SetActive(false);
        WinText.text = PreTextPlayerWin + GamePlayHandler.currentPlayer;

        if (GamePlayHandler.currentPlayer == PlayerName.Player1 && GamePlayHandler.enemyPlayer == EnemyType.Computer )
            Audiomanager.PlaySound(Audiomanager.Sounds.Win);
        else if (GamePlayHandler.currentPlayer == PlayerName.Player2 && GamePlayHandler.enemyPlayer == EnemyType.Computer)
            Audiomanager.PlaySound(Audiomanager.Sounds.Loose);

    }


    public void ShowEndGameMenu()
    {
        EndGameMenuContainer.SetActive(true);
    }
    public void KIvsMultiplayer(bool toggle)
    {
        if (toggle)
        {
            GamePlayHandler.enemyPlayer = EnemyType.Computer;
        }
        else
        {
            GamePlayHandler.enemyPlayer = EnemyType.Human;
        }
    }

    public void ButtonFunctions()
    {
        Audiomanager.PlaySound(Audiomanager.Sounds.Button);
        string currentname = EventSystem.current.currentSelectedGameObject.name;

        switch (currentname.ToLower())
        {
            case "start":
                SceneManager.LoadScene("GamePlay");
                GameManager.SwitchStateTo(GameStates.LoadGame);
                break;
            case "exit":
                if (GameManager.currentGamestate == GameStates.InGame|| GameManager.currentGamestate == GameStates.InMainMenu)
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }
                else if(GameManager.currentGamestate == GameStates.InHelp|| GameManager.currentGamestate == GameStates.InSettings)
                {
                    HelpMenuContainer.SetActive(false);
                    SettingsContainer.SetActive(false);
                    MainMenuContainer.SetActive(true);
                    GameManager.SwitchStateTo(GameStates.InMainMenu);
                }
                break;
            case "settings":
                GameManager.SwitchStateTo(GameStates.InSettings);
                HelpMenuContainer.SetActive(false);
                SettingsContainer.SetActive(true);
                MainMenuContainer.SetActive(false);
                break;
            case "help":
                GameManager.SwitchStateTo(GameStates.InHelp);
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
                SceneManager.LoadScene("Menu");
                GameManager.SwitchStateTo(GameStates.LoadMenu);
                break;
            case "restart":
                
                ResetScene();
                GameManager.SwitchStateTo(GameStates.LoadGame);
                break;

            default:
                Debug.LogWarning("Button: " + currentname + " not defined");
                break;
        }
    }

    void ResetScene()
    {
        Time.timeScale = 1;
        EndGameMenuContainer.SetActive(true);
        PauseMenuContainer.SetActive(true);
        currentPlayerText.gameObject.SetActive(true);


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
                if (b.name.Equals(buttonName))
                {
                    result = b;
                    break;
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

    //TODO:To be allocated if switched into GameScene
    void allocateGameGUIElements()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button b in buttons)
        {
            b.onClick.AddListener(() => ButtonFunctions());
        }
        AllocateToggels();
        AllocateText();
        AllocateSlider();
        AllocateDropdown();

    }


    void AllocateToggels()
    {
        Toggle[] ts = FindObjectsOfType<Toggle>();
        foreach (Toggle t in ts)
        {
            if (t.name == "KI")
            {
                T_KiToggle = t;
                T_KiToggle.onValueChanged.AddListener(delegate {KIvsHumanToggle();});
            }
            if (t.name == "Human")
            {
                T_HumanToggle = t;
            }
            if (T_KiToggle != null && T_HumanToggle != null)
            {
                break;
            }
        }
        if (T_KiToggle == null)
        {
            Debug.LogWarning("KI Toggle could not be allocated");
        }
        if (T_HumanToggle == null)
        {
            Debug.LogWarning("Human Toggle could not be allocated");
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

                ChooseSpritePlayer1.onValueChanged.AddListener(delegate {
                    changeSprite(PlayerName.Player1, ChooseSpritePlayer1.value);
                });
                int i1 = CoinSprites.FindIndex(x => x == GameManager.SpritePlayer1);
                ChooseSpritePlayer1.value = i1;
            }         
            if (d.name == "DDPlayer2Sprite")
            {
                ChooseSpritePlayer2 = d;
                ChooseSpritePlayer2.ClearOptions();
                ChooseSpritePlayer2.AddOptions(CoinSpriteDD);
                ChooseSpritePlayer2.onValueChanged.AddListener(delegate {
                    changeSprite(PlayerName.Player2, ChooseSpritePlayer2.value);
                });
                int i2 = CoinSprites.FindIndex(x => x == GameManager.SpritePlayer2);
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
        if (T_KiToggle.isOn)
        {
            GamePlayHandler.enemyPlayer = EnemyType.Computer;
        }
        else 
        {
            GamePlayHandler.enemyPlayer = EnemyType.Human;
        }
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
    void changeSprite(PlayerName playerName, int spriteIndex)
    {
        switch (playerName)
        {
            case PlayerName.Player1:
                GameManager.SpritePlayer1 = CoinSprites[spriteIndex];
                break;
            case PlayerName.Player2:
                GameManager.SpritePlayer2 = CoinSprites[spriteIndex];
                break;
            case PlayerName.Empty:
                break;
            default:
                break;
        }
    }

}
