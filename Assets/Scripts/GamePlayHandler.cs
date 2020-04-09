using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayHandler : MonoBehaviour
{
    GameGrid GameGrid;
    [SerializeField]    GameObject CoinPrefab;  

    [SerializeField]    GameObject currentCoin;

    [SerializeField]    GameObject Stack1;   
    [SerializeField]    GameObject Stack2;
    [SerializeField]    GameObject FieldContainer;

    [SerializeField]    GameObject GameElementsContainer;

    [SerializeField]    int currentCollum;

    public Timer RemainingTime;

    int numberofTry = 0;

    // Start is called before the first frame update
    void Awake()
    {
        RemainingTime = gameObject.AddComponent<Timer>();
        RemainingTime.Duration = GameManager.currentPlayer_ThinkTime;
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
        if (GameGrid != null)
        {
            GameGrid.AllEntrySlots = null;
            GameGrid.gridslots = null;
        }
        GameGrid = new GameGrid();
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

        if (GameManager.currentPlayer.playingOrder == 0)
        {
            currentCoin.transform.position = GameGrid.entryslots[0].WorldPosition_Center;
            currentCollum = 0;
        }
        else if (GameManager.currentPlayer.playingOrder == 1)
        {
            currentCoin.transform.position = GameGrid.entryslots[GameGrid.entryslots.Length - 1].WorldPosition_Center;
            currentCollum = GameGrid.entryslots.Length - 1;
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
        if (GameManager.currentPlayer.playingOrder == 0)
               nextPlayer = GameManager.Players[1];
        else
            nextPlayer = GameManager.Players[0];

        currentCoin = nextPlayer.CoinStack[0];

        Stack1.SetActive(false);
        Stack2.SetActive(false);
        numberofTry = 0;
        if (nextPlayer.playerType == Player.PlayerType.Human)
        {
            if (nextPlayer.playingOrder ==0)
            {
                States.SetTurnState(States.Enum.HumanPlayer1Turn);
                Stack1.SetActive(true);
                currentCoin.transform.position = GameGrid.entryslots[0].WorldPosition_Center;
                currentCollum = 0;
            }
            else
            {
                States.SetTurnState(States.Enum.HumanPlayer2Turn);
                Stack2.SetActive(true);
                currentCoin.transform.position = GameGrid.entryslots[GameGrid.entryslots.Length - 1].WorldPosition_Center;
                currentCollum = GameGrid.entryslots.Length - 1;
            }
            RemainingTime.ForceTimerReset(GameManager.currentPlayer_ThinkTime);
        }
        else
        {
            States.SetTurnState(States.Enum.ComputerPlayerTurn);
            Stack2.SetActive(true);
            currentCoin.transform.position = GameGrid.entryslots[GameGrid.entryslots.Length - 1].WorldPosition_Center;
            currentCollum = GameGrid.entryslots.Length - 1;
            RemainingTime.ForceTimerReset(2);
            RemainingTime.ForceTimerReset(3);
        }

        //GameManager.currentPlayer.CoinStack.Remove(currentCoin);

        GameManager.currentPlayer = nextPlayer;

        EventManager.CallUpdateUIOnTurnEndEvent();
    }
    public void ReturnCoinToStack(Coin coin)
    {
        currentCoin.transform.position = Vector3.zero;
        currentCoin = null;

        coin.Owner.CoinStack.Add(coin.gameObject);
        GameGrid.RemoveCoin(coin);
        if (coin.Owner.playingOrder == 0)
        {
            coin.gameObject.transform.SetParent(Stack1.transform,false);
        }
        else
        {
            coin.gameObject.transform.SetParent(Stack2.transform,false);
        }
        // remove from grid
    }

    void GeneratePlayerCoinStacks()
    {
        GameManager.Players[0].CoinStack.Clear();
        GameManager.Players[1].CoinStack.Clear();
        Stack1.transform.position = new Vector3(GameGrid.startPosition.x - 1, GameGrid.startPosition.y, 0);
        Stack2.transform.position = new Vector3(-GameGrid.startPosition.x + 1, GameGrid.startPosition.y, 0);

        for (int i = 0; i < GameGrid.NbOfCells/2; i++)
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
            

            if (RemainingTime.Finished&& GameManager.currentPlayer.playerType == Player.PlayerType.Human)
            {
                RandomHumanMove();
            }
            if (RemainingTime.Finished && GameManager.currentPlayer.playerType == Player.PlayerType.Computer)
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
            Audiomanager.PlaySound(Audiomanager.Sounds.Drop);
            if (!CheckIfWin())
            {
                //changeToHumanPlayer();
                SetNextTurn();
            }
        }
    }

    
    void RandomHumanMove()
    {
        if (GameGrid.AddCoinAtPosition(Random.Range(0, GameGrid.entryslots.Length), currentCoin.GetComponent<Coin>()))
        {
            if (!CheckIfWin())
            {
                SetNextTurn();
            }
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
            if (!CheckIfWin())
            {
                SetNextTurn();
            }
        }
        else if (numberofTry < 10)
        {
            numberofTry++;
            ComputerMove();
        }
    }

    // TODO: more efficient if ony the new Coin is checked
    public bool CheckIfWin()
    {
        bool won = false;
        for (int x = 0; x < GameGrid.Width ; x++)
        {
            for (int y = 0; y < GameGrid.Height; y++)
            {
                 won = (GameGrid.collectNeigbours(GameGrid.getGridElement(x, y), GameManager.currentPlayer));
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
