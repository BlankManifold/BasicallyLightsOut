using System;
using Godot;
using GArrayInt = Godot.Collections.Array<int>;

namespace Globals
{
    public enum PiecesType
    {
        A, B
    }

    public enum Mode
    {
        NORMAL, TIMED
    }


    static public class Utilities
    {
        static public int[] IdToCoords(int id, Vector2 frameDimensions)
        {
            int[] coords = { 0, 0 };
            coords[0] = Math.DivRem(id, (int)frameDimensions[1], out coords[1]);
            return coords;
        }

        static public int CoordsToId(Vector2 coords, Vector2 frameDimensions)
        {
            return (int)(coords[0] * frameDimensions[1] + coords[1]);
        }
        static public int CoordsToId(int row, int col, Vector2 frameDimensions)
        {
            return (int)(row * frameDimensions[1] + col);
        }
        static public int CoordsToId(int row, int col, int[] frameDimensions)
        {
            return (int)(row * frameDimensions[1] + col);
        }
        static public string GetPiecesScenePath(PiecesType type)
        {
            switch (type)
            {
                case PiecesType.A:
                    return "scenes/TypeA.tscn";
                default:
                    return "scenes/TypeA.tscn";
            }
        }
        static public string GetPuzzleMenagerScenePath()
        {
            return "scenes/PuzzleManager.tscn";
        }
        static public string GetPuzzleCreationUIScenePath()
        {
            return "scenes/PuzzleCreationUI.tscn";
        }
        static public string GetPuzzleDataResourcePath(string type, int id)
        {
            return $"puzzleResource/{type}/p_{id}.tres";
        }
    }

    public struct RandomManager
    {
        public static RandomNumberGenerator rng = new RandomNumberGenerator();
        public static Random rnd = new Random();
    }

    static public class SymmetriesManager
    {
        public static bool MainDiagonalCheck(Godot.Collections.Array<int> config, Vector2 dims)
        {
            int count = 0;
            int numberOfPieces = (int)(dims[0] * dims[1]) - (int)(dims[1]);

            for (int row = 0; row < dims[0] - 1; row++)
            {
                for (int col = row + 1; col < dims[1]; col++)
                {
                    int id1 = Utilities.CoordsToId(row, col, dims);
                    int id2 = Utilities.CoordsToId(col, row, dims);

                    if (config[id1] == config[id2])
                    {
                        count++;
                    }
                }

            }

            float symRatio = 2 * (float)count / numberOfPieces;
            return (symRatio >= 0.75);
        }
        public static bool SecondaryDiagonalCheck(Godot.Collections.Array<int> config, Vector2 dims)
        {
            int count = 0;
            int numberOfPieces = (int)(dims[0] * dims[1]) - (int)(dims[1]);

            int id1, id2;

            for (int row = 0; row < dims[0] - 1; row++)
            {
                for (int col = 0; col < dims[1] - 1 - row; col++)
                {
                    id1 = Utilities.CoordsToId(row, col, dims);
                    id2 = Utilities.CoordsToId((int)dims[1] - 1 - col, (int)dims[0] - 1 - row, dims);

                    if (config[id1] == config[id2])
                    {
                        count++;
                    }
                }

            }

            float symRatio = 2 * (float)count / numberOfPieces;
            return (symRatio >= 0.75);
        }
        public static bool VerticalCheck(Godot.Collections.Array<int> config, Vector2 dims)
        {
            int rows = (int)dims[0];
            int cols = (int)dims[1];

            int count = 0;
            int numberOfPieces = rows * cols;
            if (dims[0] % 2 != 0)
            {
                numberOfPieces -= rows;
            }
            int target = cols - 1;
            int maxCol = cols / 2 - 1;
            int id2;
            int id1 = 0;


            for (int row = 0; row < dims[0]; row++)
            {
                id1 = row * cols;

                for (int col = 0; col <= maxCol; col++)
                {
                    id2 = target - id1;

                    if (config[id1] == config[id2])
                    {
                        count++;
                    }
                    id1++;
                }
                target += cols;
            }

            float symRatio = 2 * (float)count / numberOfPieces;
            return (symRatio >= 0.75);
        }
        public static bool HorizontalCheck(Godot.Collections.Array<int> config, Vector2 dims)
        {
            int rows = (int)dims[0];
            int cols = (int)dims[1];

            int count = 0;
            int numberOfPieces = (rows - 1) * cols;
            int baseTarget = numberOfPieces;

            if (dims[1] % 2 != 0)
            {
                numberOfPieces -= cols;
            }

            int maxId = cols * ((int)(dims[0] / 2)) - 1;
            int id2, target;

            for (int id1 = 0; id1 <= maxId; id1++)
            {
                target = baseTarget + 2 * (id1 % cols);

                id2 = target - id1;
                if (config[id1] == config[id2])
                {
                    count++;
                }

            }

            float symRatio = 2 * (float)count / numberOfPieces;
            return (symRatio >= 0.75);
        }

