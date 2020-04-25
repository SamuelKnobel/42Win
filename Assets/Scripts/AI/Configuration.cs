using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;

/// <summary>
/// A configuration of the game
/// </summary>
public class Configuration
{
    #region Fields

    public bool WinningConfiguration;

    public int PlayerIndex; // Player that made the move that lead to the current Configuration.
    public int[,] SimplifiedGrid;
    public Vector2 lastMove = new Vector2(-1, -1);

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="binContents">contents of each bin</param>
    public Configuration(GameGrid grid, int playerIndex)
    {
        if (GameManager.FirstMove)
        {
            playerIndex = -1;
            GameManager.FirstMove = false;
        }
        SimplifiedGrid = null;
        SimplifiedGrid = new int[grid.Width, grid.Height];
        for (int i = 0; i < grid.Width; i++)
        {
            for (int j = 0; j < grid.Height; j++)
            {
                GridSlot slot = grid.gridslots[i, j];
                if (!slot.isFilled)
                {
                    SimplifiedGrid[i, j] = 0;
                }
                else
                {
                    SimplifiedGrid[i, j] = slot.Owner.PlayerIndex + 1;
                }
            }
        }
        if (playerIndex == 0)
        {
            playerIndex = 1;
        }
        else if (playerIndex == 1)
        {
            playerIndex = 0;
        }
        PlayerIndex = playerIndex;
    }
    /// <summary>
    /// For Creating the next hypothetic Configuration. 
    /// </summary>
    /// <param name="simplifiedGrid">current Grid</param>
    /// <param name="newMove">Next Move to Make</param>
    /// <param name="NextPlayerIndex">Player that is making that next move</param>
    public Configuration(int[,] simplifiedGrid, Vector2 newMove,  int NextPlayerIndex)
    {
        SimplifiedGrid = null;

        SimplifiedGrid = new int[simplifiedGrid.GetLength(0), simplifiedGrid.GetLength(1)];

        for (int x = 0; x < simplifiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < simplifiedGrid.GetLength(1); y++)
            {
                SimplifiedGrid[x, y] = simplifiedGrid[x, y];
            }
        }
        if (!CheckOutSide(newMove))
        {
            SimplifiedGrid[(int)newMove.x, (int)newMove.y] = NextPlayerIndex + 1;
        }
        else
            Debug.LogError("Invalid Move: " + newMove.ToString());
        PlayerIndex = NextPlayerIndex;
        lastMove = newMove;
        if (CheckNeigbours(newMove,4))
        {
            WinningConfiguration = true;
        }       
    }

    /// <summary>
    /// For Creating an Instance of the current Configuration. 
    /// </summary>
    /// <param name="simplifiedGrid">current Grid</param>
    public Configuration(Configuration config)
    {
        SimplifiedGrid = null;
        SimplifiedGrid = new int[config.SimplifiedGrid.GetLength(0), config.SimplifiedGrid.GetLength(1)];

        for (int x = 0; x < config.SimplifiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < config.SimplifiedGrid.GetLength(1); y++)
            {
                SimplifiedGrid[x, y] = config.SimplifiedGrid[x, y];
            }
        }
        PlayerIndex = config.PlayerIndex;
        WinningConfiguration = config.WinningConfiguration;
        lastMove = config.lastMove;
    }

    #endregion

    #region Properties

    public bool checkNewCoin(int x, int y)
    {
        bool win = CheckNeigbours(new Vector2(x, y),4);
        return win;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Converts the configuration to a string
    /// </summary>
    /// <returns>the string</returns>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("PlayerIndex: " + PlayerIndex + ", Winning:" + WinningConfiguration + "\n");
        //builder.Append("Configuration: "+ "\n");

        for (int y = SimplifiedGrid.GetLength(1)-1; y >=0 ; y--)
        {
            for (int x = 0; x < SimplifiedGrid.GetLength(0); x++)
            {
                builder.Append(" " + SimplifiedGrid[x, y] + " ");
            }

            builder.Append("\n");
        }
        return builder.ToString();
    }


    public bool CheckNeigbours(Vector2 gridSlot, int SlotsSize)
    {
        bool result = true;

        result = CheckWinning(getEast(gridSlot, SlotsSize));
        if (result)
            return result;

        result = CheckWinning(getNorth(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckWinning(getNorthEast(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckWinning(getNorthWest(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckWinning(getWest(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckWinning(getSouthWest(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckWinning(getSouth(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckWinning(getSouthEast(gridSlot, SlotsSize));
        if (result)
            return result;
        return result;
    }

    bool CheckOutSide(Vector2 pos)
    {
        bool result= false;
        if (pos.x > SimplifiedGrid.GetLength(0) - 1)
            result= true;
        if (pos.y > SimplifiedGrid.GetLength(1) - 1)
            result = true;   
        if (pos.y <0)
            result = true;   
        if (pos.x <0)
            result = true;
        return result;
    }

    bool CheckWinning(List<int> slots)
    {
        foreach (var item in slots)
        {
            if (item == 0)
            {
                return false;
            }
        }

        //if (slots.Count == 4)
        //{
        int sum = getSum(slots.ToArray());
        if ((sum == slots.Count) || (sum == 2* slots.Count))
          return true;
        else
            return false;
        //}
        //else
        //    result = false;

    }
    public bool CheckNeighbourCloseToWin(Vector2 gridSlot, int SlotsSize)
    {
        bool result = true;

        result = CheckCloseToWin(getEast(gridSlot, SlotsSize));
        if (result)
            return result;

        result = CheckCloseToWin(getNorth(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckCloseToWin(getNorthEast(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckCloseToWin(getNorthWest(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckCloseToWin(getWest(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckCloseToWin(getSouthWest(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckCloseToWin(getSouth(gridSlot, SlotsSize));
        if (result)
            return result;
        result = CheckCloseToWin(getSouthEast(gridSlot, SlotsSize));
        if (result)
            return result;
        return result;
    }
    bool CheckCloseToWin(List<int> slots)
    {
        if (slots.Count == 4)
        {
            int sum = getSum(slots.ToArray());
            if ((sum == slots.Count-1) || (sum == 2 * slots.Count-2))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public List<int> getEast(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(SlotsSize-1, 0);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x + i, (int)start.y]);
            }
        }
        return temp;
    }
    public List<int> getNorthEast(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(SlotsSize-1, SlotsSize-1);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x + i, (int)start.y+i]);
            }
        }
        return temp;
    }
    public List<int> getNorth(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(0, SlotsSize-1);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x, (int)start.y + i]);
            }
        }
        return temp;
    }
    public List<int> getNorthWest(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(-SlotsSize+1, SlotsSize-1);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x-i, (int)start.y + i]);
            }
        }
        return temp;
    }
    public List<int> getWest(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(-SlotsSize+1, 0);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x - i, (int)start.y]);
            }
        }
        return temp;
    }
    public List<int> getSouthWest(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(-SlotsSize+1, -SlotsSize+1);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x - i, (int)start.y - i]);
            }
        }
        return temp;
    }
    public List<int> getSouth(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(0, -SlotsSize+1);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x, (int)start.y - i]);
            }
        }
        return temp;
    }
    public List<int> getSouthEast(Vector2 start, int SlotsSize)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(SlotsSize-1, -SlotsSize+1);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= SlotsSize-1; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x + i, (int)start.y - i]);
            }
        }
        return temp;
    }


    #endregion

    int getSum(int[] array)
    {
        int sum = 0;
        if (array.Length > 0)
        {
            foreach (int item in array)
            {
                sum += item;
            }
        }
        else
            sum = -1;
        return sum;
    }




}
