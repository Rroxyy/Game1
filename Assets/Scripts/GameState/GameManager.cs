using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public GameStateMachine gameStateMachine { get;private set; }
    
    public BootGameState bootGameState{ get;private set; }
    public LoadingGameState loadingGameState{ get;private set; }
    
    
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        gameStateMachine = new GameStateMachine();

        bootGameState = new BootGameState(gameStateMachine);
        loadingGameState = new LoadingGameState(gameStateMachine);
        
        gameStateMachine.Initialize(bootGameState);
    }


    private void Update()
    {
        gameStateMachine.Update();
    }
}