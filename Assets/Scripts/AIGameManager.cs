using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class AIGameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    private BoardManager boardManager;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier for Player1. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefabP1;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier for Player2. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefabP2;

    [SerializeField]
    [Tooltip("The Prefab for the Target. Denotes which spaces a piece can target with attacks.")]
    private GameObject SpacePrefab;

    [SerializeField]
    [Tooltip("The Prefab for the Space. Denotes which spaces a piece can move to.")]
    private GameObject TargetPrefab;

    [HideInInspector]
<<<<<<< Updated upstream
    public GameState state;
=======
    public PlayerTurn playerTurn;
    [HideInInspector]
    public GameState? gameState;
>>>>>>> Stashed changes

    private Vector2Int selected;

    //During Select, array value indicates the previous space the piece would move from for if we implement a moving animation
    //During Attack, array value simply has 0 for invalid and 1 for valid
    private int[] pboard = new int[100];

    //Array to track targets and the number
    //Might be a better way to do this
    private GameObject[] targets = new GameObject[100];
    private int targetnum = 0;


    void Awake()
    {
<<<<<<< Updated upstream
        state = GameState.P1Turn;
=======
        playerTurn = PlayerTurn.Player1;
        gameState = null;
>>>>>>> Stashed changes

        //Marks avaliable spaces as not selected
        for (int i = 0; i < 100; i++)
        {
            pboard[i] = -1;
        }
    }

    void Start() {
        CreatePiece(new Vector2Int(0, 0), PieceType.Player1);
        CreatePiece(new Vector2Int(1, 0), PieceType.Player1);
        CreatePiece(new Vector2Int(2, 0), PieceType.Player1);
        CreatePiece(new Vector2Int(3, 0), PieceType.Player1);

        CreatePiece(new Vector2Int(9, 9), PieceType.Player2);
        CreatePiece(new Vector2Int(8, 9), PieceType.Player2);
        CreatePiece(new Vector2Int(7, 9), PieceType.Player2);
        CreatePiece(new Vector2Int(6, 9), PieceType.Player2);
    }

    void Update()
    {
<<<<<<< Updated upstream
        if(state == GameState.P2Turn)
        {
            Debug.Log("AI Turn");

            state = GameState.P2Selected;
            AITurn();
            state = state.GetCurrentTurn().GetSwitchPlayersTurns();
=======
        if(playerTurn == PlayerTurn.Player2 && gameState == null)
        {
            Debug.Log("AI Turn");

            gameState = GameState.Selected;
            AITurn();
            gameState = null;
            playerTurn.SwitchPlayers();
>>>>>>> Stashed changes
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //reads mouse position and converts to grid position
            var nPiecePos = MousePositionToPiecePosition();
            if (!nPiecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var pieceGridPos = nPiecePos.Value;

<<<<<<< Updated upstream
            if (state == GameState.P1Turn)
=======
            if (gameState == null)
>>>>>>> Stashed changes
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    // Disable this until currency is implemented
                    Debug.Log("Creating a piece is currently disabled");

                    // var player = state == GameState.P1Turn ? PieceType.Player1 : PieceType.Player2;
                    // CreatePiece(pieceGridPos, player);
                    // state = state.GetSwitchPlayersTurns();
                }
                else
                {
                    //If selecting one of your pieces, mark as selected and calculate avaliable spaces
                    if (ValidPieceType(piece))
                    {
                        selected = pieceGridPos;
<<<<<<< Updated upstream
                        state = state.GetNextState();
=======
                        gameState = GameState.Selected;
>>>>>>> Stashed changes
                        GetSpaces(pieceGridPos.x + (pieceGridPos.y * 10), piece.Movement);
                    }
                    else
                    {
                        Debug.Log("This is not the current player's piece");
                    }
                }
            }
<<<<<<< Updated upstream
            else if (state == GameState.P1Selected)
=======
            else if (gameState == GameState.Selected)
>>>>>>> Stashed changes
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                var isNotSpace = piece != null && piece.Type != PieceType.Space;
                var sameSpace = pieceGridPos == selected;

                if ((piece == null || isNotSpace) && !sameSpace)
                {
                    ClearSpaces();
<<<<<<< Updated upstream
                    state = state.GetCurrentTurn();
=======
                    gameState = null;
                    playerTurn.SwitchPlayers();
>>>>>>> Stashed changes
                }
                //If an avaliable space or current space is selected
                else
                {
                    if (!sameSpace)
                    {
                        boardManager.boardState.MovePiece(selected, pieceGridPos);
                        piece = boardManager.boardState.GetPiece(pieceGridPos);
                        selected = pieceGridPos;
                    }
                    ClearSpaces();
<<<<<<< Updated upstream
                    state = state.GetNextState();
=======
                    gameState = GameState.Attack;
>>>>>>> Stashed changes
                    GetAttacks(pieceGridPos.x + (pieceGridPos.y * 10), piece.Range);

                    if (targetnum == 0)
                    {
                        ClearSpaces();
<<<<<<< Updated upstream
                        state = state.GetCurrentTurn().GetSwitchPlayersTurns();
                    }
                }
            }
            else if (state == GameState.P1Attack)
=======
                        gameState = null;
                        playerTurn.SwitchPlayers();
                    }
                }
            }
            else if (gameState == GameState.Attack)
