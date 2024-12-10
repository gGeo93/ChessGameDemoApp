using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Bishop : Piece, IMove
{
    public override PieceName Name { get; set; }

    public void Move(Square to)
    {
        throw new NotImplementedException();
    }
}
