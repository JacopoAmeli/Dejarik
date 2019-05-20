using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DejTree {
    
    public DejTree parentNode;
    public List<DejTree> childrenNodes;
    public static int Player_Turn = 2;
    public float nodeValue = 0f;
    public static float Eval(DejarikChessDuD[] state)
    {
        float result=0f;
        for(int i=0; i<25;i++)
        {
            if(state[i] != null)
            {
                if (state[i].Owner == Player_Turn)
                {
                    if (state[i].pieceType == DejarikChessPiece.PieceTypeEnum.Weak)
                        result += 5f;
                    else
                        result += 10f;
                }
                else
                {
                    if (state[i].pieceType == DejarikChessPiece.PieceTypeEnum.Weak)
                        result -= 5f;
                    else
                        result -= 10f;
                }
            }         
        }
        return result;
    }

    //depth is "turn depth" which means, not the tree depth
    //but rather the number of times the turn changed 
    public static bool CutOffTest(DejarikChessDuD[] state, int depth)
    {
        bool statetest = false;
        int totalAIpieces = 0;
        int totalNonAIpieces = 0;

        for(int i=0; i<25;i++)
        {
            if(state[i]!=null)
            {
                if (state[i].Owner == Player_Turn)
                    totalAIpieces += 1;
                else
                    totalNonAIpieces += 1;
            }
        }
        if ((totalAIpieces > 0 && totalNonAIpieces == 0) || (totalAIpieces==0 && totalNonAIpieces>0))
            statetest = true;
        return ((depth>=2)||statetest);
    }
}
