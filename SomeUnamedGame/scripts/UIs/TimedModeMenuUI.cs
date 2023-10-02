using Godot;

namespace UIs
{
    public partial class TimedModeMenuUI : MenusTemplate
    {
        public override void _Ready()
        {
            base._Ready();

            Managers.MainManager mainNode = (Managers.MainManager)GetTree().GetNodesInGroup("Main")[0];

            Callable targetCallableBack = new Callable(mainNode, $"_on_{Name}_BackButton_down");
            GetNode<Button>("BackButton").ButtonDown += () => targetCallableBack.Call();
            
            Callable targetCallableStats = new Callable(mainNode, $"_on_{Name}_StatsButton_down");
            GetNode<Button>("StatsButton").ButtonDown += () => targetCallableStats.Call();
        }

    }
}
