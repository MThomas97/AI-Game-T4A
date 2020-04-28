using System;

//KS - Action is used to perform the given method. Note: It's called a KSAction as I ran into a conflict with the C# type Action.
public class KSAction : Behaviour
{
    protected Func<bool> actionMethod;

    public KSAction(Func<bool> method) => actionMethod = method;

    public override Status Update(ref string outDebug, int branchDepth)
    {
        outDebug = outDebug.Remove(outDebug.Length - 1);
        outDebug += "(" + actionMethod.Method.Name + ")\n";

        return actionMethod() ? Status.Success : Status.Failure;
    }
}
