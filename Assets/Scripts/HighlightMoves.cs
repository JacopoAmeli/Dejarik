using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightMoves : MonoBehaviour {

    public GameObject highlightPrefab;
    private List<HighlighterScript> highlights;
    private BoardManager board;
    public static HighlightMoves Instance { get; set; }
    //private Gradient blueGradient;
    private Gradient redGradient;
    private Gradient greenGradient;
    // Use this for initialization
    void Start () {
        Instance = this;
        board = FindObjectOfType<BoardManager>();
        highlights = new List<HighlighterScript>();
        Vector3 pos = Vector3.zero;
        pos.y = 0.1f;
        for (int i=0;i<25;i++)
        {
            GameObject go = Instantiate(highlightPrefab);
            go.transform.position = pos;
            HighlighterScript hs = go.GetComponent<HighlighterScript>();
            hs.Disable();
            highlights.Add(hs);
        }
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
        //blueGradient = g;
        g = new Gradient();
        gck = new GradientColorKey[2];
        gck[0].color = Color.red;
        gck[0].time = 0.0F;
        gck[1].color = Color.red;
        gck[1].time = 1.0F;
        g.SetKeys(gck, gak);
        redGradient = g;
        g = new Gradient();
        gck = new GradientColorKey[2];
        gck[0].color = Color.green;
        gck[0].time = 0.0F;
        gck[1].color = Color.green;
        gck[1].time = 1.0F;
        g.SetKeys(gck, gak);
        greenGradient = g;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void HighlightAllowedMoves(bool[] moves)
    {
        HideHighlights();
        for (int i = 0; i<25; i++)
        {
            if ( moves[i] )
            {
                HighlighterScript hs = highlights[i];
                if (board.getPieceInSector(i)==null)
                    hs.ChangeColor(greenGradient);
                else
                    hs.ChangeColor(redGradient);
                hs.HighlightSection(i);
                hs.Enable();
            }
        }
    }
    public void HideHighlights()
    {
        foreach (HighlighterScript go in highlights)
            go.Disable() ;
    }
}
