using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public SelectedPiece? selectedPiece;

    public void OnPieceClicked(string selectedPiece)
    {
        this.selectedPiece = selectedPiece switch
        {
            "Tnt" => SelectedPiece.Tnt,
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
    Tnt
}
