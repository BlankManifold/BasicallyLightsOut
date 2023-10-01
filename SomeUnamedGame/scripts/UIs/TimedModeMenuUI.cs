using Godot;

namespace UIs
{
    public partial class TimedModeMenuUI : MenusTemplate
    {
        public override void _Ready()
        {
            base._Ready();

            Managers.MainManager mainNode = (Managers.MainManager)GetTree().GetNodesInGroup("Main")[0];

            string targetMethod = $"_on_{Name}_BackButton_down";
            Callable targetCallable = new Callable(mainNode, targetMethod);
            GetNode<TextureButton>("BackButton").ButtonDown += () => targetCallable.Call();
            
            targetMethod = $"_on_{Name}_StatsButton_down";
            targetCallable = new Callable(mainNode, targetMethod);
            GetNode<TextureButton>("StatsButton").ButtonDown += () => targetCallable.Call();
        }

    }
}
