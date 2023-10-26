using UnityEngine;
using UnityEngine.InputSystem;



public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    private BoardManager boardManager;

    [SerializeField]
    [Tooltip("The Prefab for the Soldier. Soldier is currently a placeholder for a piece.")]
    private GameObject SoldierPrefab;

    [SerializeField]
    [Tooltip("The Prefab for the Space. Denotes which spaces a piece can move to.")]
    private GameObject SpacePrefab;

    [HideInInspector]
    public GameState state;

    private Vector2Int selected;

    //Array value indicates the previous space the piece would move from for if we implement a moving animation
    private int[] board = new int[100];


    void Awake()
    {
        state = GameState.P1Turn;

        //Marks avaliable spaces as not selected
        for (int i = 0; i < 100; i++)
        {
            board[i] = -1;
        }
    }
    


    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //reads mouse position and converts to grid position
            var piecePos = MousePositionToPiecePosition();
            if (!piecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var (pieceGridPos, pieceGlobalPos) = piecePos.Value;

            if (state.IsTurn())
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    var player = state == GameState.P1Turn ? PieceType.Player1 : PieceType.Player2;
                    CreatePiece(pieceGridPos, pieceGlobalPos, player);
                    state = state.GetSwitchPlayersTurns();
                }
                else
                {
                    //If selecting one of your pieces, mark as selected and calculate avaliable spaces
                    if (ValidPieceType(piece))
                    {
                        selected = pieceGridPos;
                        state = state.GetToggleSelected();
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

                if (piece == null || isNotSpace)
                {
                    state = state.GetToggleSelected();
                }
                //Move piece if an avaliable space is selected
                else
                {
                    boardManager.boardState.MovePiece(selected, pieceGridPos);
                    state = state.GetToggleSelected().GetSwitchPlayersTurns();
                }

                ClearSpaces();
            }


        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            var piecePos = MousePositionToPiecePosition();
            if (!piecePos.HasValue)
            {
                Debug.Log("Mouse is not over board");
                return;
            }
            var (pieceGridPos, _) = piecePos.Value;

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
                state = state.GetToggleSelected();
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
    private void CreatePiece(Vector2Int pieceGridPos, Vector2 pieceGlobalPos, PieceType player)
    {
        Debug.Log($"Creating piece on grid position: {pieceGridPos}");

        var soldierObj = Instantiate(SoldierPrefab, pieceGlobalPos, Quaternion.identity);
        boardManager.boardState.SetPiece(new Soldier(soldierObj, player), pieceGridPos);
    }

    /// <summary>
    /// Converts the mouse's position to a piece's position.
    /// </summary>
    /// <returns> The piece's grid position and global position. </returns>
    public (Vector2Int, Vector2)? MousePositionToPiecePosition()
    {
        var mouseScreenPos = Mouse.current.position.ReadValue();
        var mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(mouseScreenPos);

        var gridPos = boardManager.WorldPosToGridPos(mouseWorldPos);

        if (!gridPos.HasValue)
        {
            return null;
        }

        var worldPos = boardManager.GridPosToWorldPos(gridPos.Value);

        return (gridPos.Value, worldPos);
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
    /// Clears the board array and all Space pieces
    /// </summary>
    public void ClearSpaces()
    {
        for (int i = 0; i < 100; i++)
        {
            if(board[i] >= 0)
            {
                board[i] = -1;
                IPiece piece = boardManager.boardState.GetPiece(new Vector2Int(i % 10, i / 10));
                if (piece != null && piece.Type == PieceType.Space)
                {
                    boardManager.boardState.SetPiece(null, new Vector2Int(i % 10, i / 10));
                }
            }
        }
    }
}
