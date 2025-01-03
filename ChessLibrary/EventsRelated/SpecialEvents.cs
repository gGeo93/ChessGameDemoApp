using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;

namespace ChessLibrary.EventsRelated;

public static class SpecialEvents
{
    public static Func<(int? xpass, int? ypass)> pawnHasJustMovedTwice { get; set; }
    public static Func<ChessBoard, Square, WhoseTurn, bool> kingIsChecked { get; set; }
    public static Func<ChessBoard, Square, Square, bool, WhoseTurn, bool> kingIsMate { get; set; }
}
