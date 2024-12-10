using ChessLibrary.BoardRelated;

namespace ChessLibrary.EventsRelated;

public static class Events
{
    public static Action<ChessBoard> OnCheck;
    public static Action<ChessBoard> OnMate;
    public static Action<ChessBoard> OnStalemate;
    public static Action<ChessBoard> OnCastlingShort;
    public static Action<ChessBoard> OnCastlingLong;
    public static Action<ChessBoard> OnEnPassant;
}
