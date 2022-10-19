using Godot;

namespace UIs
{
    public class TimedModeMenuUI : MenusTemplate
    {
        public override void _Ready()
        {
            base._Ready();

            string targetMethod = $"_on_{Name}_BackButton_down";
            Godot.Object mainNode = (Godot.Object)GetTree().GetNodesInGroup("Main")[0];

            GetNode<TextureButton>("BackButton").Connect("button_down", mainNode, targetMethod);
        }

    }
}