        public static bool MainDiagonalCheckScramble(Godot.Collections.Array<int> config, Vector2 dims, int scrambleLenght)
        {
            int count = 0;

            for (int row = 0; row < dims[0] - 1; row++)
            {
                for (int col = row + 1; col < dims[1]; col++)
                {
                    int id1 = Utilities.CoordsToId(row, col, dims);
                    if (config[id1] == 0)
                    {
                        continue;
                    }

                    int id2 = Utilities.CoordsToId(col, row, dims);

                    if (config[id2] == 1)
                    {
                        count = count + 2;
                    }
                }
            }

            for (int row = 0; row < dims[0]; row++)
            {
                int id = Utilities.CoordsToId(row, row, dims);
                if (config[id] == 1)
                {
                    count++;
                }
            }

            float symRatio = (float)count / scrambleLenght;
            return (symRatio >= 0.75);
        }
        public static bool SecondaryDiagonalCheckScramble(Godot.Collections.Array<int> config, Vector2 dims, int scrambleLenght)
        {
            int count = 0;

            int id1, id2;

            for (int row = 0; row < dims[0] - 1; row++)
            {
                for (int col = 0; col < dims[1] - 1 - row; col++)
                {
                    id1 = Utilities.CoordsToId(row, col, dims);
                    if (config[id1] == 0)
                        continue;

                    id2 = Utilities.CoordsToId((int)dims[1] - 1 - col, (int)dims[0] - 1 - row, dims);
                    if (config[id2] == 1)
                    {
                        count = count + 2;
                    }
                }

            }

            for (int row = 0; row < dims[0]; row++)
            {
                int id = Utilities.CoordsToId(row, (int)dims[0] - 1 - row, dims);
                if (config[id] == 1)
                {
                    count++;
                }
            }

            float symRatio = (float)count / scrambleLenght;
            return (symRatio >= 0.75);
        }
        public static bool VerticalCheckScramble(Godot.Collections.Array<int> config, Vector2 dims, int scrambleLenght)
        {
            int rows = (int)dims[0];
            int cols = (int)dims[1];

            int count = 0;

            int target = cols - 1;
            int maxCol = cols / 2 - 1;
            int id2;
            int id1 = 0;


            for (int row = 0; row < dims[0]; row++)
            {
                id1 = row * cols;

                for (int col = 0; col <= maxCol; col++)
                {

                    if (config[id1] == 0)
                        continue;

                    id2 = target - id1;
                    if (config[id2] == 1)
                    {
                        count++;
                    }

                    id1++;
                }
                target +=  cols;
            }

            float symRatio = 2 * (float)count / scrambleLenght;
            return (symRatio >= 0.75);
        }
        public static bool HorizontalCheckScramble(Godot.Collections.Array<int> config, Vector2 dims, int scrambleLenght)
        {
            int rows = (int)dims[0];
            int cols = (int)dims[1];

            int count = 0;
            int baseTarget = (rows - 1) * cols;



            int maxId = cols * ((int)(dims[0] / 2)) - 1;
            int id2, target;

            for (int id1 = 0; id1 <= maxId; id1++)
            {
                target = baseTarget + 2 * (id1 % cols);

                if (config[id1] == 0)
                    continue;

                id2 = target - id1;
                if (config[id2] == 1)
                {
                    count++;
                }
            }

            float symRatio = 2 * (float)count / scrambleLenght;
            return (symRatio >= 0.75);
        }
    }

    static public class EntropyManager
    {
        public struct ResultArray
        {

            public GArrayInt Array;
            public Vector2 Dims;

            public ResultArray(GArrayInt array, Vector2 dims)
            {
                this.Array = array;
                this.Dims = dims;
            }
        }
        public static ResultArray ConvolutionSquareOverlap2x2(GArrayInt A, int[] dims)
        {
            float flux = 0;
            int i = 0;
            GArrayInt result = new GArrayInt() { };
            result.Resize((dims[0] - 1) * (dims[1] - 1));

            for (int id = 0; id < A.Count - dims[1] - 1; id++)
            {
                if ((id + 1) % dims[1] == 0)
                {
                    continue;
                }

                flux = ColorManager.GetValue(A[id]) + ColorManager.GetValue(A[id + 1]) + ColorManager.GetValue(A[id + dims[1]]) + ColorManager.GetValue(A[id + dims[1] + 1]);

                if (flux > 2.5)
                {
                    result[i] = 1;
                }
                else if (flux < 1.5)
                {
                    result[i] = 0;
                }
                else
                {
                    result[i] = 2;
                }

                i++;
            }
            ResultArray output = new ResultArray(result, new Vector2(dims[0] - 1, dims[1] - 1));
            return output;
        }

        private static float GetNNIntensity(int id, int row, int col, int[] dims, GArrayInt A)
        {
            int cont = 0;
            float flux = 0;

            if (row > 0)
            {
                cont++;
                flux += ColorManager.GetValue(A[id - dims[1]]);
            }

            if (col > 0)
            {
                cont++;
                flux += ColorManager.GetValue(A[id - 1]);
            }

            if (row < dims[0] - 1)
            {
                cont++;
                flux += ColorManager.GetValue(A[id + dims[1]]);
            }

            if (col < dims[1] - 1)
            {
                cont++;
                flux += ColorManager.GetValue(A[id + 1]);
            }

            return flux / cont;
        }

        public static ResultArray ConvolutionNNOverlap(GArrayInt A, int[] dims)
        {

            GArrayInt result = new GArrayInt() { };
            result.Resize(dims[0] * dims[1]);

            float intensity = 0f;
            int id = 0;

            for (int row = 0; row < dims[0]; row++)
            {
                for (int col = 0; col < dims[1]; col++)
                {
                    intensity = GetNNIntensity(id, row, col, dims, A);

                    if (intensity > 0.65)
                    {
                        result[id] = 1;
                    }
                    else if (intensity < 0.35)
                    {
                        result[id] = 0;
                    }
                    else
                    {
                        result[id] = 2;
                    }
                    id++;
                }
            }

            ResultArray output = new ResultArray(result, new Vector2(dims[0], dims[1]));
            return output;
        }
    }
}
