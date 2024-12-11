using ChessLibrary.BoardRelated;
using ChessLibrary.HellpingMethods;
using System;

namespace ChessLibrary.PieceRelated;

public class King : Piece, IMove
{
    public bool Movement(Square from, Square to)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        if (MathF.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo) != 1 || MathF.Abs(pseudoCoorFrom.yFrom - pseudoCoorTo.yTo) != 1)
            return false;
        if (MathF.Abs(pseudoCoorFrom.xFrom - pseudoCoorFrom.yFrom) != 1 || MathF.Abs(pseudoCoorTo.xTo - pseudoCoorTo.yTo) != 1)
            return false;
        
        return true;
    }
}
