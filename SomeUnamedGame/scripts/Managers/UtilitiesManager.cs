using System;
using Godot;

namespace Globals
{
    public enum PiecesType
    {
        A, B
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

}
