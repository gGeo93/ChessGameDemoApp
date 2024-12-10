using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Pawn : Piece, IMove
{
    public bool HasMovedOnceAtLeast { get; set; }

    public void Movement(Square to)
    {
        throw new NotImplementedException();
    }
}
