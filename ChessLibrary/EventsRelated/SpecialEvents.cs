using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;

namespace ChessLibrary.EventsRelated;

public static class SpecialEvents
{
    public static Func<(int? xpass, int? ypass)> pawnHasJustMovedTwice;
    public static Func<ChessBoard, Square, WhoseTurn, bool> whiteKingIsChecked;
    public static Func<ChessBoard, Square, WhoseTurn, bool> blackKingIsChecked;
}
