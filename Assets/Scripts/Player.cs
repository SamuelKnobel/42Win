using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class Player 
{
    public enum PlayerType
    {
        Human, Computer
    }
    public string playerName = "NotDefined";

    public PlayerType playerType;
    public bool isPlaying;
    public int PlayerIndex;

    public Sprite playerSprite;

    public List<GameObject> CoinStack = new List<GameObject>();

    public MinimaxTree<Configuration> tree;

    List<Configuration> newConfigurations =
    new List<Configuration>();

    LinkedList<MinimaxTreeNode<Configuration>> nodeList =
        new LinkedList<MinimaxTreeNode<Configuration>>();



    public Player(string name, PlayerType type, int order)
    {
        playerName = name;
        playerType = type;
        PlayerIndex = order;
    }
    public void TreeBuilder(int currentPlayerInd)
    {
        //tree = StartBuildTree(new Configuration(GameManager.GameGrid), playingOrder);

        //Debug.Log("start.");
        ////EventManager.CallThreadEvent(2);
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        
        Configuration StartConfig = new Configuration(GameManager.GameGrid, currentPlayerInd);
        StartBuildTree(StartConfig);
        //sw.Stop();
        //Debug.Log("Three Build : Done! Elapsed time: " + sw.ElapsedMilliseconds / 1000f);
    }
    public int GetNextMove()
    {
        Debug.Log("Start MINMAX");
        //StartBuildTree(new Configuration(GameManager.GameGrid), playingOrder);
        Minimax(tree.Root, true);
        Debug.Log(tree.ToString());
        //find child node with maximum score
        IList<MinimaxTreeNode<Configuration>> children =
            tree.Root.Children;

        MinimaxTreeNode<Configuration> maxChildNode = children[0];
        for (int i = 1; i < children.Count; i++)
        {
            if (children[i].MinimaxScore > maxChildNode.MinimaxScore)
            {
                maxChildNode = children[i];
            }
        }

        Debug.Log(maxChildNode.Value.ToString());
        Debug.Log(maxChildNode.Value.lastMove);
        Debug.Log(maxChildNode.Value.WinningPlayer);
        if (maxChildNode.MinimaxScore == 0)
        {
            Debug.Log("R");
            return -1;// UnityEngine.Random.Range(0, GameManager.GameGrid.entryslots.Length);
        }
        else
            return (int)maxChildNode.Value.lastMove.x;
    }


    void StartBuildTree(Configuration currConfiguration)
    {
        tree = new MinimaxTree<Configuration>(currConfiguration);
        nodeList.Clear();
        nodeList.AddLast(tree.Root);
        MinimaxTreeNode<Configuration> currentNode = nodeList.First.Value;

        while (nodeList.Count > 0)
        {
            currentNode = nodeList.First.Value;
            nodeList.RemoveFirst();
            List<Configuration> children =
                GetNextConfigurations(currentNode.Value);

            //foreach (Configuration child in children)
            //{
            for (int i = 0; i < children.Count; i++)
            {

                MinimaxTreeNode<Configuration> childNode =
                    new MinimaxTreeNode<Configuration>(
                        children[i], currentNode);
                ////if (childNode.determineDepth(childNode) < GameManager.currentAI_ThinkDepth)
                //if (childNode.determineDepth(childNode) < 5)
                //{
                    //tree.AddNode(childNode);
                    int threadNbr = i;

                    //BuildTreePart(childNode.Value, threadNbr);
                ThreadQueuer.StartThreadedFunction(() => { BuildTreePart(childNode.Value, threadNbr); });
                //if (!childNode.Value.WinningConfiguration)
                //    nodeList.AddLast(childNode);
                //}
            }


        }
        //}
        //Debug.Log(tree.ToString());
        //return tree;
    }
    public List<Configuration> GetNextConfigurations(
       Configuration currentConfiguration)
    {
        List<Configuration> newConfigurations =    new List<Configuration>();
        for (int x = 0; x < currentConfiguration.SimplifiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < currentConfiguration.SimplifiedGrid.GetLength(1); y++)
            {
                if (currentConfiguration.SimplifiedGrid[x, y] == 0)
                {
                    int ChildIndex = 0;
                    if (currentConfiguration.PlayerIndex == 0)
                        ChildIndex = 1;
                    else if (currentConfiguration.PlayerIndex == 1)
                        ChildIndex = 0;
                    Configuration NewConf = new Configuration(currentConfiguration.SimplifiedGrid, new Vector2(x, y), ChildIndex);
                    newConfigurations.Add(NewConf);
                    //Debug.Log(NewConf.ToString());
                    break;
                }
            }
        }
        return newConfigurations;
    }
    void BuildTreePart(Configuration currConfig, int threadNumber )
    {
        //Debug.Log("start.");
        //EventManager.CallThreadEvent(2);
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Configuration childconfig = new Configuration(currConfig);
        LinkedList<MinimaxTreeNode<Configuration>> PartNodeList = new LinkedList<MinimaxTreeNode<Configuration>>();
        MinimaxTree<Configuration> Childtree = new MinimaxTree<Configuration>(childconfig);
        //Debug.Log(Childtree.ToString());
        PartNodeList.AddLast(Childtree.Root);
        MinimaxTreeNode<Configuration> currentChildNode =
        PartNodeList.First.Value;

        while (PartNodeList.Count > 0)
        {
            //Debug.Log(PartNodeList.Count);
            currentChildNode = PartNodeList.First.Value;
            PartNodeList.RemoveFirst();
            
            if (currentChildNode.Value.WinningConfiguration)
            {
                //Debug.Log(currentChildNode.Value.ToString());
                continue;
            }
            List<Configuration> NextChildren =
                GetNextConfigurations(currentChildNode.Value);

            foreach (Configuration child in NextChildren)
            {
                MinimaxTreeNode<Configuration> childNode =
                    new MinimaxTreeNode<Configuration>(
                        child, currentChildNode);

                if (childNode.determineDepth(childNode) < GameManager.currentAI_ThinkDepth)
                    if (childNode.determineDepth(childNode) < 5)
                {
                    Childtree.AddNode(childNode);
                    if (!childNode.Value.WinningConfiguration)
                        PartNodeList.AddLast(childNode);
                }
            }
        }

        tree.AddBranch(Childtree, tree);
        //Action aFunction = () =>
        //{

        //    Debug.Log("The results of the child thread are being applied to a Unity GameObject safely.");

        //};
        //ThreadQueuer.QueueMainThreadFunction(aFunction);
        sw.Stop();
        Debug.Log("Three Build : Done! Elapsed time: " + sw.ElapsedMilliseconds / 1000f);
        EventManager.CallThreadEndEvent((float)threadNumber);

    }
   

    /// <summary>
    /// Assigns minimax scores to the tree nodes
    /// </summary>
    /// <param name="SubTree">tree to mark with scores</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void Minimax(MinimaxTreeNode<Configuration> SubTree,
        bool maximizing)
    {
        // recurse on children
        IList<MinimaxTreeNode<Configuration>> children = SubTree.Children;
        if (children.Count > 0)
        {
            foreach (MinimaxTreeNode<Configuration> child in children)
            {
                // toggle maximizing as we move down
                Minimax(child, !maximizing);
            }

            // set default node minimax score
            if (maximizing)
            {
                SubTree.MinimaxScore = int.MinValue;
            }
            else
            {
                SubTree.MinimaxScore = int.MaxValue;
            }

            // find maximum or minimum value in children
            foreach (MinimaxTreeNode<Configuration> child in children)
            {
                if (maximizing)
                {
                    // check for higher minimax score
                    if (child.MinimaxScore > SubTree.MinimaxScore)
                    {
                        SubTree.MinimaxScore = child.MinimaxScore;
                    }
                }
                else
                {
                    // minimizing, check for lower minimax score
                    if (child.MinimaxScore < SubTree.MinimaxScore)
                    {
                        SubTree.MinimaxScore = child.MinimaxScore;
                    }
                }
            }
        }
        else
        {
            // leaf nodes are the base case
            AssignHeuristicMinimaxScore(SubTree, maximizing);
        }
    }

    /// <summary>
    /// Assigns the end of game minimax score
    /// </summary>
    /// <param name="node">node to mark with score</param> 
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignEndOfGameMinimaxScore(MinimaxTreeNode<Configuration> node,
        bool maximizing)
    {
        if (maximizing)
        {
            // Player2 Wins
            node.MinimaxScore = 0 - Mathf.Pow(10, -node.determineDepth(node)+1);
        }
        else
        {
            // Player1 Wins
            node.MinimaxScore = 0 + Mathf.Pow(10,-node.determineDepth(node)+1);
        }
    }

    /// <summary>
    /// Assigns a heuristic minimax score to the given node 
    /// omes into action if the search tree is finished, but its not the end , resptecively its no win or loose 
    /// </summary>
    /// <param name="node">node to mark with score</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignHeuristicMinimaxScore(
        MinimaxTreeNode<Configuration> node,
        bool maximizing)
    {
        // might have reached an end-of-game configuration
        if (node.Value.WinningConfiguration)
        {
            AssignEndOfGameMinimaxScore(node, maximizing);
        }
        else
        {
            // use a heuristic evaluation function to score the node

            // Rule 1: if there is only 1 Bin filled with 1 Bears: we dont wannt take this one
            //if (node.Value.NonEmptyBins.Count==1 && node.Value.TotalNumberOfBears == 1)
            //{
            //    if (maximizing)
            //    {
            //        node.MinimaxScore = 0;
            //    }
            //    else
            //    {
            //        node.MinimaxScore = 1;
            //    }
            //}
            ////Rule 2: if there is only one Bin and there are 2 Bears in it-- > player 2 will win
            //else if (node.Value.NonEmptyBins.Count == 1 && node.Value.TotalNumberOfBears == 2)
            //{
            //    if (maximizing)
            //    {
            //        node.MinimaxScore = 1;
            //    }
            //    else
            //    {
            //        node.MinimaxScore = 0;
            //    }
            //}
            // Rule 2: if there are only 2 Bins filled with one Bear each: the player1 will winn
            //else if (node.Value.NonEmptyBins.Count == 2 && node.Value.TotalNumberOfBears == 2)
            //{
            //    if (maximizing)
            //    {
            //        node.MinimaxScore = 1;
            //    }
            //    else
            //    {
            //        node.MinimaxScore = 0;
            //    }
            //}
            //else
            node.MinimaxScore = 0f;
        }
    }

}
