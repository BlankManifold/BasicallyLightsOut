using Godot;


namespace Managers
{
    partial class MainManager : Node2D
    {
        // [Export]
        // private bool _devToolsEnabled = false;
        private NormalModeManager _normalPuzzle;
        private TimedModeManager _timedPuzzle;
        private UIs.PuzzleUI _puzzleUI;
        private UIs.OptionsUI _optionsUI;
        private UIs.NormalModeMenuUI _normalModeMenu;
        private UIs.TimedModeMenuUI _timedModeMenu;
        private UIs.MenusTemplate _mainMenu;
        private UIs.MenusTemplate _statsMenu;
        private CanvasLayer _puzzleLayer;
        //private Label _debugLabel;
        private Globals.Mode _mode;



        public override void _Ready()
        {
            GD.Randomize();

            Globals.GridInfoManager.InitAreaSize(GetViewport().GetVisibleRect().Size, new Vector3(50, 200, 100), new Vector2(1, 1));
            _puzzleUI = GetNode<UIs.PuzzleUI>("%PuzzleUI");
            _optionsUI = GetNode<UIs.OptionsUI>("%OptionsUI");
            _puzzleLayer = GetNode<CanvasLayer>("PuzzleLayer");
            _mainMenu = GetNode<UIs.MenusTemplate>("%MainMenuUI");
            _statsMenu = GetNode<UIs.MenusTemplate>("%StatsMenuUI");
            _normalModeMenu = GetNode<UIs.NormalModeMenuUI>("%NormalModeMenuUI");
            _timedModeMenu = GetNode<UIs.TimedModeMenuUI>("%TimedModeMenuUI");

            _normalPuzzle = GetNode<NormalModeManager>("%NormalPuzzle"); ;
            _timedPuzzle = GetNode<TimedModeManager>("%TimedPuzzle"); ;
            
            _puzzleUI.Visible = false;
            _normalModeMenu.Visible = false;
            _timedModeMenu.Visible = false;
            _optionsUI.Visible = false;


            // if (_devToolsEnabled)
            // {
            //     _debugLabel = new Label();
            //     _puzzleLayer.AddChild(_debugLabel);
            //     _debugLabel.RectScale = new Vector2(2, 2);
            //     PackedScene puzzleCreationUIScene = ResourceLoader.Load<PackedScene>(Globals.Paths.PuzzleCreationUIScenePath);
            //     DevTools.PuzzleCreationUI puzzleCreationUI = puzzleCreationUIScene.Instantiate<DevTools.PuzzleCreationUI>();
            //     _puzzleLayer.AddChild(puzzleCreationUI);

            //     puzzleCreationUI.GetNode<DevTools.PuzzleCreationManager>("PuzzleCreationManager").Init(_puzzleManager);
            // }
        }



        public void _on_MainMenuUI_UIButton_down(string name)
        {
            switch (name)
            {
                case "NormalMode":
                    _mode = Globals.Mode.NORMAL;
                    _mainMenu.Visible = false;
                    _normalModeMenu.Visible = true;
                    break;

                case "TimedMode":
                    _mode = Globals.Mode.TIMED;
                    _mainMenu.Visible = false;
                    _timedModeMenu.Visible = true;
                    break;

                case "Options":
                    break;

                default:
                    break;
            }
        }
        public void _on_PuzzleUI_UIButton_down(string name)
        {
            switch (name)
            {
                case "RestartButton":
                    _normalPuzzle.Restart();
                    break;

                case "ResetButton":
                    _puzzleUI.ActiveState(UIs.PuzzleUI.State.START);
                    _timedPuzzle.CreateSequence();
                    break;

                case "OptionsButton":
                    break;

                case "ConvolutionButton":
                    _timedPuzzle.CreateConvolution(Globals.EntropyManager.ConvolutionSquareOverlap2x2);
                    break;
                case "NNMFButton":
                    _timedPuzzle.CreateConvolution(Globals.EntropyManager.ConvolutionNNOverlap);
                    break;

                case "BackButton":
                    _puzzleUI.Disactive();

                    switch (_mode)
                    {
                        case Globals.Mode.NORMAL:
                            _normalPuzzle.ClearAll();
                            _normalModeMenu.Visible = true;
                            break;

                        case Globals.Mode.TIMED:
                            _timedPuzzle.ClearAll();
                            _timedModeMenu.Visible = true;
                            break;
                    }
                    break;
            }
        }
        public void _on_NormalModeMenuUI_UIButton_down(string name)
        {
            _normalModeMenu.Visible = false;
            _puzzleUI.ActiveAs(Globals.Mode.NORMAL);

            string[] typeAndId = name.Split('_');
            string type = typeAndId[0];
            int id = typeAndId[1].ToInt();

            Resource puzzleDataRes = ResourceLoader.Load(Globals.Paths.GetPuzzleDataResourcePath(type, id));
            _normalPuzzle.InitPuzzle(puzzleDataRes);
            _normalPuzzle.CreateSequence();
        }
        public void _on_TimedModeMenuUI_UIButton_down(string name)
        {
            _timedModeMenu.Visible = false;
            _puzzleUI.ActiveAs(Globals.Mode.TIMED);

            string[] dims = name.Split('x');
            int dim = dims[0].ToInt();
            
            _timedPuzzle.InitPuzzle(dim);
            _timedPuzzle.CreateSequence();
        }
        public void _on_NormalModeMenuUI_BackButton_down()
        {
            _mainMenu.Visible = true;
            _normalModeMenu.Visible = false;
        }
        public void _on_TimedModeMenuUI_BackButton_down()
        {
            _mainMenu.Visible = true;
            _timedModeMenu.Visible = false;
        }
        public void _on_TimedModeMenuUI_StatsButton_down()
        {
            _timedModeMenu.Visible = false;
            _statsMenu.Visible = true;
        }



        // public void _on_AddBadConfig_button_down()
        // {
        //     string code = Globals.Utilities.CreateBinaryCode(_timedPuzzle.CurrentConfiguration);
        //     File file = new File();

        //     file.Open(Globals.Paths.BadConfig4x4path, File.ModeFlags.ReadWrite);
        //     file.SeekEnd();
        //     file.StoreLine(code);
        //     file.Close();
        // }
    }

}