using ChessLibrary.BoardRelated;
using ChessLibrary.SpecialOccasionsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;
using ChessLibrary.RulesRelated;
using ChessLibrary.EventsRelated;
using BusinessLogic;

namespace ChessUIForm;

public partial class ChessboardForm : Form
{
    #region [BusinessLogicLayer]
    MiddleLayerLogic layerLogic;
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
        frontBoard = new Button[8, 8];
        moveParts = new Button[2]; 
        layerLogic = InstancesContruction<MiddleLayerLogic>();
        ColorsRender();
        FrontBoardImagesFill();
    }
    #endregion

    #region [TouchSquareEvent]
    private void square_pressed(object sender, EventArgs e)
    {
        #region [RetreiveData]
        ColorsRender();

        layerLogic.gameManager.MoveCompletionCounter += 1;

        String square = ((Button)sender).Name;

        (int x, int y) = square.FromRealToProgrammingCoordinates();

        layerLogic.coordinates[layerLogic.gameManager.MoveCompletionCounter - 1] = (x, y);

        var chessBoard = layerLogic.gameManager.ChessBoard;
        #endregion

        #region [Constraints]
        if (layerLogic.gameManager.WhoPlays == WhoseTurn.White && chessBoard.Board[x, y].Apiece?.Color == PieceInfo.BLACK
            ||
            layerLogic.gameManager.WhoPlays == WhoseTurn.Black && chessBoard.Board[x, y].Apiece?.Color == PieceInfo.WHITE)
        {
            layerLogic.gameManager.MoveCompletionCounter = 0;
            return;
        }
        else if (!chessBoard.Board[x, y].ApieceOccupySquare && layerLogic.gameManager.MoveCompletionCounter == 1)
        {
            layerLogic.gameManager.MoveCompletionCounter = 0;
            return;
        }
        #endregion

        #region [FirstHalfOfTheMove]
        else if (chessBoard.Board[x, y].ApieceOccupySquare && layerLogic.gameManager.MoveCompletionCounter == 1)
        {
            layerLogic.currentBoardRelatedInfo = new BoardRelatedInfo()
            {
                Apiece = new Piece
                {
                    Name = chessBoard.Board[x, y].Apiece!.Name,
                    Color = chessBoard.Board[x, y].Apiece!.Color
                },
                ApieceOccupySquare = chessBoard.Board[x, y].ApieceOccupySquare,
                ASquare = chessBoard.Board[x, y].ASquare,
            };
            layerLogic.boardRelatedInfoMove[0] = layerLogic.currentBoardRelatedInfo;
            if (moveParts[1] != null)
                frontBoard[x, y].Image = null;
            moveParts[0] = ((Button)sender);
            squareColor = moveParts[0].BackColor;
            moveParts[0].BackColor = Color.Brown;
        }
        #endregion

        #region [SecondHalfOfTheMove]
        else if (layerLogic.gameManager.MoveCompletionCounter == 2)
        {
            if (PieceIsPinned(chessBoard, x, y))
            {
                layerLogic.gameManager.MoveCompletionCounter = 1;
                return;
            }
            ReColoringKingsSquare();
            layerLogic.currentBoardRelatedInfo = new BoardRelatedInfo()
            {
                Apiece = new Piece
                {
                    Name = layerLogic.boardRelatedInfoMove[0].Apiece!.Name,
                    Color = layerLogic.boardRelatedInfoMove[0].Apiece!.Color
                },
                ASquare = chessBoard.Board[x, y].ASquare
            };
            layerLogic.boardRelatedInfoMove[1] = layerLogic.currentBoardRelatedInfo;
            frontBoard[x, y] = ((Button)sender);
            moveParts[0].BackColor = squareColor;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = false;

            bool canMoveChosenWay = layerLogic.currentBoardRelatedInfo.Apiece.Name.CanPerfomeThisMove(layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare, layerLogic.gameManager.WhoPlays, layerLogic.currentBoardRelatedInfo.Apiece.Color);

            bool isThereNoObstacle = layerLogic.currentBoardRelatedInfo.Apiece.Name.ThereIsNoObstacle(layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare, chessBoard, layerLogic.gameManager.WhoPlays);

            bool canCutEnPass = chessBoard.Board.CanTakeEnPassant(layerLogic.gameManager.WhoPlays, layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare);

            bool canCastleShort = chessBoard.KingCanCastleShort(layerLogic.gameManager.WhoPlays);

            bool canCastleLong = chessBoard.KingCanCastleLong(layerLogic.gameManager.WhoPlays);

            if (canCastleShort && layerLogic.boardRelatedInfoMove[1].Apiece?.Name == PieceName.KING && layerLogic.boardRelatedInfoMove[1].ASquare.Letter == 'g')
            {
                if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 1)
                {
                    frontBoard[7, 6].Image = frontBoard[7, 4].Image;
                    frontBoard[7, 4].Image = null;
                    frontBoard[7, 5].Image = frontBoard[7, 7].Image;
                    frontBoard[7, 7].Image = null;
                    chessBoard.Board[7, 5].Apiece = chessBoard.Board[7, 7].Apiece;
                    chessBoard.Board[7, 5].ApieceOccupySquare = true;
                    chessBoard.Board[7, 6].Apiece = chessBoard.Board[7, 4].Apiece;
                    chessBoard.Board[7, 6].ApieceOccupySquare = true;
                    chessBoard.Board[7, 4].Apiece = null;
                    chessBoard.Board[7, 4].ApieceOccupySquare = false;
                    chessBoard.Board[7, 7].Apiece = null;
                    chessBoard.Board[7, 7].ApieceOccupySquare = false;
                    layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
                    ColorsRender();
                    moveParts = new Button[2];
                    layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
                    layerLogic.gameManager.Move = new Square[2];
                    layerLogic.coordinates = new (int x, int y)[2];
                    layerLogic.gameManager.MoveCompletionCounter = 0;
                    return;
                }
                else if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 8)
                {
                    frontBoard[0, 6].Image = frontBoard[0, 4].Image;
                    frontBoard[0, 4].Image = null;
                    frontBoard[0, 5].Image = frontBoard[0, 7].Image;
                    frontBoard[0, 7].Image = null;
                    chessBoard.Board[0, 5].Apiece = chessBoard.Board[0, 7].Apiece;
                    chessBoard.Board[0, 5].ApieceOccupySquare = true;
                    chessBoard.Board[0, 6].Apiece = chessBoard.Board[0, 4].Apiece;
                    chessBoard.Board[0, 5].ApieceOccupySquare = true;
                    chessBoard.Board[0, 4].Apiece = null;
                    chessBoard.Board[0, 4].ApieceOccupySquare = false;
                    chessBoard.Board[0, 7].Apiece = null;
                    chessBoard.Board[0, 7].ApieceOccupySquare = false;
                    layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
                    ColorsRender();
                    moveParts = new Button[2];
                    layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
                    layerLogic.gameManager.Move = new Square[2];
                    layerLogic.coordinates = new (int x, int y)[2];
                    layerLogic.gameManager.MoveCompletionCounter = 0;
                    return;
                }
            }
            else if (canCastleLong && layerLogic.boardRelatedInfoMove[1].Apiece?.Name == PieceName.KING && layerLogic.boardRelatedInfoMove[1].ASquare.Letter == 'c')
            {
                if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 1)
                {
                    frontBoard[7, 2].Image = frontBoard[7, 4].Image;
                    frontBoard[7, 4].Image = null;
                    frontBoard[7, 3].Image = frontBoard[7, 0].Image;
                    frontBoard[7, 0].Image = null;
                    chessBoard.Board[7, 3].Apiece = chessBoard.Board[7, 0].Apiece;
                    chessBoard.Board[7, 3].ApieceOccupySquare = true;
                    chessBoard.Board[7, 2].Apiece = chessBoard.Board[7, 4].Apiece;
                    chessBoard.Board[7, 2].ApieceOccupySquare = true;
                    chessBoard.Board[7, 4].Apiece = null;
                    chessBoard.Board[7, 4].ApieceOccupySquare = false;
                    chessBoard.Board[7, 0].Apiece = null;
                    chessBoard.Board[7, 0].ApieceOccupySquare = false;
                    layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
                    ColorsRender();
                    moveParts = new Button[2];
                    layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
                    layerLogic.gameManager.Move = new Square[2];
                    layerLogic.coordinates = new (int x, int y)[2];
                    layerLogic.gameManager.MoveCompletionCounter = 0;
                    return;
                }
                else if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 8)
                {
                    frontBoard[0, 2].Image = frontBoard[0, 4].Image;
                    frontBoard[0, 4].Image = null;
                    frontBoard[0, 3].Image = frontBoard[0, 0].Image;
                    frontBoard[0, 0].Image = null;
                    chessBoard.Board[0, 3].Apiece = chessBoard.Board[0, 0].Apiece;
                    chessBoard.Board[0, 3].ApieceOccupySquare = true;
                    chessBoard.Board[0, 2].Apiece = chessBoard.Board[0, 4].Apiece;
                    chessBoard.Board[0, 2].ApieceOccupySquare = true;
                    chessBoard.Board[0, 4].Apiece = null;
                    chessBoard.Board[0, 4].ApieceOccupySquare = false;
                    chessBoard.Board[0, 0].Apiece = null;
                    chessBoard.Board[0, 0].ApieceOccupySquare = false;
                    layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
                    ColorsRender();
                    moveParts = new Button[2];
                    layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
                    layerLogic.gameManager.Move = new Square[2];
                    layerLogic.coordinates = new (int x, int y)[2];
                    layerLogic.gameManager.MoveCompletionCounter = 0;
                    return;
                }
            }
            else if (canCutEnPass)
            {
                ((Button)sender).Image = moveParts[0].Image;
                chessBoard.Board[x, y].ApieceOccupySquare = true;
                chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = false;
                chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = null;
                moveParts[0].Image = null;
                frontBoard[layerLogic.gameManager.WhoPlays == WhoseTurn.White ? x + 1 : x - 1, y].Image = null;
                layerLogic.gameManager.MoveCompletionCounter = 0;
                layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
                ColorsRender();
                moveParts = new Button[2];
                layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
                layerLogic.gameManager.Move = new Square[2];
                layerLogic.coordinates = new (int x, int y)[2];
                layerLogic.gameManager.ChessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
                layerLogic.gameManager.ChessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
                SpecialEvents.pawnHasJustMovedTwice = () => (-1, -1);
                layerLogic.gameManager.MoveCompletionCounter = 0;
                return;
            }
            else if (canMoveChosenWay == false || isThereNoObstacle == false)
            {
                chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = true;
                chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = layerLogic.boardRelatedInfoMove[0].Apiece;
                layerLogic.gameManager.MoveCompletionCounter = 0;
                moveParts = new Button[2];
                layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
                layerLogic.gameManager.Move = new Square[2];
                layerLogic.coordinates = new (int x, int y)[2];
                return;
            }


            if (chessBoard.Board[x, y].Apiece?.Name == PieceName.KING && layerLogic.gameManager.WhoPlays == WhoseTurn.White)
                SpecialEvents.BlackKingHasMoved = () => true;
            if (chessBoard.Board[x, y].Apiece?.Name == PieceName.KING && layerLogic.gameManager.WhoPlays == WhoseTurn.Black)
                SpecialEvents.WhiteKingHasMoved = () => true;

            ((Button)sender).Image = moveParts[0].Image;
            chessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
            chessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
            chessBoard.Board[x, y].ApieceOccupySquare = true;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = false;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = null;
            moveParts[0].Image = null;
            layerLogic.gameManager.MoveCompletionCounter = 0;
            var kingPosition = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? layerLogic.gameManager.BlackKingPosition : layerLogic.gameManager.WhiteKingPosition;
            if (SpecialEvents.kingIsChecked.Invoke(chessBoard, kingPosition, layerLogic.gameManager.WhoPlays))
            {
                String kingSquare = kingPosition.Letter.ToString() + kingPosition.Number.ToString();
                (int kx, int ky) = kingSquare.FromRealToProgrammingCoordinates();
                (int xc, int yc) = chessBoard.ThePieceGivingCheckCoordinates(kingPosition, layerLogic.gameManager.WhoPlays);
                frontBoard[kx, ky].BackColor =
                    SpecialEvents.kingIsMate.Invoke(chessBoard, kingPosition, chessBoard.Board[xc, yc].ASquare, true, layerLogic.gameManager.WhoPlays)
                    ? Color.Red : Color.DarkOrange;
            }

            layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
            ColorsRender();

            chessBoard.WholeGameChessBoard.Add(chessBoard.Board);
            moveParts = new Button[2];
        }
        #endregion

        #region[UpdateBackendBoard]
        layerLogic.gameManager.ChessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
        layerLogic.gameManager.ChessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
        #endregion
    }
    #endregion
    #region [HelpingMethods]
    private bool PieceIsPinned(ChessBoard chessBoard, int xto, int yto)
    {
        return false;
    }
    #endregion
    #region [OtherUiRelatedMethods]
    private void ReColoringKingsSquare()
    {
        var kings = layerLogic.chessBoard.GetKingsPositions();
        String whiteKingSquare = kings.whiteKing.Letter + kings.whiteKing.Number.ToString();
        String blackKingSquare = kings.blackKing.Letter + kings.blackKing.Number.ToString();
        (int wkx, int wky) = whiteKingSquare.FromRealToProgrammingCoordinates();
        (int bkx, int bky) = blackKingSquare.FromRealToProgrammingCoordinates();
        layerLogic.gameManager.WhiteKingPosition.Color = kings.whiteKing.Color;
        layerLogic.gameManager.BlackKingPosition.Color = kings.blackKing.Color;
        frontBoard[wkx, wky].BackColor = layerLogic.gameManager.WhiteKingPosition.Color == SquareColor.WHITE ? Color.White : Color.DimGray;
        frontBoard[bkx, bky].BackColor = layerLogic.gameManager.BlackKingPosition.Color == SquareColor.WHITE ? Color.White : Color.DimGray;
    }
    
    private void ColorsRender()
    {
        this.whoPlaysLabel.ForeColor = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? Color.White : Color.Black;
        this.whoPlaysLabel.Text = layerLogic.gameManager.WhoPlays.ToString();
    }
    private void FrontBoardImagesFill()
    {
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
    private T InstancesContruction<T>() where T : class, new() => new T();
    #endregion
}


