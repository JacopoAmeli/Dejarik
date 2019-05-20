using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChessOverlay : MonoBehaviour {

    UIChessToggle[] allToggles;
    public bool isActive=false;
    public string showing = "";
    void Start()
    {
        allToggles = FindObjectsOfType<UIChessToggle>();
    }
    public void ShowChessOverlay(string pieceName)
    {
        foreach(UIChessToggle i in allToggles)
        {
            if (i.ChessName.Equals(pieceName+"Chess"))
            {
                i.Enable();
                isActive = true;
                showing = pieceName;
                return;
            }
        }
        
    }
    public void ShowEndOverlay(int playerWon)
    {
        string player = "One";
        if (playerWon == 2)
            player = "Two";
        foreach(UIChessToggle i in allToggles)
        {
            if (i.ChessName.Equals("Player" + player + "Wins"))
            {
                i.Enable();
                isActive = true;
                showing = "Player" + player+"Wins";
                return;
            }
        }
    }
    public void UpdateTurn(int turn)
    {
        if(turn==1)
        {
            foreach (UIChessToggle i in allToggles)
            {
                if (i.ChessName.Equals("Turn1"))
                {
                    i.Enable();
                    isActive = true;
                    showing = "Turn1";
                    return;
                }
                if (i.ChessName.Equals("Turn2"))
                    i.Disable();
            }
        }
        else
        {

            foreach (UIChessToggle i in allToggles)
            {
                if (i.ChessName.Equals("Turn2"))
                {
                    i.Enable();
                    isActive = true;
                    showing = "Turn2";
                    return;
                }
                if (i.ChessName.Equals("Turn1"))
                    i.Disable();
            }
        }
    }
	public void HideChessOverlay()
    {
        foreach (UIChessToggle i in allToggles)
                i.Disable();
        isActive = false;
        return;
    }
}
