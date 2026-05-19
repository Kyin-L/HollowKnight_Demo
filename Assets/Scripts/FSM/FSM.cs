using System.Collections.Generic;

// IState.cs - ×´Ì¬½Ó¿Ú
public interface IState
{
    void OnEnter();
    void OnUpdate();
    void OnFixedUpdate();
    void OnExit();
}

public class FSM<T> where T : IState
{
    private Dictionary<System.Type, T> states = new Dictionary<System.Type, T>();
    private T currentState;

    public void AddState(T state) => states[state.GetType()] = state;

    public void ChangeState<TState>() where TState : T
    {
        currentState?.OnExit();
        currentState = states[typeof(TState)];
        currentState.OnEnter();
    }

    public void OnUpdate() => currentState?.OnUpdate();
    public void OnFixedUpdate() => currentState?.OnFixedUpdate();
}