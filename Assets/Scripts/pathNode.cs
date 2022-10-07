using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathNode
{

    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public float posX;
    public float posY;

    public int caminhoPeso = 1;

    public bool parede = false;

    public pathNode cameFromNode;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

}
