


public class LoadingGameState : GameState
{
    public LoadingGameState(GameStateMachine _gameStateMachine) : base(_gameStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        
        
        
    }
    public override void Update()
    {
        base.Update();
        gameStateMachine.ChangeState(GameManager.instance.gameplayGameState);

    }
    public override void Exit()
    {
        base.Exit();
        
    }

  
}