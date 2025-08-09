using Godot;
using System;
using System.Collections.Generic;
using Knight.State;

namespace Knight.State.StateMachine;

[GlobalClass]
public partial class StateMachine : Node
{
    private Dictionary<String, State> stateDict = [];
    private State currentState;

    [Export]
    private String initialStateName;

    public override void _Ready()
    {
        base._Ready();

        // 将所有的state子节点注册到状态机中
        foreach (var child in GetChildren())
        {
            if (child is State)
            {
                RegisterState((State)child);
            }
        }
        if (stateDict.ContainsKey(initialStateName))
        {
            currentState = stateDict[initialStateName];
        }
    }

    public void StartMachine() {
        if (currentState != null)
        {
            currentState.Enter();
        }
        else
        {
            GD.Print("No initial State");
        }

    }


    private void RegisterState(State state)
    {
        try
        {
            stateDict.Add(state.name, state);
            state.StateChange += OnChangeState;
        }
        catch
        {
            GD.Print("state add error, which state name is ", state.name);
        }
    }

    public void StateMachineProcess(double delta)
    {
        if (currentState != null)
        {
            currentState.StateProcess(delta);
        }
    }

    public void StateMachinePhysicsProcess(double delta)
    {
        if (currentState != null)
        {
            currentState.StatePhysicsProcess(delta);
        }
    }


    // State Signal Handler

    public void OnChangeState(String stateName) {

        if (stateDict.ContainsKey(stateName) && stateDict[stateName] != currentState)
        {
            currentState.Exit();
            currentState = stateDict[stateName];
            currentState.Enter();
        }
        else
        {
            GD.Print("State Not Found!!!. stateName = ", stateName);
        }
    }


}
