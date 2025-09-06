

/// <summary>
/// 启动游戏状态
/// </summary>
public class BootGameState : GameState
{
    public BootGameState(GameStateMachine _gameStateMachine) : base(_gameStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        
        
        
        gameStateMachine.ChangeState(GameManager.instance.loadingGameState);
    }
}