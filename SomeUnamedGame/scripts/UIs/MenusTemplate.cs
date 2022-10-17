using System;
using Godot;

namespace UIs
{
    public class MenusTemplate : Control
    {
        public override void _Ready()
        {
            ConnectButton();
        }

        private void ConnectButton()
        {
            string targetMethod = $"_on_{Name}_UIButton_down";
            Godot.Object mainNode = (Godot.Object)GetTree().GetNodesInGroup("Main")[0];

            foreach (TextureButton button in GetTree().GetNodesInGroup($"{Name}Button"))
            {
                button.Connect("button_down", mainNode, targetMethod, new Godot.Collections.Array() { button.Name });
            }
        }

        protected new void SetVisible(bool visible)
        {
            Visible = visible;
        } 

    }
}