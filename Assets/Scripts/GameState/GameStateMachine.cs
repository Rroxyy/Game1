public class GameStateMachine
{
    public GameState currentState{ get; private set; }

    public void Initialize(GameState initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    public void ChangeState(GameState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState.Update();
    }
    
}