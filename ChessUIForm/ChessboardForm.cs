using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessUIForm;

public partial class ChessboardForm : Form
{
    #region [BackendFields]
    GamingProcess gameManager;
    BoardRelatedInfo currentBoardRelatedInfo;
    BoardRelatedInfo[] boardRelatedInfoMove;
    #endregion

    #region [UIFields]
    Button[] moveParts;
    Color squareColor;
    #endregion

    #region [Init]
    public ChessboardForm()
    {
        InitializeComponent();
        gameManager = new GamingProcess();
        moveParts = new Button[2];
        gameManager.WhoPlays = WhoseTurn.White;
        boardRelatedInfoMove = new BoardRelatedInfo[2];
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

        gameManager.Move[gameManager.MoveCompletionCounter - 1] = new Square { Letter = square[0], Number = int.Parse(square[1].ToString()), Color = chessBoard.Board[x, y].ASquare.Color };
        #endregion

        #region[MovementConstraints]

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
            currentBoardRelatedInfo = new BoardRelatedInfo() { Apiece = new Piece { Name = chessBoard.Board[x, y].Apiece.Name }, ASquare = chessBoard.Board[x, y].ASquare };
            boardRelatedInfoMove[0] = currentBoardRelatedInfo;
            if (moveParts[1] != null)
                moveParts[1].Image = null;
            moveParts[0] = ((Button)sender);
            squareColor = moveParts[0].BackColor;
            moveParts[0].BackColor = Color.Brown;
        }
        #endregion
        
        #region [SecondHalfOfTheMove]
        else if (gameManager.MoveCompletionCounter == 2)
        {
            currentBoardRelatedInfo = new BoardRelatedInfo() { Apiece = new Piece { Name = boardRelatedInfoMove[0].Apiece.Name }, ASquare = chessBoard.Board[x, y].ASquare  };
            boardRelatedInfoMove[1] = currentBoardRelatedInfo;
            moveParts[1] = ((Button)sender);
            moveParts[0].BackColor = squareColor;

            bool canMoveChosenWay = gameManager.Constraints.
                CanPerfomeThisMove(
                currentBoardRelatedInfo.Apiece.Name, 
                boardRelatedInfoMove[0].ASquare, 
                boardRelatedInfoMove[1].ASquare, gameManager.WhoPlays
                );
            if (!canMoveChosenWay)
            {
                gameManager.MoveCompletionCounter = 0;
                moveParts = new Button[2];
                boardRelatedInfoMove = new BoardRelatedInfo[2];
                gameManager.Move = new Square[2];
                return;
            }
            
            ((Button)sender).Image = moveParts[0].Image;
            moveParts[0].Image = null;
            gameManager.MoveCompletionCounter = 0;
            gameManager.WhoPlays = gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
            moveParts = new Button[2];
        }
        #endregion

        #region[UpdateBackendBoard]
        chessBoard.Board[x, y].Apiece = currentBoardRelatedInfo.Apiece;
        chessBoard.Board[x, y].ASquare = currentBoardRelatedInfo.ASquare;
        #endregion
    }
    #endregion
}
