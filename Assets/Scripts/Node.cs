using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> neighoubrs;
    public int x;
    public int y;
    public Node()
    {
        neighoubrs = new List<Node>();
    }
    public float DistanceTo(Node n)
    {
        if (n == null)
        {
            Debug.LogError("this should never happen");
        }
        return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
    }
    public Collider2D getNodeHex()
    {
        return GameObject.Find("Hex_" + x + "_" + y).GetComponent<Collider2D>();
    }
}