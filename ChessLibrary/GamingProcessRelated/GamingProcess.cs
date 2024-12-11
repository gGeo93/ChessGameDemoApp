using ChessLibrary.BoardRelated;

namespace ChessLibrary.GamingProcessRelated;

public class GamingProcess
{
    public ChessBoard ChessBoard { get; set; }
    public MaterialLeft Material { get; set; }
    public WhoseTurn WhoPlays { get; set; }
    public int MoveCompletionCounter { get; set; }
    public Square[] Move { get; set; }
    public GamingProcess()
    {
        ChessBoard = new ChessBoard();
        Material = new MaterialLeft();
        WhoPlays = WhoseTurn.White;
        Move = new Square[2];
        MoveCompletionCounter = 0;
    }
}
