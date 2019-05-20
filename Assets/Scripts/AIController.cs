using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AIController : MonoBehaviour
{
    [Range(1, 4)]
    public int Player_Turn = 2;
    [Range(1, 4)]
    public int Enemy_Turn = 1;
    private BoardManager board;
    private DejarikChessPiece selectedChessPiece;
    public float coroutineInterval = 2*0.016667f;

    // Use this for initialization
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        StartCoroutine(MainCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }
    //This is the main Coroutine, executing every "coroutineInterval" instead of every frame
    private IEnumerator MainCoroutine()
    {
        //Build a minmax tree for imperfect, adversarial, stochastic alpha-beta search.
        //Every max/min node is separated by a chance node. 
        //Some chance nodes have only 1 child, with probability==1 (deterministic movements)
        //Every turn contains two moves. 

        //Wait for board to initialize
        while (!board.initialized)
            yield return new WaitForSeconds(2* coroutineInterval);
        DejarikChessPiece[] currentRealState = new DejarikChessPiece[25];
        DejarikChessDuD[] state = new DejarikChessDuD[25];
        float v = -2000f;
        float oldv = v;
        int fromSector = -1;
        int toSector = -1;
        while (!board.gameEnded)
        {
            //version 1: we wait for our turn to calculate everything.
            while (board.currentTurn != this.Player_Turn)
                yield return new WaitForSeconds(5 * coroutineInterval);
            Debug.Log("Starting search");
            //We get the current state of the game
            for (int i = 0; i < 25; i++)
            {
                currentRealState[i] = board.getPieceInSector(i);
                if (board.getPieceInSector(i) != null)
                    state[i] = new DejarikChessDuD(currentRealState[i].Name,currentRealState[i].Attack, currentRealState[i].Defense, currentRealState[i].Movement,
                        currentRealState[i].CurrentSector, currentRealState[i].Owner, currentRealState[i].pieceType, currentRealState[i].AllPossibleMoves());
                else
                    state[i] = null;
            }
            //Simulating first move at level 0
            float alpha = -1000f;
            float beta = 1000f;
            int turnDepth = 0;
            bool vFound = false;
            bool secondMove = (board.actionsLeft == 1);
            if (board.waitingForPush)
            {
                int[] possiblePushSectors = BoardManager.AdiacentSectors(board.pushed.CurrentSector);
                fromSector = board.pushed.CurrentSector;
                for (int i = 0; i < possiblePushSectors.Length; i++)
                {
                    if (state[i] == null)
                    {
                        //we can move the piece into sector "i"
                        DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(state, state[board.pushed.CurrentSector], i);
                        if ((board.originalTurn == Player_Turn && !secondMove) || (board.originalTurn == Enemy_Turn && secondMove))
                        {
                            //next action is taken by AI
                            v = Utilities.Max(v, MaxValue(newState, turnDepth, secondMove, alpha, beta));
                        }
                        else
                        {
                            //next action is taken by a Real Person (tm)
                            v = Utilities.Max(v, MinValue(newState, turnDepth, secondMove, alpha, beta));
                        }
                        if(oldv!=v)
                            toSector = i;
                        if (v >= beta)
                            break;
                        if (v > alpha)
                            alpha = v;
                        oldv = v;
                    }
                }
            }
            else
            {
                if (DejTree.CutOffTest(state, turnDepth)) //should be false until last move or a certain depth
                    v = DejTree.Eval(state);
                else
                {
                    v = -420f;
                    int i = 0;
                    for (i = 0; i < 25; i++)
                    {
                        if (state[i] != null && state[i].Owner == Player_Turn)
                        {
                            Debug.Log("Starting to evaluate actions available for piece" + state[i].Name);
                            if (state[i].AllPossibleMoves() == null)
                            {
                                Debug.Log("Possible moves for " + state[i].Name + " are null");
                                continue;
                            }
                            int j = 0;
                            for (j = 0; j < 25; j++)
                            {
                                if (state[i].AllPossibleMoves()[j])
                                {
                                    if (state[j] == null)
                                    {
                                        //We are "Moving" the piece to an empty sector.
                                        DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(state, state[i], j);
                                        if (secondMove)
                                        {
                                            //This is the last move of the turn, next move is going to be enemy
                                            v = Utilities.Max(v, MinValue(newState, turnDepth + 1, false, alpha, beta));
                                        }
                                        else
                                        {
                                            //next move is still ai, but is the last (second)
                                            v = Utilities.Max(v, MaxValue(newState, turnDepth, true, alpha, beta));
                                        }
                                    }
                                    else
                                    {
                                        //We are "Attacking" another piece
                                        //This is the last move of the turn, next move is going to be enemy
                                        v = Utilities.Max(v, ChanceValue(state, i, j, turnDepth, secondMove, alpha, beta, Player_Turn));
                                    }
                                    if (oldv != v)
                                    {
                                        fromSector = i;
                                        toSector = j;
                                    }
                                    if (v >= beta)
                                    {
                                        //return v;
                                        //break out of both cycles and move on
                                        vFound = true;
                                        break;
                                    }
                                    if (v > alpha)
                                    {
                                        alpha = v;
                                    }
                                    oldv = v;
                                    Debug.Log("Finished to evaluate action " + j + " for piece" + state[i].Name);
                                }
                                yield return new WaitForSeconds(coroutineInterval);
                            }
                            //I need to return control to Unity
                            Debug.Log("Finished to evaluate actions available for piece" + state[i].Name);
                            yield return new WaitForSeconds(coroutineInterval);
                            if (vFound)
                                break;
                        }
                    }
                }
            }
            // the action of the node with value v
            selectedChessPiece = currentRealState[fromSector];
            if (currentRealState[toSector] == null)
            {
                if (board.waitingForPush)
                    PushChessPiece(toSector);
                else
                    MoveChessPiece(toSector);
            }
            else
            {
                //attack
                AttackChessPiece(toSector);
            }
        }
        
    }


    private float MaxValue(DejarikChessDuD[] state, int turnDepth, bool secondMove, float alpha, float beta)
    {
        float v = 0f;
        if (DejTree.CutOffTest(state, turnDepth)) //should be false until last move or a certain depth
            v = DejTree.Eval(state);
        else
        {
            v = -420f;
            for (int i = 0; i < 25; i++)
            {
                if (state[i] != null && state[i].Owner == Player_Turn && state[i].AllPossibleMoves()!=null)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        if (state[i].AllPossibleMoves()[j])
                        {
                            if (state[j] == null)
                            {
                                DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(state, state[i], j);
                                //We are "Moving" the piece to an empty sector.
                                if (secondMove)
                                {
                                    //This is the last move of the turn, next move is going to be enemy
                                    v = Utilities.Max(v, MinValue(newState, turnDepth + 1, false, alpha, beta));
                                }
                                else
                                {
                                    //next move is still ai, but is the last (second)
                                    v = Utilities.Max(v, MaxValue(newState, turnDepth, true, alpha, beta));
                                }
                            }
                            else
                            {
                                //We are "Attacking" another piece
                                //This is the last move of the turn, next move is going to be enemy
                                v = Utilities.Max(v, ChanceValue(state, i, j, turnDepth, secondMove, alpha, beta, Player_Turn));
                            }
                            if (v >= beta)
                            {
                                return v;
                            }
                            if (v > alpha)
                                alpha = v;
                        }
                    }
                }

            }
        }
        return v;
    }
    private float MaxValuePush(DejarikChessDuD[] state, int sector, int turnDepth, bool secondMove, float alpha, float beta, int realTurn)
    {
        float v = -420f;
        int[] possiblePushSectors = BoardManager.AdiacentSectors(sector);
        for (int i = 0; i < possiblePushSectors.Length; i++)
        {
            if (state[i] == null)
            {
                //we can move the piece into sector "i"
                DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(state, state[sector], i);
                if (secondMove)
                    turnDepth++;
                if ((realTurn == Player_Turn && !secondMove) || (realTurn == Enemy_Turn && secondMove))
                {
                    //next action is taken by AI
                    v = Utilities.Max(v, MaxValue(newState, turnDepth, secondMove, alpha, beta));
                }
                else
                {
                    //next action is taken by a Real Person (tm)
                    v = Utilities.Max(v, MinValue(newState, turnDepth, secondMove, alpha, beta));
                }
                if (v >= beta)
                    return v;
                if (v > alpha)
                    alpha = v;
            }
        }
        return v;
    }
    private float MinValue(DejarikChessDuD[] state, int turnDepth, bool secondMove, float alpha, float beta)
    {
        if (DejTree.CutOffTest(state, turnDepth))
            return DejTree.Eval(state);
        else
        {
            float v = 420f;
            for (int i = 0; i < 25; i++)
            {
                if (state[i] != null && state[i].Owner == Enemy_Turn)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        if (state[i].AllPossibleMoves()[j])
                        {
                            if (state[j] == null)
                            {
                                //We are "Moving" the piece to an empty sector.
                                DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(state, state[i], j);
                                if (secondMove)
                                {
                                    //This is the last move of the turn, next move is going to be ai
                                    v = Utilities.Min(v, MaxValue(newState, turnDepth + 1, false, alpha, beta));
                                }
                                else
                                {
                                    //next move is still the real person
                                    v = Utilities.Min(v, MinValue(newState, turnDepth, true, alpha, beta));
                                }
                            }
                            else
                            {
                                //We are "Attacking" another piece
                                v = Utilities.Max(v, ChanceValue(state, i, j, turnDepth, secondMove, alpha, beta, Enemy_Turn));
                            }
                            if (v <= alpha)
                                return v;
                            if (v < beta)
                                beta = v;
                        }
                    }
                }
            }
            return v;
        }
    }

    private float MinValuePush(DejarikChessDuD[] state, int sector, int turnDepth, bool secondMove, float alpha, float beta, int realTurn)
    {
        float v = 420f;
        int[] possiblePushSectors = BoardManager.AdiacentSectors(sector);
        for (int i = 0; i < possiblePushSectors.Length; i++)
        {
            if (state[i] == null)
            {
                //we can move the piece into sector "i"
                DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(state, state[sector], i);
                if ((realTurn == Player_Turn && !secondMove) || (realTurn == Enemy_Turn && secondMove))
                {
                    //next action is taken by AI
                    v = Utilities.Min(v, MaxValue(newState, turnDepth, secondMove, alpha, beta));
                }
                else
                {
                    //next action is taken by a Real Person (tm)
                    v = Utilities.Min(v, MinValue(newState, turnDepth, secondMove, alpha, beta));
                }
                if (v <= alpha)
                    return v;
                if (v < beta)
                    beta = v;
            }
        }
        return v;
    }
    private float ChanceValue(DejarikChessDuD[] state, int fromSector, int toSector, int turnDepth, bool secondMove, float alpha, float beta, int realTurn)
    {
        float result = 0f;
        float[] chanceValues = Utilities.CalculateProbabilities(state[fromSector], state[toSector]);
        if (realTurn != Player_Turn)
        {
            result += chanceValues[1] * MinValuePush(state, toSector, turnDepth, secondMove, alpha, beta, realTurn);
            result += chanceValues[2] * MaxValuePush(state, fromSector, turnDepth, secondMove, alpha, beta, realTurn);
            if (!secondMove)
            {
                result += chanceValues[0] * MinValue(Utilities.FakeMoveChessPiece(state, state[fromSector], toSector), turnDepth, true, alpha, beta);
                result += chanceValues[3] * MinValue(Utilities.FakeCounterKill(state, fromSector), turnDepth, true, alpha, beta);
            }
            else
            {
                secondMove = false;
                turnDepth++;
                result += chanceValues[0] * MaxValue(Utilities.FakeMoveChessPiece(state, state[fromSector], toSector), turnDepth, secondMove, alpha, beta);
                result += chanceValues[3] * MaxValue(Utilities.FakeCounterKill(state, fromSector), turnDepth, secondMove, alpha, beta);
            }
        }
        else
        {
            result += chanceValues[1] * MaxValuePush(state, toSector, turnDepth, secondMove, alpha, beta, realTurn);
            result += chanceValues[2] * MinValuePush(state, fromSector, turnDepth, secondMove, alpha, beta, realTurn);
            if (!secondMove)
            {
                result += chanceValues[0] * MaxValue(Utilities.FakeMoveChessPiece(state, state[fromSector], toSector), turnDepth, true, alpha, beta);
                result += chanceValues[3] * MaxValue(Utilities.FakeCounterKill(state, fromSector), turnDepth, true, alpha, beta);
            }
            else
            {
                secondMove = false;
                turnDepth++;
                result += chanceValues[0] * MinValue(Utilities.FakeMoveChessPiece(state, state[fromSector], toSector), turnDepth, secondMove, alpha, beta);
                result += chanceValues[3] * MinValue(Utilities.FakeCounterKill(state, fromSector), turnDepth, secondMove, alpha, beta);
            }
        }
        return result;
    }
    private void PushChessPiece(int sector)
    {
        board.PushChessPiece(sector);
    }
    private void AttackChessPiece(int attackSector)
    {
        if (attackSector < 0 || attackSector > 24 || board.getPieceInSector(attackSector) == null)
            return;
        if (selectedChessPiece.Owner != board.getPieceInSector(attackSector).Owner)
        {
            BoardManager.ATTACK_RESULT result = board.AttackChessPiece(selectedChessPiece.CurrentSector, attackSector);
            if (result == BoardManager.ATTACK_RESULT.Kill)
            {
                Debug.Log("Kill");
                selectedChessPiece = null;
            }
            else if (result == BoardManager.ATTACK_RESULT.Push)
            {
                bool[] possiblePush = new bool[25];
                for (int i = 0; i < 25; i++)
                {
                    possiblePush[i] = (BoardManager.AreAdiacent(i, attackSector)
                        && board.getPieceInSector(i) == null); ;
                }
                Debug.Log("Push");
                selectedChessPiece = null;
            }
            else if (result == BoardManager.ATTACK_RESULT.Counterpush)
            {
                bool[] possiblePush = new bool[25];
                for (int i = 0; i < 25; i++)
                {
                    possiblePush[i] = (BoardManager.AreAdiacent(i, selectedChessPiece.CurrentSector)
                        && board.getPieceInSector(i) == null);
                }
                Debug.Log("Counterpush");
                selectedChessPiece = null;
            }
            else if (result == BoardManager.ATTACK_RESULT.Counterkill)
            {
                Debug.Log("Counterkill");
                selectedChessPiece = null;
            }
        }
    }
    private void SelectChessPiece(int selectSector)
    {
        if (board.getPieceInSector(selectSector) == null || selectSector < 0 || selectSector > 24)
            return;
        else if (board.getPieceInSector(selectSector).Owner != board.currentTurn)
            return;
        selectedChessPiece = board.getPieceInSector(selectSector);
    }
    private void MoveChessPiece(int moveSector)
    {
        if (moveSector < 0 || moveSector > 24)
            return;
        if (selectedChessPiece.AllPossibleMoves()[moveSector])
        {
            board.MoveChessPiece(selectedChessPiece.CurrentSector, moveSector);
        }
        selectedChessPiece = null;
    }

}
