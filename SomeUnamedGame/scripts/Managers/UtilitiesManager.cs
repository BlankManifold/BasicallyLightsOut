using System;
using Godot;

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
                id1 = row*cols;

                for (int col = 0; col <= maxCol; col++)
                {
                    id2 = target - id1;

                    if (config[id1] == config[id2])
                    {
                        count++;
                    }
                    id1++;
                }
                target += 2 * cols;
            }

            float symRatio = 2 * (float)count / numberOfPieces;
            return (symRatio >= 0.90);
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
                target = baseTarget +  2*(id1 % cols);

                id2 = target - id1;
                if (config[id1] == config[id2])
                {
                    count++;
                }

            }

            float symRatio = 2 * (float)count / numberOfPieces;
            return (symRatio >= 0.90);
        }
    }
}
