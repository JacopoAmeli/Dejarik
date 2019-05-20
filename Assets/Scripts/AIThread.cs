using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AIThread : MonoBehaviour
{
    [Range(1, 4)]
    public static int Player_Turn = 2;
    [Range(1, 4)]
    public static int Enemy_Turn = 1;
    public bool _threadRunning = false;
    public int fromSector = -1;
    public int toSector = -1;
    Thread _thread=null;
    DejarikChessDuD[] realState;
    bool secondMove;
    bool waitingForPush = false;
    DejarikChessDuD pushedPiece = null;
    int originalTurn;

    public bool initialized = false;

    private void Start()
    {
        initialized = true;
    }
    
    public void StartThread(DejarikChessDuD[] state, bool secondMove,bool waitingForPush,DejarikChessDuD pushedPiece,int originalTurn)
    {
        // Begin our heavy work on a new thread.
        realState = state;
        this.secondMove = secondMove;
        this.waitingForPush = waitingForPush;
        this.pushedPiece = pushedPiece;
        this.originalTurn = originalTurn;
        StopThread();
        _thread = new Thread(ThreadedWork);
        _thread.Start();
    }
    public void StartThread(DejarikChessDuD[] state, bool secondMove)
    {
        // Begin our heavy work on a new thread.
        realState = state;
        this.secondMove = secondMove;
        this.waitingForPush = false;
        this.pushedPiece = null;
        StopThread();
        _thread = new Thread(ThreadedWork);
        _thread.Start();
    }
    void ThreadedWork()
    {
        try
        {
            _threadRunning = true;
            bool workDone = false;
            for(int i=0; i<25;i++)
            {
                int[] adiacents = BoardManager.AdiacentSectors(i);
                for (int j = 0; j < adiacents.Length; j++)
                    if (adiacents[j] < 0 || adiacents[j] > 24)
                        Debug.Log("AdiacentSectors fail: sector " +i+" has adiacent "+adiacents[j] );
            }
            // This pattern lets us interrupt the work at a safe point if neeeded.
            int turnDepth = 0;
            float oldv = -1000f;
            float v = -1000f;
            float alpha = -1000f;
            float beta = 1000f;
            if (waitingForPush)
            {
                int[] possiblePushSectors = BoardManager.AdiacentSectors(pushedPiece.CurrentSector);
                fromSector = pushedPiece.CurrentSector;
                int found = 0;
                int sectorFound = -1;
                for (int i = 0; i < possiblePushSectors.Length; i++)
                {
                    if (realState[possiblePushSectors[i]] == null)
                    {
                        found++;
                        sectorFound = possiblePushSectors[i];
                    }
                }
                if (found == 1)
                {
                    toSector = sectorFound;
                }
                else
                {
                    for (int i = 0; _threadRunning && i < possiblePushSectors.Length; i++)
                    {
                        if (realState[possiblePushSectors[i]] == null)
                        {
                            //we can move the piece into sector "i"
                            DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(realState, realState[pushedPiece.CurrentSector], possiblePushSectors[i]);
                            if (originalTurn == Player_Turn)
                            {
                                //next action is taken by AI
                                v = Utilities.Max(v, MaxValue(newState, turnDepth, secondMove, alpha, beta));
                            }
                            else
                            {
                                //next action is taken by a Real Person (tm)
                                v = Utilities.Max(v, MinValue(newState, turnDepth, secondMove, alpha, beta));
                            }
                            if (oldv != v)
                                toSector = possiblePushSectors[i];
                            if (v >= beta)
                            {
                                workDone = true;
                                break;
                            }
                            if (v > alpha)
                                alpha = v;
                            oldv = v;
                        }
                    }
                }
                workDone = true;
            }
            else
            {

                if (DejTree.CutOffTest(realState, turnDepth)) //should be false until last move or a certain depth
                    v = DejTree.Eval(realState);
                else
                {
                    int i = 0;
                    for (i = 0; _threadRunning && i < 25; i++)
                    {
                        if (realState[i] != null && realState[i].Owner == Player_Turn)
                        {
                            //Debug.Log("Starting to evaluate actions available for piece" + state[i].Name);
                            if (realState[i].AllPossibleMoves() == null)
                            {
                                // Debug.Log("Possible moves for " + state[i].Name + " are null");
                                continue;
                            }
                            int j = 0;
                            for (j = 0; _threadRunning && j < 25; j++)
                            {
                                bool possibleMove = realState[i].AllPossibleMoves()[j];
                                if (possibleMove)
                                {
                                    if (realState[j] == null)
                                    {
                                        //We are "Moving" the piece to an empty sector.
                                        DejarikChessDuD[] newState = Utilities.FakeMoveChessPiece(realState, realState[i], j);
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
                                        int nextTurn = Player_Turn;
                                        if (secondMove)
                                        {
                                            nextTurn = Enemy_Turn;
                                        }
                                        DejarikChessDuD[] dudState = Utilities.Clone(realState);
                                        v = Utilities.Max(v, ChanceValue(dudState, i, j, turnDepth, secondMove, alpha, beta, nextTurn));
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
                                        workDone = true;
                                        break;
                                    }
                                    if (v > alpha)
                                        alpha = v;
                                    oldv = v;
                                }
                                // Debug.Log("Finished to evaluate action " + j + " for piece" + state[i].Name);
                            }
                            //Debug.Log("Finished to evaluate actions available for piece" + state[i].Name);
                            if (workDone)
                                break;
                        }
                    }
                }
            }
            workDone = true;
            _threadRunning = false;
        }
        catch ( System.Exception e )
        {
            System.Console.WriteLine(e.Message);
        }
    }

    void OnDisable()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (_threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }
    void StopThread()
    {
        if (_threadRunning && _thread!=null)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
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
                if (state[i] != null && state[i].Owner == Player_Turn && state[i].AllPossibleMoves() != null)
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
                                int nextTurn = Player_Turn;
                                if (secondMove)
                                {
                                    nextTurn = Enemy_Turn;
                                }
                                DejarikChessDuD[] dudState = Utilities.Clone(state);
                                v = Utilities.Max(v, ChanceValue(dudState, i, j, turnDepth, secondMove, alpha, beta,nextTurn));
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
        DejarikChessDuD[] newState = null;
        if (!Utilities.isAdiacentFree(sector,state))
        {
            newState = Utilities.Clone(state);
            if (realTurn == Player_Turn)
            {
                //next action is taken by AI
                v = Utilities.Max(v, MaxValue(newState, turnDepth, secondMove, alpha, beta));
            }
            else
            {
                //next action is taken by a Real Person (tm)
                v = Utilities.Max(v, MinValue(newState, turnDepth, secondMove, alpha, beta));
            }
        }
        int[] possiblePushSectors = BoardManager.AdiacentSectors(sector);
        for (int i = 0; i < possiblePushSectors.Length; i++)
        {
            if (state[possiblePushSectors[i]] == null)
            {
                //we can move the piece into sector "i"
                newState = Utilities.FakeMoveChessPiece(state, state[sector], possiblePushSectors[i]);
                if (realTurn == Player_Turn)
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
                                int nextTurn=Enemy_Turn;
                                if (secondMove)
                                    nextTurn = Player_Turn;
                                DejarikChessDuD[] dudState = Utilities.Clone(state);
                                v = Utilities.Max(v, ChanceValue(dudState, i, j, turnDepth, secondMove, alpha, beta, nextTurn));
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
        DejarikChessDuD[] newState = null;
        if (!Utilities.isAdiacentFree(sector, state))
        {
            newState = Utilities.Clone(state);
            if (realTurn == Player_Turn)
            {
                //next action is taken by AI
                v = Utilities.Max(v, MaxValue(newState, turnDepth, secondMove, alpha, beta));
            }
            else
            {
                //next action is taken by a Real Person (tm)
                v = Utilities.Max(v, MinValue(newState, turnDepth, secondMove, alpha, beta));
            }
        }
        int[] possiblePushSectors = BoardManager.AdiacentSectors(sector);
        for (int i = 0; i < possiblePushSectors.Length; i++)
        {
            if (state[possiblePushSectors[i]] == null)
            {
                //we can move the piece into sector "i"
                newState = Utilities.FakeMoveChessPiece(state, state[sector], possiblePushSectors[i]);
                if (realTurn == Player_Turn )
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
        DejarikChessDuD[] dudState = Utilities.Clone(state);
        // state[fromSector] is the turn that originated this simulation. realTurn is the next turn. if they are different, secondMove should be false.
        //also, the change of turn increases turnDepth
        if (state[fromSector].Owner != realTurn)
        {
            secondMove = false;
            turnDepth++;
        }
        else
            secondMove = true;
        if (state[fromSector].Owner==Enemy_Turn)
        {
            result += chanceValues[1] * MinValuePush(dudState, toSector, turnDepth, secondMove, alpha, beta, realTurn);
            result += chanceValues[2] * MaxValuePush(dudState, fromSector, turnDepth, secondMove, alpha, beta, realTurn);
        }
        else
        {
            result += chanceValues[1] * MaxValuePush(dudState, toSector, turnDepth, secondMove, alpha, beta, realTurn);
            result += chanceValues[2] * MinValuePush(dudState, fromSector, turnDepth, secondMove, alpha, beta, realTurn);
        }
        if (realTurn == Player_Turn)
        {
            result += chanceValues[0] * MaxValue(Utilities.FakeMoveChessPiece(state, state[fromSector], toSector), turnDepth, true, alpha, beta);
            result += chanceValues[3] * MaxValue(Utilities.FakeCounterKill(state, fromSector), turnDepth, true, alpha, beta);
        }
        else
        {
            result += chanceValues[0] * MinValue(Utilities.FakeMoveChessPiece(state, state[fromSector], toSector), turnDepth, secondMove, alpha, beta);
            result += chanceValues[3] * MinValue(Utilities.FakeCounterKill(state, fromSector), turnDepth, secondMove, alpha, beta);
        }
        return result;
    }
}