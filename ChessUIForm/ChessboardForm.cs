using ChessLibrary.BoardRelated;
using ChessLibrary.SpecialOccasionsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;
using ChessLibrary.RulesRelated;
using ChessLibrary.EventsRelated;
using System;
using System.Collections;
using System.Linq;

namespace ChessUIForm;

public partial class ChessboardForm : Form
{
    #region [LibraryFields]
    GamingProcess gameManager;
    BoardRelatedInfo currentBoardRelatedInfo;
    BoardRelatedInfo[] boardRelatedInfoMove;
    ChessBoard chessBoard;
    (int x, int y)[] coordinates;
    #endregion

    #region [UIFields]
    Button[,] frontBoard;
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
        coordinates = new (int x, int y)[2];
        ColorsRender();
        FrontBoardImagesFill();
    }
    #endregion

    #region [TouchSquareEvent]
    private void square_pressed(object sender, EventArgs e)
    {
        #region [RetreiveData]
        ColorsRender();

        gameManager.MoveCompletionCounter += 1;

        String square = ((Button)sender).Name;

        (int x, int y) = square.FromRealToProgrammingCoordinates();

        coordinates[gameManager.MoveCompletionCounter - 1] = (x, y);

        chessBoard = gameManager.ChessBoard;
        #endregion

        
        if (!chessBoard.Board[x, y].ApieceOccupySquare && gameManager.MoveCompletionCounter == 1)
        {
            gameManager.MoveCompletionCounter = 0;
            return;
        }
        #region [FirstHalfOfTheMove]
        else if (chessBoard.Board[x, y].ApieceOccupySquare && gameManager.MoveCompletionCounter == 1)
        {
            var kings = GetKingsPositions();
            currentBoardRelatedInfo = new BoardRelatedInfo()
            {
                Apiece = new Piece
                {
                    Name = chessBoard.Board[x, y].Apiece.Name,
                    Color = chessBoard.Board[x, y].Apiece.Color
                },
                ApieceOccupySquare = chessBoard.Board[x, y].ApieceOccupySquare,
                ASquare = chessBoard.Board[x, y].ASquare,
            };
            boardRelatedInfoMove[0] = currentBoardRelatedInfo;
            if (moveParts[1] != null)
                frontBoard[x, y].Image = null;
            moveParts[0] = ((Button)sender);
            squareColor = moveParts[0].BackColor;
            moveParts[0].BackColor = Color.Brown;
        }
        #endregion

        #region [SecondHalfOfTheMove]
        else if (gameManager.MoveCompletionCounter == 2)
        {
            currentBoardRelatedInfo = new BoardRelatedInfo()
            {
                Apiece = new Piece
                {
                    Name = boardRelatedInfoMove[0].Apiece.Name,
                    Color = boardRelatedInfoMove[0].Apiece.Color
                },
                ASquare = chessBoard.Board[x, y].ASquare
            };
            boardRelatedInfoMove[1] = currentBoardRelatedInfo;
            frontBoard[x, y] = ((Button)sender);
            moveParts[0].BackColor = squareColor;
            chessBoard.Board[coordinates[0].x, coordinates[0].y].ApieceOccupySquare = false;

            bool canMoveChosenWay = currentBoardRelatedInfo.Apiece.Name.CanPerfomeThisMove(boardRelatedInfoMove[0].ASquare, boardRelatedInfoMove[1].ASquare, gameManager.WhoPlays, currentBoardRelatedInfo.Apiece.Color);

            bool isThereNoObstacle = currentBoardRelatedInfo.Apiece.Name.ThereIsNoObstacle(boardRelatedInfoMove[0].ASquare, boardRelatedInfoMove[1].ASquare, chessBoard, gameManager.WhoPlays);

            bool canCutEnPass = chessBoard.Board.CanTakeEnPassant(gameManager.WhoPlays, boardRelatedInfoMove[0].ASquare, boardRelatedInfoMove[1].ASquare);
            
            if (canCutEnPass)
            {
                ((Button)sender).Image = moveParts[0].Image;
                chessBoard.Board[x, y].ApieceOccupySquare = true;
                chessBoard.Board[coordinates[0].x, coordinates[0].y].ApieceOccupySquare = false;
                chessBoard.Board[coordinates[0].x, coordinates[0].y].Apiece = null;
                moveParts[0].Image = null;
                frontBoard[gameManager.WhoPlays == WhoseTurn.White ? x + 1 : x - 1, y].Image = null;
                gameManager.MoveCompletionCounter = 0;
                gameManager.WhoPlays = gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
                ColorsRender();
                moveParts = new Button[2];
                boardRelatedInfoMove = new BoardRelatedInfo[2];
                gameManager.Move = new Square[2];
                coordinates = new (int x, int y)[2];
                gameManager.ChessBoard.Board[x, y].Apiece = currentBoardRelatedInfo.Apiece;
                gameManager.ChessBoard.Board[x, y].ASquare = currentBoardRelatedInfo.ASquare;
                SpecialEvents.pawnHasJustMovedTwice = () => (-1, -1);
                return;
            }
            else if (canMoveChosenWay == false || isThereNoObstacle == false)
            {
                chessBoard.Board[coordinates[0].x, coordinates[0].y].ApieceOccupySquare = true;
                chessBoard.Board[coordinates[0].x, coordinates[0].y].Apiece = boardRelatedInfoMove[0].Apiece;
                gameManager.MoveCompletionCounter = 0;
                moveParts = new Button[2];
                boardRelatedInfoMove = new BoardRelatedInfo[2];
                gameManager.Move = new Square[2];
                coordinates = new (int x, int y)[2];
                return;
            }
            ((Button)sender).Image = moveParts[0].Image;
            chessBoard.Board[x, y].Apiece = currentBoardRelatedInfo.Apiece;
            chessBoard.Board[x, y].ASquare = currentBoardRelatedInfo.ASquare;
            chessBoard.Board[x, y].ApieceOccupySquare = true;
            chessBoard.Board[coordinates[0].x, coordinates[0].y].ApieceOccupySquare = false;
            chessBoard.Board[coordinates[0].x, coordinates[0].y].Apiece = null;
            moveParts[0].Image = null;
            gameManager.MoveCompletionCounter = 0;
            var kingPosition = gameManager.WhoPlays == WhoseTurn.White ? gameManager.BlackKingPosition : gameManager.WhiteKingPosition;
            if (SpecialEvents.kingIsChecked.Invoke(chessBoard, kingPosition, gameManager.WhoPlays))
            {
                String kingSquare = kingPosition.Letter.ToString() + kingPosition.Number.ToString();
                (int kx, int ky) = kingSquare.FromRealToProgrammingCoordinates();
                frontBoard[kx, ky].BackColor = 
                    SpecialEvents.kingIsMate.Invoke(chessBoard, kingPosition, chessBoard.Board[x, y].ASquare ,true, gameManager.WhoPlays)
                    ? Color.Red : Color.DarkOrange;
            }
            gameManager.WhoPlays = gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
            ColorsRender();
            moveParts = new Button[2];
        }
        #endregion

        #region[UpdateBackendBoard]
        gameManager.ChessBoard.Board[x, y].Apiece = currentBoardRelatedInfo.Apiece;
        gameManager.ChessBoard.Board[x, y].ASquare = currentBoardRelatedInfo.ASquare;
        #endregion
    }
    private (King whiteKing, King blackKing ) GetKingsPositions()
    {
        Piece white = new King();
        Piece black = new King();
        for (int i = 0; i < 8; i++) 
            for (int j = 0; j < 8; j++)
                if (GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece?.Name == PieceName.KING)
                {
                    if (GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece.Color == PieceInfo.WHITE)
                        white = GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece;
                    if (GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece.Color == PieceInfo.BLACK)
                        black = GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece;
                }
        return ((King)white!, (King)black!);
    }
    private void ColorsRender()
    {
        this.whoPlaysLabel.ForeColor = gameManager.WhoPlays == WhoseTurn.White ? Color.White : Color.Black;
        this.whoPlaysLabel.Text = gameManager.WhoPlays.ToString();
    }
    private void FrontBoardImagesFill()
    {
        frontBoard = new Button[8, 8];
        frontBoard[0, 0] = this.a8;
        frontBoard[0, 1] = this.b8;
        frontBoard[0, 2] = this.c8;
        frontBoard[0, 3] = this.d8;
        frontBoard[0, 4] = this.e8;
        frontBoard[0, 5] = this.f8;
        frontBoard[0, 6] = this.g8;
        frontBoard[0, 7] = this.h8;
        frontBoard[1, 0] = this.a7;
        frontBoard[1, 1] = this.b7;
        frontBoard[1, 2] = this.c7;
        frontBoard[1, 3] = this.d7;
        frontBoard[1, 4] = this.e7;
        frontBoard[1, 5] = this.f7;
        frontBoard[1, 6] = this.g7;
        frontBoard[1, 7] = this.h7;
        frontBoard[2, 0] = this.a6;
        frontBoard[2, 1] = this.b6;
        frontBoard[2, 2] = this.c6;
        frontBoard[2, 3] = this.d6;
        frontBoard[2, 4] = this.e6;
        frontBoard[2, 5] = this.f6;
        frontBoard[2, 6] = this.g6;
        frontBoard[2, 7] = this.h6;
        frontBoard[3, 0] = this.a5;
        frontBoard[3, 1] = this.b5;
        frontBoard[3, 2] = this.c5;
        frontBoard[3, 3] = this.d5;
        frontBoard[3, 4] = this.e5;
        frontBoard[3, 5] = this.f5;
        frontBoard[3, 6] = this.g5;
        frontBoard[3, 7] = this.h5;
        frontBoard[4, 0] = this.a4;
        frontBoard[4, 1] = this.b4;
        frontBoard[4, 2] = this.c4;
        frontBoard[4, 3] = this.d4;
        frontBoard[4, 4] = this.e4;
        frontBoard[4, 5] = this.f4;
        frontBoard[4, 6] = this.g4;
        frontBoard[4, 7] = this.h4;
        frontBoard[5, 0] = this.a3;
        frontBoard[5, 1] = this.b3;
        frontBoard[5, 2] = this.c3;
        frontBoard[5, 3] = this.d3;
        frontBoard[5, 4] = this.e3;
        frontBoard[5, 5] = this.f3;
        frontBoard[5, 6] = this.g3;
        frontBoard[5, 7] = this.h3;
        frontBoard[6, 0] = this.a2;
        frontBoard[6, 1] = this.b2;
        frontBoard[6, 2] = this.c2;
        frontBoard[6, 3] = this.d2;
        frontBoard[6, 4] = this.e2;
        frontBoard[6, 5] = this.f2;
        frontBoard[6, 6] = this.g2;
        frontBoard[6, 7] = this.h2;
        frontBoard[7, 0] = this.a1;
        frontBoard[7, 1] = this.b1;
        frontBoard[7, 2] = this.c1;
        frontBoard[7, 3] = this.d1;
        frontBoard[7, 4] = this.e1;
        frontBoard[7, 5] = this.f1;
        frontBoard[7, 6] = this.g1;
        frontBoard[7, 7] = this.h1;
    }
    #endregion
}
