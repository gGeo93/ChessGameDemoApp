using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public class Piece
{
    public PieceName Name { get; set; }
    public PieceInfo Color { get; set; }
    public bool IsStillOnBoard { get; set; }

}
