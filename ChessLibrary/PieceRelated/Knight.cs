using ChessLibrary.BoardRelated;
using ChessLibrary.HellpingMethods;

namespace ChessLibrary.PieceRelated;

public class Knight : Piece, IMove
{
    public bool Movement(Square from, Square to)
    {
        if(from.Color == to.Color)
            return false;

        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);
        
        if (Math.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo) == 1 && Math.Abs(pseudoCoorFrom.yFrom - pseudoCoorTo.yTo) == 2)
            return true;
        if(Math.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo) == 2 && Math.Abs(pseudoCoorFrom.yFrom - pseudoCoorTo.yTo) == 1)
            return true;

        return false;
    }
}
