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

    List<int> bins = new List<int>();
    public int[,] SimplifiedGrid;
    public Vector2 lastMove = new Vector2(-1, -1);

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="binContents">contents of each bin</param>
    public Configuration(GameGrid grid)
    {
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
                    SimplifiedGrid[i, j] = slot.Owner.playingOrder + 1;
                }
            }
        }
    }
    public Configuration(int[,] simplifiedGrid)
    {
        SimplifiedGrid = new int[simplifiedGrid.GetLength(0), simplifiedGrid.GetLength(1)];

        for (int x = 0; x < simplifiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < simplifiedGrid.GetLength(1); y++)
            {
                SimplifiedGrid[x, y] = simplifiedGrid[x, y];
            }
        }
    }


    #endregion

    #region Properties





    public bool FourConnected
    {
        get
        {
            if (SimplifiedGrid == null)
                return false;
            bool win= false;
            for (int x = 0; x < SimplifiedGrid.GetLength(0); x++)
            {
                for (int y = 0; y < SimplifiedGrid.GetLength(1); y++)
                {
                   
                    win = CheckNeigbours(new Vector2(x, y));
                    if (win)
                    {
                        //Debug.Log("WinningPlayer:" + WinningPlayer);
                        //Debug.Log("WinningConfig:" + this.ToString()); ;


                        return win;

                    }
                }
            }
            //Debug.Log("Config:" + this.ToString()); ;
            //Debug.Log("Winning:" + win); ;

            return win;
        }
    }
    public bool WinningConfiguration;

    public int WinningPlayer;


    /// <summary>
    /// Gets a read-only list of the bin contents
    /// </summary>
    public IList<int> Bins
    {
        get { return bins.AsReadOnly(); }
    }

    /// <summary>
    /// Gets whether all the bins in the configuration are empty
    /// </summary>
    public bool Empty
    {
        get
        {
            foreach (int bin in bins)
            {
                if (bin > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public IList<int> NonEmptyBins
    {
        get {
            List<int> binCounts = new List<int>();
            foreach (int bin in Bins)
            {
                if (bin> 0)
                {
                    binCounts.Add(bin);
                }
            }
            return binCounts.AsReadOnly(); 
        }
    }
    public int TotalNumberOfBears
    {
        get
        {
            int total = 0;
            foreach (int bin in Bins)
            {
                total = total + bin;
            }
            return total;
        }
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
        builder.Append("Configuration: "+ "\n");

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


    public bool CheckNeigbours(Vector2 gridSlot)
    {
        //string str1 = "";
        //foreach (var item in getEast(gridSlot))
        //{
        //    str1 = str1 + " , " + item;
        //}
        //string str2 = "";
        //foreach (var item in getNorthEast(gridSlot))
        //{
        //    str2 = str2 + " , " + item;
        //}
        //string str3 = "";
        //foreach (var item in getNorth(gridSlot))
        //{
        //    str3 = str3 + " , " + item;
        //}
        //string str4 = "";
        //foreach (var item in getNorthWest(gridSlot))
        //{
        //    str4 = str4 + " , " + item;
        //}

        //Debug.Log("East:" + str1 + "\n"+ "NorthEast:" + str2 + "\n" + "North:" + str3 + "\n" + "NorthWest:" + str4 + "\n");

        bool result = true;

        result = CheckOwner(getEast(gridSlot));
        if (result)
            return result;

        result = CheckOwner(getNorth(gridSlot));
        if (result)
            return result;

        result = CheckOwner(getNorthEast(gridSlot));
        if (result)
            return result;

        result = CheckOwner(getNorthWest(gridSlot));
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

    bool CheckOwner(List<int> slots)
    {
        WinningPlayer = -1;
        bool result = true;
       
        if (slots.Count == 4)
        {
            int ind = 0;
           
            while (slots[ind]==0)
            {
                ind++;
                if (ind > slots.Count-1)
                    return false;
            }
            WinningPlayer = slots[ind];
               
            foreach (var item in slots)
            {
                if (item != WinningPlayer)
                {
                    result = false;
                    break;
                }
            }

        }
        else
            result = false;
        WinningPlayer = WinningPlayer - 1;
        return result;

    }
    public List<int> getEast(Vector2 start)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(3, 0);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x + i, (int)start.y]);
            }
        }
        return temp;
    }
    public List<int> getNorthEast(Vector2 start)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(3, 3);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x + i, (int)start.y+i]);
            }
        }
        return temp;
    }
    public List<int> getNorth(Vector2 start)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(0, 3);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x, (int)start.y + i]);
            }
        }
        return temp;
    }
    public List<int> getNorthWest(Vector2 start)
    {
        List<int> temp = new List<int>();
        Vector2 ende = start + new Vector2(-3, 3);
        if (!CheckOutSide(ende))
        {
            for (int i = 0; i <= 3; i++)
            {
                temp.Add(SimplifiedGrid[(int)start.x-i, (int)start.y + i]);
            }
        }
        return temp;
    }





    #endregion
}
