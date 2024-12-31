using ChessLibrary.BoardRelated;
using ChessLibrary.RulesRelated;

namespace ChessLibrary.GamingProcessRelated;

public class GamingProcess
{
    public static GamingProcess Instance{ get; private set; }
    public ChessBoard ChessBoard { get; set; }
    public MaterialLeft Material { get; set; }
    public WhoseTurn WhoPlays { get; set; }
    public int MoveCompletionCounter { get; set; }
    public Square[] Move { get; set; }
    public Square WhiteKingPosition { get; set; }
    public Square BlackKingPosition { get; set; }
    public GamingProcess()
    {
        Instance = this;
        ChessBoard = new ChessBoard();
        Material = new MaterialLeft();
        WhoPlays = WhoseTurn.White;
        Move = new Square[2];
        MoveCompletionCounter = 0;
        WhiteKingPosition = new Square { Color = PieceRelated.SquareColor.BLACK, Letter = 'e', Number = 1 };
        BlackKingPosition = new Square { Color = PieceRelated.SquareColor.WHITE, Letter = 'e', Number = 8 };
    }
}
