using Godot;
using System;
using System.Data;

namespace Knight.State;

[GlobalClass]
public partial class State : Node
{

    [Export]
    public String name { private set; get; }

    [Signal]
    public delegate void StateChangeEventHandler(String stateName);


    public virtual void Enter()
    {
    }
    public virtual void Exit() { }

    public virtual void StatePhysicsProcess(double delta)
    {

    }
    public virtual void StateProcess(double delta)
    {

    }

}

