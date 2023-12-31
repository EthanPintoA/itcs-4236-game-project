using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class AIGameManager : MonoBehaviour
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
    private GameObject HelicopterPrefabP2;
    [SerializeField]
    private GameObject KingPrefabP1;
    [SerializeField]
    private GameObject KingPrefabP2;

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
    //sprites
    public Sprite movedSoldier;
    public Sprite movedTank;
    public Sprite movedSniper;
    public Sprite movedKing;
    public Sprite movedHeli;
    public Sprite P1Soldier;
    public Sprite P1Tank;
    public Sprite P1Sniper;
    public Sprite P1King;
    public Sprite P1Heli;
    public Sprite P2Soldier;
    public Sprite P2Tank;
    public Sprite P2Sniper;
    public Sprite P2King;
    public Sprite P2Heli;

    [HideInInspector]
    public PlayerTurn playerTurn;
    [HideInInspector]
    public GameState? gameState;

    [HideInInspector]
    public Vector2Int? selected;
    private List<IPiece> movedPieceList = new List<IPiece>();
    private int randomNumber; //Determines AI Shop Strategy

    void Awake()
    {
        playerTurn = PlayerTurn.Player1;
        gameState = null;
        selected = null;
    }

    void Start()
    {
        selected = null;
        CreatePiece(new Vector2Int(0, 0), PieceType.Player1, SelectedPiece.King, true);
        CreatePiece(new Vector2Int(2, 0), PieceType.Player1, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(1, 0), PieceType.Player1, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(3, 0), PieceType.Player1, SelectedPiece.Soldier, true);

        CreatePiece(new Vector2Int(9, 9), PieceType.Player2, SelectedPiece.King, true);
        CreatePiece(new Vector2Int(8, 9), PieceType.Player2, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(7, 9), PieceType.Player2, SelectedPiece.Soldier, true);
        CreatePiece(new Vector2Int(6, 9), PieceType.Player2, SelectedPiece.Soldier, true);
        randomNumber = Random.Range(1, 5);
        //Debug.Log("random num = " + randomNumber);
    }

    void Update()
    {
        if (playerTurn == PlayerTurn.Player2 && gameState == null)
        {
            Debug.Log("AI Turn");

            gameState = GameState.Selected;
            AITurn();
            gameState = null;
            playerTurn.SwitchPlayers();
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

            if (gameState == null)//empty square selected (place piece)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    //check if piece is being placed on first 2 rows
                    if (pieceGridPos.y == 0 || pieceGridPos.y == 1)
                    {
                        CreatePiece(pieceGridPos, PieceType.Player1, shopManager.selectedPiece);
                        shopManager.selectedPiece = null;
                    }
                }
                else if (//wall selected (place tnt)
                    piece.Type == PieceType.Wall
                    && shopManager.selectedPiece == SelectedPiece.Tnt
                )
                {
                    //check and subtract coins from wallet
                    int playerCoins = int.Parse(P1Coins.text);
                    if (playerCoins < 50)
                    {
                        return; // Not enough coins to place TNT
                    }
                    else
                    {
                        //place tnt
                        if (HasNeighbor(pieceGridPos, PieceType.Player1))
                        {
                            boardManager.boardState.SetPiece(null, pieceGridPos);
                            shopManager.selectedPiece = null;
                            P1Coins.text = (playerCoins - 50).ToString();
                        }
                        else
                        {
                            Debug.Log("TNT must be placed next to a piece owned by the current player");
                        }
                    }

                }
                else//piece is selected (if your piece: you can move it)
                {
                    selected = pieceGridPos;

                    //If selecting one of your pieces, mark as selected and calculate avaliable spaces
                    if (ValidPieceType(piece))
                    {
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
            else if (gameState == GameState.Selected) // move a selected piece
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
                IPiece selectedPiece = boardManager.boardState.GetPiece(selected.Value);

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

                        boardManager.boardState.MovePiece(selected.Value, pieceGridPos);
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
                        selected = null;
                        // add piece to moved
                        setSpriteToMoved(selectedPiece);
                        movedPieceList.Add(selectedPiece);
                        // playerTurn.SwitchPlayers(); //turn not ended after done moving
                    }
                    else
                    {
                        setSpriteToMoved(selectedPiece);
                        movedPieceList.Add(selectedPiece);
                        CreateTargets(PossiblePositions);
                    }
                }
                else
                {
                    ClearSpacesAndTargets();
                    gameState = null;
                    selected = null;
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
                        // add piece to moved
                        IPiece piece = boardManager.boardState.GetPiece(selected.Value);
                        setSpriteToMoved(piece);
                        movedPieceList.Add(piece);
                        //Debug.Log("Length of movedPieceList after attack: " + movedPieceList.Count);

                        boardManager.boardState.AttackPiece(selected.Value, pieceGridPos);
                        if (boardManager.DidPlayerWin(PieceType.Player1))
                        {
                            Debug.Log($"Player 1 won!");
                            SceneManager.LoadScene("P1WinScene");
                        }
                    }
                }

                ClearSpacesAndTargets();
                gameState = null;
                selected = null;
                // add piece to list
                // playerTurn.SwitchPlayers(); //turn no longer switched after attack
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
                else if (ValidPieceType(piece))//delete ur own piece
                {
                    boardManager.boardState.SetPiece(null, pieceGridPos);
                    //check for king suicide
                    if (boardManager.DidPlayerWin(PieceType.Player2))
                    {
                        Debug.Log($"Player 2 won!");
                        SceneManager.LoadScene("P2WinScene");
                    }
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
                selected = null;
            }
            else if (gameState == GameState.Attack)
            {
                ClearSpacesAndTargets();
                gameState = null;
                selected = null;
            }
        }
    }

    /// <summary>
    /// Checks if the piece is the correct type for the current player
    /// </summary>
    private bool ValidPieceType(IPiece piece)
    {
        if (movedPieceList.Contains(piece))
        {
            return false;
        }
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
            else if (selectedPiece == SelectedPiece.Helicopter && (int.Parse(P1Coins.text) >= 100 || walletOverride))
            {
                var helicopterObj = Instantiate(HelicopterPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Helicopter(helicopterObj, player), pieceGridPos);
                if (!walletOverride) { P1Coins.text = (int.Parse(P1Coins.text) - 100).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.King && walletOverride)
            {
                var kingObj = Instantiate(KingPrefabP1, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new King(kingObj, player), pieceGridPos);
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
            else if (selectedPiece == SelectedPiece.Helicopter && (int.Parse(P2Coins.text) >= 100 || walletOverride))
            {
                // FIXME: Use the P2 helicopter prefab
                var helicopterObj = Instantiate(HelicopterPrefabP2, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new Helicopter(helicopterObj, player), pieceGridPos);
                if (!walletOverride) { P2Coins.text = (int.Parse(P2Coins.text) - 100).ToString(); }
            }
            else if (selectedPiece == SelectedPiece.King && walletOverride)
            {
                var kingObj = Instantiate(KingPrefabP2, pieceGlobalPos, Quaternion.identity);
                boardManager.boardState.SetPiece(new King(kingObj, player), pieceGridPos);
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
                        return piece == null
                            || (
                                piece.Type == playerTurn.GetPlayerPiece()
                                && piece is not Helicopter
                                && piece is not King
                            );
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
    private void setSpriteToMoved(IPiece piece)
    {
        if (piece is Soldier)
        {
            piece.GameObject.GetComponent<SpriteRenderer>().sprite = movedSoldier;
        }
        else if (piece is Tank)
        {
            piece.GameObject.GetComponent<SpriteRenderer>().sprite = movedTank;
        }
        else if (piece is Sniper)
        {
            piece.GameObject.GetComponent<SpriteRenderer>().sprite = movedSniper;
        }
        else if (piece is King)
        {
            piece.GameObject.GetComponent<SpriteRenderer>().sprite = movedKing;
        }
        else if (piece is Helicopter)
        {
            piece.GameObject.GetComponent<SpriteRenderer>().sprite = movedHeli;
        }
    }
    public void switchTurn()
    {
        foreach (var piece in movedPieceList)
        {
            var player = playerTurn.GetPlayerPiece();
            if (player == PieceType.Player1)
            {
                if (piece is Soldier)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P1Soldier;
                }
                else if (piece is Tank)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P1Tank;
                }
                else if (piece is Sniper)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P1Sniper;
                }
                else if (piece is King)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P1King;
                }
                else if (piece is Helicopter)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P1Heli;
                }
            }
            else if (player == PieceType.Player2)
            {
                if (piece is Soldier)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P2Soldier;
                }
                else if (piece is Tank)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P2Tank;
                }
                else if (piece is Sniper)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P2Sniper;
                }
                else if (piece is King)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P2King;
                }
                else if (piece is Helicopter)
                {
                    piece.GameObject.GetComponent<SpriteRenderer>().sprite = P2Heli;
                }
            }
        }
        ClearSpacesAndTargets();
        gameState = null;
        selected = null;
        movedPieceList.Clear();
        shopManager.selectedPiece = null;
        playerTurn.SwitchPlayers();
    }

    public void AITurn()
    {
        //shop piece placement
        if (randomNumber == 1 && int.Parse(P2Coins.text) >= 70)//tank spam
        {
            // //1/5 chance of just spending a turn bombing the map
            // int randomNumber2 = Random.Range(1, 1);
            // if (randomNumber2 == 1)
            // {
            //     List<Vector2Int> allPositions = new List<Vector2Int>
            //     {
            //         new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0),
            //         new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(7, 0), new Vector2Int(8, 0), new Vector2Int(9, 0),

            //         new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1), new Vector2Int(4, 1),
            //         new Vector2Int(5, 1), new Vector2Int(6, 1), new Vector2Int(7, 1), new Vector2Int(8, 1), new Vector2Int(9, 1),

            //         new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(4, 2),
            //         new Vector2Int(5, 2), new Vector2Int(6, 2), new Vector2Int(7, 2), new Vector2Int(8, 2), new Vector2Int(9, 2),

            //         new Vector2Int(0, 3), new Vector2Int(1, 3), new Vector2Int(2, 3), new Vector2Int(3, 3), new Vector2Int(4, 3),
            //         new Vector2Int(5, 3), new Vector2Int(6, 3), new Vector2Int(7, 3), new Vector2Int(8, 3), new Vector2Int(9, 3),

            //         new Vector2Int(0, 4), new Vector2Int(1, 4), new Vector2Int(2, 4), new Vector2Int(3, 4), new Vector2Int(4, 4),
            //         new Vector2Int(5, 4), new Vector2Int(6, 4), new Vector2Int(7, 4), new Vector2Int(8, 4), new Vector2Int(9, 4),

            //         new Vector2Int(0, 5), new Vector2Int(1, 5), new Vector2Int(2, 5), new Vector2Int(3, 5), new Vector2Int(4, 5),
            //         new Vector2Int(5, 5), new Vector2Int(6, 5), new Vector2Int(7, 5), new Vector2Int(8, 5), new Vector2Int(9, 5),

            //         new Vector2Int(0, 6), new Vector2Int(1, 6), new Vector2Int(2, 6), new Vector2Int(3, 6), new Vector2Int(4, 6),
            //         new Vector2Int(5, 6), new Vector2Int(6, 6), new Vector2Int(7, 6), new Vector2Int(8, 6), new Vector2Int(9, 6),

            //         new Vector2Int(0, 7), new Vector2Int(1, 7), new Vector2Int(2, 7), new Vector2Int(3, 7), new Vector2Int(4, 7),
            //         new Vector2Int(5, 7), new Vector2Int(6, 7), new Vector2Int(7, 7), new Vector2Int(8, 7), new Vector2Int(9, 7),

            //         new Vector2Int(0, 8), new Vector2Int(1, 8), new Vector2Int(2, 8), new Vector2Int(3, 8), new Vector2Int(4, 8),
            //         new Vector2Int(5, 8), new Vector2Int(6, 8), new Vector2Int(7, 8), new Vector2Int(8, 8), new Vector2Int(9, 8),

            //         new Vector2Int(0, 9), new Vector2Int(1, 9), new Vector2Int(2, 9), new Vector2Int(3, 9), new Vector2Int(4, 9),
            //         new Vector2Int(5, 9), new Vector2Int(6, 9), new Vector2Int(7, 9), new Vector2Int(8, 9), new Vector2Int(9, 9),
            //     };
            //     foreach (Vector2Int pieceGridPos in allPositions)
            //     {
            //         Debug.Log("in foreach");
            //         IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
            //         if (piece != null && piece.Type == PieceType.Wall)
            //         {
            //             Debug.Log("piece = wall");
            //             //check and subtract coins from wallet
            //             int playerCoins = int.Parse(P2Coins.text);
            //             if (playerCoins > 50)
            //             {
            //                 Debug.Log("bombing");
            //                 //place tnt
            //                 if (HasNeighbor(pieceGridPos, PieceType.Player1))
            //                 {
            //                     boardManager.boardState.SetPiece(null, pieceGridPos);
            //                     shopManager.selectedPiece = null;
            //                     P2Coins.text = (playerCoins - 50).ToString();
            //                 }
            //             }
            //         }
            //         else
            //         {
            //             break;
            //         }
            //     }
            // }


            Vector2Int[] vector2IntValues = new Vector2Int[]
            {
                new Vector2Int(3, 8),
                new Vector2Int(2, 8),
                new Vector2Int(1, 8),
                new Vector2Int(0, 8),
                new Vector2Int(3, 9),
                new Vector2Int(2, 9),
                new Vector2Int(1, 9),
                new Vector2Int(0, 9),
                new Vector2Int(4, 9),
                new Vector2Int(5, 9),
                new Vector2Int(6, 9),
                new Vector2Int(7, 9),
                new Vector2Int(8, 9),
                new Vector2Int(9, 9)
            };
            // bool maxTanks = false;
            foreach (Vector2Int pieceGridPos in vector2IntValues)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Tank);
                }
            }
            //if extra money use for bombs
            if (int.Parse(P2Coins.text) >= 150)
            {
                Vector2Int[] bombPositions = new Vector2Int[]
                {
                    new Vector2Int(2, 7),
                    new Vector2Int(1, 7),
                    new Vector2Int(0, 7)
                };
                foreach (Vector2Int pieceGridPos in bombPositions)
                {
                    IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                    if (piece.Type == PieceType.Wall)
                    {
                        //check and subtract coins from wallet
                        int playerCoins = int.Parse(P2Coins.text);
                        if (playerCoins < 50)
                        {
                            break; // Not enough coins to place TNT
                        }
                        else
                        {
                            //place tnt
                            if (HasNeighbor(pieceGridPos, PieceType.Player2))
                            {
                                boardManager.boardState.SetPiece(null, pieceGridPos);
                                shopManager.selectedPiece = null;
                                P2Coins.text = (playerCoins - 50).ToString();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (randomNumber == 2 && int.Parse(P2Coins.text) >= 40)//sniper spam
        {
            Vector2Int[] vector2IntValues = new Vector2Int[]
            {
                new Vector2Int(3, 8),
                new Vector2Int(2, 8),
                new Vector2Int(1, 8),
                new Vector2Int(0, 8),
                new Vector2Int(3, 9),
                new Vector2Int(2, 9),
                new Vector2Int(1, 9),
                new Vector2Int(0, 9),
                new Vector2Int(4, 9),
                new Vector2Int(5, 9),
                new Vector2Int(6, 9),
                new Vector2Int(7, 9),
                new Vector2Int(8, 9),
                new Vector2Int(9, 9)
            };
            foreach (Vector2Int pieceGridPos in vector2IntValues)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Sniper);
                }
            }
            //if extra money use for bombs
            if (int.Parse(P2Coins.text) >= 150)
            {
                Vector2Int[] bombPositions = new Vector2Int[]
                {
                    new Vector2Int(2, 7),
                    new Vector2Int(1, 7),
                    new Vector2Int(0, 7)
                };
                foreach (Vector2Int pieceGridPos in bombPositions)
                {
                    IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                    if (piece.Type == PieceType.Wall)
                    {
                        //check and subtract coins from wallet
                        int playerCoins = int.Parse(P2Coins.text);
                        if (playerCoins < 50)
                        {
                            break; // Not enough coins to place TNT
                        }
                        else
                        {
                            //place tnt
                            if (HasNeighbor(pieceGridPos, PieceType.Player2))
                            {
                                boardManager.boardState.SetPiece(null, pieceGridPos);
                                shopManager.selectedPiece = null;
                                P2Coins.text = (playerCoins - 50).ToString();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (randomNumber == 3 && int.Parse(P2Coins.text) >= 40)//tank sniper spam
        {
            Vector2Int[] vector2IntValues = new Vector2Int[]
                {
                new Vector2Int(3, 8),
                new Vector2Int(2, 8),
                new Vector2Int(1, 8),
                new Vector2Int(0, 8),
                new Vector2Int(3, 9),
                new Vector2Int(2, 9),
                new Vector2Int(1, 9),
                new Vector2Int(0, 9),
                new Vector2Int(4, 9),
                new Vector2Int(5, 9),
                new Vector2Int(6, 9),
                new Vector2Int(7, 9),
                new Vector2Int(8, 9),
                new Vector2Int(9, 9)
                };
            foreach (Vector2Int pieceGridPos in vector2IntValues)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    int randomNumber2 = Random.Range(1, 3);
                    if (randomNumber2 == 1)
                    {
                        CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Sniper);
                    }
                    else if (randomNumber2 == 2)
                    {
                        CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Tank);
                    }

                }
            }
            //if extra money use for bombs
            if (int.Parse(P2Coins.text) >= 150)
            {
                Vector2Int[] bombPositions = new Vector2Int[]
                {
                new Vector2Int(2, 7),
                new Vector2Int(1, 7),
                new Vector2Int(0, 7)
                };
                foreach (Vector2Int pieceGridPos in bombPositions)
                {
                    IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                    if (piece.Type == PieceType.Wall)
                    {
                        //check and subtract coins from wallet
                        int playerCoins = int.Parse(P2Coins.text);
                        if (playerCoins < 50)
                        {
                            break; // Not enough coins to place TNT
                        }
                        else
                        {
                            //place tnt
                            if (HasNeighbor(pieceGridPos, PieceType.Player2))
                            {
                                boardManager.boardState.SetPiece(null, pieceGridPos);
                                shopManager.selectedPiece = null;
                                P2Coins.text = (playerCoins - 50).ToString();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        if (randomNumber == 4 && int.Parse(P2Coins.text) >= 40)//tank sniper spam
        {
            Vector2Int[] vector2IntValues = new Vector2Int[]
                {
                new Vector2Int(3, 8),
                new Vector2Int(2, 8),
                new Vector2Int(1, 8),
                new Vector2Int(0, 8),
                new Vector2Int(3, 9),
                new Vector2Int(2, 9),
                new Vector2Int(1, 9),
                new Vector2Int(0, 9),
                new Vector2Int(4, 9),
                new Vector2Int(5, 9),
                new Vector2Int(6, 9),
                new Vector2Int(7, 9),
                new Vector2Int(8, 9),
                new Vector2Int(9, 9)
                };
            foreach (Vector2Int pieceGridPos in vector2IntValues)
            {
                IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                if (piece == null)
                {
                    int randomNumber2 = Random.Range(1, 4);
                    if (randomNumber2 == 1)
                    {
                        CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Sniper);
                    }
                    else if (randomNumber2 == 2)
                    {
                        CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Tank);
                    }
                    else if (randomNumber2 == 3)
                    {
                        CreatePiece(pieceGridPos, PieceType.Player2, SelectedPiece.Soldier);
                    }

                }
            }
            //if extra money use for bombs
            if (int.Parse(P2Coins.text) >= 150)
            {
                Vector2Int[] bombPositions = new Vector2Int[]
                {
                new Vector2Int(2, 7),
                new Vector2Int(1, 7),
                new Vector2Int(0, 7)
                };
                foreach (Vector2Int pieceGridPos in bombPositions)
                {
                    IPiece piece = boardManager.boardState.GetPiece(pieceGridPos);
                    if (piece.Type == PieceType.Wall)
                    {
                        //check and subtract coins from wallet
                        int playerCoins = int.Parse(P2Coins.text);
                        if (playerCoins < 50)
                        {
                            break; // Not enough coins to place TNT
                        }
                        else
                        {
                            //place tnt
                            if (HasNeighbor(pieceGridPos, PieceType.Player2))
                            {
                                boardManager.boardState.SetPiece(null, pieceGridPos);
                                shopManager.selectedPiece = null;
                                P2Coins.text = (playerCoins - 50).ToString();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        int pcount = 0;
        List<IPiece> pieces = new List<IPiece>();
        List<Vector2Int> piecespaces = new List<Vector2Int>();

        int epcount = 0;
        List<Vector2Int> epiecespaces = new List<Vector2Int>();

        //Get friendly and enemy pieces
        for (int i = 0; i < 100; i++)
        {
            Vector2Int gridpos = new Vector2Int(i % 10, i / 10);
            IPiece piece = boardManager.boardState.GetPiece(gridpos);
            if (piece != null && piece.Type == PieceType.Player2)
            {
                pieces.Add(piece);
                piecespaces.Add(gridpos);
                pcount++;
            }
            else if (piece != null && piece.Type == PieceType.Player1)
            {
                epiecespaces.Add(gridpos);
                epcount++;
            }
        }

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

        Vector2Int[][][] paths = new Vector2Int[pcount][][];
        int[][] distances = new int[pcount][];
        
        for (int i = 0; i < pcount; i++)
        {
            paths[i] = new Vector2Int[epcount][];
            distances[i] = new int[epcount];
        }

        //Generate a path from each friendly piece to each enemy piece and record the length
        for (int i = 0; i < pcount; i++)
        {
            for (int j = 0; j < epcount; j++)
            {
                var grid = boardManager.boardState.Pieces.Select(piece => piece is Wall).ToArray();
                paths[i][j] = new AStar(grid, 10, 10, piecespaces[i], epiecespaces[j]).GetPath();
                distances[i][j] = paths[i][j].Length - 1;
            }
        }

        //Movement and attacking
        while (piecemoved.Any(x => x == false))
        {
            int mindis = 100;
            int minpiece = -1;
            int minepiece = -1;

            //Find the shortest path and the coresponding friendly and enemy piece that haven't been moved or defeated
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

            int move = pieces[minpiece].Movement;
            int distance = distances[minpiece][minepiece];

            //Move piece, but King only moves if an enemy is in range
            if (!(pieces[minpiece] is King && distance > 2))
            {
                if (move >= distance)
                {
                    move = distance - 1;
                }

                Vector2Int piecegridPos = piecespaces[minpiece];
                while (move > 0)
                {
                    //Move along the path
                    Vector2Int newspace = paths[minpiece][minepiece][move];
                    Vector2Int next = paths[minpiece][minepiece][move-1];

                    //Check if space is occupied
                    IPiece piece = boardManager.boardState.GetPiece(newspace);
                    if (piece == null)
                    {
                        boardManager.boardState.MovePiece(piecegridPos, newspace);
                        piecegridPos = newspace;
                        break;
                    }
                    else if((newspace - next).sqrMagnitude == 2) //Move cardinal if diagonal is blocked
                    {
                        Vector2Int testspace = new Vector2Int(newspace.x, next.y);
                        piece = boardManager.boardState.GetPiece(testspace);
                        if (piece == null)
                        {
                            boardManager.boardState.MovePiece(piecegridPos, testspace);
                            piecegridPos = testspace;
                            break;
                        }

                        testspace = new Vector2Int(next.x, newspace.y);
                        piece = boardManager.boardState.GetPiece(testspace);
                        if (piece == null)
                        {
                            boardManager.boardState.MovePiece(piecegridPos, testspace);
                            piecegridPos = testspace;
                            break;
                        }
                    }
                    else if ((newspace - next).sqrMagnitude == 1) //Move diagonal if cardinal is blocked
                    {
                        if (newspace.x == next.x)
                        {
                            if(newspace.x > 0)
                            {
                                Vector2Int testspace = new Vector2Int(newspace.x - 1, newspace.y);
                                piece = boardManager.boardState.GetPiece(testspace);
                                if (piece == null)
                                {
                                    boardManager.boardState.MovePiece(piecegridPos, testspace);
                                    piecegridPos = testspace;
                                    break;
                                }
                            }

                            if (newspace.x < 9)
                            {
                                Vector2Int testspace = new Vector2Int(newspace.x + 1, newspace.y);
                                piece = boardManager.boardState.GetPiece(testspace);
                                if (piece == null)
                                {
                                    boardManager.boardState.MovePiece(piecegridPos, testspace);
                                    piecegridPos = testspace;
                                    break;
                                }
                            }
                        }
                        else if(newspace.y == next.y)
                        {
                            if (newspace.y > 0)
                            {
                                Vector2Int testspace = new Vector2Int(newspace.x, newspace.y - 1);
                                piece = boardManager.boardState.GetPiece(testspace);
                                if (piece == null)
                                {
                                    boardManager.boardState.MovePiece(piecegridPos, testspace);
                                    piecegridPos = testspace;
                                    break;
                                }
                            }

                            if (newspace.y < 9)
                            {
                                Vector2Int testspace = new Vector2Int(newspace.x, newspace.y + 1);
                                piece = boardManager.boardState.GetPiece(testspace);
                                if (piece == null)
                                {
                                    boardManager.boardState.MovePiece(piecegridPos, testspace);
                                    piecegridPos = testspace;
                                    break;
                                }
                            }
                        }
                    }

                    move--;
                }

                var possiblePositions = GetAttackOptions(pieces[minpiece], piecegridPos);

                //Attack furthest target if possible
                if (possiblePositions.Length > 0)
                {
                    int attackdis = 0;
                    int attackpiece = minepiece;
                    Vector2Int attackpos = epiecespaces[minepiece];

                    foreach (var pos in possiblePositions)
                    {
                        int newdis = (piecegridPos - pos).sqrMagnitude;
                        if (newdis > attackdis)
                        {
                            attackdis = newdis;
                            attackpos = pos;
                            for (int i = 0; i < epcount; i++)
                            {
                                if (epiecespaces[i] == attackpos)
                                {
                                    attackpiece = i;
                                    break;
                                }
                            }

                        }
                    }

                    boardManager.boardState.AttackPiece(piecegridPos, attackpos);
                    if (boardManager.DidPlayerWin(PieceType.Player2))
                    {
                        Debug.Log($"Enemy won!");
                        SceneManager.LoadScene("EnemyWinScene");
                        break;
                    }

                    if (boardManager.boardState.GetPiece(attackpos) == null)
                    {
                        piecedefeated[attackpiece] = true;
                    }
                }
            }

            piecemoved[minpiece] = true;
        }
    }
}
