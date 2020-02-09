using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    BehaviourTree bt = new BehaviourTree();

    void Start()
    {
        Sequence sequence = new Sequence(
            new DebugBehaviour("Test", Status.Success),
            new DebugBehaviour("Snore", Status.Success),
            new DebugBehaviour("Woah were working", Status.Failure),
            new DebugBehaviour(":C", Status.Success)
            );

        bt.AddRoot(sequence);
    }

    void Update()
    {
        bt.Tick();
    }
}



public class DebugBehaviour : Behaviour
{
    protected string debugMessage;
    protected Status returnStatus;

    public DebugBehaviour(string newDebugMessage, Status newReturnStatus)
    {
        debugMessage = newDebugMessage;
        returnStatus = newReturnStatus;
    }

    public DebugBehaviour()
    {
        debugMessage = "Nil";
        returnStatus = Status.Failure;
    }


    public override Status Update()
    {
        Debug.Log(debugMessage);

        return returnStatus;
    }
}
