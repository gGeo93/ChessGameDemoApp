using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Rook : Piece, IMove
{
    public bool Movement(Square from, Square to)
    {
        if (from.Letter == to.Letter || from.Number == to.Number)
            return true;
        return false;
    }
}
