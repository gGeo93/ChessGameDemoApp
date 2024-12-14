using ChessLibrary.BoardRelated;
using ChessLibrary.HellpingMethods;

namespace ChessLibrary.PieceRelated;

public class Bishop : Piece, IMove
{
    public bool Movement(Square from, Square to)
    {
        if (from.Color != to.Color)
            return false;

        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        if (Math.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo) == Math.Abs(pseudoCoorFrom.yFrom - pseudoCoorTo.yTo))     
            return true;

        return false;
    }
}
