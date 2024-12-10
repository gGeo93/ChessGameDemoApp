using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Rook : Piece, IMove
{
    public override PieceName Name { get; set; }

    public void Move(Square to)
    {
        throw new NotImplementedException();
    }
}
