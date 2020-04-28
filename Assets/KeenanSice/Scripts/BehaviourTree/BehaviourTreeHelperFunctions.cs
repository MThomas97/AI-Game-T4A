using System;

public static class BehaviourTreeHelperFunctions
{
    //KS - These functions will make the tree easier to read.
    public static BehaviourTree behaviourTree(Behaviour newRoot)
    {
        return new BehaviourTree(newRoot);
    }

    public static Condition condition(Func<bool> method, Behaviour newSuccessBehaviour, Behaviour newFailureBehaviour)
    {
        return new Condition(method, newSuccessBehaviour, newFailureBehaviour);
    }

    public static Composite composite(params Behaviour[] newChildren)
    {
        return new Composite(newChildren);
    }

    public static KSAction action(Func<bool> method)
    {
        return new KSAction(method);
    }

    public static KSAction action(bool outcome = true)
    {
        return new KSAction(() => outcome);
    }

    public static ActiveSelector activeSelector(params Behaviour[] newChildren)
    {
        return new ActiveSelector(newChildren);
    }

    public static Decorator decorator(Behaviour newChild)
    {
        return new Decorator(newChild);
    }

    public static Filter filter(params Behaviour[] newChildren)
    {
        return new Filter(newChildren);
    }

    public static Sequence sequence(params Behaviour[] newChildren)
    {
        return new Sequence(newChildren);
    }

    public static Monitor monitor(Policy success, Policy failure)
    {
        return new Monitor(success, failure);
    }

    public static Parallel parallel(Policy success, Policy failure)
    {
        return new Parallel(success, failure);
    }

    public static Repeat repeat(Behaviour newChild, int newCounterLimit)
    {
        return new Repeat(newChild, newCounterLimit);
    }

    public static Selector selector(params Behaviour[] newChildren)
    {
        return new Selector(newChildren);
    }
}
