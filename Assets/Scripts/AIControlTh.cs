using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AIThread))]
public class AIControlTh : MonoBehaviour
{
    [Range(1, 4)]
    public int Player_Turn = 2;
    [Range(1, 4)]
    public int Enemy_Turn = 1;
    private BoardManager board;
    private DejarikChessPiece selectedChessPiece;
    public float coroutineInterval = 2 * 0.017f;
    private AIThread threadInterface;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Awake()
    {
        board = FindObjectOfType<BoardManager>();
        threadInterface = GetComponent<AIThread>();
        StartCoroutine(MainCoroutine());
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
            yield return new WaitForSeconds(2 * coroutineInterval);
        while (!threadInterface.initialized)
            yield return new WaitForSeconds(2 * coroutineInterval);
        DejarikChessPiece[] currentRealState = new DejarikChessPiece[25];
        DejarikChessDuD[] state = new DejarikChessDuD[25];
        int fromSector = -1;
        int toSector = -1;
        while (!board.gameEnded)
        {
            //version 1: we wait for our turn to calculate everything.
            while (board.currentTurn != this.Player_Turn)
                yield return new WaitForSeconds(6 * coroutineInterval);
            if (board.gameEnded)
                break;
            //We get the current state of the game
            for (int i = 0; i < 25; i++)
            {
                currentRealState[i] = board.getPieceInSector(i);
            }
            state = Utilities.Convert(currentRealState);
            Debug.Log("Collected current state of the game");
            fromSector = -1;
            toSector = -1;
            //Simulating first move at level 0
            bool secondMove = (board.actionsLeft == 1);
            if (board.waitingForPush && board.currentTurn==Player_Turn)
            {
                Debug.Log("Starting search for a push");
                threadInterface.StartThread(state, secondMove, board.waitingForPush, state[board.pushed.CurrentSector], board.originalTurn);
            }
            else if (board.currentTurn==Player_Turn)
            {
                if (secondMove)
                    Debug.Log("Starting search for second move");
                else
                    Debug.Log("Starting search for first move");
                threadInterface.StartThread(state, secondMove);
            }
            yield return new WaitForSeconds(30 * coroutineInterval);
            while (threadInterface._threadRunning)
                yield return new WaitForSeconds(5 * coroutineInterval);
            
            fromSector = threadInterface.fromSector;
            toSector = threadInterface.toSector;

            Debug.Log("Computation over, move " + state[fromSector].Name + " to sector " + toSector);
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
            yield return new WaitForSeconds(6 * coroutineInterval);
        }

    }
    private void PushChessPiece(int sector)
    {
        if (sector < 0 || sector > 24 || board.getPieceInSector(sector) != null
            || selectedChessPiece == null || !BoardManager.AreAdiacent(sector,selectedChessPiece.CurrentSector))
        {
            Debug.Log("Wrong move: push " + selectedChessPiece.Name + " to " + sector);
            return;
        }
        board.PushChessPiece(sector);
    }
    private void AttackChessPiece(int attackSector)
    {
        if (attackSector < 0 || attackSector > 24 || board.getPieceInSector(attackSector) == null 
            || selectedChessPiece==null|| selectedChessPiece.Owner == board.getPieceInSector(attackSector).Owner
            || !selectedChessPiece.AllPossibleMoves()[attackSector])
        {
            Debug.Log("Wrong move: attack from " + selectedChessPiece.Name + " to "+attackSector);
            return;
        }
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
    private void MoveChessPiece(int moveSector)
    {
        if (moveSector < 0 || moveSector > 24 || board.getPieceInSector(moveSector) != null
            || selectedChessPiece == null || !selectedChessPiece.AllPossibleMoves()[moveSector])
        {
            Debug.Log("Wrong move: " + selectedChessPiece.Name + " to " + moveSector);
            return;
        }
        board.MoveChessPiece(selectedChessPiece.CurrentSector, moveSector);
        selectedChessPiece = null;
    }

}
