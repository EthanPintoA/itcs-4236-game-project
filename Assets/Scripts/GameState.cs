public enum GameState
{
    P1Turn,
    P2Turn,
    P1Selected,
    P2Selected,
    P1Attack,
    P2Attack
}

// Write an extension method for the GameState enum to switch between the two players turns

public static class GameStateExtensions
{
    /// <summary>
    /// Returns true if the state is either P1Turn or P2Turn.
    /// </summary>
    public static bool IsTurn(this GameState state)
    {
        return state == GameState.P1Turn || state == GameState.P2Turn;
    }

    /// <summary>
    /// Returns true if the state is either P1Selected or P2Selected.
    /// </summary>
    public static bool IsSelected(this GameState state)
    {
        return state == GameState.P1Selected || state == GameState.P2Selected;
    }

    /// <summary>
    /// Returns true if the state is either P1Attack or P2Attack.
    /// </summary>
    public static bool IsAttack(this GameState state)
    {
        return state == GameState.P1Attack || state == GameState.P2Attack;
    }

    /// <summary>
    /// Switches the player's turn.
    /// Only works when the state is either P1Turn or P2Turn.
    /// <br/>
    /// E.g. P1Turn -> P2Turn
    /// </summary>
    public static GameState GetSwitchPlayersTurns(this GameState state)
    {
        return state switch
        {
            GameState.P1Turn => GameState.P2Turn,
            GameState.P2Turn => GameState.P1Turn,
            _ => state
        };
    }

    /// <summary>
    /// Toggles between turn, selected, and attack states.
    /// <br/>
    /// E.g. P1Turn -> P1Selected -> P1Attack
    /// </summary>
    public static GameState GetNextState(this GameState state)
    {
        return state switch
        {
            GameState.P1Turn => GameState.P1Selected,
            GameState.P2Turn => GameState.P2Selected,
            GameState.P1Selected => GameState.P1Attack,
            GameState.P2Selected => GameState.P2Attack,
            GameState.P1Attack => GameState.P1Turn,
            GameState.P2Attack => GameState.P2Turn,
            _ => state
        };
    }

    /// <summary>
    /// Returns to the current player's turn.
    /// <br/>
    /// E.g. P1Selected -> P1Turn
    /// </summary>
    public static GameState GetCurrentTurn(this GameState state)
    {
        return state switch
        {
            GameState.P1Selected => GameState.P1Turn,
            GameState.P2Selected => GameState.P2Turn,
            GameState.P1Attack => GameState.P1Turn,
            GameState.P2Attack => GameState.P2Turn,
            _ => state
        };
    }
}