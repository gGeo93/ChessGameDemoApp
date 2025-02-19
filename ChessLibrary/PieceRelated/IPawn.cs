using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;

namespace ChessLibrary.PieceRelated;

public interface IPawn
{
    bool IsOnInitialSquare { get; set; }
    PieceInfo? pieceinfo { get; set; }
}
