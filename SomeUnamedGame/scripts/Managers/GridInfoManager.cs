using Godot;


namespace Globals
{
    static public class GridInfoManager
    {

        static private Vector2 _viewportSize;
        static private Vector2 _areaSize;
        static  private Vector3 _borders;
        static private Vector2 _areaCenter;

        static public void InitAreaSize(Vector2 viewportSize, Vector3 borders)
        {
            _borders = borders;
            _viewportSize = viewportSize;
            _areaSize[0] = viewportSize[0] - 2*borders[0]; 
            _areaSize[1] = viewportSize[1] - borders[1] - borders[2];
            
            Vector2 shift = new Vector2(borders[0], borders[1]);
            _areaCenter = _areaSize/2 + shift;
        }       

        static public int GetMaxTypeAExtents(Vector2 frameDimensions, Vector2 separation)
        {
            Vector2 totalSeparation = separation*(frameDimensions - Vector2.One);
            Vector2 extentsVec = (_areaSize - totalSeparation) / frameDimensions; 
            return (int)(Mathf.Min(extentsVec[0], extentsVec[1])/2);
        }
        static public Vector2 GetStartPosition(Vector2 frameDimension, Vector2 pieceExtents, Vector2 separation)
        {
            Vector2 dimensionsShift = frameDimension - Vector2.One;
            Vector2 widthShift = dimensionsShift*pieceExtents;
            
            Vector2 separationShift = dimensionsShift * separation / 2;
            
            return _areaCenter - widthShift - separationShift;
        }
    }
}