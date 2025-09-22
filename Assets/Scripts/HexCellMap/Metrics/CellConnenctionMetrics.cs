using System.Collections.Generic;



public static class CellConnectionMetrics
{
    public static List<int> LOD0_P2 = new List<int>() { 10,12,17};
    
    public static List<int> LOD0_P3 = new List<int>() { 16,18,23};

    public static List<HexAllDirection> connectionDirections = new List<HexAllDirection>()
    {
        HexAllDirection.Deg0,       //Right
        HexAllDirection.Deg90,      //Down
        HexAllDirection.Deg180,     //Left
        HexAllDirection.Deg270,     //Up
    };
}