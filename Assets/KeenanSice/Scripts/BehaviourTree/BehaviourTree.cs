public class BehaviourTree
{
    protected Behaviour root;

    public BehaviourTree(Behaviour newRoot) => root = newRoot;

    public void Tick(ref string outDebug, int branchDepth)
    {
        root.Tick(ref outDebug, branchDepth);
    }

    public void SetRoot(Behaviour newRoot)
    {
        root = newRoot;
    }
}
