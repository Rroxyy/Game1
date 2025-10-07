public enum CellPart
{
    CellBody,
    CellConnection,
    CellGapTriangle
}

/////////////////////////
///		LU		UR    ///
/// L				R ///
///		DL		RD    ///
/////////////////////////
public enum CellBodyDirection
{
    R,
    RD,
    DL,
    L,
    LU,
    UR,
    
}

public enum CellAllDirection
{
    Deg0   = 0,  // 正右
    Deg30  = 1,  // 右下角
    Deg60  = 2,  // 右下边
    Deg90  = 3,  // 下右角
    Deg120 = 4,  // 左下边
    Deg150 = 5,  // 左下角
    Deg180 = 6,  // 正左
    Deg210 = 7,  // 左上角
    Deg240 = 8,  // 左上边
    Deg270 = 9,  // 上左角
    Deg300 = 10, // 右上边
    Deg330 = 11  // 右上角
}

public class BaseCellMetrics
{
    
    
}