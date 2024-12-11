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

        if (MathF.Abs(pseudoCoorFrom.xFrom - pseudoCoorFrom.yFrom) != MathF.Abs(pseudoCoorTo.xTo - pseudoCoorTo.yTo))
            return false;

        return true;
    }
}
