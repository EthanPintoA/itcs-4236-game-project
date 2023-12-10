using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;


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
    private GameObject SniperPrefabP1;
    [SerializeField]
    private GameObject SniperPrefabP2;
    [SerializeField]
    private GameObject TankPrefabP1;
    [SerializeField]
    private GameObject TankPrefabP2;

    [SerializeField]
    [Tooltip("The Prefab for the Target. Denotes which spaces a piece can target with attacks.")]
    private GameObject SpacePrefab;

    [SerializeField]
    [Tooltip("The Prefab for the Space. Denotes which spaces a piece can move to.")]
    private GameObject TargetPrefab;
    [Header("Player wallets")]
    public TMP_Text P1Coins;
    public TMP_Text P2Coins;
    [HideInInspector]
    public PlayerTurn playerTurn;
    [HideInInspector]
    public GameState? gameState;


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
        playerTurn = PlayerTurn.Player1;
        gameState = null;

        //Marks avaliable spaces as not selected
        for (int i = 0; i < 100; i++)
        {
            board[i] = -1;
        }
    }

    void Start()
    {
        CreatePiece(new Vector2Int(0, 0), PieceType.Player1, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(2, 0), PieceType.Player1, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(1, 0), PieceType.Player1, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(3, 0), PieceType.Player1, SelectedPiece.Soldier, true);

        CreatePiece(new Vector2Int(9, 9), PieceType.Player2, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(8, 9), PieceType.Player2, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(7, 9), PieceType.Player2, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(6, 9), PieceType.Player2, SelectedPiece.Soldier, true);
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

            if (gameState == null)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    // Disable this until currency is implemented
                    // Debug.Log("Creating a piece is currently disabled");

                    var player = playerTurn.GetPlayerPiece();
                    CreatePiece(pieceGridPos, player, shopManager.selectedPiece);
                    shopManager.selectedPiece = null;
                    // playerTurn.SwitchPlayers();
                }
                else if (
                    piece.Type == PieceType.Wall
                    && shopManager.selectedPiece == SelectedPiece.Tnt
                )
                {
                    //check and subtract coins from wallet
                    var player = playerTurn.GetPlayerPiece();
                    int playerCoins = (player == PieceType.Player1) ? int.Parse(P1Coins.text) : int.Parse(P2Coins.text);
                    if (playerCoins < 50)
                    {
                        return; // Not enough coins to place TNT
                    }
                    else
                    {
                        //place tnt
                        var requiredPiece = playerTurn.GetPlayerPiece();
                        if (HasNeighbor(pieceGridPos, requiredPiece))
                        {
                            boardManager.boardState.SetPiece(null, pieceGridPos);
                            shopManager.selectedPiece = null;
                            if (player == PieceType.Player1)
                            {
                                P1Coins.text = (playerCoins - 50).ToString();
                            }
                            else if (player == PieceType.Player2)
                            {
                                P2Coins.text = (playerCoins - 50).ToString();
                            }
                        }
                        else
                        {
                            Debug.Log("TNT must be placed next to a piece owned by the current player");
                        }
                    }

                }
                else
                {
                    //If selecting one of your pieces, mark as selected and calculate avaliable spaces
                    if (ValidPieceType(piece))
                    {
                        selected = pieceGridPos;
                        gameState = GameState.Selected;
                        GetSpaces(pieceGridPos.x + (pieceGridPos.y * 10), piece.Movement);
                    }
                    else
                    {
                        Debug.Log("This is not the current player's piece");
                    }
                }
            }
            else if (gameState == GameState.Selected)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                var isNotSpace = piece != null && piece.Type != PieceType.Space;
                var sameSpace = pieceGridPos == selected;

                if ((piece == null || isNotSpace) && !sameSpace)
                {
                    ClearSpaces();
                    gameState = null;
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
                    gameState = GameState.Attack;
                    GetAttacks(pieceGridPos.x + (pieceGridPos.y * 10), piece.Range);

                    if (targetnum == 0)
                    {
                        ClearSpaces();
                        gameState = null;
                        playerTurn.SwitchPlayers();
                    }
                }
            }
            else if (gameState == GameState.Attack)
            {
                int gridNum = pieceGridPos.x + (pieceGridPos.y * 10);
                bool isTargeted = false;
                for (int i = 0; i < targetnum; i++)
                {
                    Vector2Int targetGridPos = (Vector2Int)boardManager.WorldPosToGridPos((Vector2)targets[i].transform.position);
                    int targetNum = targetGridPos.x + (targetGridPos.y * 10);
                    if (gridNum == targetNum)
                    {
                        isTargeted = true;
                        break;
                    }
                }

                //If a targeted space is selected, attack piece
                if (isTargeted)
                {
                    boardManager.boardState.AttackPiece(selected, pieceGridPos);
                    var currentPlayer = playerTurn.GetPlayerPiece();
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
                gameState = null;
                playerTurn.SwitchPlayers();
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

            if (gameState == null)
            {
                Debug.Log($"Deleting piece at {pieceGridPos}");

                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);

                if (piece == null)
                {
                    Debug.Log("There is no piece here");
                }
                else if (ValidPieceType(piece))
                {
                    boardManager.boardState.SetPiece(null, pieceGridPos);
                    playerTurn.SwitchPlayers();
                }
                else
                {
                    Debug.Log("This is not the current player's piece");
                }
            }
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
            }
        }
    }

    /// <summary>
    /// Checks if the piece is the correct type for the current player
    /// </summary>
    private bool ValidPieceType(IPiece piece)
    {
        return (playerTurn == PlayerTurn.Player1 && piece.Type == PieceType.Player1)
            || (playerTurn == PlayerTurn.Player2 && piece.Type == PieceType.Player2);
    }

    /// <summary>
    /// Creates a piece at the given position.
    /// <br/>
    /// Currently only creates a Soldier.
    /// </summary>
    private void CreatePiece(Vector2Int pieceGridPos, PieceType player, SelectedPiece? selectedPiece, bool walletOverride = false)
    {
        Debug.Log($"Creating piece on grid position: {pieceGridPos}");

        var pieceGlobalPos = boardManager.GridPosToWorldPos(pieceGridPos);

        if (player == PieceType.Player1)
        {
            if (selectedPiece == SelectedPiece.Soldier && (int.Parse(P1Coins.text) >= 10 || walletOverride))
            {
                var soldierObj = Instantiate(SoldierPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Soldier(soldierObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 10).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Sniper && (int.Parse(P1Coins.text) >= 40 || walletOverride))
            {
                var sniperObj = Instantiate(SniperPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Tank(sniperObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 40).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Tank && (int.Parse(P1Coins.text) >= 70 || walletOverride))
            {
                var tankObj = Instantiate(TankPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Tank(tankObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 70).ToString(); }
            }
        }
        else
        {
            if (selectedPiece == SelectedPiece.Soldier && (int.Parse(P2Coins.text) >= 10 || walletOverride))
            {
                var soldierObj = Instantiate(SoldierPrefabP2, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Soldier(soldierObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 10).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Sniper && (int.Parse(P2Coins.text) >= 40 || walletOverride))
            {
                var sniperObj = Instantiate(SniperPrefabP2, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Tank(sniperObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 40).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Tank && (int.Parse(P2Coins.text) >= 70 || walletOverride))
            {
                var tankObj = Instantiate(TankPrefabP2, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Tank(tankObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 70).ToString(); }
            }
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

        if (move == 0)
        {
            return;
        }

        //Up
        if (cspace > 9 && board[cspace - 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 10) % 10, (cspace - 10) / 10));
            if (npiece == null || (playerTurn == PlayerTurn.Player1 && npiece.Type == PieceType.Player1) || (playerTurn == PlayerTurn.Player2 && npiece.Type == PieceType.Player2))
            {
                board[cspace - 10] = cspace;
                GetSpaces(cspace - 10, move - 1);
            }
        }

        //Down
        if (cspace < 90 && board[cspace + 10] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 10) % 10, (cspace + 10) / 10));
            if (npiece == null || (playerTurn == PlayerTurn.Player1 && npiece.Type == PieceType.Player1) || (playerTurn == PlayerTurn.Player2 && npiece.Type == PieceType.Player2))
            {
                board[cspace + 10] = cspace;
                GetSpaces(cspace + 10, move - 1);
            }
        }

        //Left
        if (cspace % 10 != 0 && board[cspace - 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace - 1) % 10, (cspace - 1) / 10));
            if (npiece == null || (playerTurn == PlayerTurn.Player1 && npiece.Type == PieceType.Player1) || (playerTurn == PlayerTurn.Player2 && npiece.Type == PieceType.Player2))
            {
                board[cspace - 1] = cspace;
                GetSpaces(cspace - 1, move - 1);
            }
        }

        //Right
        if (cspace % 10 != 9 && board[cspace + 1] < 0)
        {
            IPiece npiece = boardManager.boardState.GetPiece(new Vector2Int((cspace + 1) % 10, (cspace + 1) / 10));
            if (npiece == null || (playerTurn == PlayerTurn.Player1 && npiece.Type == PieceType.Player1) || (playerTurn == PlayerTurn.Player2 && npiece.Type == PieceType.Player2))
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
        if (piece != null && ((playerTurn == PlayerTurn.Player1 && piece.Type == PieceType.Player2) || (playerTurn == PlayerTurn.Player2 && piece.Type == PieceType.Player1))) //Add Wall attack?
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
            if (board[i] >= 0)
            {
                board[i] = -1;

                if (gameState == GameState.Selected)
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
