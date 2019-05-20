using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HighlighterScript))]
public class DejarikChessPiece : MonoBehaviour {

    public string Name="";
    [Range(1,10)]
    public int Attack = 0;
    [Range(2,10)]
    public int Defense = 0;
     [Range(1,4)]
    public int Movement = 0;
    [Range(0f, 5f)]
    public float PlayerRadius = 0.05f;
    public int CurrentSector { get; set; }
    public int Owner { get; set; }
    public enum PieceTypeEnum { Offensive, Defensive, Balanced, Weak}
    public PieceTypeEnum pieceType;
    public BoardManager board;
    private bool[] possibleMoves;
    private HighlighterScript highlightPlayer;
    private Gradient redGradient;
    private Gradient blueGradient;

    public void Awake()
    {
        Gradient g;
        GradientColorKey[] gck;
        GradientAlphaKey[] gak;
        g = new Gradient();
        gck = new GradientColorKey[2];
        gck[0].color = Color.blue;
        gck[0].time = 0.0F;
        gck[1].color = Color.blue;
        gck[1].time = 1.0F;
        gak = new GradientAlphaKey[2];
        gak[0].alpha = 1.0F;
        gak[0].time = 0.0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;
        g.SetKeys(gck, gak);
        blueGradient = g;
        g = new Gradient();
        gck = new GradientColorKey[2];
        gck[0].color = Color.red;
        gck[0].time = 0.0F;
        gck[1].color = Color.red;
        gck[1].time = 1.0F;
        g.SetKeys(gck, gak);
        redGradient = g;
        board = FindObjectOfType<BoardManager>();
        
    }
    public void Dead()
    {
        if (highlightPlayer != null)
            highlightPlayer.Disable();
    }
    public bool Equals(DejarikChessPiece other)
    {
        return (other.Name.Equals(Name) && other.Owner == Owner);
    }
    public void UpdateCircle()
    {
        if(highlightPlayer==null)
            highlightPlayer = GetComponent<HighlighterScript>();
        if (Owner == 1)
        {
            highlightPlayer.ChangeColor(blueGradient);
        }
        else
        {
            highlightPlayer.ChangeColor(redGradient);
        }
        highlightPlayer.DrawCirclePlayer(gameObject.transform.position, PlayerRadius);
    }
    public void UpdatePossibleMoves()
    {
        
        possibleMoves = new bool[25];
        for (int i = 0; i < 25; i++)
        {
            possibleMoves[i] = PossibleMove(i);
        }
    }
    public void UpdatePossibleMoves(DejarikChessPiece[] state)
    {
        possibleMoves = new bool[25];
        for (int i = 0; i < 25; i++)
        {
            possibleMoves[i] = PossibleMove(i,state);
        }
    }
    public bool PossibleMove(int sector, DejarikChessPiece[] state)
    {

        if (sector < 0 || sector > 24 || sector == CurrentSector)
            return false;
        // Only two types of move: actual moving, and attacking. 
        // Moving requires using all "Movement" through adiacent, non occupied sectors.
        // Attacking requires having an enemy piece adiacent. 
        if (state[sector]== null)
        {
            //We check if we can reach an unoccupied sector by moving
            return PossibleMoveRecur(state, -1, CurrentSector, sector, Movement);
        }
        else
        {
            //sector is occupied, we check if it's adiacent
            if (state[sector].Owner == this.Owner)
            {
                return false;
            }
            else
            {
                return BoardManager.AreAdiacent(CurrentSector, sector);
            }
        }
    }
    public bool PossibleMove(int sector)
    {
        if (sector < 0 || sector > 24|| sector==CurrentSector)
            return false;
        // Only two types of move: actual moving, and attacking. 
        // Moving requires using all "Movement" through adiacent, non occupied sectors.
        // Attacking requires having an enemy piece adiacent. 
        if(board.getPieceInSector(sector)==null)
        {
            //We check if we can reach an unoccupied sector by moving
            return PossibleMoveRecur(-1, CurrentSector, sector, Movement);
        }
        else
        {
            //sector is occupied, we check if it's adiacent
            if (board.getPieceInSector(sector).Owner == this.Owner)
            {
                return false;
            }
            else
            {
                return BoardManager.AreAdiacent(CurrentSector, sector);
            }
        }
    }
    public bool PossibleMoveRecur(DejarikChessPiece[] state,int prevSect, int startSect, int endSect, int movesLeft)
    {
        if (state[endSect] != null)
            return false;
        if (movesLeft == 0)
            return false;
        if (BoardManager.AreAdiacent(startSect, endSect) && movesLeft == 1)
            return true;
        else
        {
            int[] allAdiacents = BoardManager.AdiacentSectors(startSect);
            foreach (int i in allAdiacents)
            {
                if (i != prevSect)
                    if (PossibleMoveRecur(state,startSect, i, endSect, movesLeft - 1))
                        return true;
            }
            return false;
        }
    }
    public bool PossibleMoveRecur(int prevSect,int startSect, int endSect, int movesLeft)
    {
        if (board.getPieceInSector(endSect) != null)
            return false;
        if (movesLeft == 0)
            return false;
        if (BoardManager.AreAdiacent(startSect, endSect) && movesLeft == 1)
            return true;
        else
        {
            int[] allAdiacents = BoardManager.AdiacentSectors(startSect);
            foreach (int i in allAdiacents)
            {
                if (i != prevSect)
                    if (PossibleMoveRecur(startSect, i, endSect, movesLeft - 1))
                        return true;
            }
            return false;
        }        
    }
    public bool[] AllPossibleMoves()
    {
        return possibleMoves;
    }

}
