﻿using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;
using ChessLibrary.RulesRelated;

namespace ChessUIForm;

public partial class ChessboardForm : Form
{
    #region [BackendFields]
    GamingProcess gameManager;
    BoardRelatedInfo currentBoardRelatedInfo;
    BoardRelatedInfo[] boardRelatedInfoMove;
    ChessBoard chessBoard;
    (int x, int y)[] coordinates;
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
        coordinates = new (int x, int y)[2];
    }
    #endregion

    #region [TouchSquareEvent]
    private void square_pressed(object sender, EventArgs e)
    {
        #region [RetreiveData]
        gameManager.MoveCompletionCounter += 1;

        String square = ((Button)sender).Name;
        
        (int x, int y) = square.FromRealToProgrammingCoordinates();

        coordinates[gameManager.MoveCompletionCounter - 1] = (x, y);

        chessBoard = gameManager.ChessBoard;

        //gameManager.Move[gameManager.MoveCompletionCounter - 1] = new Square { Letter = square[0], Number = int.Parse(square[1].ToString()), Color = chessBoard.Board[x, y].ASquare.Color };
        #endregion

        #region[MovementConstraints]

        #endregion
        
        #region [FutureConstraints]
        if (!chessBoard.Board[x, y].ApieceOccupySqsuare && gameManager.MoveCompletionCounter == 1)
        {
            gameManager.MoveCompletionCounter  = 0;
            return;
        }
        #endregion

        #region [FirstHalfOfTheMove]
        else if (chessBoard.Board[x, y].ApieceOccupySqsuare && gameManager.MoveCompletionCounter == 1)
        {
            currentBoardRelatedInfo = new BoardRelatedInfo() { 
                Apiece = new Piece 
                { 
                    Name = chessBoard.Board[x, y].Apiece.Name, 
                    Color = chessBoard.Board[x,y].Apiece.Color
                }, 
                ApieceOccupySqsuare = true, 
                ASquare = chessBoard.Board[x, y].ASquare,
            };
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
            currentBoardRelatedInfo = new BoardRelatedInfo() { 
                Apiece = new Piece 
                { 
                    Name = boardRelatedInfoMove[0].Apiece.Name,
                    Color = boardRelatedInfoMove[0].Apiece.Color
                }, 
                ASquare = chessBoard.Board[x, y].ASquare  
            };
            boardRelatedInfoMove[1] = currentBoardRelatedInfo;
            moveParts[1] = ((Button)sender);
            moveParts[0].BackColor = squareColor;
            chessBoard.Board[coordinates[0].x, coordinates[0].y].ApieceOccupySqsuare = false;

            bool canMoveChosenWay = currentBoardRelatedInfo.Apiece.Name.CanPerfomeThisMove(boardRelatedInfoMove[0].ASquare, boardRelatedInfoMove[1].ASquare, gameManager.WhoPlays);
              
            bool isThereNoObstacle = currentBoardRelatedInfo.Apiece.Name.ThereIsNoObstacle(boardRelatedInfoMove[0].ASquare,boardRelatedInfoMove[1].ASquare, chessBoard, gameManager.WhoPlays);
            
            if (!canMoveChosenWay || !isThereNoObstacle)
            {
                chessBoard.Board[coordinates[0].x, coordinates[0].y].ApieceOccupySqsuare = true;
                chessBoard.Board[coordinates[0].x, coordinates[0].y].Apiece = boardRelatedInfoMove[0].Apiece;
                gameManager.MoveCompletionCounter = 0;
                moveParts = new Button[2];
                boardRelatedInfoMove = new BoardRelatedInfo[2];
                gameManager.Move = new Square[2];
                coordinates = new (int x, int y)[2];
                return;
            }
            ((Button)sender).Image = moveParts[0].Image;
            chessBoard.Board[x, y].ApieceOccupySqsuare = true;
            moveParts[0].Image = null;
            gameManager.MoveCompletionCounter = 0;
            gameManager.WhoPlays = gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
            moveParts = new Button[2];
        }
        #endregion

        #region[UpdateBackendBoard]
        gameManager.ChessBoard.Board[x, y].Apiece = currentBoardRelatedInfo.Apiece;
        gameManager.ChessBoard.Board[x, y].ASquare = currentBoardRelatedInfo.ASquare;
        #endregion
    }
    #endregion
}
