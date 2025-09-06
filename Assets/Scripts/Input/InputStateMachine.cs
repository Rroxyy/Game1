

public class InputStateMachine
{
    public InputState currentState{ get; private set; }

    public void Initialize(InputState _inputState)
    {
        this.currentState = _inputState;
        _inputState.OnEnter();
    }

    public void ChangeInputState(InputState _inputState)
    {
        currentState.OnExit();
        currentState = _inputState;
        _inputState.OnEnter();
    }

    public void Update()
    {
        currentState.OnUpdate();
    }    
}
