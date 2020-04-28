public class Decorator : Behaviour
{
    protected Behaviour child;

    public Decorator(Behaviour newChild) => child = newChild;
}
