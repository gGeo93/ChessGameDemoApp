using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Pawn : Piece, IMove
{
    public bool HasMovedOnceAtLeast { get; set; }

    public bool Movement(Square from, Square to)
    {
        throw new NotImplementedException();
    }
}
