using ChessLibrary.PieceRelated;

namespace ChessLibrary.BoardRelated;

public class BoardRelatedInfo
{
    public Piece? Apiece { get; set; }
    public Square ASquare { get; set; }
    public bool ApieceOccupySquare { get; set; }
    public bool KingCanSeat { get; set; }
}
