using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.PieceRelated;

namespace BusinessLogic;

public class MiddleLayerLogic
{
    public GamingProcess gameManager { get; set; }
    public BoardRelatedInfo currentBoardRelatedInfo;
    public BoardRelatedInfo[] boardRelatedInfoMove;
    public ChessBoard chessBoard;
    public (int x, int y)[] coordinates;
    public MiddleLayerLogic()
    {
        gameManager = new GamingProcess();
        gameManager.WhoPlays = WhoseTurn.White;
        boardRelatedInfoMove = new BoardRelatedInfo[2];
        coordinates = new (int x, int y)[2];
    }
}
