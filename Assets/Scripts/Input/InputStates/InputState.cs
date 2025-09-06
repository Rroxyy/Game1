using UnityEngine;


public class InputState:MonoBehaviour
{

    /// <summary>
    /// 状态进入时调用
    /// </summary>
    public virtual void OnEnter() 
    {
        // 默认实现可以为空
    }

    /// <summary>
    /// 状态退出时调用
    /// </summary>
    public virtual void OnExit() 
    {
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    public virtual void OnUpdate() 
    {
    }

   

   
    
}