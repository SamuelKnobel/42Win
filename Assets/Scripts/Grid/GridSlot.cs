using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSlot 
{

    public enum SlotSort
    {
        gridSlot, entrySlot
    }
    Vector2 SlotSize;
    GameGrid grid;
    public Vector2 LocalPosition = Vector2.zero;
    public Vector2 WorldPosition_Corner
    {
        get
        {
           return LocalPosition + grid.startPosition;
        }
    }
    public Vector2 WorldPosition_Center
    {
        get
        {
            return WorldPosition_Corner + new Vector2(SlotSize.x / 2, SlotSize.y / 2);
        }
    }

    public Coin FillingCoin = null;

    public bool isFilled
    {
        get
        {
            if (FillingCoin == null)
                return false;
            else
                return true;
        }
    }
   
    public Player Owner
    {
        get
        {
            if (!isFilled)
            {
                return null;
            }
            else
                return FillingCoin.Owner;

        }
    }

    public SlotSort slotSort;

    public List<GridSlot> NeighboursEast = new List<GridSlot>();
    public List<GridSlot> NeighboursNorthEast = new List<GridSlot>();
    public List<GridSlot> NeighboursNorth = new List<GridSlot>();
    public List<GridSlot> NeighboursNorthWest = new List<GridSlot>();

    public GridSlot(int localX, int localY, GameGrid grid, Vector2 size, SlotSort sort)
    {
        LocalPosition = new Vector2(localX, localY);
        this.grid = grid;
        SlotSize = size;
        slotSort = sort;
    }   

    public void fillwithCoin(Coin coin)
    {
        FillingCoin = coin;
    }

   public bool IsMouseOver()
   {
        bool mouseOver = false;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //currentCoin.transform.position = new Vector3(pos.x, pos.y, 0);
        if (WorldPosition_Corner.x < pos.x && 
            pos.x < WorldPosition_Corner.x+ SlotSize.x&&
            WorldPosition_Corner.y < pos.y &&
            pos.y < WorldPosition_Corner.y + SlotSize.y)
        {
            mouseOver = true;
            //Debug.Log(WorldPosition_Center);
        }
        else
            mouseOver = false;


        return mouseOver;
   }
    public void SetCoinToPosition(GameObject coin)
    {
        coin.transform.position = WorldPosition_Center;
    }


    public List<GridSlot> getEast()
    {
        List<GridSlot> temp = new List<GridSlot>();
        Vector2 ende = LocalPosition + new Vector2(3, 0);

        if (grid.getGridElement(ende) != null)
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(grid.getGridElement(LocalPosition+ new Vector2(i,0)));
            }
            return temp;
        }
        else
            return temp;
    }
    public List<GridSlot> getNorthEast()
    {
        List<GridSlot> temp = new List<GridSlot>();
        Vector2 ende = LocalPosition + new Vector2(3, 3);

        if (grid.getGridElement(ende) != null)
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(grid.getGridElement(LocalPosition + i* Vector2.one));
            }
            return temp;
        }
        else
            return temp;
    }

    public List<GridSlot> getNorth()
    {
        List<GridSlot> temp = new List<GridSlot>();
        Vector2 ende = LocalPosition + new Vector2(0, 3);

        if (grid.getGridElement(ende) != null)
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(grid.getGridElement(LocalPosition + new Vector2(0, i)));

            }
            return temp;
        }
        else
            return temp;
    }
    public List<GridSlot> getNorthWest()
    {
        List<GridSlot> temp = new List<GridSlot>();
        Vector2 ende = LocalPosition + new Vector2(-3, 3);

        if (grid.getGridElement(ende) != null)
        {
            for (int i = 0; i <= 3; i++)
            {
                GridSlot gridSlot = grid.getGridElement(LocalPosition + new Vector2(-i, i));
                if (gridSlot.isFilled)
                {
                    temp.Add(gridSlot);
                }
            }
            return temp;
        }
        else
            return temp;
    }





}
