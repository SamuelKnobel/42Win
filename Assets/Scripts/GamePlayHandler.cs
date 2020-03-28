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

    public static PlayerName currentPlayer = PlayerName.Player2;
    public static PlayerType enemyPlayer = PlayerType.Human;



    // Start is called before the first frame update
    void Start()
    {
        StartGame();

        //Debug.Log(GameGrid.EmptySlots.Count);
        //Debug.Log(GameGrid.FullSlots.Count);
    }
   public void StartGame()
    {
        changePlayer();

        FindObjectOfType<HUD>().ShowPlayerInfo();

        GameGrid = new GameGrid();

        GeneratePlayerCoinStacks();
        GameManager.currentGamestate = GameStates.InGame;
    }

    void GeneratePlayerCoinStacks()
    {
        for (int i = 0; i < GameGrid.NbOfCells/2; i++)
        {
            GameObject StackPlayer1 = Instantiate(CoinPrefab);
            StackPlayer1.transform.position = new Vector3(GameGrid.startPosition.x - 1, GameGrid.startPosition.y, 0);
            StackPlayer1.tag = "Coin";
            StackPlayer1.GetComponent<MeshRenderer>().material = Material_P1;
            StackPlayer1.transform.SetParent(Stack1.transform);
            StackPlayer1.GetComponent<Coin>().owner = PlayerName.Player1;

            GameObject StackPlayer2 = Instantiate(CoinPrefab);
            StackPlayer2.transform.position = new Vector3(-GameGrid.startPosition.x + 1, GameGrid.startPosition.y, 0);
            StackPlayer2.tag = "Coin";
            StackPlayer2.GetComponent<MeshRenderer>().material = Material_P2;
            StackPlayer2.transform.SetParent(Stack2.transform);
            StackPlayer2.GetComponent<Coin>().owner = PlayerName.Player2;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)& currentCoin == null)
        {
            CollectCoin();
        }
        MoveCoin();
        if (Input.GetMouseButtonUp(0)& currentCoin != null)
        {
            ReleaseCoin();
        }
        //if (coinToMove != null)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        ConfirmCoinPosition();
        //        //Debug.Log(GameGrid.EmptySlots.Count);
        //        //Debug.Log(GameGrid.FullSlots.Count);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.C))
        {
            print(GameGrid.collectNeigbours(GameGrid.getGridElement(0, 0), PlayerName.Player1));
            print(GameGrid.collectNeigbours(GameGrid.getGridElement(3, 0), PlayerName.Player1));
            //GameGrid.collectNeigbours(GameGrid.getGridElement(2, 2), PlayerName.Player1);
            //GameGrid.collectNeigbours(GameGrid.getGridElement(0, 2));
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
        
    public void changePlayer()
    {
        if (currentPlayer.Equals(PlayerName.Player1))
        {
            currentPlayer = PlayerName.Player2;
            Stack1.SetActive(false);
            Stack2.SetActive(true);
        }
        else if (currentPlayer.Equals(PlayerName.Player2))
        {
            currentPlayer = PlayerName.Player1;
            Stack2.SetActive(false);
            Stack1.SetActive(true);

        }
        FindObjectOfType<HUD>().ShowPlayerInfo();
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
                    GameElementsContainer.SetActive(false);
                    FindObjectOfType<HUD>().ShowPlayerWin();
                    break;

                }
            }
        }
    }



}
