using ChessLibrary.GamingProcessRelated;

namespace ChessLibrary.PieceRelated;

public interface IPawn
{
    bool IsOnInitialSquare { get; set; }
    WhoseTurn WhoPlays { get; set; }
}
