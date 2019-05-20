using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealNode : DejTree {

    public DejarikChessPiece[] NodeState;
    public int nodeDepth; //real "turn Depth"
    public int turn;
    public DejarikChessPiece pieceToPush=null;
    public int fromSector;
    public int toSector;

    public RealNode(DejarikChessPiece[] nodeState,int depth,int turn)
    {
        this.NodeState = nodeState;
        this.nodeDepth = depth;
        this.childrenNodes = new List<DejTree>();
        this.turn = turn;
    }
    public bool StateEquals(DejarikChessPiece[] otherState)
    {
        bool result = true;
        for (int i = 0; i < 25; i++)
        {
            if ((NodeState[i] == null && otherState[i] != null) ||
                (NodeState[i] != null && otherState[i] == null) ||
                !otherState[i].Equals(NodeState[i]))
            {
                result = false;
                break;
            }
        }
        return result;
    }
}
