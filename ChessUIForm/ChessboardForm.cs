using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessUIForm;

public partial class ChessboardForm : Form
{
    #region [BackendFields]
    GamingProcess gameManager;
    BoardRelatedInfo boardRelatedInfo;
    #endregion

    #region [UIFields]
    Image? initPlace;
    Image? futurePlace;
    #endregion

    #region [Init]
    public ChessboardForm()
    {
        InitializeComponent();
        gameManager = new GamingProcess();
        gameManager.WhoPlays = WhoseTurn.White;
    }
    #endregion

    #region [TouchSquareEvent]
    private void square_pressed(object sender, EventArgs e)
    {
        #region [RetreiveData]
        gameManager.MoveCompletionCounter += 1;

        String square = ((Button)sender).Name;
        
        (int x, int y) = square.FromRealToProgrammingCoordinates();
        
        ChessBoard chessBoard = gameManager.ChessBoard;
        #endregion

        #region [FutureConstraints]
        if (chessBoard.Board[x, y].Apiece is null && gameManager.MoveCompletionCounter == 1)
        {
            gameManager.MoveCompletionCounter  = 0;
            return;
        }
        #endregion

        #region [FirstHalfOfTheMove]
        else if (chessBoard.Board[x, y].Apiece is not null && gameManager.MoveCompletionCounter == 1)
        {
            boardRelatedInfo = new BoardRelatedInfo() { Apiece = new Piece { Name = chessBoard.Board[x, y].Apiece.Name }, ASquare = chessBoard.Board[x, y].ASquare };
            initPlace = ((Button)sender).Image;
            ((Button)sender).Image = null;
        }
        #endregion
        
        #region [SecondHalfOfTheMove]
        else if (gameManager.MoveCompletionCounter == 2)
        {
            boardRelatedInfo = new BoardRelatedInfo() { Apiece = new Piece { Name = chessBoard.Board[x, y].Apiece.Name }, ASquare = chessBoard.Board[x, y].ASquare };
            ((Button)sender).Image = initPlace;
            gameManager.MoveCompletionCounter = 0;
            gameManager.WhoPlays = gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
        }
        #endregion

        #region[UpdateBackendBoard]
        chessBoard.Board[x, y].Apiece = boardRelatedInfo.Apiece;
        chessBoard.Board[x, y].ASquare = boardRelatedInfo.ASquare;
        #endregion
    }
    #endregion
}
