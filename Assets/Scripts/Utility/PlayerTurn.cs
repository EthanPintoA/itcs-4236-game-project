using System.ComponentModel;

public enum PlayerTurn
{
    Player1,
    Player2
}

public static class PlayerTurnExtensions
{
    public static void SwitchPlayers(this ref PlayerTurn turn)
    {
        turn = turn switch
        {
            PlayerTurn.Player1 => PlayerTurn.Player2,
            PlayerTurn.Player2 => PlayerTurn.Player1,
            _ => turn
        };
    }

    public static PieceType GetPlayerPiece(this PlayerTurn turn)
    {
        return turn switch
        {
            PlayerTurn.Player1 => PieceType.Player1,
            PlayerTurn.Player2 => PieceType.Player2,
            _ => throw new InvalidEnumArgumentException(nameof(turn), (int)turn, typeof(PlayerTurn))
        };
    }
}
