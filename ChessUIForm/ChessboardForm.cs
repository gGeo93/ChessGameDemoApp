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
    ChessBoard chessBoard;
    int moveCounter;
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
        chessBoard = InstancesContruction<ChessBoard>();
        moveCounter = 0;
        layerLogic.gameManager.MoveCompletionCounter = 0;
        ColorsRender();
        FrontBoardImagesFill();
    }
    #endregion

    #region [TouchSquareEvent]
    private void square_pressed(object sender, EventArgs e)
    {
        #region [MoveInitilization]
        int x, y;
        MoveInitilization(sender, out x, out y);
        #endregion

        #region [Constraints]
        if (FirstConstraints(x, y, chessBoard))
            return;
        #endregion

        #region [FirstHalfOfTheMove]
        FirstHalfMove(sender, x, y, chessBoard);
        #endregion

        #region [SecondHalfOfTheMove]
        SecondHalfMove(sender, x, y, chessBoard);
        #endregion

        #region[LastUpdate]
        LastBackBoardUpdate(x, y);
        #endregion
    }

    private void MoveInitilization(object sender, out int x, out int y)
    {
        ColorsRender();

        HalfMoveCounter();

        string square = SquarePressed(sender);

        (x, y) = square.FromVisualToProgrammingCoordinates();

        CoordinatesStorage(x, y);

    }

    private bool SecondHalfMove(object sender, int x, int y, ChessBoard chessBoard)
    {
        if (moveCounter == 2)
        {
            string previousSquare = layerLogic.PreviousSquare();

            (int xfrom, int yfrom) = previousSquare.FromVisualToProgrammingCoordinates();

            if (AbsolutePinCase(x, y, chessBoard, xfrom, yfrom))
                return true;

            ReColoringKingsSquare();

            CurrentBoardRelatedInfo(x, y, chessBoard);

            layerLogic.boardRelatedInfoMove[1] = layerLogic.currentBoardRelatedInfo;

            FirstFrontBoardUpdate(sender, x, y);

            FrontSquareColorUpdate();

            bool canMoveChosenWay = CanMoveChosenWay();
            bool isThereNoObstacle = IsThereNoObstacle(chessBoard);
            bool canCutEnPass = CanCutEnPass(chessBoard);
            bool canCastleShort = CanCastleShort(chessBoard);
            bool canCastleLong = CanCastleLong(chessBoard);

            if (CastlingShortCase(chessBoard, canCastleShort))
                return true;

            if (CastlingLongCase(chessBoard, canCastleLong))
                return true;

            if (EnPassentCase(sender, x, y, chessBoard, canCutEnPass))
                return true;

            if (LastMovementContraints(chessBoard, canMoveChosenWay, isThereNoObstacle))
                return true;

            KingHasMovedChecking(x, y, chessBoard);

            SecondFrontBoardUpdate(sender);

            BackBoardUpdate(x, y, chessBoard);

            ClearPreiviousPressedSquare();

            MoveCounterReset();

            Square kingPosition = GetKingSquare();

            BoardColorsRefresh();

            KingCheckAndPossibleMate(chessBoard, kingPosition);

            ParametersReset();

            UpdateRooksPossibleMove(chessBoard);

            GamingProcessUpdate(chessBoard);

            return false;
        }
        return false;
    }

    private void UpdateRooksPossibleMove(ChessBoard chessBoard)
    {
        layerLogic.RooksMovingState = chessBoard.RooksCheck(layerLogic.RooksMovingState);
    }

    private void LastBackBoardUpdate(int x, int y)
    {
        layerLogic.gameManager.ChessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
        layerLogic.gameManager.ChessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
    }

    private void ParametersReset()
    {
        WhoPlaysChanges();

        ColorsRender();

        moveParts = new Button[2];
    }

    private void GamingProcessUpdate(ChessBoard chessBoard)
    {
        chessBoard.WholeGameChessBoard.Add(chessBoard.Board);
    }

    private void WhoPlaysChanges()
    {
        layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
    }

    private void KingCheckAndPossibleMate(ChessBoard chessBoard, Square kingPosition)
    {
        if (SpecialEvents.kingIsChecked.Invoke(chessBoard, kingPosition, layerLogic.gameManager.WhoPlays, false))
        {
            String kingSquare = kingPosition.Letter.ToString() + kingPosition.Number.ToString();
            
            (int kx, int ky) = kingSquare.FromVisualToProgrammingCoordinates();
            
            (int xc, int yc) = chessBoard.ThePieceGivingCheckCoordinates(kingPosition, layerLogic.gameManager.WhoPlays);
            
            frontBoard[kx, ky].BackColor =
                SpecialEvents.kingIsMate.Invoke(chessBoard, kingPosition, chessBoard.Board[xc, yc].ASquare, true, layerLogic.gameManager.WhoPlays)
                ? Color.Red : Color.DarkOrange;
        }
    }

    private Square GetKingSquare()
    {
        return layerLogic.gameManager.WhoPlays == WhoseTurn.White ? layerLogic.gameManager.BlackKingPosition : layerLogic.gameManager.WhiteKingPosition;
    }

    private void MoveCounterReset()
    {
        moveCounter = 0;
    }

    private void ClearPreiviousPressedSquare()
    {
        moveParts[0].Image = null;
    }

    private void BackBoardUpdate(int x, int y, ChessBoard chessBoard)
    {
        chessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
        chessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
        chessBoard.Board[x, y].ApieceOccupySquare = true;
        chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = false;
        chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = null;
    }

    private void SecondFrontBoardUpdate(object sender)
    {
        ((Button)sender).Image = moveParts[0].Image;
    }

    private void KingHasMovedChecking(int x, int y, ChessBoard chessBoard)
    {
        if (chessBoard.Board[x, y].Apiece?.Name == PieceName.KING && layerLogic.gameManager.WhoPlays == WhoseTurn.White)
            SpecialEvents.BlackKingHasMoved = () => true;
        if (chessBoard.Board[x, y].Apiece?.Name == PieceName.KING && layerLogic.gameManager.WhoPlays == WhoseTurn.Black)
            SpecialEvents.WhiteKingHasMoved = () => true;
    }

    private bool LastMovementContraints(ChessBoard chessBoard, bool canMoveChosenWay, bool isThereNoObstacle)
    {
        if (canMoveChosenWay == false || isThereNoObstacle == false)
        {
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = true;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = layerLogic.boardRelatedInfoMove[0].Apiece;
            moveCounter = 0;
            moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            return true;
        }
        return false;
    }

    private bool EnPassentCase(object sender, int x, int y, ChessBoard chessBoard, bool canCutEnPass)
    {
        if (canCutEnPass)
        {
            ((Button)sender).Image = moveParts[0].Image;
            chessBoard.Board[x, y].ApieceOccupySquare = true;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = false;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = null;
            moveParts[0].Image = null;
            frontBoard[layerLogic.gameManager.WhoPlays == WhoseTurn.White ? x + 1 : x - 1, y].Image = null;
            moveCounter = 0;
            layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
            ColorsRender();
            moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            layerLogic.gameManager.ChessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
            layerLogic.gameManager.ChessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
            SpecialEvents.pawnHasJustMovedTwice = () => (-1, -1);
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private bool CastlingLongCase(ChessBoard chessBoard, bool canCastleLong)
    {
        if (canCastleLong && layerLogic.boardRelatedInfoMove[1].Apiece?.Name == PieceName.KING && layerLogic.boardRelatedInfoMove[1].ASquare.Letter == 'c')
        {
            if (WhiteLongCastlingProcess(chessBoard))
                return true;
            if (BlackLongCastlingProcess(chessBoard))
                return true;
        }
        return false;
    }

    private bool BlackLongCastlingProcess(ChessBoard chessBoard)
    {
        if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 8 && layerLogic.RooksMovingState[0])
        {
            frontBoard[0, 2].Image = frontBoard[0, 4].Image;
            frontBoard[0, 4].Image = null;
            frontBoard[0, 3].Image = frontBoard[0, 0].Image;
            frontBoard[0, 0].Image = null;
            chessBoard.Board[0, 3].Apiece = chessBoard.Board[0, 0].Apiece;
            chessBoard.Board[0, 3].ApieceOccupySquare = true;
            chessBoard.Board[0, 2].Apiece = new King { Color = PieceInfo.BLACK, Name = PieceName.KING };
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
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private bool WhiteLongCastlingProcess(ChessBoard chessBoard)
    {
        if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 1 && layerLogic.RooksMovingState[2])
        {
            frontBoard[7, 2].Image = frontBoard[7, 4].Image;
            frontBoard[7, 4].Image = null;
            frontBoard[7, 3].Image = frontBoard[7, 0].Image;
            frontBoard[7, 0].Image = null;
            chessBoard.Board[7, 3].Apiece = chessBoard.Board[7, 0].Apiece;
            chessBoard.Board[7, 3].ApieceOccupySquare = true;
            chessBoard.Board[7, 2].Apiece = new King { Color = PieceInfo.WHITE, Name = PieceName.KING };
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
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private bool CastlingShortCase(ChessBoard chessBoard, bool canCastleShort)
    {
        if (canCastleShort && layerLogic.boardRelatedInfoMove[1].Apiece?.Name == PieceName.KING && layerLogic.boardRelatedInfoMove[1].ASquare.Letter == 'g')
        {
            if (WhiteShortCastlingProcess(chessBoard))
                return true;
            if (BlackShortCastlingProcess(chessBoard))
                return true;
        }
        return false;
    }

    private bool BlackShortCastlingProcess(ChessBoard chessBoard)
    {
        if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 8 && layerLogic.RooksMovingState[1])
        {
            frontBoard[0, 6].Image = frontBoard[0, 4].Image;
            frontBoard[0, 4].Image = null;
            frontBoard[0, 5].Image = frontBoard[0, 7].Image;
            frontBoard[0, 7].Image = null;
            chessBoard.Board[0, 5].Apiece = chessBoard.Board[0, 7].Apiece;
            chessBoard.Board[0, 5].ApieceOccupySquare = true;
            chessBoard.Board[0, 6].Apiece = new King { Color = PieceInfo.BLACK, Name = PieceName.KING };
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
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private bool WhiteShortCastlingProcess(ChessBoard chessBoard)
    {
        if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 1 && layerLogic.RooksMovingState[3])
        {
            frontBoard[7, 6].Image = frontBoard[7, 4].Image;
            frontBoard[7, 4].Image = null;
            frontBoard[7, 5].Image = frontBoard[7, 7].Image;
            frontBoard[7, 7].Image = null;
            chessBoard.Board[7, 5].Apiece = chessBoard.Board[7, 7].Apiece;
            chessBoard.Board[7, 5].ApieceOccupySquare = true;
            chessBoard.Board[7, 6].Apiece = new King { Color = PieceInfo.WHITE, Name = PieceName.KING };
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
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private bool CanCastleLong(ChessBoard chessBoard)
    {
        return chessBoard.KingCanCastleLong(layerLogic.gameManager.WhoPlays);
    }

    private bool CanCastleShort(ChessBoard chessBoard)
    {
        return chessBoard.KingCanCastleShort(layerLogic.gameManager.WhoPlays);
    }

    private bool CanCutEnPass(ChessBoard chessBoard)
    {
        return chessBoard.Board.CanTakeEnPassant(layerLogic.gameManager.WhoPlays, layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare);
    }

    private bool IsThereNoObstacle(ChessBoard chessBoard)
    {
        return layerLogic.currentBoardRelatedInfo.Apiece.Name.ThereIsNoObstacle(layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare, chessBoard, layerLogic.gameManager.WhoPlays);
    }

    private bool CanMoveChosenWay()
    {
        return layerLogic.currentBoardRelatedInfo.Apiece.Name.CanPerfomeThisMove(layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare, layerLogic.gameManager.WhoPlays, layerLogic.currentBoardRelatedInfo.Apiece.Color);
    }

    private void FrontSquareColorUpdate()
    {
        moveParts[0].BackColor = squareColor;
    }

    private void FirstFrontBoardUpdate(object sender, int x, int y)
    {
        frontBoard[x, y] = ((Button)sender);
    }

    private void CurrentBoardRelatedInfo(int x, int y, ChessBoard chessBoard)
    {
        layerLogic.currentBoardRelatedInfo = new BoardRelatedInfo()
        {
            Apiece = new Piece
            {
                Name = layerLogic.boardRelatedInfoMove[0].Apiece!.Name,
                Color = layerLogic.boardRelatedInfoMove[0].Apiece!.Color
            },
            ASquare = chessBoard.Board[x, y].ASquare
        };
    }

    private bool AbsolutePinCase(int x, int y, ChessBoard chessBoard, int xfrom, int yfrom)
    {
        if (PieceIsPinned(chessBoard, xfrom, yfrom, x, y))
        {
            frontBoard[xfrom, yfrom].BackColor = chessBoard.Board[xfrom, yfrom].ASquare.Color == SquareColor.WHITE ? Color.White : Color.DimGray;
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private void FirstHalfMove(object sender, int x, int y, ChessBoard chessBoard)
    {
        if (chessBoard.Board[x, y].ApieceOccupySquare && moveCounter == 1)
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
    }

    private bool FirstConstraints(int x, int y, ChessBoard chessBoard)
    {
        if (!chessBoard.Board[x, y].ApieceOccupySquare && moveCounter == 1)
        {
            moveCounter = 0;
            return true;
        }
        return false;
    }

    private ChessBoard BackChessBoard()
    {
        return layerLogic.gameManager.ChessBoard;
    }

    private void CoordinatesStorage(int x, int y)
    {
        layerLogic.coordinates[moveCounter - 1] = (x, y);
    }

    private string SquarePressed(object sender)
    {
        return ((Button)sender).Name;
    }

    private void HalfMoveCounter()
    {
        moveCounter += 1;
    }
    #endregion

    private bool PieceIsPinned(ChessBoard chessBoard,int xfrom, int yfrom, int xto, int yto)
    {
        chessBoard.Board[xto, yto].Apiece = chessBoard.Board[xfrom, yfrom].Apiece;
        chessBoard.Board[xto, yto].ApieceOccupySquare = true;

        chessBoard.Board[xfrom, yfrom].Apiece = null;
        chessBoard.Board[xfrom, yfrom].ApieceOccupySquare = false;

        var kingPosition = chessBoard.Board[xto, yto].Apiece?.Name == PieceName.KING ? chessBoard.Board[xto, yto].ASquare : layerLogic.gameManager.WhoPlays == WhoseTurn.Black ? layerLogic.gameManager.BlackKingPosition : layerLogic.gameManager.WhiteKingPosition;

        if (SpecialEvents.kingIsChecked.Invoke(chessBoard, kingPosition, layerLogic.gameManager.WhoPlays, true))
        {
            chessBoard.Board[xfrom, yfrom].Apiece = chessBoard.Board[xto, yto].Apiece;
            chessBoard.Board[xfrom, yfrom].ApieceOccupySquare = true;

            chessBoard.Board[xto, yto].Apiece = null;
            chessBoard.Board[xto, yto].ApieceOccupySquare = false;
            
            return true;
        }
        else if(chessBoard.Board[xto, yto].Apiece?.Name != PieceName.KING)
        {
            chessBoard.Board[xfrom, yfrom].Apiece = chessBoard.Board[xto, yto].Apiece;
            chessBoard.Board[xfrom, yfrom].ApieceOccupySquare = true;

            chessBoard.Board[xto, yto].Apiece = null;
            chessBoard.Board[xto, yto].ApieceOccupySquare = false;

            return false;
        }
        return false;
    }
    private void ReColoringKingsSquare()
    {
        var kings = chessBoard.GetKingsPositions();
        String whiteKingSquare = kings.whiteKing.Letter + kings.whiteKing.Number.ToString();
        String blackKingSquare = kings.blackKing.Letter + kings.blackKing.Number.ToString();
        (int wkx, int wky) = whiteKingSquare.FromVisualToProgrammingCoordinates();
        (int bkx, int bky) = blackKingSquare.FromVisualToProgrammingCoordinates();
        layerLogic.gameManager.WhiteKingPosition = kings.whiteKing;
        layerLogic.gameManager.BlackKingPosition = kings.blackKing;
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
    private void BoardColorsRefresh()
    {
        for (int i = 0; i < chessBoard.Board.GetLength(0); i++)
        {
            for (int j = 0; j < chessBoard.Board.GetLength(1); j++)
            {
                if ((i + j) % 2 == 0)
                {
                    chessBoard.Board[i, j].ASquare.Color = SquareColor.WHITE;
                    frontBoard[i, j].BackColor = Color.White;
                }
                else
                {
                    chessBoard.Board[i, j].ASquare.Color = SquareColor.BLACK;
                    frontBoard[i, j].BackColor = Color.DimGray;
                }

            }
            
        }
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
}


