using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayHandler : MonoBehaviour
{

    GameGrid GameGrid;
    [SerializeField]
    GameObject CoinPrefab;  
    [SerializeField]
    GameObject ArrowPrefab;

    [SerializeField]
    GameObject currentCoin;
    [SerializeField]
    GameObject coinToMove;

    //bool startMovement;
    [SerializeField]
    Material Material_P1;
    [SerializeField]
    Material Material_P2;


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

    public static PlayerName currentPlayer = PlayerName.Player2;
    public static PlayerType enemyPlayer = PlayerType.Human;

    public List<GameObject> StackPlayer1 = new List<GameObject>();
    public List<GameObject> StackPlayer2 = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        StartGame();

        //Debug.Log(GameGrid.EmptySlots.Count);
        //Debug.Log(GameGrid.FullSlots.Count);
    }
   public void StartGame()
    {
        RemainingTime = gameObject.AddComponent<Timer>();



        GameGrid = new GameGrid();

        GeneratePlayerCoinStacks();

        changePlayer();

        GameManager.currentGamestate = GameStates.InGame;

    }

    void GeneratePlayerCoinStacks()
    {
        for (int i = 0; i < GameGrid.NbOfCells/2; i++)
        {
            GameObject NewCoinP1 = Instantiate(CoinPrefab);
            NewCoinP1.transform.position = new Vector3(GameGrid.startPosition.x - 1, GameGrid.startPosition.y, 0);
            NewCoinP1.tag = "Coin";
            NewCoinP1.GetComponent<MeshRenderer>().material = Material_P1;
            NewCoinP1.transform.SetParent(Stack1.transform);
            NewCoinP1.GetComponent<Coin>().owner = PlayerName.Player1;
            StackPlayer1.Add(NewCoinP1);

            GameObject newCoinP2 = Instantiate(CoinPrefab);
            newCoinP2.transform.position = new Vector3(-GameGrid.startPosition.x + 1, GameGrid.startPosition.y, 0);
            newCoinP2.tag = "Coin";
            newCoinP2.GetComponent<MeshRenderer>().material = Material_P2;
            newCoinP2.transform.SetParent(Stack2.transform);
            newCoinP2.GetComponent<Coin>().owner = PlayerName.Player2;
            StackPlayer2.Add(newCoinP2);

        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0) & currentCoin == null)
        //{
        //    CollectCoin();
        //}
        //MoveCoin();
        //if (Input.GetMouseButtonUp(0) & currentCoin != null)
        //{
        //    ReleaseCoin();
        //}
        //if (coinToMove != null)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        ConfirmCoinPosition();
        //        //Debug.Log(GameGrid.EmptySlots.Count);
        //        //Debug.Log(GameGrid.FullSlots.Count);
        //    }
        //}
        if (GameManager.currentGamestate == GameStates.InGame)
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
            if (RemainingTime.Finished)
            {
                RandomMove();
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
            changePlayer();
        }
    }

    void CollectCoin()
    {
        //coinToMove = null;
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Coin")
            {
                currentCoin = hit.collider.gameObject;
            }
        }
    }

    void MoveCoin()
    {
        if (currentCoin != null)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentCoin.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }

    void ReleaseCoin()
    {
        GridSlot gS = getCurrentEntryslot();
        if (gS != null)
        {
            coinToMove = currentCoin;
            currentCollum = (int)gS.LocalPosition.x;
            //gS.SetCoinToPosition(currentCoin.gameObject);
            if (GameGrid.AddCoinAtPosition(currentCollum, coinToMove.GetComponent<Coin>()))
            {
                coinToMove.transform.SetParent(FieldContainer.transform);
                CheckIfWin();
                changePlayer();
            }
        }
       
        currentCoin = null;
    }

    GridSlot getCurrentEntryslot()
    {
        for (int i = 0; i < GameGrid.AllEntrySlots.Count; i++)
        {
            if (GameGrid.AllEntrySlots[i].IsMouseOver())
            {
                return (GameGrid.AllEntrySlots[i]);
            }
        }
        return null;
    }

    int numberofTry = 0;
    void RandomMove()
    {
        if (GameGrid.AddCoinAtPosition(Random.Range(0, GameGrid.entryslots.Length), currentCoin.GetComponent<Coin>()))
        {
            currentCoin.transform.SetParent(FieldContainer.transform);
            CheckIfWin();
            changePlayer();
        }
        else if (numberofTry<10)
        {
            numberofTry++;
            RandomMove();
        }
    }
        
    public void changePlayer()
    {
        if (enemyPlayer == PlayerType.Human)
            RemainingTime.Duration = GameManager.currentPlayer_ThinkTime;
        else if (enemyPlayer == PlayerType.Computer)
            RemainingTime.Duration = GameManager.currentAI_ThinkTime;
        print(GameManager.currentPlayer_ThinkTime);
        print(GameManager.currentAI_ThinkTime);


        RemainingTime.Run();
        if (currentPlayer.Equals(PlayerName.Player1))
        {
            StackPlayer1.Remove(currentCoin);
            currentPlayer = PlayerName.Player2;
            Stack1.SetActive(false);
            Stack2.SetActive(true);
        }
        else if (currentPlayer.Equals(PlayerName.Player2))
        {
            StackPlayer2.Remove(currentCoin);
            currentPlayer = PlayerName.Player1;
            Stack2.SetActive(false);
            Stack1.SetActive(true);

        }
        FindObjectOfType<HUD>().ShowPlayerInfo();

        if (currentPlayer == PlayerName.Player1)
        {
            currentCoin = StackPlayer1[0];
            currentCoin.transform.position = GameGrid.entryslots[0].WorldPosition_Center;
            currentCollum = 0;
        }
        else if (currentPlayer == PlayerName.Player2)
        {
            currentCoin = StackPlayer2[0];
            currentCoin.transform.position = GameGrid.entryslots[GameGrid.entryslots.Length - 1].WorldPosition_Center;
            currentCollum = GameGrid.entryslots.Length - 1;
        }



    }

    // TODO: more efficient if ony the new Coin is checked
    public void CheckIfWin()
    {
        for (int x = 0; x < GameGrid.Width ; x++)
        {
            for (int y = 0; y < GameGrid.Height; y++)
            {
                bool won = (GameGrid.collectNeigbours(GameGrid.getGridElement(x, y),currentPlayer));
                if (won)
                {
                    Debug.Log("Win" + currentPlayer);
                    //GameElementsContainer.SetActive(false);
                    FindObjectOfType<HUD>().ShowPlayerWin();
                    GameManager.currentGamestate = GameStates.GameEnd;
                    break;

                }
            }
        }
    }



}
