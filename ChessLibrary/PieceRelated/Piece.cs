using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public abstract class Piece
{
    public abstract PieceName Name { get; set; }
    public PieceColor Color { get; set; }
    public bool IsStillOnBoard { get; set; }
}
