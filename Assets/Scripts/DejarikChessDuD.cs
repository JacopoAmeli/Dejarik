using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DejarikChessDuD  {

    public string Name = "";
    public int Attack = 0;
    public int Defense = 0;
    public int Movement = 0;
    public int CurrentSector { get; set; }
    public int Owner { get; set; }
    public bool[] possibleMoves;
    public DejarikChessPiece.PieceTypeEnum pieceType;
    public DejarikChessDuD(string Name,int Attack,int Defense,int Movement,int CurrentSector,int Owner, DejarikChessPiece.PieceTypeEnum type,bool[] possibleMoves)
    {
        this.Name = Name;
        this.Attack = Attack;
        this.Defense = Defense;
        this.Movement = Movement;
        this.CurrentSector = CurrentSector;
        this.Owner = Owner;
        this.pieceType = type;
        this.possibleMoves = new bool[25];
        for(int j=0; j<25; j++)
        {
            this.possibleMoves[j] = possibleMoves[j];
        }

    }
    public bool Equals(DejarikChessDuD other)
    {
        return (other.Name.Equals(Name) && other.Owner == Owner);
    }
    public void UpdatePossibleMoves(DejarikChessDuD[] state)
    {
        possibleMoves = new bool[25];
        for (int i = 0; i < 25; i++)
        {
            possibleMoves[i] = PossibleMove(i, state);
        }
    }
    public bool PossibleMove(int sector, DejarikChessDuD[] state)
    {

        if (sector < 0 || sector > 24 || sector == CurrentSector)
            return false;
        // Only two types of move: actual moving, and attacking. 
        // Moving requires using all "Movement" through adiacent, non occupied sectors.
        // Attacking requires having an enemy piece adiacent. 
        if (state[sector] == null)
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
    
    public bool PossibleMoveRecur(DejarikChessDuD[] state, int prevSect, int startSect, int endSect, int movesLeft)
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
                    if (PossibleMoveRecur(state, startSect, i, endSect, movesLeft - 1))
                        return true;
            }
            return false;
        }
    }
    public bool[] AllPossibleMoves()
    {
        return possibleMoves;
    }
    public override string ToString()
    {
        return this.Name + "DuD";
    }
}
