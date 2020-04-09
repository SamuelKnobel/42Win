using System;
using System.Collections;
using System.Collections.Generic;
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

    public Player(string name, PlayerType type, int order)
    {
        playerName = name;
        playerType = type;
        playingOrder = order;

    }



}
