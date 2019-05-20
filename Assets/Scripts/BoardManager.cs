using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BoardManager : MonoBehaviour {
    //Chesspiece prefabs
    public List<GameObject> DejarikOffensivePiecePrefabs;
    public List<GameObject> DejarikDefensivePiecePrefabs;
    public List<GameObject> DejarikBalancedPiecePrefabs;
    public List<GameObject> DejarikWeakPiecePrefabs;
    //active pieces
    private List<GameObject>[] activePieces;
    //this is the "actual" board
    private DejarikChessPiece[] gameChessPieces;
  
    //starting positions
    private List<int>[] startPos;
    //Distance of tile centers from board center
    private float Inner_Tile_Center_Radius = 2.46f;
    private float Outer_Tile_Center_Radius = 4.08f;
    private Vector3[] killedPos;
    public int currentTurn;
    public int originalTurn;
    public bool waitingForPush;
    public DejarikChessPiece pushed;
    public int actionsLeft;
    public int playerNum;
    public bool initialized=false;
    public bool gameEnded = false;
    public int playerWon = -1;
    public enum ATTACK_RESULT {Kill,Push,Counterpush,Counterkill}
   
    
    // Use this for initialization
    void Start()
    {
        //TODO how to change player num (menu)
        playerNum = 2; //max=4
        actionsLeft = 2;
        waitingForPush = false;
        pushed = null;
        startPos = new List<int>[4];
        killedPos = new Vector3[4];
        if (playerNum==2)
        {
            startPos[0] = new List<int> { 16, 18, 20, 22 };
            startPos[1] = new List<int> { 10, 8, 6, 4 };
            killedPos[0] = new Vector3(6f, 0, -4f);
            killedPos[1] = new Vector3(6f, 0, 4f);
        }
        else if (playerNum>=4)
        {
            startPos[0] = new List<int> { 17, 18, 19, 20 };
            startPos[1] = new List<int> { 5, 6, 7, 8 };
            startPos[2] = new List<int> { 11, 12, 13, 14 };
            startPos[3] = new List<int> { 1, 2, 23, 24 };
            killedPos[0] = new Vector3(-3f, 0, -6f);
            killedPos[1] = new Vector3(3f, 0, 6f);
            killedPos[2] = new Vector3(6f, 0, -3f);
            killedPos[3] = new Vector3(-6f, 0, 3f);
        }

        gameChessPieces = new DejarikChessPiece[25];
        SpawnAllChessPieces();
        UpdateInitialPossibleMoves();
        //I choose the first turn randomly
        currentTurn = Mathf.RoundToInt(Random.value* (playerNum-1) + 1f);
        initialized = true;
    }
    private void KillCount(int player)
    {
        if(playerNum==2)
        {
            if (player == 1)
            {
                killedPos[0].z++;            }
            else
                killedPos[1].z--;
        }
        else if(playerNum==4)
        {
            //TODO
        }
    }
    // Update is called once per frame
    void Update()
    {
        //TODO check conditions for turn change?
    }
    public void UpdateInitialPossibleMoves()
    {
        foreach (DejarikChessPiece i in gameChessPieces)
        {
            if(i!=null)
                i.UpdatePossibleMoves(gameChessPieces);
        }
    }
    public void UpdateAllPossibleMoves()
    {
        foreach (DejarikChessPiece i in gameChessPieces)
        {
            if (i != null)
                i.UpdatePossibleMoves();
        }
    }
    public DejarikChessPiece getPieceInSector(int sector)
    {
        return gameChessPieces[sector];
    }
    public void MoveChessPiece(int fromSector, int toSector)
    {
        DejarikChessPiece selectedChessPiece=gameChessPieces[fromSector];
        if (selectedChessPiece == null)
            return;
        else
        {
            //move mode
            gameChessPieces[fromSector] = null;
            selectedChessPiece.transform.position = GetTileCenter(toSector);
            selectedChessPiece.CurrentSector = toSector;
            gameChessPieces[toSector] = selectedChessPiece;
        }
        TurnCheck();

    }
    private void TurnCheck()
    {
        actionsLeft--;
        if (actionsLeft == 0)
        {
            actionsLeft = 2;
            currentTurn = (currentTurn % playerNum) + 1;
        }
        UpdateAllPossibleMoves();
    }
    private void EndCheck()
    {
        if(playerNum==2)
        {
            int piecesPlayerOneLeft = 0;
            DejarikChessPiece playerOneChamp = null ;
            DejarikChessPiece playerTwoChamp= null;
            int piecesPlayerTwoLeft = 0;
            for (int i = 0; i < 25; i++)
            {
                if (gameChessPieces[i]!= null)
                {
                    if (gameChessPieces[i].Owner == 1)
                    {
                        piecesPlayerOneLeft++;
                        playerOneChamp = gameChessPieces[i];
                    }
                    else
                    {
                        piecesPlayerTwoLeft++;
                        playerTwoChamp = gameChessPieces[i];
                    }
                }
            }
            if(piecesPlayerTwoLeft==1 && piecesPlayerOneLeft==1)
            {
                //Battle to the death
                bool stillFighting = true;
                while(stillFighting)
                {
                    int totalAttack = 0;
                    int totalDefense = 0;
                    int attack = 0;
                    int defense = 0;
                    if (currentTurn == 1)
                    {
                        attack = playerOneChamp.Attack;
                        defense = playerTwoChamp.Defense;
                    }
                    else
                    {
                        attack = playerTwoChamp.Attack;
                        defense = playerOneChamp.Defense;
                    }
                    //roll 1d6 per stat
                    for (int i = 0; i < attack; i++)
                        totalAttack += Mathf.RoundToInt(Random.value * 5 + 1f);
                    for (int i = 0; i < defense; i++)
                        totalDefense += Mathf.RoundToInt(Random.value * 5 + 1f);
                    int result = totalAttack - totalDefense;
                    if (result>=7)
                    {
                        //player of current turn wins
                        Debug.Log("Player " + currentTurn + " wins!");
                        playerWon = currentTurn;
                        stillFighting = false;
                        gameEnded = true;
                    }
                    else if (result<=-7)
                    {
                        //other player wins
                        Debug.Log("Player " + ((currentTurn % playerNum) + 1) + " wins!");
                        playerWon = ((currentTurn % playerNum) + 1);
                        stillFighting = false;
                        gameEnded = true;
                    }
                    currentTurn = (currentTurn % playerNum) + 1;
                }
            }
            else if(piecesPlayerOneLeft==0)
            {
                //player two wins
                Debug.Log("Player Two wins!");
                playerWon = 2;
                gameEnded = true;
            }
            else if(piecesPlayerTwoLeft==0)
            {
                //player one wins
                Debug.Log("Player One wins!");
                playerWon = 1;
                gameEnded = true;
            }
        }
        
    }
    public void PushChessPiece( int toSector)
    {
        if ( !AreAdiacent(pushed.CurrentSector,toSector)|| gameChessPieces[toSector]!=null)
            return;
        else
        { //move mode
            gameChessPieces[pushed.CurrentSector] = null;
            pushed.transform.position = GetTileCenter(toSector);
            pushed.CurrentSector = toSector;
            gameChessPieces[toSector] = pushed;
            currentTurn = originalTurn;
            pushed = null;
            waitingForPush = false;
            UpdateAllPossibleMoves();
        }

    }
    public bool isAdiacentFree(int sector)
    {
        bool result = false;
        int[] adiacents = AdiacentSectors(sector);
        foreach ( int i in adiacents)
        {
            if (gameChessPieces[i] == null)
                result = true;
        }
        return result;
    }
    public ATTACK_RESULT AttackChessPiece(int fromSector,int toSector)
    {
        //check conditions for attack?
        //attack mode
        //get stats ready
        int attack = gameChessPieces[fromSector].Attack;
        int totalAttack = 0;
        int defense = gameChessPieces[toSector].Defense;
        int totalDefense = 0;
        //roll 1d6 per stat
        for (int i = 0; i < attack; i++)
            totalAttack += Mathf.RoundToInt(Random.value * 5 + 1f);
        for (int i = 0; i < defense; i++)
            totalDefense += Mathf.RoundToInt(Random.value * 5 + 1f);
        int result = totalAttack - totalDefense;
        if (result >= 7)
        {
            //attack beats defense by at least 7, attack kills defense and moves in
            DejarikChessPiece killedPiece = gameChessPieces[toSector];
            killedPiece.transform.position = killedPos[killedPiece.Owner - 1];
            killedPiece.Dead();
            KillCount(killedPiece.Owner);
            DejarikChessPiece winnerPiece = gameChessPieces[fromSector];
            gameChessPieces[fromSector] = null;
            winnerPiece.CurrentSector = toSector;
            winnerPiece.transform.position = GetTileCenter(toSector);
            gameChessPieces[toSector] = winnerPiece;
            TurnCheck();
            EndCheck();
            return ATTACK_RESULT.Kill;
        }
        else if (result > 0)
        {
            //attack beats defense by equal or less than 6, push
            TurnCheck();
            if (isAdiacentFree(toSector))
            {
                waitingForPush = true;
                originalTurn = currentTurn;
                currentTurn = gameChessPieces[fromSector].Owner;
                pushed = gameChessPieces[toSector];
            }
            return ATTACK_RESULT.Push;
        }
        else if (result <= 0 && result > -7)
        {
            //defense counter pushes attack
            TurnCheck();
            if (isAdiacentFree(fromSector))
            {
                waitingForPush = true;
                originalTurn = currentTurn;
                currentTurn = gameChessPieces[toSector].Owner;
                pushed = gameChessPieces[fromSector];
            }
            return ATTACK_RESULT.Counterpush;
        }
        else
        {
            //defense kills attack
            DejarikChessPiece killedPiece = gameChessPieces[fromSector];
            killedPiece.Dead();
            killedPiece.transform.position = killedPos[killedPiece.Owner - 1];
            KillCount(killedPiece.Owner);
            gameChessPieces[fromSector] = null;
            TurnCheck();
            EndCheck();
            return ATTACK_RESULT.Counterkill;
        }
    }
    
    private void SpawnChessPiece(int index, List<GameObject> pieceList, int sector ,int player)
    {
        Vector3 pos = GetTileCenter(sector);
        Quaternion rotation = Quaternion.identity;
        rotation.y = 180;
        GameObject go = Instantiate(pieceList[index], pos, rotation) as GameObject;
        pieceList.RemoveAt(index);
        go.transform.SetParent(transform);
        DejarikChessPiece pieceScript = go.GetComponent<DejarikChessPiece>();
        pieceScript.CurrentSector = sector;
        pieceScript.Owner = player;
        activePieces[player - 1].Add(go);
        gameChessPieces[sector] = pieceScript;
        pieceScript.board = this;
    }
    private void SpawnAllChessPieces()
    {
        activePieces = new List<GameObject>[4]; 
        activePieces[0] = new List<GameObject>();
        activePieces[1]= new List<GameObject>();
        activePieces[2] = new List<GameObject>();
        activePieces[3] = new List<GameObject>();
        for(int i=0;i<playerNum;i++)
        {
            //Spawn Offensive Piece
            int index = Mathf.RoundToInt(Random.value * (DejarikOffensivePiecePrefabs.Count - 1));
            int sectorIndex = Mathf.RoundToInt(Random.value * (startPos[i].Count - 1));
            int sector = startPos[i][sectorIndex];
            startPos[i].RemoveAt(sectorIndex);
            SpawnChessPiece(index, DejarikOffensivePiecePrefabs, sector, i + 1);
            //Spawn Defensive Piece
            index = Mathf.RoundToInt(Random.value * (DejarikDefensivePiecePrefabs.Count - 1));
            sectorIndex = Mathf.RoundToInt(Random.value * (startPos[i].Count - 1));
            sector = startPos[i][sectorIndex];
            startPos[i].RemoveAt(sectorIndex);
            SpawnChessPiece(index, DejarikDefensivePiecePrefabs, sector, i + 1);
            //Spawn Balanced Piece
            index = Mathf.RoundToInt(Random.value * (DejarikBalancedPiecePrefabs.Count - 1));
            sectorIndex = Mathf.RoundToInt(Random.value * (startPos[i].Count - 1));
            sector = startPos[i][sectorIndex];
            startPos[i].RemoveAt(sectorIndex);
            SpawnChessPiece(index, DejarikBalancedPiecePrefabs, sector, i + 1);
            //Spawn Weak Piece
            index = Mathf.RoundToInt(Random.value * (DejarikWeakPiecePrefabs.Count - 1));
            sectorIndex = Mathf.RoundToInt(Random.value * (startPos[i].Count - 1));
            sector = startPos[i][sectorIndex];
            startPos[i].RemoveAt(sectorIndex);
            SpawnChessPiece(index, DejarikWeakPiecePrefabs, sector, i + 1);
        }
       
    }
    private Vector3 GetTileCenter(int section)
    {
        Vector3 pos = gameObject.transform.position;
        pos.y = pos.y + 0.1f;
        if (section == 0)
            return pos;
        if ((section%2==1))
        {
            pos.x = gameObject.transform.position.x + ((Mathf.Cos(Mathf.PI * section / 12) )* Inner_Tile_Center_Radius);
            pos.z = gameObject.transform.position.z + ((Mathf.Sin(Mathf.PI * section / 12) )* Inner_Tile_Center_Radius);
        }
        else
        {
            pos.x = gameObject.transform.position.x +(( Mathf.Cos(Mathf.PI *(section-1) / 12))* Outer_Tile_Center_Radius);
            pos.z = gameObject.transform.position.z +(( Mathf.Sin(Mathf.PI *(section-1) / 12)) * Outer_Tile_Center_Radius);
        }
        return pos;
    }
    public static bool AreAdiacent(int sector1,int sector2)
    {
        if (sector1 == -1 || sector2 == -1)
            return false;
        //center
        if(sector1==0)
        {
            return ((sector2 % 2) == 1);
        }
        else if(sector2==0)
        {
            return ((sector1 % 2) == 1);
        }
        //outer
        else if (sector1%2==0)
        {
            //1. if the sector is still outer 2.adiacent inner 3.other outer
            if (((sector1 + 2) % 24 == sector2 % 24) || (sector1 - 1 == sector2) || ((sector2 +2) % 24 == sector1 % 24))
            {
                return true;
            }
            else
                return false;
        }
        //inner
        else if (sector1%2==1)
        {
            //1. if the sector is still inner 2. adiacent outer 3. other inner
            if (((sector1 + 2) % 24 == sector2 % 24) || (sector1 + 1 == sector2) || ((sector2 +2) % 24 == sector1 % 24))
            {
                return true;
            }
            else
                return false;
        }
        return false;
    }
    public static int[] AdiacentSectors(int sector)
    {
        int[] result=null;
        if(sector==0)
        {
            result = new int[12];
            for (int i = 0; i < 12; i++)
                result[i] = i * 2 + 1;
        }
        else if(sector%2==0)
        {
            result = new int[3];
            if (sector == 2)
                result[0] = 24;
            else
                result[0] = sector - 2;
            result[1] = sector - 1;
            if (sector == 22)
                result[2] = 24;
            else
                result[2] = (sector + 2)%24;
        }
        else
        {
            result = new int[4];
            if (sector == 1)
                result[0] = 23;
            else
                result[0] = sector - 2;
            result[1] = 0;
            result[2] = (sector + 2 )% 24;
            result[3] = sector + 1;
        }
        return result;
    }
}