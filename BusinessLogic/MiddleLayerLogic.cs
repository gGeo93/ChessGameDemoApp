using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.PieceRelated;

namespace BusinessLogic;

public class MiddleLayerLogic
{
    public GamingProcess gameManager { get; set; }
    public BoardRelatedInfo currentBoardRelatedInfo { get; set; }
    public BoardRelatedInfo[] boardRelatedInfoMove  { get; set; }
    public ChessBoard chessBoard { get; set; }
    public bool[] RooksMovingState { get; set; }
    public (int x, int y)[] coordinates { get; set; }
    public int moveCounter { get; set; }
    public int xpromotion { get; set; }
    public int ypromotion { get; set; }
    public MiddleLayerLogic()
    {
        gameManager = new GamingProcess();
        gameManager.WhoPlays = WhoseTurn.White;
        boardRelatedInfoMove = new BoardRelatedInfo[2];
        coordinates = new (int x, int y)[2];
        RooksMovingState = [true, true, true, true];
    }
    public string PreviousSquare() => boardRelatedInfoMove[0].ASquare.Letter + boardRelatedInfoMove[0].ASquare.Number.ToString();
    
}
