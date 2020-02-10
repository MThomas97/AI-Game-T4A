using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    BehaviourTree bt;

    void Start()
    {
        bt = new BehaviourTree(
            new Sequence(
                new DebugBehaviour("Test", Status.Success),
                new Condition(TestFunc,
                    new DebugBehaviour("We passed!", Status.Success),
                    new DebugBehaviour("We failed :C", Status.Failure)
                ),
                new DebugBehaviour("Snore", Status.Success),
                new DebugBehaviour("Woah were working", Status.Failure),
                new DebugBehaviour(":C", Status.Success)
            )
        );
    }

    void Update()
    {
        bt.Tick();
    }

    bool TestFunc()
    {
        return false;
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
