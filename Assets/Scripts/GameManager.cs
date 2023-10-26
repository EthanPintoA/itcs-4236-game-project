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

            if (state == GameState.P1Turn || state == GameState.P2Turn)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    //create soldier on the grid position
                    Debug.Log($"Creating piece at {pieceGridPos}");

                    var soldierObj = Instantiate(SoldierPrefab, pieceGlobalPos, Quaternion.identity);
                    boardManager.boardState.SetPiece(new Soldier(soldierObj, PieceType.Player1), pieceGridPos);
                }
                else
                {
                    //If selecting one of your pieces, mark as selected and calculate avaliable spaces
                    if (state == GameState.P1Turn && piece.Type == PieceType.Player1)
                    {
                        selected = pieceGridPos;
                        state = GameState.P1Selected;
                        GetSpaces(pieceGridPos.x + (pieceGridPos.y * 10), piece.Movement);
                    }
                    else if (state == GameState.P2Turn && piece.Type == PieceType.Player2)
                    {
                        selected = pieceGridPos;
                        state = GameState.P2Selected;
                        GetSpaces(pieceGridPos.x + (pieceGridPos.y * 10), piece.Movement);
                    }
                }
            }
            else if (state == GameState.P1Selected || state == GameState.P2Selected)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);

                //Move piece if an avaliable space is selected
                if(piece != null && piece.Type == PieceType.Space)
                {
                    boardManager.boardState.MovePiece(selected, pieceGridPos);
                }

                ClearSpaces();

                if (state == GameState.P1Selected)
                {
                    state = GameState.P1Turn;
                }
                else if (state == GameState.P2Selected)
                {
                    state = GameState.P2Turn;
                }
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

            if (state == GameState.P1Turn || state == GameState.P2Turn)
            {
                Debug.Log($"Deleting piece at {pieceGridPos}");

                boardManager.boardState.SetPiece(null, pieceGridPos);
            }
            else if (state == GameState.P1Selected || state == GameState.P2Selected)
            {
                ClearSpaces();
                if(state == GameState.P1Selected)
                {
                    state = GameState.P1Turn;
                }
                else if (state == GameState.P2Selected)
                {
                    state = GameState.P2Turn;
                }
            }
        }
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

public enum GameState
{
    P1Turn,
    P2Turn,
    P1Selected,
    P2Selected
}