>>>>>>> Stashed changes
            {
                int gridNum = pieceGridPos.x + (pieceGridPos.y * 10);
                bool isTargeted = false;
                for(int i = 0; i < targetnum; i++)
                {
                    Vector2Int targetGridPos = (Vector2Int)boardManager.WorldPosToGridPos((Vector2)targets[i].transform.position);
                    int targetNum = targetGridPos.x + (targetGridPos.y * 10);
                    if(gridNum == targetNum)
                    {
                        isTargeted = true;
                        break;
                    }
                }

                //If a targeted space is selected, attack piece
                if (isTargeted)
                {
                    boardManager.boardState.AttackPiece(selected, pieceGridPos);
                    if (boardManager.DidPlayerWin(PieceType.Player1))
                    {
                        Debug.Log($"Player {PieceType.Player1} won!");
                        SceneManager.LoadScene("P1WinScene");
                    }
                }
                
                ClearSpaces();
<<<<<<< Updated upstream
                state = state.GetNextState().GetSwitchPlayersTurns();
=======
                gameState = null;
                playerTurn.SwitchPlayers();
>>>>>>> Stashed changes
            }
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var nPiecePos = MousePositionToPiecePosition();
            if (!nPiecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var pieceGridPos = nPiecePos.Value;

<<<<<<< Updated upstream
            if (state == GameState.P1Turn)
=======
            if (gameState == null)
>>>>>>> Stashed changes
            {
                Debug.Log($"Deleting piece at {pieceGridPos}");

                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                
                if (piece == null) {
                    Debug.Log("There is no piece here");
                }
                else if (ValidPieceType(piece))
                {
                    boardManager.boardState.SetPiece(null, pieceGridPos);
<<<<<<< Updated upstream
                    state = state.GetSwitchPlayersTurns();
=======
                    gameState = null;
                    playerTurn.SwitchPlayers();
>>>>>>> Stashed changes
                }
                else
                {
                    Debug.Log("This is not the current player's piece");
                }
            }
<<<<<<< Updated upstream
            else if (state == GameState.P1Selected)
            {
                ClearSpaces();
                state = state.GetCurrentTurn();
            }
            else if (state == GameState.P1Attack)
            {
                ClearSpaces();
                state = state.GetCurrentTurn().GetSwitchPlayersTurns();
=======
            else if (gameState == GameState.Selected)
            {
                ClearSpaces();
                gameState = null;
            }
            else if (gameState == GameState.Attack)
            {
                ClearSpaces();
                gameState = null;
                playerTurn.SwitchPlayers();
>>>>>>> Stashed changes
            }
        }
    }

    /// <summary>
    /// Checks if the piece is the correct type for the current player
    /// </summary>
    private bool ValidPieceType(IPiece piece)
    {
<<<<<<< Updated upstream
        return (state == GameState.P1Turn && piece.Type == PieceType.Player1)
            || (state == GameState.P2Turn && piece.Type == PieceType.Player2);
=======
        return (playerTurn == PlayerTurn.Player1 && piece.Type == PieceType.Player1)
            || (playerTurn == PlayerTurn.Player2 && piece.Type == PieceType.Player2);
>>>>>>> Stashed changes
    }

    /// <summary>
    /// Creates a piece at the given position.
    /// <br/>
    /// Currently only creates a Soldier.
    /// </summary>
    private void CreatePiece(Vector2Int pieceGridPos, PieceType player)
    {
        Debug.Log($"Creating piece on grid position: {pieceGridPos}");
        
        var pieceGlobalPos = boardManager.GridPosToWorldPos(pieceGridPos);

        if(player == PieceType.Player1)
        {
            var soldierObj = Instantiate(SoldierPrefabP1, pieceGlobalPos, Quaternion.identity);
            boardManager.boardState.SetPiece(new Soldier(soldierObj, player), pieceGridPos);
        }
        else
        {
            var soldierObj = Instantiate(SoldierPrefabP2, pieceGlobalPos, Quaternion.identity);
            boardManager.boardState.SetPiece(new Soldier(soldierObj, player), pieceGridPos);
        }
    }

    /// <summary>
    /// Converts the mouse's position to a piece's position.
    /// </summary>
    /// <returns> The piece's grid position and global position. </returns>
    public Vector2Int? MousePositionToPiecePosition()
    {
        var mouseScreenPos = Mouse.current.position.ReadValue();
        var mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);

        var gridPos = boardManager.WorldPosToGridPos(mouseWorldPos);

        if (!gridPos.HasValue)
        {
            return null;
        }

        return gridPos.Value;
    }

    /// <summary>
    /// Gets all avaliable spaces that a piece can move to and puts a Space piece on them
    /// </summary>
    public void GetSpaces(int cspace, int move)
    {
        //Set starting space to 0 to avoid backtracking
        if (pboard[cspace] < 0)
        {
            pboard[cspace] = 0;
        }

        //Set Space Piece
        if (boardManager.boardState.GetPiece(new Vector2Int(cspace % 10, cspace / 10)) == null)
        {
            var spaceObj = Instantiate(SpacePrefab, boardManager.GridPosToWorldPos(new Vector2Int(cspace % 10, cspace / 10)), Quaternion.identity);
            boardManager.boardState.SetPiece(new Space(spaceObj), new Vector2Int(cspace % 10, cspace / 10));
        }

        if(move == 0)
        {
            return;
        }

        //Up
        if(cspace > 9 && pboard[cspace - 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 10) % 10, (cspace - 10) / 10));
            if (npiece == null || npiece.Type == PieceType.Player1)
            {
                pboard[cspace - 10] = cspace;
                GetSpaces(cspace - 10, move - 1);
            }
        }

        //Down
        if (cspace < 90 && pboard[cspace + 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 10) % 10, (cspace + 10) / 10));
            if (npiece == null || npiece.Type == PieceType.Player1)
            {
                pboard[cspace + 10] = cspace;
                GetSpaces(cspace + 10, move - 1);
            }
        }

        //Left
        if (cspace % 10 != 0 && pboard[cspace - 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 1) % 10, (cspace - 1) / 10));
            if (npiece == null || npiece.Type == PieceType.Player1)
            {
                pboard[cspace - 1] = cspace;
                GetSpaces(cspace - 1, move - 1);
            }
        }

        //Right
        if (cspace % 10 != 9 && pboard[cspace + 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 1) % 10, (cspace + 1) / 10));
            if (npiece == null || npiece.Type == PieceType.Player1)
            {
                pboard[cspace + 1] = cspace;
                GetSpaces(cspace + 1, move - 1);
            }
        }
    }

    public void GetAttacks(int cspace, int range)
    {
        IPiece piece = boardManager.boardState.GetPiece(new Vector2Int((cspace) % 10, (cspace) / 10));
        if (piece != null && piece.Type == PieceType.Player2) //Add Wall attack?
        {
            pboard[cspace] = 1;

            //Add Target Indicator
            var targetPos = (Vector3)boardManager.GridPosToWorldPos(new Vector2Int(cspace % 10, cspace / 10));
            targetPos.z = -1; // So that the target is in front of the piece
            var targetObj = Instantiate(TargetPrefab, targetPos, Quaternion.identity);
            targets[targetnum] = targetObj;
            targetnum++;
        }
        else
        {
            pboard[cspace] = 0;
        }

        if (range == 0)
        {
            return;
        }

        //Up
        if (cspace > 9 && pboard[cspace - 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 10) % 10, (cspace - 10) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace - 10, range - 1);
            }
        }

        //Down
        if (cspace < 90 && pboard[cspace + 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 10) % 10, (cspace + 10) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace + 10, range - 1);
            }
        }

        //Left
        if (cspace % 10 != 0 && pboard[cspace - 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 1) % 10, (cspace - 1) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace - 1, range - 1);
            }
        }

        //Right
        if (cspace % 10 != 9 && pboard[cspace + 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 1) % 10, (cspace + 1) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace + 1, range - 1);
            }
        }
    }

    /// <summary>
    /// Clears the board array and all Space pieces
    /// </summary>
    public void ClearSpaces()
    {
        for (int i = 0; i < 100; i++)
        {
            if(pboard[i] >= 0)
            {
                pboard[i] = -1;

<<<<<<< Updated upstream
                if (state.IsSelected())
=======
                if (gameState == GameState.Selected)
>>>>>>> Stashed changes
                {
                    IPiece piece = boardManager.boardState.GetPiece(new Vector2Int(i % 10, i / 10));
                    if (piece != null && piece.Type == PieceType.Space)
                    {
                        boardManager.boardState.SetPiece(null, new Vector2Int(i % 10, i / 10));
                    }
                }
            }
        }

        for (int i = 0; i < targetnum; i++)
        {
            Object.Destroy(targets[i]);
            targets[i] = null;
        }
        targetnum = 0;
    }

    public void AITurn()
    {
        int pcount = 0;
        IPiece[] pieces = new IPiece[100];
        int[] piecespaces = new int[100];

        int epcount = 0;
        int[] epiecespaces = new int[100];

        for (int i = 0; i < 100; i++)
        {
            IPiece piece = boardManager.boardState.GetPiece(new Vector2Int(i % 10, i / 10));
            if (piece != null && piece.Type == PieceType.Player2)
            {
<<<<<<< Updated upstream
                Debug.Log("Piece: " + pcount);
                Debug.Log("Space " + i);

=======
>>>>>>> Stashed changes
                pieces[pcount] = piece;
                piecespaces[pcount] = i;
                pcount++;
            }
            else if (piece != null && piece.Type == PieceType.Player1)
            {
<<<<<<< Updated upstream
                Debug.Log("EPiece: " + epcount);
                Debug.Log("Space " + i);
=======
>>>>>>> Stashed changes
                epiecespaces[epcount] = i;
                epcount++;
            }
        }

<<<<<<< Updated upstream
        Debug.Log("Pcount: " + pcount);
        Debug.Log("EPcount " + epcount);

=======
>>>>>>> Stashed changes
        bool[] piecemoved = new bool[pcount];
        for (int i = 0; i < pcount; i++)
        {
            piecemoved[i] = false;
        }

        bool[] piecedefeated = new bool[epcount];
        for (int i = 0; i < epcount; i++)
        {
            piecedefeated[i] = false;
        }

        int[][][] paths = new int[pcount][][];
        int[][] distances = new int[pcount][];

        for (int i = 0; i < pcount; i++)
        {
            paths[i] = new int[epcount][];
            distances[i] = new int[epcount];
        }

        for (int i = 0; i < pcount; i++)
        {
            for (int j = 0; j < epcount; j++)
            {
                paths[i][j] = GetPath(piecespaces[i], epiecespaces[j]);
                distances[i][j] = GetDistance(piecespaces[i], epiecespaces[j], paths[i][j]);
            }
        }

        while (piecemoved.Any(x => x == false))
        {
            int mindis = 100;
            int minpiece = -1;
            int minepiece = -1;

            for (int i = 0; i < pcount; i++)
            {
                if (!piecemoved[i])
                {
                    for (int j = 0; j < epcount; j++)
                    {
                        if (!piecedefeated[j])
                        {
                            if (distances[i][j] < mindis)
                            {
                                mindis = distances[i][j];
                                minpiece = i;
                                minepiece = j;
                            }
                        }
                    }
                }
            }

<<<<<<< Updated upstream
            Debug.Log("Minpiece: " + minpiece);
            Debug.Log("MinEpiece: " + minepiece);

            int move = pieces[minpiece].Movement;
            int distance = distances[minpiece][minepiece];

            Debug.Log("MoveDis: " + distance);

=======
            int move = pieces[minpiece].Movement;
            int distance = distances[minpiece][minepiece];

>>>>>>> Stashed changes
            if (move >= distance)
            {
                move = distance - 1;
            }

            int pspace = piecespaces[minpiece];
            while (move > 0)
            {
                int cspace = pspace;
                for (int i = 0; i < move; i++)
                {
<<<<<<< Updated upstream
                    //Debug.Log("CSpace: " + cspace);
=======
>>>>>>> Stashed changes
                    cspace = paths[minpiece][minepiece][cspace];
                }

                IPiece piece = boardManager.boardState.GetPiece(new Vector2Int(cspace % 10, cspace / 10));
                if (piece == null)
                {
<<<<<<< Updated upstream
                    Debug.Log("Move: " + minpiece + " " + pspace + " " + cspace);
=======
>>>>>>> Stashed changes
                    boardManager.boardState.MovePiece(new Vector2Int(pspace % 10, pspace / 10), new Vector2Int(cspace % 10, cspace / 10));
                    pspace = cspace;
                    break;
                }
                else
                {
                    move--;
                }
            }

            if (move + pieces[minpiece].Range >= distance)
            {
                int espace = epiecespaces[minepiece];

<<<<<<< Updated upstream
                Debug.Log("Attack: " + minpiece + " " + pspace + " " + espace);

=======
>>>>>>> Stashed changes
                boardManager.boardState.AttackPiece(new Vector2Int(pspace % 10, pspace / 10), new Vector2Int(espace % 10, espace / 10));
                if (boardManager.DidPlayerWin(PieceType.Player2))
                {
                    Debug.Log($"Player {PieceType.Player2} won!");
                    SceneManager.LoadScene("P2WinScene");
                }

                if (boardManager.boardState.GetPiece(new Vector2Int(espace % 10, espace / 10)) == null)
                {
                    piecedefeated[minepiece] = true;
                }
            }

            piecemoved[minpiece] = true;
        }
    }
    public int[] GetPath(int cspace, int espace)
    {
        int[] board = new int[100];

        for (int i = 0; i < 100; i++)
        {
            board[i] = -1;
        }

        board[espace] = 0;

        GetPathRecursive(cspace, espace, board);

        return board;
    }
    public void GetPathRecursive(int cspace, int espace, int[] board)
    {
        if (cspace == espace)
        {
            return;
        }

        //Up
        if (espace > 9 && board[espace - 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((espace - 10) % 10, (espace - 10) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                board[espace - 10] = espace;
                GetPathRecursive(cspace, espace - 10, board);
            }
        }

        //Down
        if (espace < 90 && board[espace + 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((espace + 10) % 10, (espace + 10) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                board[espace + 10] = espace;
                GetPathRecursive(cspace, espace + 10, board);
            }
        }

        //Left
        if (espace % 10 != 0 && board[espace - 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((espace - 1) % 10, (espace - 1) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                board[espace - 1] = espace;
                GetPathRecursive(cspace, espace - 1, board);
            }
        }

        //Right
        if (espace % 10 != 9 && board[espace + 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((espace + 1) % 10, (espace + 1) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                board[espace + 1] = espace;
                GetPathRecursive(cspace, espace + 1, board);
            }
        }
    }

    public int GetDistance(int cspace, int espace, int[] board)
    {
        int distance = 0;
<<<<<<< Updated upstream
        Debug.Log("cspace: " + cspace);
        Debug.Log("espace: " + espace);
=======
>>>>>>> Stashed changes

        while (cspace != espace)
        {
            cspace = board[cspace];
            distance++;
        }

<<<<<<< Updated upstream
        Debug.Log("distance: " + distance);
=======
>>>>>>> Stashed changes
        return distance;
    }
}
