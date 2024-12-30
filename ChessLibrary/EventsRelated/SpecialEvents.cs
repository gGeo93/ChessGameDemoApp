using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;

namespace ChessLibrary.EventsRelated;

public class SpecialEvents
{
    public static Func<(int? xpass, int? ypass)> pawnHasJustMovedTwice;
    public static Func<ChessBoard, Square, WhoseTurn, bool> whiteKingIsChecked;
    public static Func<ChessBoard, Square, Square, bool, WhoseTurn, bool> whiteKingIsMate;
    public static Func<ChessBoard, Square, WhoseTurn, bool> blackKingIsChecked;
}
