using Godot;
using System;

namespace UIs
{
    partial class PuzzleUI : MenusTemplate
    {
        public enum State { START, INSPECTING, TIMEDSOLVING, NORMALSOLVING, TIMEDSOLVED, NORMALSOLVED }


        private Label _movesLabel;
        private Label _timerLabel;
        private Label _solvedLabel;
        private Control _normalModeUI;
        private Control _timedModeUI;
        private TextureButton _startButton;
        private Timer _inspectionTimer;
        private float _inspectionTime = 10.99f;
        private float _startTime;
        private State _state;

        [Signal]
        delegate void ChangeVisibleEventHandler(bool visible);


        public override void _Ready()
        {

            base._Ready();

            _normalModeUI = GetNode<Control>("NormalModeUI");
            _timedModeUI = GetNode<Control>("TimedModeUI");

            _movesLabel = _normalModeUI.GetNode<Label>("MovesLabel");
            _timerLabel = _timedModeUI.GetNode<Label>("TimerLabel");
            _solvedLabel = _timedModeUI.GetNode<Label>("%SolvedLabel");

            _startButton = _timedModeUI.GetNode<TextureButton>("StartButton");
            _inspectionTimer = _timedModeUI.GetNode<Timer>("Timer");
            _inspectionTimer.WaitTime = _inspectionTime;

        }
        public override void _Process(double delta)
        {
            switch (_state)
            {
                case State.INSPECTING:
                    UpdateInspectionTimerLabel();
                    break;

                case State.TIMEDSOLVING:
                    UpdateTimerLabel();
                    break;

                default:
                    break;
            }
        }


        public void ActiveAs(Globals.Mode mode)
        {
            Visible = true;

            switch (mode)
            {
                case Globals.Mode.NORMAL:
                    _normalModeUI.Visible = true;
                    _timedModeUI.Visible = false;
                    ActiveState(State.NORMALSOLVING);
                    break;

                case Globals.Mode.TIMED:
                    _timedModeUI.Visible = true;
                    _normalModeUI.Visible = false;
                    ActiveState(State.START);
                    break;

                default:
                    break;
            }
        }
        public void ActiveState(State state)
        {
            _state = state;

            switch (state)
            {
                case State.START:
                    EmitSignal(nameof(ChangeVisible), false);
                    _timerLabel.Visible = false;
                    _solvedLabel.Visible = false;
                    _inspectionTimer.WaitTime = _inspectionTime;
                    _startButton.Visible = true;
                    break;

                case State.INSPECTING:
                    _startButton.Visible = false;
                    _timerLabel.Visible = true;
                    break;

                case State.TIMEDSOLVING:
                    SetStartTime();
                    break;

                case State.NORMALSOLVING:
                    EmitSignal(nameof(ChangeVisible), true);
                    break;

                case State.TIMEDSOLVED:
                    EmitSignal(nameof(ChangeVisible), false);
                    _timerLabel.Visible = false;
                    _solvedLabel.Text = $"Solved in\n{_timerLabel.Text}";
                    _solvedLabel.Visible = true;
                    break;


                default:
                    break;
            }

        }
        public void Disactive()
        {
            _timedModeUI.Visible = false;
            _normalModeUI.Visible = false;
            Visible = false;
            EmitSignal(nameof(ChangeVisible), false);
        }


        private void UpdateInspectionTimerLabel()
        {
            double timeLeft = _inspectionTimer.TimeLeft;
            int secLeft = (int)timeLeft;
            _timerLabel.Text = secLeft.ToString();
        }
        private void SetStartTime() => _startTime = OS.GetTicksMsec();
        private void UpdateTimerLabel()
        {
            float time = OS.GetTicksMsec() - _startTime;
            float seconds = time / 1000;

            TimeSpan formatTime = TimeSpan.FromSeconds(seconds);

            if (seconds > 60)
            {
                _timerLabel.Text = formatTime.ToString(@"mm\:ss\.ff");
                return;
            }

            _timerLabel.Text = formatTime.ToString(@"s\.ff");
        }


        public void _on_Puzzle_Solved()
        {
            switch (_state)
            {
                case State.TIMEDSOLVING:
                    ActiveState(State.TIMEDSOLVED);
                    break;
                case State.NORMALSOLVING:
                    ActiveState(State.NORMALSOLVED);
                    break;
            }
        }
        public void _on_Puzzle_ChangedMovesCounter(int moveCounter)
        {
            if (_state == State.INSPECTING && moveCounter == 1)
                ActiveState(State.TIMEDSOLVING);

            _movesLabel.Text = moveCounter.ToString();
        }
        public void _on_StartButton_button_down()
        {
            _inspectionTimer.Start();
            ActiveState(State.INSPECTING);

            EmitSignal(nameof(ChangeVisible), true);
        }
        public void _on_Timer_timeout()
        {
            if (_state == State.INSPECTING)
                ActiveState(State.TIMEDSOLVING);
        }


    }
}