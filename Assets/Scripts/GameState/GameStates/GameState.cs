public class GameState
{
    protected GameStateMachine gameStateMachine;

    public GameState(GameStateMachine _gameStateMachine)
    {
        gameStateMachine = _gameStateMachine;
    }
    
    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void Update()
    {
        
    }
    
}