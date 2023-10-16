using Godot;

using GArrayInt = Godot.Collections.Array<int>;


namespace Managers
{
    public partial class ScrambleManager
    {
        private int[] _range = null;
        public int[] Range { get; set; }
        private Godot.Collections.Dictionary<string, bool> _badConfig4x4Dict = new Godot.Collections.Dictionary<string, bool>() { };

        public ScrambleManager()
        {
            // LoadBadConfig();
        }


        public void CreateRandomScramble(TimedModeManager timedPuzzle)
        {
            int numberOfPieces = timedPuzzle.Dim * timedPuzzle.Dim;
            int scrambleLenght = (int)(numberOfPieces) / 2;

            if (_range != null)
                scrambleLenght = Globals.RandomManager.rnd.Next(scrambleLenght - _range[0], scrambleLenght + _range[1]);

            GArrayInt testConfigScrambled = new GArrayInt() { };
            testConfigScrambled.Resize(numberOfPieces);

            do
            {
                timedPuzzle.Scramble.Resize(numberOfPieces);
                for (int i = 0; i < numberOfPieces; i++)
                {
                    timedPuzzle.Scramble[i] = i;
                    timedPuzzle.StartConfiguration[i] = 1;
                }

                int idToRemove;
                for (int i = 0; i < scrambleLenght; i++)
                {
                    idToRemove = Globals.RandomManager.rnd.Next(0, timedPuzzle.Scramble.Count);
                    timedPuzzle.StartConfiguration[timedPuzzle.Scramble[idToRemove]] = 0;
                    timedPuzzle.Scramble.RemoveAt(idToRemove);
                }

                FromScrambleConfigToConfig(ref testConfigScrambled, timedPuzzle.FrameDimensions, timedPuzzle.StartConfiguration);
                timedPuzzle.SetBinaryCode(testConfigScrambled);
            }
            while (
                    Globals.SymmetriesManager.MainDiagonalCheckScramble(timedPuzzle.StartConfiguration, timedPuzzle.FrameDimensions, scrambleLenght, 0.85f) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheckScramble(timedPuzzle.StartConfiguration, timedPuzzle.FrameDimensions, scrambleLenght, 0.85f)
                );

            timedPuzzle.StartConfiguration = testConfigScrambled;
        }
        public void CreateRandomScrambleRC(TimedModeManager timedPuzzle)
        {
            int numberOfPieces = timedPuzzle.Dim * timedPuzzle.Dim;
            int scrambleLenght = (int)(numberOfPieces) / 2;

            if (_range != null)
                scrambleLenght = Globals.RandomManager.rnd.Next(scrambleLenght - _range[0], scrambleLenght + _range[1]);


            timedPuzzle.Scramble.Clear();

            GArrayInt testConfigScrambled = new GArrayInt() { };
            testConfigScrambled.Resize(numberOfPieces);
            for (int i = 0; i < numberOfPieces; i++)
                timedPuzzle.StartConfiguration[i] = 0;


            do
            {
                timedPuzzle.Scramble.Clear();
                for (int i = 0; i < numberOfPieces; i++)
                    timedPuzzle.StartConfiguration[i] = 0;


                for (int row = 0; row < timedPuzzle.Dim; row++)
                {
                    int col = Globals.RandomManager.rnd.Next(0, timedPuzzle.Dim);
                    int id = Globals.Utilities.CoordsToId(row, col, timedPuzzle.FrameDimensions);

                    timedPuzzle.Scramble.Add(id);
                    timedPuzzle.StartConfiguration[id] = 1;
                }

                for (int col = 0; col < timedPuzzle.Dim; col++)
                {
                    int id, row, cont = 0;
                    do
                    {
                        row = Globals.RandomManager.rnd.Next(0, timedPuzzle.Dim);
                        id = Globals.Utilities.CoordsToId(row, col, timedPuzzle.FrameDimensions);
                        cont++;
                    } while (timedPuzzle.Scramble.Contains(id) && cont < timedPuzzle.Dim);

                    timedPuzzle.Scramble.Add(id);
                    timedPuzzle.StartConfiguration[id] = 1;
                }

                FromScrambleConfigToConfig(ref testConfigScrambled, timedPuzzle.FrameDimensions, timedPuzzle.StartConfiguration);
                timedPuzzle.SetBinaryCode(testConfigScrambled);
            }
            while (
                    IsBadConfig(timedPuzzle.Dim, timedPuzzle.GetBinaryCode()) ||
                    Globals.SymmetriesManager.MainDiagonalCheckScramble(timedPuzzle.StartConfiguration, timedPuzzle.FrameDimensions, scrambleLenght, 1f) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheckScramble(timedPuzzle.StartConfiguration, timedPuzzle.FrameDimensions, scrambleLenght, 1f) ||
                    Globals.SymmetriesManager.MainDiagonalCheck(testConfigScrambled, timedPuzzle.FrameDimensions, 1f) ||
                    Globals.SymmetriesManager.SecondaryDiagonalCheck(testConfigScrambled, timedPuzzle.FrameDimensions, 1f) ||
                    Globals.SymmetriesManager.VerticalCheck(testConfigScrambled, timedPuzzle.FrameDimensions, 1f) ||
                    Globals.SymmetriesManager.HorizontalCheck(testConfigScrambled, timedPuzzle.FrameDimensions, 1f)
                );


            timedPuzzle.StartConfiguration = testConfigScrambled;
        }



        private static void FromScrambleConfigToConfig(ref GArrayInt config, Vector2 frameDimensions, GArrayInt scrambleConfig)
        {
            for (int i = 0; i < config.Count; i++)
            {
                int cont = scrambleConfig[i];
                foreach (int id in Globals.Utilities.GetNeighbours(i, frameDimensions))
                    cont += scrambleConfig[id];

                if (cont % 2 == 0)
                    config[i] = 0;
                else
                    config[i] = 1;
            }
        }
        private void LoadBadConfig()
        {
            FileAccess file = FileAccess.Open(Globals.Paths.BadConfig4x4path, FileAccess.ModeFlags.Read);
            while (!file.EofReached())
            {
                string line = file.GetLine();
                _badConfig4x4Dict[line] = true;
            }
            file.Close();
        }
        private bool IsBadConfig(int dim, string code)
        {
            switch (dim)
            {
                case 4:
                    return _badConfig4x4Dict.ContainsKey(code);
            }
            return true;
        }

    }
}


// GArrayInt toBeRemoved = new GArrayInt() { numberOfPieces - 1, numberOfPieces - (int)timedPuzzle.FrameDimensions[1], (int)timedPuzzle.FrameDimensions[1] - 1, 0 };
// int toBeRemovedLimit = 2;

// for (int i = 0; i < toBeRemovedLimit; i++)
// {
//     toBeRemoved.RemoveAt(Globals.RandomManager.rnd.Next(0, toBeRemoved.Count));
// }

// foreach (int idx in toBeRemoved)
// {
//     prepScramble.RemoveAt(idx);
//     timedPuzzle.StartConfiguration[idx] = 0;
// }
//int toBeRemovedLength = (int)(numberOfPieces) / 2 - toBeRemoved.Count;