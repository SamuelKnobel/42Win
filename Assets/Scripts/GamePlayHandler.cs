using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayHandler : MonoBehaviour
{
    GameGrid GameGrid;
    [SerializeField]
    GameObject CoinPrefab;  

    [SerializeField]
    GameObject currentCoin;

    [SerializeField]
    GameObject Stack1;   
    [SerializeField]
    GameObject Stack2;
    [SerializeField]
    GameObject FieldContainer;

    [SerializeField]
    GameObject GameElementsContainer;

    [SerializeField]
    int currentCollum;

    public Timer RemainingTime;
    public Timer RemainingTimeAIMove;

    public List<GameObject> StackPlayer1 = new List<GameObject>();
    public List<GameObject> StackPlayer2 = new List<GameObject>();

    int numberofTry = 0;

    // Start is called before the first frame update
    void Awake()
    {
        RemainingTime = gameObject.AddComponent<Timer>();
        RemainingTimeAIMove = gameObject.AddComponent<Timer>();
        
    }
   public void StartGame()
    {
        foreach (Transform child in Stack1.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Stack2.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in FieldContainer.transform)
        {
            Destroy(child.gameObject);
        }
        if (GameGrid != null)
        {
            GameGrid.AllEntrySlots = null;
            GameGrid.gridslots = null;
        }
        GameGrid = new GameGrid();
        GeneratePlayerCoinStacks();
        changeToHumanPlayer();
    }

    void GeneratePlayerCoinStacks()
    {
        StackPlayer1.Clear();
        StackPlayer2.Clear();

        for (int i = 0; i < GameGrid.NbOfCells/2; i++)
        {
            GameObject NewCoinP1 = Instantiate(CoinPrefab);
            NewCoinP1.transform.position = new Vector3(GameGrid.startPosition.x - 1, GameGrid.startPosition.y, 0);
            NewCoinP1.tag = "Coin";
            NewCoinP1.GetComponent<SpriteRenderer>().sprite = GameManager.SpritePlayer1;
            NewCoinP1.transform.SetParent(Stack1.transform);
            NewCoinP1.GetComponent<Coin>().owner = PlayerName.Player1;
            StackPlayer1.Add(NewCoinP1);

            GameObject newCoinP2 = Instantiate(CoinPrefab);
            newCoinP2.transform.position = new Vector3(-GameGrid.startPosition.x + 1, GameGrid.startPosition.y, 0);
            newCoinP2.tag = "Coin";
            newCoinP2.GetComponent<SpriteRenderer>().sprite = GameManager.SpritePlayer2;
            newCoinP2.transform.SetParent(Stack2.transform);
            newCoinP2.GetComponent<Coin>().owner = PlayerName.Player2;
            StackPlayer2.Add(newCoinP2);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (States.compareState(States.currentGameState, States.Enum.Game_InGame))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveCoinLeft();
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveCoinRight();
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (GameManager.enemyPlayer == EnemyType.Computer)
                {
                    if (GameManager.currentPlayer == PlayerName.Player1)
                    {
                        DropCoin();
                    }
                }
                else
                    DropCoin();
            }

            if (RemainingTime.Finished&& GameManager.currentPlayer == PlayerName.Player1)
            {
                RandomHumanMove();
            }
            if (RemainingTimeAIMove.Finished && GameManager.currentPlayer == PlayerName.Player2)
            {
                ComputerMove();
            }
        }

    }
    private void MoveCoinLeft()
    {
        currentCollum--;
        if (currentCollum >= 0)
        {
            currentCoin.transform.position = GameGrid.entryslots[currentCollum].WorldPosition_Center;
        }
        else
            currentCollum = 0;
    }
    private void MoveCoinRight()
    {
        currentCollum++;
        if (currentCollum < GameGrid.entryslots.Length)
        {
            currentCoin.transform.position = GameGrid.entryslots[currentCollum].WorldPosition_Center;
        }
        else
            currentCollum = GameGrid.entryslots.Length - 1;
    }
    private void DropCoin()
    {
        if (GameGrid.AddCoinAtPosition(currentCollum, currentCoin.GetComponent<Coin>()))
        {
            currentCoin.transform.SetParent(FieldContainer.transform);
            CheckIfWin();
            changeToHumanPlayer();
            Audiomanager.PlaySound(Audiomanager.Sounds.Drop);
        }
    }

    
    void RandomHumanMove()
    {
        if (GameGrid.AddCoinAtPosition(Random.Range(0, GameGrid.entryslots.Length), currentCoin.GetComponent<Coin>()))
        {
            currentCoin.transform.SetParent(FieldContainer.transform);
            CheckIfWin();
            changeToHumanPlayer();
        }
        else if (numberofTry<10)
        {
            numberofTry++;
            RandomHumanMove();
        }
    }
    void ComputerMove()
    {
        if (GameGrid.AddCoinAtPosition(Random.Range(0, GameGrid.entryslots.Length), currentCoin.GetComponent<Coin>()))
        {
            currentCoin.transform.SetParent(FieldContainer.transform);
            CheckIfWin();
            changeToHumanPlayer();
        }
        else if (numberofTry < 10)
        {
            numberofTry++;
            ComputerMove();
        }
    }
        
    public void changeToHumanPlayer()
    {
        numberofTry = 0;
        RemainingTime.Duration = GameManager.currentPlayer_ThinkTime;  
        RemainingTime.Run();
        if (GameManager.currentPlayer.Equals(PlayerName.Player1))
        {
            StackPlayer1.Remove(currentCoin);
            GameManager.currentPlayer = PlayerName.Player2;
            Stack1.SetActive(false);
            Stack2.SetActive(true);
        }
        else if (GameManager.currentPlayer.Equals(PlayerName.Player2))
        {
            StackPlayer2.Remove(currentCoin);
            GameManager.currentPlayer = PlayerName.Player1;
            Stack2.SetActive(false);
            Stack1.SetActive(true);

        }
        FindObjectOfType<HUD>().ShowPlayerInfo();

        if (GameManager.currentPlayer == PlayerName.Player1)
        {
            currentCoin = StackPlayer1[0];
            currentCoin.transform.position = GameGrid.entryslots[0].WorldPosition_Center;
            currentCollum = 0;
        }
        else if (GameManager.currentPlayer == PlayerName.Player2)
        {
            currentCoin = StackPlayer2[0];
            currentCoin.transform.position = GameGrid.entryslots[GameGrid.entryslots.Length - 1].WorldPosition_Center;
            currentCollum = GameGrid.entryslots.Length - 1;
        }
        if (GameManager.currentPlayer == PlayerName.Player2 && GameManager.enemyPlayer == EnemyType.Computer)
        {
            ChangeToComputerPlayer();
        }
    }

    public void ChangeToComputerPlayer()
    {
        numberofTry = 0;
        RemainingTimeAIMove.Duration = 2;
        RemainingTimeAIMove.Run();
        RemainingTime.Duration = 3.01f;
        RemainingTime.Run();

    }

    // TODO: more efficient if ony the new Coin is checked
    public void CheckIfWin()
    {
        for (int x = 0; x < GameGrid.Width ; x++)
        {
            for (int y = 0; y < GameGrid.Height; y++)
            {
                bool won = (GameGrid.collectNeigbours(GameGrid.getGridElement(x, y), GameManager.currentPlayer));
                if (won)
                {
                    Debug.Log("Win" + GameManager.currentPlayer);
                    //GameElementsContainer.SetActive(false);
                    FindObjectOfType<HUD>().ShowEndGameMenu();
                    States.SetGameState(States.Enum.Game_End);
                    FindObjectOfType<HUD>().ShowPlayerWin();
                    // TODO Call Event 
                    break;

                }
            }
        }
    }



}
