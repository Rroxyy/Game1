


public class LoadingGameState : GameState
{
    public LoadingGameState(GameStateMachine _gameStateMachine) : base(_gameStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        HexCellMapManager.instance.GenerateHexMap();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}