using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField]
    private BoardManager boardManager;

    [SerializeField]
    private ShopManager shopManager;

    [Header("Game Objects")]
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
    public GameState state;

    private Vector2Int selected;

    //During Select, array value indicates the previous space the piece would move from for if we implement a moving animation
    //During Attack, array value simply has 0 for invalid and 1 for valid
    private int[] board = new int[100];

    //Array to track targets and the number
    //Might be a better way to do this
    private GameObject[] targets = new GameObject[100];
    private int targetnum = 0;


    void Awake()
    {
        state = GameState.P1Turn;

        //Marks avaliable spaces as not selected
        for (int i = 0; i < 100; i++)
        {
            board[i] = -1;
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
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //reads mouse position and converts to grid position
            var nPiecePos = MousePositionToPiecePosition();
            if (!nPiecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var pieceGridPos = nPiecePos.Value;

            if (state.IsTurn())
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
                else if (
                    piece.Type == PieceType.Wall
                    && shopManager.selectedPiece == SelectedPiece.Tnt
                )
                {
                    var requiredPiece = GameState.P1Turn == state ? PieceType.Player1 : PieceType.Player2;
                    if (HasNeighbor(pieceGridPos, requiredPiece))
                    {
                        boardManager.boardState.SetPiece(null, pieceGridPos);
                        shopManager.selectedPiece = null;
                    } else {
                        Debug.Log("TNT must be placed next to a piece owned by the current player");
                    }
                }
                else
                {
                    //If selecting one of your pieces, mark as selected and calculate avaliable spaces
                    if (ValidPieceType(piece))
                    {
                        selected = pieceGridPos;
                        state = state.GetNextState();
                        GetSpaces(pieceGridPos.x + (pieceGridPos.y * 10), piece.Movement);
                    }
                    else
                    {
                        Debug.Log("This is not the current player's piece");
                    }
                }
            }
            else if (state.IsSelected())
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                var isNotSpace = piece != null && piece.Type != PieceType.Space;
                var sameSpace = pieceGridPos == selected;

                if ((piece == null || isNotSpace) && !sameSpace)
                {
                    ClearSpaces();
                    state = state.GetCurrentTurn();
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
                    state = state.GetNextState();
                    GetAttacks(pieceGridPos.x + (pieceGridPos.y * 10), piece.Range);

                    if (targetnum == 0)
                    {
                        ClearSpaces();
                        state = state.GetCurrentTurn().GetSwitchPlayersTurns();
                    }
                }
            }
            else if (state.IsAttack())
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
                    var currentPlayer =
                        (state == GameState.P1Attack) ? PieceType.Player1 : PieceType.Player2;
                    if (boardManager.DidPlayerWin(currentPlayer))
                    {
                        Debug.Log($"Player {currentPlayer} won!");
                        if (currentPlayer == PieceType.Player1)
                        {
                            SceneManager.LoadScene("P1WinScene");
                        }
                        else
                        {
                            SceneManager.LoadScene("P2WinScene");
                        }
                    }
                }
                
                ClearSpaces();
                state = state.GetNextState().GetSwitchPlayersTurns();
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

            if (state.IsTurn())
            {
                Debug.Log($"Deleting piece at {pieceGridPos}");

                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                
                if (piece == null) {
                    Debug.Log("There is no piece here");
                }
                else if (ValidPieceType(piece))
                {
                    boardManager.boardState.SetPiece(null, pieceGridPos);
                    state = state.GetSwitchPlayersTurns();
                }
                else
                {
                    Debug.Log("This is not the current player's piece");
                }
            }
            else if (state.IsSelected())
            {
                ClearSpaces();
                state = state.GetCurrentTurn();
            }
            else if (state.IsAttack())
            {
                ClearSpaces();
                state = state.GetCurrentTurn().GetSwitchPlayersTurns();
            }
        }
    }

    /// <summary>
    /// Checks if the piece is the correct type for the current player
    /// </summary>
    private bool ValidPieceType(IPiece piece)
    {
        return (state == GameState.P1Turn && piece.Type == PieceType.Player1)
            || (state == GameState.P2Turn && piece.Type == PieceType.Player2);
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
    /// Checks if the player can use the tnt piece at the given position.
    /// <br/>
    /// The player can only use the tnt piece if there is a piece owned by the player
    /// next to the tnt piece.
    /// </summary>
    private bool HasNeighbor(Vector2Int gridPos, PieceType player)
    {
        var neighborsPosDiff = new Vector2Int[]
        {
            new(-1, -1),
            new(0, -1),
            new(1, -1),
            new(-1, 0),
            new(1, 0),
            new(-1, 1),
            new(0, 1),
            new(1, 1),
        };

        // The player can only use the tnt piece if there is a piece owned by the player
        // next to the tnt piece
        foreach (var posDiff in neighborsPosDiff)
        {
            var neighborPos = gridPos + posDiff;
            var neighborPiece = boardManager.boardState.GetPiece(neighborPos);

            if (neighborPiece != null && neighborPiece.Type == player)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets all avaliable spaces that a piece can move to and puts a Space piece on them
    /// </summary>
    public void GetSpaces(int cspace, int move)
    {
        //Set starting space to 0 to avoid backtracking
        if (board[cspace] < 0)
        {
            board[cspace] = 0;
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
        if(cspace > 9 && board[cspace - 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 10) % 10, (cspace - 10) / 10));
            if (npiece == null || (state == GameState.P1Selected && npiece.Type == PieceType.Player1) || (state == GameState.P2Selected && npiece.Type == PieceType.Player2))
            {
                board[cspace - 10] = cspace;
                GetSpaces(cspace - 10, move - 1);
            }
        }

        //Down
        if (cspace < 90 && board[cspace + 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 10) % 10, (cspace + 10) / 10));
            if (npiece == null || (state == GameState.P1Selected && npiece.Type == PieceType.Player1) || (state == GameState.P2Selected && npiece.Type == PieceType.Player2))
            {
                board[cspace + 10] = cspace;
                GetSpaces(cspace + 10, move - 1);
            }
        }

        //Left
        if (cspace % 10 != 0 && board[cspace - 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 1) % 10, (cspace - 1) / 10));
            if (npiece == null || (state == GameState.P1Selected && npiece.Type == PieceType.Player1) || (state == GameState.P2Selected && npiece.Type == PieceType.Player2))
            {
                board[cspace - 1] = cspace;
                GetSpaces(cspace - 1, move - 1);
            }
        }

        //Right
        if (cspace % 10 != 9 && board[cspace + 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 1) % 10, (cspace + 1) / 10));
            if (npiece == null || (state == GameState.P1Selected && npiece.Type == PieceType.Player1) || (state == GameState.P2Selected && npiece.Type == PieceType.Player2))
            {
                board[cspace + 1] = cspace;
                GetSpaces(cspace + 1, move - 1);
            }
        }
    }

    /// <summary>
    /// Gets all avaliable spaces that a piece can attack and puts a Target on them
    /// </summary>
    public void GetAttacks(int cspace, int range)
    {
        IPiece piece = boardManager.boardState.GetPiece(new Vector2Int((cspace) % 10, (cspace) / 10));
        if (piece != null && ((state == GameState.P1Attack && piece.Type == PieceType.Player2) || (state == GameState.P2Attack && piece.Type == PieceType.Player1))) //Add Wall attack?
        {
            board[cspace] = 1;

            //Add Target Indicator
            var targetPos = (Vector3)boardManager.GridPosToWorldPos(new Vector2Int(cspace % 10, cspace / 10));
            targetPos.z = -1; // So that the target is in front of the piece
            var targetObj = Instantiate(TargetPrefab, targetPos, Quaternion.identity);
            targets[targetnum] = targetObj;
            targetnum++;
        }
        else
        {
            board[cspace] = 0;
        }

        if (range == 0)
        {
            return;
        }

        //Up
        if (cspace > 9 && board[cspace - 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 10) % 10, (cspace - 10) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace - 10, range - 1);
            }
        }

        //Down
        if (cspace < 90 && board[cspace + 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 10) % 10, (cspace + 10) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace + 10, range - 1);
            }   
        }

        //Left
        if (cspace % 10 != 0 && board[cspace - 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 1) % 10, (cspace - 1) / 10));
            if (npiece == null || npiece.Type != PieceType.Wall)
            {
                GetAttacks(cspace - 1, range - 1);
            }
        }

        //Right
        if (cspace % 10 != 9 && board[cspace + 1] < 0)
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
            if(board[i] >= 0)
            {
                board[i] = -1;

                if (state.IsSelected())
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
}
