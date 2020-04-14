﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public int playingOrder;

    public Sprite playerSprite;

    public List<GameObject> CoinStack = new List<GameObject>();

    MinimaxTree<Configuration> tree;

    List<Configuration> newConfigurations =
    new List<Configuration>();

    LinkedList<MinimaxTreeNode<Configuration>> nodeList =
        new LinkedList<MinimaxTreeNode<Configuration>>();



    public Player(string name, PlayerType type, int order)
    {
        playerName = name;
        playerType = type;
        playingOrder = order;
    }

    public int TakeTurn()
    {
        tree = BuildTree(new Configuration(GameManager.GameGrid), playingOrder);
        Minimax(tree.Root, false);
        //Debug.Log(tree.ToString());
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
        //Debug.Log(maxChildNode.Value.ToString());
        //Debug.Log(maxChildNode.Value.lastMove);
        if (maxChildNode.MinimaxScore == 0.5)
        {
            Debug.Log("R");
            return UnityEngine.Random.Range(0, GameManager.GameGrid.entryslots.Length);
        }
        else
            return (int)maxChildNode.Value.lastMove.x;
    }


    public List<Configuration> GetNextConfigurations(
        Configuration currentConfiguration, int playerOrder)
    {
        newConfigurations.Clear();
        for (int x = 0; x < currentConfiguration.SimplifiedGrid.GetLength(0); x++)
        {
            for (int y = 0; y < currentConfiguration.SimplifiedGrid.GetLength(1); y++)
            {
                if (currentConfiguration.SimplifiedGrid[x, y] == 0)             
                {
                    Configuration NewConf = new Configuration(currentConfiguration.SimplifiedGrid);
                    NewConf.SimplifiedGrid[x, y] = playerOrder + 1;
                    newConfigurations.Add(NewConf);
                    NewConf.lastMove = new Vector2(x, y);
                    break;
                }
            }
        }
        return newConfigurations;
    }

    MinimaxTree<Configuration> BuildTree(Configuration currConfiguration, int PlayerStart)
    {
        MinimaxTree<Configuration> tree =
    new MinimaxTree<Configuration>(currConfiguration,PlayerStart);
        nodeList.Clear();
        nodeList.AddLast(tree.Root);
        while (nodeList.Count > 0)
        {
            MinimaxTreeNode<Configuration> currentNode =
                nodeList.First.Value;
            nodeList.RemoveFirst(); 
            if (currentNode.Value.FourConnected)
            {
                currentNode.Value.WinningConfiguration = true;
                Debug.Log(currentNode.Value.ToString());
                continue;
            }

            List<Configuration> children =
                GetNextConfigurations(currentNode.Value,currentNode.playerOrder);
            int ChildOrder;
            foreach (Configuration child in children)
            {
                if (currentNode.playerOrder == 0)
                    ChildOrder = 1;
                else
                    ChildOrder = 0;

                MinimaxTreeNode<Configuration> childNode =
                    new MinimaxTreeNode<Configuration>(
                        child, currentNode,ChildOrder);

                if (childNode.determineDepth(childNode) < GameManager.currentAI_ThinkDepth)
                {
                    tree.AddNode(childNode);
                    nodeList.AddLast(childNode);
                }
            }
        }
        return tree;
    }

    /// <summary>
    /// Assigns minimax scores to the tree nodes
    /// </summary>
    /// <param name="tree">tree to mark with scores</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void Minimax(MinimaxTreeNode<Configuration> tree,
        bool maximizing)
    {
        // recurse on children
        IList<MinimaxTreeNode<Configuration>> children = tree.Children;
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
                tree.MinimaxScore = int.MinValue;
            }
            else
            {
                tree.MinimaxScore = int.MaxValue;
            }

            // find maximum or minimum value in children
            foreach (MinimaxTreeNode<Configuration> child in children)
            {
                if (maximizing)
                {
                    // check for higher minimax score
                    if (child.MinimaxScore > tree.MinimaxScore)
                    {
                        tree.MinimaxScore = child.MinimaxScore;
                    }
                }
                else
                {
                    // minimizing, check for lower minimax score
                    if (child.MinimaxScore < tree.MinimaxScore)
                    {
                        tree.MinimaxScore = child.MinimaxScore;
                    }
                }
            }
        }
        else
        {
            // leaf nodes are the base case
            AssignHeuristicMinimaxScore(tree, maximizing);
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
            // other player took the last teddy
            node.MinimaxScore = 1;
        }
        else
        {
            // we took the last teddy
            node.MinimaxScore = 0;
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
            node.MinimaxScore = 0.5f;
        }
    }

}
