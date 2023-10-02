using System;
using Godot;

namespace UIs
{
    public partial class MenusTemplate : Control
    {
        public override void _Ready()
        {
            ConnectButton();
        }

        private void ConnectButton()
        {
            string targetMethod = $"_on_{Name}_UIButton_down";
            Managers.MainManager mainNode = (Managers.MainManager)GetTree().GetNodesInGroup("Main")[0];
            
            foreach (BaseButton button in GetTree().GetNodesInGroup($"{Name}Button"))
            { 
                Callable targetCallable = new Callable(mainNode, targetMethod);
                button.ButtonDown += () => targetCallable.Call(new Variant[]{button.Name});
            }
        }
       
        protected void SetVisible(bool visible)
        {
            Visible = visible;
        } 

    }
}