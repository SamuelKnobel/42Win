using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntrySlot 
{
    Vector2 SlotSize;
    GameGrid Grid;
    public Vector2 LocalPosition = Vector2.zero;

    public Vector2 WorldPosition_Corner
    {
        get
        {
            return LocalPosition + Grid.startPosition;
        }
    }
    public Vector2 WorldPosition_Center
    {
        get
        {
            return WorldPosition_Corner + new Vector2(SlotSize.x / 2, SlotSize.y / 2);
        }
    }


    public bool isFilled;
    public GameObject FillingCoin_GO;


    public GridEntrySlot(int localX, int localY, GameGrid grid, Vector2 size)
    {
        Grid = grid;
        SlotSize = size;
        LocalPosition = new Vector2(localX, localY);
    }

}
