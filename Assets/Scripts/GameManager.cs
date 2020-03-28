using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // TODO: Stats Tracken for KI

    public static GameManager gameManager;


    public static GameStates currentGamestate;
    public static float currentAI_ThinkTime;
    public static int currentAI_ThinkDepth;
    public static float currentPlayer_ThinkTime;

    public GameStates state;

    private void Awake()
    {
        currentGamestate = GameStates.LoadGame;
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
        state = currentGamestate;


    }
}
