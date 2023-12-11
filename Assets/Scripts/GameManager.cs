using System.Linq;
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
    private GameObject HelicopterPrefabP1;

    [SerializeField]
    [Tooltip("The Prefab for the Target. Denotes which spaces a piece can target with attacks.")]
    private GameObject SpacePrefab;

    [SerializeField]
    [Tooltip("The Prefab for the Space. Denotes which spaces a piece can move to.")]
    private GameObject TargetPrefab;

    [SerializeField]
    [Tooltip("The parent object for the spaces and targets.")]
    private GameObject TempEntitiesParent;

    public TMP_Text P1Coins;
    public TMP_Text P2Coins;

    [HideInInspector]
    public PlayerTurn playerTurn;
    [HideInInspector]
    public GameState? gameState;


    private Vector2Int selected;


    void Awake()
    {
        playerTurn = PlayerTurn.Player1;
        gameState = null;
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
                    //check if piece is being placed on first 2 rows
                    if (((player == PieceType.Player1) && (pieceGridPos.y == 0 || pieceGridPos.y == 1)) || ((player == PieceType.Player2) && (pieceGridPos.y == 8 || pieceGridPos.y == 9)))
                    {
                        CreatePiece(pieceGridPos, player, shopManager.selectedPiece);
                        shopManager.selectedPiece = null;
                    }


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
                        var PossiblePositions = GetMovementOptions(piece, pieceGridPos);
                        CreateSpaces(PossiblePositions);
                    }
                    else
                    {
                        Debug.Log("This is not the current player's piece");
                    }
                }
            }
            else if (gameState == GameState.Selected)
            {
                var didSelectSpace = false;
                foreach (Transform child in TempEntitiesParent.transform)
                {
                    var childGridPos = boardManager.WorldPosToGridPos(child.position);
                    // childGridPos should never be null
                    if (childGridPos.Value == pieceGridPos)
                    {
                        didSelectSpace = true;
                    }
                }

                var didSelectSamePos = pieceGridPos == selected;
                IPiece selectedPiece = boardManager.boardState.GetPiece(selected);

                if (didSelectSpace || didSelectSamePos)
                {
                    if (!didSelectSamePos)
                    {
                        // Helicopter needs to absorb the piece it moves to if it's not carrying another piece
                        if (selectedPiece is Helicopter helicopter)
                        {
                            var pieceToAbsorb = boardManager.boardState.GetPiece(pieceGridPos);
                            if (pieceToAbsorb != null)
                            {
                                boardManager.boardState.SetPiece(null, pieceGridPos);
                                helicopter.DamageBonus = pieceToAbsorb.GetDamage();
                                helicopter.Health += Mathf.FloorToInt(pieceToAbsorb.Health / 2f);
                                helicopter.Range = Mathf.Max(helicopter.Range, pieceToAbsorb.Range);
                                helicopter.CarryingAnotherPiece = true;
                                Debug.Log("Helicopter is now carrying a piece");
                            }
                        }

                        boardManager.boardState.MovePiece(selected, pieceGridPos);
                        // Update piece since was destroyed and recreated
                        selectedPiece = boardManager.boardState.GetPiece(pieceGridPos);
                        selected = pieceGridPos;
                    }

                    ClearSpacesAndTargets();
                    gameState = GameState.Attack;
                    var PossiblePositions = GetAttackOptions(selectedPiece, pieceGridPos);

                    if (PossiblePositions.Length == 0)
                    {
                        ClearSpacesAndTargets();
                        gameState = null;
                        playerTurn.SwitchPlayers();
                    }
                    else
                    {
                        CreateTargets(PossiblePositions);
                    }
                }
                else
                {
                    ClearSpacesAndTargets();
                    gameState = null;
                }
            }
            else if (gameState == GameState.Attack)
            {
                foreach (Transform child in TempEntitiesParent.transform)
                {
                    var childGridPos = boardManager.WorldPosToGridPos(child.position);
                    if (!childGridPos.HasValue)
                    {
                        Debug.LogError($"Target {child.name} is out of bounds");
                        continue;
                    }
                    //If a targetable piece is selected, attack it
                    else if (childGridPos.Value == pieceGridPos)
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
                }

                ClearSpacesAndTargets();
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
                ClearSpacesAndTargets();
                gameState = null;
            }
            else if (gameState == GameState.Attack)
            {
                ClearSpacesAndTargets();
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
                boardManager.boardState.SetPiece(new Sniper(sniperObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 40).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Tank && (int.Parse(P1Coins.text) >= 70 || walletOverride))
            {
                var tankObj = Instantiate(TankPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Tank(tankObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 70).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Helicopter && (int.Parse(P1Coins.text) >= 50 || walletOverride))
            {
                var helicopterObj = Instantiate(HelicopterPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Helicopter(helicopterObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 50).ToString(); }
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
                boardManager.boardState.SetPiece(new Sniper(sniperObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 40).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Tank && (int.Parse(P2Coins.text) >= 70 || walletOverride))
            {
                var tankObj = Instantiate(TankPrefabP2, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Tank(tankObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 70).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.Helicopter && (int.Parse(P2Coins.text) >= 50 || walletOverride))
            {
                // FIXME: Use the P2 helicopter prefab
                var helicopterObj = Instantiate(HelicopterPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Helicopter(helicopterObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 50).ToString(); }
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
    /// Gets the possible positions for a piece to move to.
    /// </summary>
    private Vector2Int[] GetMovementOptions(IPiece piece, Vector2Int pieceGridPos)
    {
        if (piece is Helicopter helicopter)
        {
            // Helicopter can move anywhere on the board
            var grid = new bool[10, 10];
            var PossiblePositions = new PossiblePositions(grid, piece.Movement, pieceGridPos).GetPositions();

            // Remove the current position. We don't want to move to the same position.
            PossiblePositions = PossiblePositions.Where(pos => pos != pieceGridPos).ToArray();

            if (helicopter.CarryingAnotherPiece)
            {
                // Ignore positions that have a piece, since we can't carry more than one piece.
                PossiblePositions = PossiblePositions.Where(pos => boardManager.boardState.GetPiece(pos) == null).ToArray();
            }
            else
            {
                // Remove positions that have a piece that we don't own.
                PossiblePositions = PossiblePositions
                    .Where(pos =>
                    {
                        var piece = boardManager.boardState.GetPiece(pos);
                        return piece == null || piece.Type == playerTurn.GetPlayerPiece();
                    })
                    .ToArray();
            }

            return PossiblePositions;
        }
        else
        {
            var grid = boardManager.boardState.Pieces.Select(piece => piece is Wall).ToArray();
            var PossiblePositions = new PossiblePositions(grid, 10, 10, piece.Movement, pieceGridPos).GetPositions();

            // Remove the current position. We don't want to move to the same position.
            PossiblePositions = PossiblePositions.Where(pos => pos != pieceGridPos).ToArray();

            // Remove positions that have a piece. We don't want to move on top of a piece.
            PossiblePositions = PossiblePositions.Where(pos => boardManager.boardState.GetPiece(pos) == null).ToArray();

            return PossiblePositions;
        }
    }

    /// <summary>
    /// Creates spaces at the given positions.
    /// </summary>
    private void CreateSpaces(Vector2Int[] positions)
    {
        foreach (var pos in positions)
        {
            var spaceWorldPos = (Vector3)boardManager.GridPosToWorldPos(pos);
            // Set z to 1 so that the space is behind the piece
            spaceWorldPos.z = 1;
            Instantiate(SpacePrefab, spaceWorldPos, Quaternion.identity, TempEntitiesParent.transform);
        }
    }

    private void ClearSpacesAndTargets()
    {
        foreach (Transform child in TempEntitiesParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private Vector2Int[] GetAttackOptions(IPiece piece, Vector2Int pieceGridPos)
    {
        var grid = boardManager.boardState.Pieces.Select(piece => piece is Wall).ToArray();
        var PossiblePositions = new TargetPossiblePositions(grid, 10, 10, piece.Range, pieceGridPos).GetPositions();

        // Remove positions that have a piece. We don't want to move on top of a piece.
        PossiblePositions = PossiblePositions
            // Can't attack a space
            .Where(pos => boardManager.boardState.GetPiece(pos) != null)
            // Can't attack your own piece
            .Where(pos => boardManager.boardState.GetPiece(pos).Type != piece.Type)
            .ToArray();

        return PossiblePositions;
    }

    private void CreateTargets(Vector2Int[] positions)
    {
        foreach (var pos in positions)
        {
            var targetWorldPos = (Vector3)boardManager.GridPosToWorldPos(pos);
            // Set z to -1 so that the target is in front of the piece
            targetWorldPos.z = -1;
            Instantiate(TargetPrefab, targetWorldPos, Quaternion.identity, TempEntitiesParent.transform);
        }
    }
}
