public class MonsterBTRunner
{
    IBTNode _rootNode;

    public MonsterBTRunner(IBTNode rootNode)
    {
        _rootNode = rootNode;
    }   

    public void Execute()
    {
        _rootNode.Evaluate();
    }
}
