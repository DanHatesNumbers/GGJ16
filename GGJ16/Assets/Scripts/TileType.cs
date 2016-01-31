using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum TileType
{
    Top, 
    LeftSlope, 
    RightSlope, 
    RightSlopeUpper, 
    RightSlopeLower, 
    LeftSlopeUpper, 
    LeftSlopeLower,
    None, 
    RightSlopeCorner, 
    LeftSlopeCorner, 
    LeftEnd, 
    RightEnd, 
    Platform,
    LeftWall,
    RightWall,
    Lava, 
    Filler
}
