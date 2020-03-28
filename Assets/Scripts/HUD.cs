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
    Toggle KiToggle;   
    [SerializeField]
    Toggle HumanToggle;
    [SerializeField]
    Slider S_Width;
    [SerializeField]
    Slider S_Height;    
    [SerializeField]
    Slider S_Difficulty;
    [SerializeField]
    Text currentPlayerText;
    string PreTextPlayerInfo = "Current Player: ";

    [SerializeField]
    Text WinText;
    string PreTextPlayerWin = "Winner: ";


    [SerializeField]
    Button[] allButtons;

    private void Awake()
    {



    }
    // Start is called before the first frame update
    void Start()
    {
        MainMenuContainer = GameObject.FindGameObjectWithTag("MainMenuContainer");
        SettingsContainer = GameObject.FindGameObjectWithTag("SettingsContainer");
        HelpMenuContainer = GameObject.FindGameObjectWithTag("HelpMenuContainer");
        allButtons = FindObjectsOfType<Button>();
        allocateGameGUIElements();
        if (HelpMenuContainer != null)
            HelpMenuContainer.SetActive(false);
        if (SettingsContainer != null)
            SettingsContainer.SetActive(false);

        GameManager.currentGamestate = GameStates.InMainMenu;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: Solve it with events!
    public void ShowPlayerInfo()
    {
        currentPlayerText.text = PreTextPlayerInfo + GamePlayHandler.currentPlayer;
    }

    public void ShowPlayerWin()
    {
        WinText.gameObject.SetActive(true);
        currentPlayerText.gameObject.SetActive(false);
        WinText.text = PreTextPlayerWin + GamePlayHandler.currentPlayer;

    }

    //TODO: Has to Start if the GameScene has loded
    public void StartGame()
    {
        KiToggle.gameObject.SetActive(false);        
        FindObjectOfType<GamePlayHandler>().StartGame();        
    }

    public void KIvsMultiplayer(bool toggle)
    {
        if (toggle)
        {
            GamePlayHandler.enemyPlayer = PlayerType.Computer;
        }
        else
        {
            GamePlayHandler.enemyPlayer = PlayerType.Human;
        }
    }

    public void ButtonFunctions()
    {
        string currentname = EventSystem.current.currentSelectedGameObject.name;

        switch (currentname.ToLower())
        {
            case "start":
                SceneManager.LoadScene("GamePlay");
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
                    GameManager.currentGamestate = GameStates.InMainMenu;
                }
                break;
            case "settings":
                GameManager.currentGamestate = GameStates.InSettings;
                HelpMenuContainer.SetActive(false);
                SettingsContainer.SetActive(true);
                MainMenuContainer.SetActive(false);
                break;
            case "help":
                GameManager.currentGamestate = GameStates.InHelp;
                HelpMenuContainer.SetActive(true);
                SettingsContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                break;
            default:
                Debug.LogWarning("Button: " + currentname + " not defined");
                break;
        }
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

    }


    void AllocateToggels()
    {
        Toggle[] ts = FindObjectsOfType<Toggle>();
        foreach (Toggle t in ts)
        {
            if (t.name == "KI")
            {
                KiToggle = t;
                KiToggle.onValueChanged.AddListener(delegate {KIvsHumanToggle();});
            }
            if (t.name == "Human")
            {
                HumanToggle = t;
            }
            if (KiToggle != null && HumanToggle != null)
            {
                break;
            }
        }
        if (KiToggle == null)
        {
            Debug.LogWarning("KI Toggle could not be allocated");
        }
        if (HumanToggle == null)
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
                S_Width.value = ConfigurationUtils.Width;
                S_Width.onValueChanged.AddListener(delegate { changeWidth(S_Width); });
            }
            if (s.name == "Height")
            {
                S_Height = s;
                S_Height.value = ConfigurationUtils.Height;
                S_Height.onValueChanged.AddListener(delegate { changeHeight(S_Width); });

            }
            if (s.name == "Difficulty")
            {

                S_Difficulty = s;
                S_Difficulty.value = 0;
                S_Difficulty.onValueChanged.AddListener(delegate { changeDiff(S_Width); });
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
        }

        if (currentPlayerText == null)
            Debug.LogWarning("TextField could not be allocated");

        if (WinText == null)
            Debug.LogWarning("TextField could not be allocated");
    }

    void KIvsHumanToggle()
    {
        if (KiToggle.isOn)
        {
            GamePlayHandler.enemyPlayer = PlayerType.Computer;
        }
        else 
        {
            GamePlayHandler.enemyPlayer = PlayerType.Human;
        }
        print(GamePlayHandler.enemyPlayer);

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

}
