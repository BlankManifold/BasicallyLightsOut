using System;

namespace Globals
{
    public enum PiecesType
    {
        A,B
    }

    static public class Utilities
    {
        static public int[] IdToCoords(int id, int[] frameDimensions)
        {
            int[] coords = {0,0};
            coords[0] = Math.DivRem((int)id, frameDimensions[1], out coords[1]);
            return coords;
        }

        static public int CoordsToId(int[] coords, int[] frameDimensions)
        {
            return (int)(coords[0]*frameDimensions[1] + coords[1]);
        }
        static public int CoordsToId(int row, int col, int[] frameDimensions)
        {
            return (int)(row*frameDimensions[1] + col);
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
        
    }
}