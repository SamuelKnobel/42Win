using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid   
{
    float starposition_x;
    float starposition_y;
    public Vector2 startPosition
    {
        get
        {
            return new Vector2(starposition_x, starposition_y);
        }
    }

    public int Width;
    public int Height;

    public GridSlot[,] gridslots;
    public GridSlot[] entryslots;

    public int NbOfCells
    {
        get
        {
            return (Width + 1) * (Height + 1);
        }
    }

    public List<GridSlot> AllSlots = new List<GridSlot>();
    public List<GridSlot> AllEntrySlots = new List<GridSlot>();


    public List<GridSlot> FullSlots
    {
        get
        {
            List<GridSlot> temp = new List<GridSlot>();
            {
                foreach (GridSlot item in AllSlots)
                {
                    if (item.isFilled)
                        temp.Add(item);
                }
            }
            return temp;
        }
    }
    public List<GridSlot> EmptySlots
    {
        get
        {
            List<GridSlot> temp = new List<GridSlot>();
            {
                foreach (GridSlot item in AllSlots)
                {
                    if (!item.isFilled)
                        temp.Add(item);
                }
            }
            return temp;
        }
    }

    public GameGrid()
    {
        Width = ConfigurationUtils.Width;
        Height = ConfigurationUtils.Height;
        if (Width%2 != 0)
            starposition_x = -(float)Width / 2;
        else
            starposition_x = -(float)Width / 2 -.5f;


        if (Height % 2 != 0)
            starposition_y = -(float)Height / 2 + 0.5f;
        else
            starposition_y = -(float)Height / 2;
        if (gridslots != null)
            gridslots = null;     
        if (entryslots != null)
            entryslots = null;

        gridslots = new GridSlot[Width, Height];
        entryslots = new GridSlot[Width];

        GenerateGridSlots();
        GenerateGridEntryPoints();
        //Debug.Log(EmptySlots.Count);
        //Debug.Log(FullSlots.Count);
    }
    public GameGrid(int width, int height)
    {
        Width = width;
        Height = height;
        if (Width % 2 != 0)
            starposition_x = -(float)Width / 2;
        else
            starposition_x = -(float)Width / 2 - .5f;

        if (Height % 2 != 0)
            starposition_y = -(float)Height / 2 + 0.5f;
        else
            starposition_y = -(float)Height / 2 ;
        gridslots = new GridSlot[Width, Height];
        entryslots = new GridSlot[Width];
        GenerateGridSlots();
        GenerateGridEntryPoints();

    }

    //Configuration configuration;
    //public Configuration Configuration
    //{
    //    get { return configuration; }
    //    set
    //    {
    //        configuration = value;
    //    }
    //}



    void GenerateGridSlots()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                gridslots[x, y] = new GridSlot(x, y, this, Vector2.one, GridSlot.SlotSort.gridSlot);
                AllSlots.Add(gridslots[x, y]);
            }
        }
    }
    void GenerateGridEntryPoints()
    {
        for (int x = 0; x < Width; x++)
        {
            int y = Height;
            entryslots[x] = new GridSlot(x, y, this, new Vector2(1,2),GridSlot.SlotSort.entrySlot);
            AllEntrySlots.Add(entryslots[x]);

        }
    }

    public Vector2 getGridElementCenterPosition(int x, int y)
    {
        if (x > Width - 1 || y > Height - 1 || x < 0 || y < 0)
            return Vector2.zero;
        else
            return gridslots[x, y].WorldPosition_Center;

    }
    public GridSlot getGridElement(int x, int y)
    {
        if (x > Width - 1 || y > Height - 1 || x < 0 || y < 0)
            return null;
        else
            return gridslots[x, y];
    }   
    public GridSlot getGridElement(Vector2 xy)
    {
        if (xy.x > Width - 1 || xy.y > Height - 1 || xy.x < 0 || xy.y < 0)
            return null;
        else
            return gridslots[(int)xy.x, (int)xy.y];
    }

    public bool AddCoinAtPosition(int x, Coin Coin)
    {
        bool positioned = false;
        for (int i = 0; i < Height; i++)
        {
            if (gridslots[x, i].isFilled)
            {
                positioned = false;
                continue;

            }
            else
            {
                Coin.transform.position = gridslots[x, i].WorldPosition_Center;
                gridslots[x, i].FillingCoin = Coin;
                Coin.tag = "Untagged";
                positioned = true;
                Coin.placed = true;
                Coin.GridSlotPosition = new Vector2(x, i);
                break;
            }
        }
        if (positioned== false)
        {
            //GameObject.Destroy(Coin);
            Debug.LogWarning("Collum Full, can not be added");
        }

        return positioned;
    }

    public void RemoveCoin(Coin coin)
    {
        gridslots[(int)coin.GridSlotPosition.x, (int)coin.GridSlotPosition.y].FillingCoin = null;
    }


    public bool collectNeigbours(GridSlot gridSlot, Player player)
    {
        bool result = true;

        result = CheckOwner(gridSlot.getEast(), player);
        if (result)
            return result;

        result = CheckOwner(gridSlot.getNorth(), player);
        if (result)
            return result;

        result = CheckOwner(gridSlot.getNorthEast(), player);
        if (result)
            return result;

        result = CheckOwner(gridSlot.getNorthWest(), player);
        if (result)
            return result;

        return result;
    }

 
    bool CheckOwner(List<GridSlot> slots, Player player)
    {
        bool result = true;

        if (slots.Count == 4)
        {
            foreach (var item in slots)
            {
                if (item.Owner != player)
                {
                    result = false;
                    break;
                }

            }
        }
        else
            result = false;
        return result;

    }


}
