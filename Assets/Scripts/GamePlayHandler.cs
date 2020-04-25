using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayHandler : MonoBehaviour
{
    [SerializeField]    GameObject CoinPrefab;  
    [SerializeField]    GameObject currentCoin;

    [SerializeField]    GameObject Stack1;   
    [SerializeField]    GameObject Stack2;
    [SerializeField]    GameObject FieldContainer;

    [SerializeField]    GameObject GameElementsContainer;

    [SerializeField]    int currentCollum;

    public Timer RemainingTime;
    //public Timer ComputerMoveTimer;

    public int NextMoveCol = -1;
    int numberofTry = 0;


    private void OnEnable()
    {
        EventManager.AllThreadsEndedEvent += SetNextMoveForComupter;
    }
    private void OnDisable()
    {
        EventManager.AllThreadsEndedEvent -= SetNextMoveForComupter;
    }



    // Start is called before the first frame update
    void Awake()
    {

        RemainingTime = gameObject.AddComponent<Timer>();
        //ComputerMoveTimer = gameObject.AddComponent<Timer>();
        RemainingTime.Duration = GameManager.currentPlayer_ThinkTime;
        //ComputerMoveTimer.Duration = .2f;
    }

   public void StartGame()
    {
        GameManager.playedCoins.Clear();
        RemainingTime.ForceTimerReset(GameManager.currentPlayer_ThinkTime);
        foreach (Transform child in Stack1.transform)
            Destroy(child.gameObject);
        foreach (Transform child in Stack2.transform)
            Destroy(child.gameObject);
        foreach (Transform child in FieldContainer.transform)
            Destroy(child.gameObject);
 
        if (GameManager.GameGrid != null)
        {
            GameManager.GameGrid.AllEntrySlots = null;
            GameManager.GameGrid.gridslots = null;
        }
        GameManager.GameGrid = new GameGrid();
        GeneratePlayerCoinStacks();

        if (States.compareState(States.currentGamePlayState, States.Enum.MultiPlayer))
        {
            GameManager.Players[0].playerType = Player.PlayerType.Human;
            GameManager.Players[1].playerType = Player.PlayerType.Human;
            GameManager.Players[1].playerName = "Human2";

        }
        else if(States.compareState(States.currentGamePlayState, States.Enum.SinglePlayer))
        {
            GameManager.Players[0].playerType = Player.PlayerType.Human;
            GameManager.Players[1].playerType = Player.PlayerType.Computer;
            GameManager.Players[1].playerName = "Computer";
        }
        GameManager.currentPlayer = GameManager.Players[0];

        if (GameManager.currentPlayer.playerType == Player.PlayerType.Human)
            States.SetTurnState(States.Enum.HumanPlayer1Turn);
        else if (GameManager.currentPlayer.playerType == Player.PlayerType.Computer)
            States.SetTurnState(States.Enum.ComputerPlayerTurn);

        currentCoin = GameManager.currentPlayer.CoinStack[0];

        if (GameManager.currentPlayer.PlayerIndex == 0)
        {
            currentCoin.transform.position = GameManager.GameGrid.entryslots[0].WorldPosition_Center;
            currentCollum = 0;
        }
        else if (GameManager.currentPlayer.PlayerIndex == 1)
        {
            currentCoin.transform.position = GameManager.GameGrid.entryslots[GameManager.GameGrid.entryslots.Length - 1].WorldPosition_Center;
            currentCollum = GameManager.GameGrid.entryslots.Length - 1;
        }
        Stack1.SetActive(true);
        Stack2.SetActive(false);

        EventManager.CallUpdateUIOnTurnEndEvent();
        States.SetGameState(States.Enum.Game_InGame);

    }

    public void SetNextTurn()
    {
        if (currentCoin != null)
        {
            currentCoin.transform.SetParent(FieldContainer.transform);
            GameManager.currentPlayer.CoinStack.Remove(currentCoin);
            GameManager.playedCoins.Add(currentCoin.GetComponent<Coin>());
        }
        currentCoin = null;

        Player nextPlayer = null;
        if (GameManager.currentPlayer.PlayerIndex == 0)
               nextPlayer = GameManager.Players[1];
        else
            nextPlayer = GameManager.Players[0];

        currentCoin = nextPlayer.CoinStack[0];

        Stack1.SetActive(false);
        Stack2.SetActive(false);
        numberofTry = 0;
        if (nextPlayer.playerType == Player.PlayerType.Human)
        {
            if (nextPlayer.PlayerIndex ==0)
            {
                States.SetTurnState(States.Enum.HumanPlayer1Turn);
                Stack1.SetActive(true);
                currentCoin.transform.position = GameManager.GameGrid.entryslots[0].WorldPosition_Center;
                currentCollum = 0;
            }
            else
            {
                States.SetTurnState(States.Enum.HumanPlayer2Turn);
                Stack2.SetActive(true);
                currentCoin.transform.position = GameManager.GameGrid.entryslots[GameManager.GameGrid.entryslots.Length - 1].WorldPosition_Center;
                currentCollum = GameManager.GameGrid.entryslots.Length - 1;
            }
            RemainingTime.ForceTimerReset(GameManager.currentPlayer_ThinkTime);
        }
        else
        {
            Debug.Log("Start Computer Move");
            States.SetTurnState(States.Enum.ComputerPlayerTurn);
            Stack2.SetActive(true);
            currentCoin.transform.position = GameManager.GameGrid.entryslots[GameManager.GameGrid.entryslots.Length - 1].WorldPosition_Center;
            currentCollum = GameManager.GameGrid.entryslots.Length - 1;
            RemainingTime.ForceTimerReset(GameManager.currentAI_ThinkTime);
            FindObjectOfType<GameManager>().StartTreeBuilding();
            nextPlayer.StartBuildTree(nextPlayer.PlayerIndex); 
        }

        GameManager.currentPlayer = nextPlayer;

        EventManager.CallUpdateUIOnTurnEndEvent();
    }
    public void ReturnCoinToStack(Coin coin)
    {
        currentCoin.transform.position = Vector3.zero;
        currentCoin = null;

        coin.Owner.CoinStack.Add(coin.gameObject);
        GameManager.GameGrid.RemoveCoin(coin);
        if (coin.Owner.PlayerIndex == 0)
        {
            coin.gameObject.transform.SetParent(Stack1.transform,false);
            coin.gameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            coin.gameObject.transform.SetParent(Stack2.transform,false);
            coin.gameObject.transform.localPosition = Vector3.zero;
        }
        SetNextTurn();
    }

    void GeneratePlayerCoinStacks()
    {
        GameManager.Players[0].CoinStack.Clear();
        GameManager.Players[1].CoinStack.Clear();
        Stack1.transform.position = new Vector3(GameManager.GameGrid.startPosition.x - 1, GameManager.GameGrid.startPosition.y, 0);
        Stack2.transform.position = new Vector3(-GameManager.GameGrid.startPosition.x + 1, GameManager.GameGrid.startPosition.y, 0);

        for (int i = 0; i < GameManager.GameGrid.NbOfCells/2; i++)
        {
            GameObject NewCoinP1 = Instantiate(CoinPrefab);
            NewCoinP1.tag = "Coin";
            NewCoinP1.GetComponent<SpriteRenderer>().sprite = GameManager.Players[0].playerSprite;
            NewCoinP1.transform.SetParent(Stack1.transform,false);
            NewCoinP1.GetComponent<Coin>().Owner = GameManager.Players[0];

            //StackPlayer1.Add(NewCoinP1);
            GameManager.Players[0].CoinStack.Add(NewCoinP1);

            GameObject NewCoinP2 = Instantiate(CoinPrefab);
            NewCoinP2.tag = "Coin";
            NewCoinP2.GetComponent<SpriteRenderer>().sprite = GameManager.Players[1].playerSprite;
            NewCoinP2.transform.SetParent(Stack2.transform,false);
            NewCoinP2.GetComponent<Coin>().Owner = GameManager.Players[1];
            //StackPlayer2.Add(NewCoinP2);
            GameManager.Players[1].CoinStack.Add(NewCoinP2);

        }
    }


    // Update is called once per frame
    void Update()
    {
        if (States.compareState(States.currentGameState, States.Enum.Game_InGame))
        {
            if (GameManager.currentPlayer.playerType == Player.PlayerType.Human)
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
                    DropCoin();
                }
            }
            

            if (RemainingTime.Finished)
            {
                switch (GameManager.currentPlayer.playerType)
                {
                    case Player.PlayerType.Human:
                        RandomMove();
                        break;
                    case Player.PlayerType.Computer:
                        ComputerMove();
                        break;
                }
            }
        }
        //if (ComputerMoveTimer.Finished)
        //{
        //    OnSetComputerMoveTimerFinished();

        //}


    }
    private void MoveCoinLeft()
    {
        currentCollum--;
        if (currentCollum >= 0)
        {
            currentCoin.transform.position = GameManager.GameGrid.entryslots[currentCollum].WorldPosition_Center;
        }
        else
            currentCollum = 0;
    }
    private void MoveCoinRight()
    {
        currentCollum++;
        if (currentCollum < GameManager.GameGrid.entryslots.Length)
        {
            currentCoin.transform.position = GameManager.GameGrid.entryslots[currentCollum].WorldPosition_Center;
        }
        else
            currentCollum = GameManager.GameGrid.entryslots.Length - 1;
    }
    private void DropCoin()
    {
        if (GameManager.GameGrid.AddCoinAtPosition(currentCollum, currentCoin.GetComponent<Coin>()))
        {
            if (GameManager.FirstMove)
            {
                GameManager.FirstMove = false;
            }
            Audiomanager.PlaySound(Audiomanager.Sounds.Drop);
            if (!CheckIfWin())
            {
                //changeToHumanPlayer();
                SetNextTurn();
            }
        }
    }

    
    void RandomMove()
    {
        if (GameManager.GameGrid.AddCoinAtPosition(Random.Range(0, GameManager.GameGrid.entryslots.Length), currentCoin.GetComponent<Coin>()))
        {
            if (!CheckIfWin())
            {
                SetNextTurn();
            }
        }
        else if (numberofTry<10)
        {
            numberofTry++;
            RandomMove();
        }
    }
    void ComputerMove()
    {
        //NextMoveCol = GameManager.currentPlayer.FindBestMove();
        if (GameManager.GameGrid.AddCoinAtPosition(NextMoveCol, currentCoin.GetComponent<Coin>()))
        {
            if (!CheckIfWin())
            {
                SetNextTurn();
            }
        }
        else
            RandomMove();
    }

    void SetNextMoveForComupter(float unused)
    {
        NextMoveCol = GameManager.currentPlayer.FindBestMove();
        print(NextMoveCol);
    }


    // TODO: more efficient if ony the new Coin is checked
    public bool CheckIfWin()
    {
        bool won = false;
        for (int x = 0; x < GameManager.GameGrid.Width ; x++)
        {
            for (int y = 0; y < GameManager.GameGrid.Height; y++)
            {
                 won = (GameManager.GameGrid.collectNeigbours(GameManager.GameGrid.getGridElement(x, y), GameManager.currentPlayer));
                if (won)
                {
                    //GameElementsContainer.SetActive(false);
                    States.SetGameState(States.Enum.Game_End);
                    EventManager.CallGameEndEvent();
                    return won;
                }
            }
        }
        return won;
    }



}
