public class Monitor : Parallel
{
    public void AddCondition(Behaviour condition)
    {
        children.Insert(0, condition);
    }

    public void AddAction(Behaviour action)
    {
        children.Add(action);
    }

    public Monitor(Policy success, Policy failure) : base(success, failure) { }
}
