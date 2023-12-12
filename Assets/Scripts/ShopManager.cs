using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public SelectedPiece? selectedPiece;

    public void OnPieceClicked(string selectedPiece)
    {
        this.selectedPiece = selectedPiece switch
        {
            "Tnt" => SelectedPiece.Tnt,
            "tank" => SelectedPiece.Tank,
            "sniper" => SelectedPiece.Sniper,
            "soldier" => SelectedPiece.Soldier,
            "Helicopter" => SelectedPiece.Helicopter,
            _ => null
        };

        if (this.selectedPiece == null)
        {
            Debug.LogError($"Piece {selectedPiece} does not exist");
        }
        else
        {
            Debug.Log($"Piece {selectedPiece} selected");
        }
    }
}

/// <summary>
/// The name of the piece that is being bought.
/// </summary>
public enum SelectedPiece
{
    Tnt,
    Soldier,
    Sniper,
    Tank,
    Helicopter,
    King
}
