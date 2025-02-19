using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.EventsRelated;

public static class SpecialEvents
{
    public static Func<(int? xpass, int? ypass)> pawnHasJustMovedTwice { get; set; }
    public static Func<BoardRelatedInfo[,], Square, WhoseTurn, bool, bool> kingIsChecked { get; set; }
    public static Func<bool> WhiteKingHasMoved { get; set; }
    public static Func<bool> BlackKingHasMoved { get; set; }
    public static Func<BoardRelatedInfo[,], Square, Square, bool, WhoseTurn, bool> kingIsMate { get; set; }
    public static Action<WhoseTurn, int, PieceName> pawnPromotes { get; set; }
}
