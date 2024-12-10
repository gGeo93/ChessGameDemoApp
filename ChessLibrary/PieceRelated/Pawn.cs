using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Pawn : Piece, IMove
{
    public override PieceName Name { get; set; }
    public bool HasMovedOnceAtLeast { get; set; }

    public void Move(Square to)
    {
        throw new NotImplementedException();
    }
}
