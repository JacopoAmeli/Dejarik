using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerControllerVSAI : MonoBehaviour
{
    //Radius of circles
    public float Outer_Radius = 4.95f;
    public float Inner_Radius = 3.1f;
    public float Center_Radius = 1.29f;
    //Distance of tile centers from board center
    private float Inner_Tile_Center_Radius = 2.46f;
    private float Outer_Tile_Center_Radius = 4.08f;
    // theta scale= 1/Number of points, also used for delta angles
    [Range(1,4)]
    public int Player_Turn = 1;

    private HighlighterScript highlighter;
    int section = -1;
    private HighlightMoves highlightAllMoves;
    private BoardManager board;
    private DejarikChessPiece selectedChessPiece;
    private UIChessOverlay chessOverlay;

    // Use this for initialization
    void Start()
    {
        highlighter = GetComponent<HighlighterScript>();
        highlighter.Disable();
        board = FindObjectOfType<BoardManager>();
        selectedChessPiece = null;
        highlightAllMoves = FindObjectOfType<HighlightMoves>();
        chessOverlay = GetComponent<UIChessOverlay>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHighlight();
        if (board.initialized)
        {
            for (int i = 0; i < 25; i++)
            {
                if (board.getPieceInSector(i) != null)
                {
                    board.getPieceInSector(i).UpdateCircle();
                }
            }
        }
        if (board.gameEnded|| (section != -1 && board.getPieceInSector(section) != null))
        {
            if (board.gameEnded)
            {
                chessOverlay.ShowEndOverlay(board.playerWon);
            }
            else
            {
                if (!chessOverlay.isActive)
                    chessOverlay.ShowChessOverlay(board.getPieceInSector(section).Name);
                else if (!chessOverlay.showing.Equals(board.getPieceInSector(section).Name))
                {
                    chessOverlay.HideChessOverlay();
                    chessOverlay.ShowChessOverlay(board.getPieceInSector(section).Name);
                }
            }
        }
        else
        {
            chessOverlay.HideChessOverlay();
        }
        chessOverlay.UpdateTurn(board.currentTurn);
        if (board.currentTurn != this.Player_Turn)
            highlightAllMoves.HideHighlights();
        else if(board.waitingForPush)
        {
            highlightAllMoves.HideHighlights();
            bool[] possiblePush = new bool[25];
            for (int i = 0; i < 25; i++)
            {
                possiblePush[i] = (BoardManager.AreAdiacent(i, board.pushed.CurrentSector)
                    && board.getPieceInSector(i) == null); 
            }
            highlightAllMoves.HighlightAllowedMoves(possiblePush);
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (section != -1 && board.currentTurn==this.Player_Turn)
            {
                if (selectedChessPiece == null)
                {
                    //push chesspiece
                    if (board.waitingForPush)
                        PushChessPiece(section);
                    //select chesspiece
                    else
                        SelectChessPiece(section);
                }
                else if (board.getPieceInSector(section) == null)
                {
                    //move chesspiece
                    MoveChessPiece(section);
                }
                else if (board.getPieceInSector(section).Owner == this.Player_Turn)
                {
                    SelectChessPiece(section);
                }
                else
                {
                    AttackChessPiece(section);
                }
            }
            else
            {
                selectedChessPiece = null;
                highlightAllMoves.HideHighlights();
            }
        }
    }
    private void PushChessPiece(int sector)
    {
        
        board.PushChessPiece(sector);
        if (!board.waitingForPush)
            highlightAllMoves.HideHighlights();
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
                highlightAllMoves.HideHighlights();
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
                highlightAllMoves.HighlightAllowedMoves(possiblePush);
                selectedChessPiece = null;
            }
            else if (result == BoardManager.ATTACK_RESULT.Counterpush)
            {
                Debug.Log("Counterpush");
                highlightAllMoves.HideHighlights();
                selectedChessPiece = null;
            }
            else if (result == BoardManager.ATTACK_RESULT.Counterkill)
            {
                Debug.Log("Counterkill");
                highlightAllMoves.HideHighlights();
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
        highlightAllMoves.HighlightAllowedMoves(selectedChessPiece.AllPossibleMoves());
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
        highlightAllMoves.HideHighlights();
    }
    private Vector3 GetTileCenter(int section)
    {
        Vector3 pos = gameObject.transform.position;
        pos.y = pos.y + 0.1f;
        if (section == 0)
            return pos;
        if ((section % 2 == 1))
        {
            pos.x = gameObject.transform.position.x + ((Mathf.Cos(Mathf.PI * section / 12)) * Inner_Tile_Center_Radius);
            pos.z = gameObject.transform.position.z + ((Mathf.Sin(Mathf.PI * section / 12)) * Inner_Tile_Center_Radius);
        }
        else
        {
            pos.x = gameObject.transform.position.x + ((Mathf.Cos(Mathf.PI * (section - 1) / 12)) * Outer_Tile_Center_Radius);
            pos.z = gameObject.transform.position.z + ((Mathf.Sin(Mathf.PI * (section - 1) / 12)) * Outer_Tile_Center_Radius);
        }
        return pos;
    }
    private void UpdateHighlight()
    {
        if (!Camera.main)
            return;
        RaycastHit hit;
        float SelectionX;
        float SelectionZ;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessBoard")))
        {
            SelectionX = hit.point.x;
            SelectionZ = hit.point.z;
            int newSection;
            newSection = PointToSection(SelectionX, SelectionZ);
            if (section == newSection)
                return;
            section = newSection;
            if (section < 0)
            {
                highlighter.Disable();
                return;
            }
            highlighter.HighlightSection(section);
        }

    }
    //returns board section from point
    private int PointToSection(float x, float z)
    {
        float radius = Mathf.Sqrt(x * x + z * z);
        if (radius > Outer_Radius)
            return -1;
        if (radius < Center_Radius)
            return 0;
        float angle = Mathf.Rad2Deg * Mathf.Atan(z / x);
        // Quadrant 1: angle positive, cos positive.
        // Quadrant 3: angle positive, cos negative.
        // Quadrant 2: angle negative, cos negative.
        // quadrant 4: angle negative, cos positive. 
        if (radius < Inner_Radius)
        { //INNER CIRCLE
            if (angle > 0)
            {
                if (x > 0)
                {
                    if (angle < 30)
                        return 1;
                    else if (angle < 60)
                        return 3;
                    else if (angle < 90)
                        return 5;
                }
                else
                {
                    if (angle < 30)
                        return 13;
                    else if (angle < 60)
                        return 15;
                    else if (angle < 90)
                        return 17;
                }
            }
            else
            {
                if (x < 0)
                {
                    if (angle < -60)
                        return 7;
                    else if (angle < -30)
                        return 9;
                    else
                        return 11;
                }
                else
                {
                    if (angle < -60)
                        return 19;
                    else if (angle < -30)
                        return 21;
                    else
                        return 23;
                }
            }
        }
        else
        { //OUTER CIRCLE
            if (angle > 0)
            {
                if (x > 0)
                {
                    if (angle < 30)
                        return 2;
                    else if (angle < 60)
                        return 4;
                    else if (angle < 90)
                        return 6;
                }
                else
                {
                    if (angle < 30)
                        return 14;
                    else if (angle < 60)
                        return 16;
                    else if (angle < 90)
                        return 18;
                }
            }
            else
            {
                if (x < 0)
                {
                    if (angle < -60)
                        return 8;
                    else if (angle < -30)
                        return 10;
                    else
                        return 12;
                }
                else
                {
                    if (angle < -60)
                        return 20;
                    else if (angle < -30)
                        return 22;
                    else
                        return 24;
                }
            }
        }
        return -1;
    }

}
