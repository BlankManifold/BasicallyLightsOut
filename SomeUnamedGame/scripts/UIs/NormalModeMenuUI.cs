using Godot;

namespace UIs
{

    public class NormalModeMenuUI : MenusTemplate
    {
        [Export]
        private Godot.Collections.Dictionary<string, int> _levelsTypeDict = new Godot.Collections.Dictionary<string, int>()
        {
            {"tutorial", 0},
            {"easy", 0},
            {"medium", 0},
            {"hard", 0}
        };

        [Export]
        private PackedScene _levelsSelectionButtonScene;

        [Export]
        private int _cols = 3;

        [Export]
        private Vector2 separation = new Vector2(20, 20);

        [Export]
        private Vector2 buttonSize = new Vector2(100, 100);


        public override void _Ready()
        {
            Visible = false;

            string buttonGroupName = $"{Name}Button";

            float sizeShift = _cols * buttonSize[0] / 2;
            float sepShift = (_cols - 1) * separation[0] / 2;
            Vector2 startPosition = new Vector2(GetViewport().GetVisibleRect().Size[0] / 2 - sizeShift - sepShift, 200);

            float shiftx = buttonSize[0] + separation[0];
            float shifty = buttonSize[1] + separation[1];
            Vector2 position = startPosition;

            foreach (var type in _levelsTypeDict.Keys)
            {
                for (int i = 0; i < _levelsTypeDict[type]; i++)
                {
                    position[0] = startPosition[0] + (i % _cols) * shiftx;
                    position[1] = startPosition[1] + ((int)(i / _cols)) * shifty;

                    LevelSelectionButton levelSelectionButton = _levelsSelectionButtonScene.Instance<LevelSelectionButton>();
                    levelSelectionButton.Name = $"{type}_{i}";
                    levelSelectionButton.AddToGroup(buttonGroupName);

                    AddChild(levelSelectionButton);

                    levelSelectionButton.UpdateLabelText(levelSelectionButton.Name);
                    levelSelectionButton.UpdateButtonSize(buttonSize);
                    levelSelectionButton.RectGlobalPosition = position;
                }

                startPosition[1] = startPosition[1] + shifty;
            }

            base._Ready();

            string targetMethod = $"_on_{Name}_BackButton_down";
            Godot.Object mainNode = (Godot.Object)GetTree().GetNodesInGroup("Main")[0];

            GetNode<TextureButton>("BackButton").Connect("button_down", mainNode, targetMethod);

        }


    }
}