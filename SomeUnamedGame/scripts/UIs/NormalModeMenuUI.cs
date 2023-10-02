using Godot;

namespace UIs
{

    public partial class NormalModeMenuUI : MenusTemplate
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
        private Godot.Collections.Array<string> _levelsTypeArray = new Godot.Collections.Array<string>()
        {
            "tutorial",
            "easy",
            "medium",
            "hard"
        };

        [Export]
        private PackedScene _levelsSelectionButtonScene;

        [Export]
        private int _cols = 3;

        [Export]
        private Vector2 _separation = new Vector2(20, 20);

        [Export]
        private Vector2 _buttonSize = new Vector2(100, 100);


        public override void _Ready()
        {
            Visible = false;

            GenerateLevelsIcons();

            base._Ready();

            string targetMethod = $"_on_{Name}_BackButton_down";

            Managers.MainManager mainNode = (Managers.MainManager)GetTree().GetNodesInGroup("Main")[0];
            Callable targetCallable = new Callable(mainNode, targetMethod);
            GetNode<Button>("BackButton").ButtonDown += () => targetCallable.Call();
        }


        private void GenerateLevelsIcons()
        {
            string buttonGroupName = $"{Name}Button";

            float sizeShift = _cols * _buttonSize[0] / 2;
            float sepShift = (_cols - 1) * _separation[0] / 2;
            Vector2 startPosition = new Vector2(GetViewport().GetVisibleRect().Size[0] / 2 - sizeShift - sepShift, 200);

            Vector2 shift = _buttonSize + _separation;
            Vector2 position = startPosition;

            foreach (string type in _levelsTypeArray)
            {
                for (int i = 0; i < _levelsTypeDict[type]; i++)
                {
                    position[0] = startPosition[0] + (i % _cols) * shift[0];
                    position[1] = startPosition[1] + ((int)(i / _cols)) * shift[1];

                    LevelSelectionButton levelSelectionButton = _levelsSelectionButtonScene.Instantiate<LevelSelectionButton>();
                    levelSelectionButton.Name = $"{type}_{i}";
                    levelSelectionButton.AddToGroup(buttonGroupName);

                    AddChild(levelSelectionButton);

                    levelSelectionButton.UpdateLabelText(levelSelectionButton.Name);
                    levelSelectionButton.UpdateButtonSize(_buttonSize);
                    levelSelectionButton.GlobalPosition = position;
                }

                startPosition[1] = startPosition[1] + shift[1];

            }

        }


    }
}