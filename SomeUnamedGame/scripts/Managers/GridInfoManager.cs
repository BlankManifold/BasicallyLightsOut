using Godot;


namespace Globals
{
    static public class GridInfoManager
    {

        static private Vector2 _viewportSize;
        static private Vector2 _areaSize;
        static private Vector3 _borders;
        static private Vector2 _separation;
        static private Vector2 _areaCenter;
        static private int _pieceExtents;

        static public void InitAreaSize(Vector2 viewportSize, Vector3 borders, Vector2 separation)
        {
            _borders = borders;
            _separation = separation;
            _viewportSize = viewportSize;
            _areaSize[0] = viewportSize[0] - 2 * borders[0];
            _areaSize[1] = viewportSize[1] - borders[1] - borders[2];

            Vector2 shift = new Vector2(borders[0], borders[1]);
            _areaCenter = _areaSize / 2 + shift;
        }

        static public int GetMaxTypeAExtents(Vector2 frameDimensions, Vector2 separation)
        {
            Vector2 dim = new Vector2(frameDimensions[1], frameDimensions[0]);

            Vector2 totalSeparation = separation * (dim - Vector2.One);
            Vector2 extentsVec = (_areaSize - totalSeparation) / dim;

            _pieceExtents = (int)(Mathf.Min(extentsVec[0], extentsVec[1]) / 2);
            return _pieceExtents;
        }
        static public Vector2 GetStartPosition(Vector2 frameDimensions, Vector2 pieceExtents, Vector2 separation)
        {
            Vector2 dim = new Vector2(frameDimensions[1], frameDimensions[0]);

            Vector2 dimensionsShift = dim - Vector2.One;
            Vector2 widthShift = dimensionsShift * pieceExtents;

            Vector2 separationShift = dimensionsShift * separation / 2;

            return _areaCenter - widthShift - separationShift;
        }

        static public int FromPosToId(Vector2 pos, Vector2 frameDimensions, Vector2 startPosition)
        {
            Vector2 origin = startPosition - _pieceExtents * Vector2.One;
            Vector2 posDiff = pos - origin;
            Vector2 pieceWidth = 2 * _pieceExtents * Vector2.One;
            Vector2 coords = (posDiff / (pieceWidth + _separation)).Floor();
            coords = new Vector2(coords[1], coords[0]);

            if (OutOfBounds(coords, frameDimensions))
                return -1;

            return Globals.Utilities.CoordsToId(coords, frameDimensions);
        }
        static public Vector2 FromIdToPos(int id, Vector2 frameDimensions, Vector2 startPosition)
        {
            int[] coords = Globals.Utilities.IdToCoords(id, frameDimensions);
            Vector2 coordsShift = new Vector2(coords[1], coords[0]);
            return startPosition + 2 * coordsShift * _pieceExtents + coordsShift * _separation;
        }

        static public bool OutOfBounds(Vector2 coords, Vector2 frameDimensions)
        {
            if (coords[0] < 0 || coords[0] > frameDimensions[0])
                return true;
            if (coords[1] < 0 || coords[1] > frameDimensions[1])
                return true;

            return false;
        }
    }
}