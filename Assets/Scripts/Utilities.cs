using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {
    //returns [0]: prob of kill, [1] prob of push, [2] prob of counterpush,[3] prob of counterkill
    public static float[] CalculateProbabilities(DejarikChessDuD att, DejarikChessDuD def)
    {
        float[] result = new float[4];
        //to save time
        int attackValue = att.Attack;
        int defenseValue = def.Defense;
        switch (attackValue)
        {
            case 2:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 0.162f;
                        result[1] = 0.702f;
                        result[2] = 0.162f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.027f;
                        result[1] = 0.431f;
                        result[2] = 0.541f;
                        result[3] = 0.027f;
                        break;
                    case 3:
                        result[0] = 0.002f;
                        result[1] = 0.151f;
                        result[2] = 0.660f;
                        result[3] = 0.221f;
                        break;
                    case 4:
                        result[0] = 0f;
                        result[1] = 0.035f;
                        result[2] = 0.437f;
                        result[3] = 0.546f;
                        break;
                    case 5:
                        result[0] = 0f;
                        result[1] = 0.006f;
                        result[2] = 0.190f;
                        result[3] = 0.808f;
                        break;
                    case 6:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.060f;
                        result[3] = 0.939f;
                        break;
                    case 7:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.015f;
                        result[3] = 0.985f;
                        break;
                    case 8:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.003f;
                        result[3] = 0.997f;
                        break;
                    case 9:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0f;
                        result[3] = 1f;
                        break;
                    case 10:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0f;
                        result[3] = 1f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 3:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 0.556f;
                        result[1] = 0.431f;
                        result[2] = 0.027f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.221f;
                        result[1] = 0.606f;
                        result[2] = 0.220f;
                        result[3] = 0.002f;
                        break;
                    case 3:
                        result[0] = 0.060f;
                        result[1] = 0.426f;
                        result[2] = 0.513f;
                        result[3] = 0.060f;
                        break;
                    case 4:
                        result[0] = 0.012f;
                        result[1] = 0.189f;
                        result[2] = 0.600f;
                        result[3] = 0.257f;
                        break;
                    case 5:
                        result[0] = 0.001f;
                        result[1] = 0.060f;
                        result[2] = 0.431f;
                        result[3] = 0.540f;
                        break;
                    case 6:
                        result[0] = 0f;
                        result[1] = 0.014f;
                        result[2] = 0.217f;
                        result[3] = 0.779f;
                        break;
                    case 7:
                        result[0] = 0f;
                        result[1] = 0.002f;
                        result[2] = 0.083f;
                        result[3] = 0.916f;
                        break;
                    case 8:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.025f;
                        result[3] = 0.974f;
                        break;
                    case 9:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.006f;
                        result[3] = 0.994f;
                        break;
                    case 10:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.001f;
                        result[3] = 0.999f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 4:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 0.848f;
                        result[1] = 0.151f;
                        result[2] = 0.002f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.546f;
                        result[1] = 0.426f;
                        result[2] = 0.060f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.257f;
                        result[1] = 0.551f;
                        result[2] = 0.254f;
                        result[3] = 0.012f;
                        break;
                    case 4:
                        result[0] = 0.090f;
                        result[1] = 0.417f;
                        result[2] = 0.491f;
                        result[3] = 0.090f;
                        break;
                    case 5:
                        result[0] = 0.024f;
                        result[1] = 0.215f;
                        result[2] = 0.559f;
                        result[3] = 0.281f;
                        break;
                    case 6:
                        result[0] = 0.005f;
                        result[1] = 0.083f;
                        result[2] = 0.425f;
                        result[3] = 0.536f;
                        break;
                    case 7:
                        result[0] = 0f;
                        result[1] = 0.025f;
                        result[2] = 0.236f;
                        result[3] = 0.757f;
                        break;
                    case 8:
                        result[0] = 0f;
                        result[1] = 0.006f;
                        result[2] = 0.103f;
                        result[3] = 0.8964f;
                        break;
                    case 9:
                        result[0] = 0f;
                        result[1] = 0.001f;
                        result[2] = 0.036f;
                        result[3] = 0.963f;
                        break;
                    case 10:
                        result[0] = 0f;
                        result[1] = 0f;
                        result[2] = 0.011f;
                        result[3] = 0.989f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 5:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 0.964f;
                        result[1] = 0.035f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.808f;
                        result[1] = 0.189f;
                        result[2] = 0.012f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.540f;
                        result[1] = 0.417f;
                        result[2] = 0.090f;
                        result[3] = 0.002f;
                        break;
                    case 4:
                        result[0] = 0.281f;
                        result[1] = 0.515f;
                        result[2] = 0.275f;
                        result[3] = 0.024f;
                        break;
                    case 5:
                        result[0] = 0.116f;
                        result[1] = 0.409f;
                        result[2] = 0.474f;
                        result[3] = 0.116f;
                        break;
                    case 6:
                        result[0] = 0.038f;
                        result[1] = 0.233f;
                        result[2] = 0.530f;
                        result[3] = 0.300f;
                        break;
                    case 7:
                        result[0] = 0.010f;
                        result[1] = 0.102f;
                        result[2] = 0.418f;
                        result[3] = 0.533f;
                        break;
                    case 8:
                        result[0] = 0.002f;
                        result[1] = 0.036f;
                        result[2] = 0.250f;
                        result[3] = 0.740f;
                        break;
                    case 9:
                        result[0] = 0f;
                        result[1] = 0.011f;
                        result[2] = 0.120f;
                        result[3] = 0.878f;
                        break;
                    case 10:
                        result[0] = 0f;
                        result[1] = 0.002f;
                        result[2] = 0.048f;
                        result[3] = 0.951f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 6:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 0.994f;
                        result[1] = 0.006f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.939f;
                        result[1] = 0.060f;
                        result[2] = 0.001f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.779f;
                        result[1] = 0.215f;
                        result[2] = 0.024f;
                        result[3] = 0f;
                        break;
                    case 4:
                        result[0] = 0.536f;
                        result[1] = 0.409f;
                        result[2] = 0.115f;
                        result[3] = 0.005f;
                        break;
                    case 5:
                        result[0] = 0.300f;
                        result[1] = 0.489f;
                        result[2] = 0.288f;
                        result[3] = 0.038f;
                        break;
                    case 6:
                        result[0] = 0.137f;
                        result[1] = 0.402f;
                        result[2] = 0.459f;
                        result[3] = 0.137f;
                        break;
                    case 7:
                        result[0] = 0.052f;
                        result[1] = 0.246f;
                        result[2] = 0.507f;
                        result[3] = 0.314f;
                        break;
                    case 8:
                        result[0] = 0.016f;
                        result[1] = 0.119f;
                        result[2] = 0.412f;
                        result[3] = 0.530f;
                        break;
                    case 9:
                        result[0] = 0.004f;
                        result[1] = 0.047f;
                        result[2] = 0.261f;
                        result[3] = 0.725f;
                        break;
                    case 10:
                        result[0] = 0.001f;
                        result[1] = 0.016f;
                        result[2] = 0.135f;
                        result[3] = 0.862f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 7:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 0.999f;
                        result[1] = 0.001f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.985f;
                        result[1] = 0.015f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.916f;
                        result[1] = 0.083f;
                        result[2] = 0.005f;
                        result[3] = 0f;
                        break;
                    case 4:
                        result[0] = 0.757f;
                        result[1] = 0.233f;
                        result[2] = 0.038f;
                        result[3] = 0.001f;
                        break;
                    case 5:
                        result[0] = 0.533f;
                        result[1] = 0.402f;
                        result[2] = 0.136f;
                        result[3] = 0.010f;
                        break;
                    case 6:
                        result[0] = 0.314f;
                        result[1] = 0.469f;
                        result[2] = 0.298f;
                        result[3] = 0.052f;
                        break;
                    case 7:
                        result[0] = 0.156f;
                        result[1] = 0.395f;
                        result[2] = 0.448f;
                        result[3] = 0.156f;
                        break;
                    case 8:
                        result[0] = 0.065f;
                        result[1] = 0.2564f;
                        result[2] = 0.488f;
                        result[3] = 0.326f;
                        break;
                    case 9:
                        result[0] = 0.023f;
                        result[1] = 0.134f;
                        result[2] = 0.406f;
                        result[3] = 0.528f;
                        break;
                    case 10:
                        result[0] = 0.007f;
                        result[1] = 0.058f;
                        result[2] = 0.269f;
                        result[3] = 0.713f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 8:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 1f;
                        result[1] = 0f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.997f;
                        result[1] = 0.003f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.974f;
                        result[1] = 0.025f;
                        result[2] = 0.001f;
                        result[3] = 0f;
                        break;
                    case 4:
                        result[0] = 0.896f;
                        result[1] = 0.102f;
                        result[2] = 0.010f;
                        result[3] = 0f;
                        break;
                    case 5:
                        result[0] = 0.740f;
                        result[1] = 0.246f;
                        result[2] = 0.052f;
                        result[3] = 0.002f;
                        break;
                    case 6:
                        result[0] = 0.530f;
                        result[1] = 0.395f;
                        result[2] = 0.153f;
                        result[3] = 0.016f;
                        break;
                    case 7:
                        result[0] = 0.326f;
                        result[1] = 0.453f;
                        result[2] = 0.305f;
                        result[3] = 0.065f;
                        break;
                    case 8:
                        result[0] = 0.172f;
                        result[1] = 0.390f;
                        result[2] = 0.437f;
                        result[3] = 0.172f;
                        break;
                    case 9:
                        result[0] = 0.078f;
                        result[1] = 0.264f;
                        result[2] = 0.473f;
                        result[3] = 0.336f;
                        break;
                    case 10:
                        result[0] = 0.030f;
                        result[1] = 0.146f;
                        result[2] = 0.401f;
                        result[3] = 0.527f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 9:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 1f;
                        result[1] = 0f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 0.999f;
                        result[1] = 0.001f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.993f;
                        result[1] = 0.006f;
                        result[2] = 0.001f;
                        result[3] = 0f;
                        break;
                    case 4:
                        result[0] = 0.963f;
                        result[1] = 0.036f;
                        result[2] = 0.002f;
                        result[3] = 0f;
                        break;
                    case 5:
                        result[0] = 0.878f;
                        result[1] = 0.119f;
                        result[2] = 0.016f;
                        result[3] = 0f;
                        break;
                    case 6:
                        result[0] = 0.725f;
                        result[1] = 0.256f;
                        result[2] = 0.065f;
                        result[3] = 0.004f;
                        break;
                    case 7:
                        result[0] = 0.528f;
                        result[1] = 0.390f;
                        result[2] = 0.168f;
                        result[3] = 0.023f;
                        break;
                    case 8:
                        result[0] = 0.336f;
                        result[1] = 0.440f;
                        result[2] = 0.310f;
                        result[3] = 0.078f;
                        break;
                    case 9:
                        result[0] = 0.186f;
                        result[1] = 0.384f;
                        result[2] = 0.429f;
                        result[3] = 0.186f;
                        break;
                    case 10:
                        result[0] = 0.090f;
                        result[1] = 0.270f;
                        result[2] = 0.460f;
                        result[3] = 0.344f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            case 10:
                switch (defenseValue)
                {
                    case 1:
                        result[0] = 1f;
                        result[1] = 0f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 2:
                        result[0] = 1f;
                        result[1] = 0f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 3:
                        result[0] = 0.999f;
                        result[1] = 0.001f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 4:
                        result[0] = 0.989f;
                        result[1] = 0.011f;
                        result[2] = 0f;
                        result[3] = 0f;
                        break;
                    case 5:
                        result[0] = 0.952f;
                        result[1] = 0.044f;
                        result[2] = 0.004f;
                        result[3] = 0f;
                        break;
                    case 6:
                        result[0] = 0.862f;
                        result[1] = 0.114f;
                        result[2] = 0.023f;
                        result[3] = 0.001f;
                        break;
                    case 7:
                        result[0] = 0.713f;
                        result[1] = 0.264f;
                        result[2] = 0.077f;
                        result[3] = 0.007f;
                        break;
                    case 8:
                        result[0] = 0.527f;
                        result[1] = 0.384f;
                        result[2] = 0.180f;
                        result[3] = 0.030f;
                        break;
                    case 9:
                        result[0] = 0.344f;
                        result[1] = 0.429f;
                        result[2] = 0.313f;
                        result[3] = 0.090f;
                        break;
                    case 10:
                        result[0] = 0.198f;
                        result[1] = 0.379f;
                        result[2] = 0.421f;
                        result[3] = 0.198f;
                        break;
                    default:
                        if (defenseValue > 10)
                        {

                        }
                        break;
                }
                break;
            default:
                if (attackValue > 10)
                {

                }
                break;

        }
        return result;
    }
    public static DejarikChessDuD[] Clone(DejarikChessDuD[] state)
    {
        DejarikChessDuD[] newState = new DejarikChessDuD[25];
        for (int i = 0; i < 25; i++)
        {
            if (state[i] != null)
                newState[i] = new DejarikChessDuD(state[i].Name, state[i].Attack, state[i].Defense, state[i].Movement,
                    state[i].CurrentSector, state[i].Owner, state[i].pieceType, state[i].AllPossibleMoves());
            else
                newState[i] = null;
        }
        return newState;
    }
    public static DejarikChessDuD[] Convert(DejarikChessPiece[] state)
    {
        DejarikChessDuD[] newState = new DejarikChessDuD[25];
        for (int i = 0; i < 25; i++)
        {
            if (state[i]!= null)
                newState[i] = new DejarikChessDuD(state[i].Name, state[i].Attack, state[i].Defense, state[i].Movement,
                    state[i].CurrentSector, state[i].Owner, state[i].pieceType, state[i].AllPossibleMoves());
            else
                newState[i] = null;
        }
        return newState;
    }
    public static DejarikChessDuD[] FakeMoveChessPiece(DejarikChessDuD[] state, DejarikChessDuD piece, int toSector)
    {
        DejarikChessDuD[] newState = Utilities.Clone(state);
        newState[piece.CurrentSector] = null;
        DejarikChessDuD newPiece = new DejarikChessDuD(piece.Name, piece.Attack, piece.Defense, piece.Movement,
            toSector, piece.Owner, piece.pieceType, piece.possibleMoves);
        newState[toSector] = newPiece;
        for(int i=0; i<25;i++)
        {
            if(newState[i]!=null)
                newState[i].UpdatePossibleMoves(newState);
        }
        return newState;
    }
    public static DejarikChessDuD[] FakeCounterKill(DejarikChessDuD[] state, int sector)
    {
        DejarikChessDuD[] newState = Utilities.Clone(state);
        newState[sector] = null;
        for (int i = 0; i < 25; i++)
        {
            if (newState[i] != null)
                newState[i].UpdatePossibleMoves(newState);
        }
        return newState;
    }
    public static bool isAdiacentFree(int sector, DejarikChessDuD[] state)
    {
        bool result = false;
        int[] adiacents = BoardManager.AdiacentSectors(sector);
        foreach (int i in adiacents)
        {
            if (state[i] == null)
                result = true;
        }
        return result;
    }
    public static float Max(float a,float b)
    {
        float max = a;
        if (b > max)
            max = b;
        return max;
    }
    public static float Min(float a, float b)
    {
        float min = a;
        if (b < min)
            min = b;
        return min;
    }
}
