﻿using System;
using System.Linq;
using BusinessLogic;
using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;
using ChessLibrary.RulesRelated;
using ChessLibrary.SpecialOccasionsRelated;
using ChessUIForm.FrontLib;

namespace ChessUIForm;

public partial class ChessboardForm : Form
{
    #region [BusinessLogicLayer]
    MiddleLayerLogic layerLogic;
    #endregion

    #region [UIFields]
    FrontLogic frontLogic;
    #endregion

    #region [Init]
    public ChessboardForm()
    {
        InitializeComponent();
        InitializeUI();
        InitializeBack();
        ColorsRender();
        FrontBoardImagesFill();
    }
    private void InitializeBack()
    {
        layerLogic = InstancesContructor<MiddleLayerLogic>();
        layerLogic.chessBoard = new ChessBoard();
        layerLogic.moveCounter = 0;
        layerLogic.gameManager.MoveCompletionCounter = 0;
        layerLogic.xpromotion = -1;
        layerLogic.ypromotion = -1;
    }
    private void InitializeUI()
    {
        frontLogic = InstancesContructor<FrontLogic>();
        frontLogic.frontBoard = new Button[8, 8];
        frontLogic.moveParts = new Button[2];
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
        if (FirstConstraints(x, y, layerLogic.chessBoard))
            return;
        #endregion

        #region [FirstHalfOfTheMove]
        FirstHalfMove(sender, x, y, layerLogic.chessBoard);
        #endregion

        #region [SecondHalfOfTheMove]
        if (SecondHalfMove(sender, x, y, layerLogic.chessBoard))
            return;
        #endregion

        #region[LastUpdate]
        LastBackBoardUpdate(x, y);
        #endregion
    }
    #endregion

    #region [EventsMainMethods]
    private void MoveInitilization(object sender, out int x, out int y)
    {
        BoardColorsRefresh();

        ColorsRender();

        HalfMoveCounter();

        string square = SquarePressed(sender);

        (x, y) = square.FromVisualToProgrammingCoordinates();

        CoordinatesStorage(x, y);

        PromotionScenario(sender, x, y);
    }
    private bool FirstConstraints(int x, int y, ChessBoard chessBoard)
    {
        if ((!chessBoard.Board[x, y].ApieceOccupySquare && layerLogic.moveCounter == 1)
            || (layerLogic.moveCounter == 1 && layerLogic.gameManager.WhoPlays == WhoseTurn.White && chessBoard.Board[x, y].Apiece?.Color == PieceInfo.BLACK)
            || (layerLogic.moveCounter == 1 && layerLogic.gameManager.WhoPlays == WhoseTurn.Black && chessBoard.Board[x, y].Apiece?.Color == PieceInfo.WHITE))
        {
            layerLogic.moveCounter = 0;
            return true;
        }
        return false;
    }
    private void FirstHalfMove(object sender, int x, int y, ChessBoard chessBoard)
    {
        if (chessBoard.Board[x, y].ApieceOccupySquare && layerLogic.moveCounter == 1)
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
            if (frontLogic.moveParts[1] != null)
                frontLogic.frontBoard[x, y].Image = null;
            frontLogic.moveParts[0] = ((Button)sender);
            frontLogic.squareColor = frontLogic.moveParts[0].BackColor;
            frontLogic.moveParts[0].BackColor = Color.Brown;
        }
    }
    private bool SecondHalfMove(object sender, int x, int y, ChessBoard chessBoard)
    {
        if (layerLogic.moveCounter == 2)
        {
            string previousSquare = layerLogic.PreviousSquare();

            (int xfrom, int yfrom) = previousSquare.FromVisualToProgrammingCoordinates();

            ReColoringKingsSquare();

            var copiedBoard = chessBoard.Clone2DimArray();

            if (AbsolutePinCase(x, y, copiedBoard, xfrom, yfrom))
                return true;

            CurrentBoardRelatedInfo(x, y, chessBoard);

            layerLogic.boardRelatedInfoMove[1] = layerLogic.currentBoardRelatedInfo;

            bool canCastleShort = CanCastleShort(chessBoard);
            bool canCastleLong = CanCastleLong(chessBoard);
            bool canMoveChosenWay = CanMoveChosenWay();
            bool isThereNoObstacle = IsThereNoObstacle(chessBoard);
            bool canCutEnPass = CanCutEnPass(chessBoard);
            bool isPawnPromoting = IsPawnPromoting(chessBoard, xfrom, yfrom, x, y);

            if (isPawnPromoting)
            {
                PromotionOptions(true);
                IsFrozenChessboard(true);
                frontLogic.OnPawnPromotion += (x, y) => { layerLogic.xpromotion = x; layerLogic.ypromotion = y; return ((Button)sender); };        
            }

            if (EnPassentCase(sender, x, y, chessBoard, canCutEnPass))
                return true;

            if (CastlingShortCase(chessBoard, canCastleShort))
                return true;

            if (CastlingLongCase(chessBoard, canCastleLong))
                return true;

            if (LastMovementContraints(chessBoard, canMoveChosenWay, isThereNoObstacle))
                return true;

            FirstFrontBoardUpdate(sender, x, y);

            FrontSquareColorUpdate();

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
    private void promotionBtn(object sender, EventArgs e)
    {
        if (layerLogic.gameManager.WhoPlays == WhoseTurn.Black)
        {
            layerLogic.gameManager.WhoPlays = WhoseTurn.White;
            for (int j = 0; j < 8; j++)
            {
                if (layerLogic.chessBoard.Board[0, j].Apiece?.Name == PieceName.PAWN)
                {
                    IsFrozenChessboard(false);
                    frontLogic.frontBoard[0, j].Image = ((Button)sender).Image;
                    layerLogic.currentBoardRelatedInfo.Apiece = PromotesTo(sender);
                    LastBackBoardUpdate(0, j);
                    Square kingPosition = GetKingSquare();
                    BoardColorsRefresh();
                    KingCheckAndPossibleMate(layerLogic.chessBoard, kingPosition);
                    layerLogic.moveCounter = 0;
                    frontLogic.OnPawnPromotion = null;
                    layerLogic.gameManager.WhoPlays = WhoseTurn.Black;
                    break;
                }
            }
        }
        else if (layerLogic.gameManager.WhoPlays == WhoseTurn.White)
        {
            layerLogic.gameManager.WhoPlays = WhoseTurn.Black;
            for (int j = 0; j < 8; j++)
            {
                if (layerLogic.chessBoard.Board[7, j].Apiece?.Name == PieceName.PAWN)
                {
                    IsFrozenChessboard(false);
                    frontLogic.frontBoard[7, j].Image = ((Button)sender).Image;
                    layerLogic.currentBoardRelatedInfo.Apiece = PromotesTo(sender);
                    LastBackBoardUpdate(7, j);
                    layerLogic.moveCounter = 2;
                    Square kingPosition = GetKingSquare();
                    BoardColorsRefresh();
                    KingCheckAndPossibleMate(layerLogic.chessBoard, kingPosition);
                    layerLogic.moveCounter = 0;
                    frontLogic.OnPawnPromotion = null;
                    layerLogic.gameManager.WhoPlays = WhoseTurn.White;
                    break;
                }
            }
        }
        PromotionOptions(false);
    }
    private object PromotionScenario(object sender, int x, int y)
    {
        if (frontLogic.OnPawnPromotion is not null)
        {
            sender = frontLogic.OnPawnPromotion.Invoke(x, y);
            frontLogic.OnPawnPromotion = null!;
            layerLogic.moveCounter = 2;
        }
        return sender;
    }

    private Piece? PromotesTo(object sender)
    {
        if (layerLogic.gameManager.WhoPlays == WhoseTurn.White)
        {
            switch (((Button)sender).Name)
            {
                case "wQpr": return new Piece { Color = PieceInfo.WHITE, Name = PieceName.QUEEN };
                case "wNpr": return new Piece { Color = PieceInfo.WHITE, Name = PieceName.KNIGHT };
                case "wRpr": return new Piece { Color = PieceInfo.WHITE, Name = PieceName.ROOK };
                case "wBpr": return new Piece { Color = PieceInfo.WHITE, Name = PieceName.BISHOP };
                default: return null;
            }
        }
        else if (layerLogic.gameManager.WhoPlays == WhoseTurn.Black)
        {
            switch (((Button)sender).Name)
            {
                case "bQpr": return new Piece { Color = PieceInfo.BLACK, Name = PieceName.QUEEN };
                case "bNpr": return new Piece { Color = PieceInfo.BLACK, Name = PieceName.KNIGHT };
                case "bRpr": return new Piece { Color = PieceInfo.BLACK, Name = PieceName.ROOK };
                case "bBpr": return new Piece { Color = PieceInfo.BLACK, Name = PieceName.BISHOP };
                default: return null;
            }
        }
        return null;
    }

    private void PromotionOptions(bool isVisible)
    {
        if(layerLogic.gameManager.WhoPlays == WhoseTurn.White)
        {
            wQpr.Visible = !wQpr.Visible;
            wNpr.Visible = !wNpr.Visible;
            wRpr.Visible = !wRpr.Visible;
            wBpr.Visible = !wBpr.Visible;
        }
        if (layerLogic.gameManager.WhoPlays == WhoseTurn.Black)
        {
            bQpr.Visible = !bQpr.Visible;
            bNpr.Visible = !bNpr.Visible;
            bRpr.Visible = !bRpr.Visible;
            bBpr.Visible = !bBpr.Visible;
        }
    }
    private void IsFrozenChessboard(bool isDisabled)
    {
        for (int i = 0; i < frontLogic.frontBoard.GetLength(0); i++)
        {
            for (int j = 0; j < frontLogic.frontBoard.GetLength(1); j++)
            {
                frontLogic.frontBoard[i, j].Enabled = !isDisabled;
            }
        }
    }
    private void LastBackBoardUpdate(int x, int y)
    {
        layerLogic.chessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
        layerLogic.chessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
    }
    #endregion

    #region [EventsSubMethods]
    private void UpdateRooksPossibleMove(ChessBoard chessBoard)
    {
        layerLogic.RooksMovingState = chessBoard.RooksCheck(layerLogic.RooksMovingState);
    }
    private void ParametersReset()
    {
        WhoPlaysChanges();

        ColorsRender();

        frontLogic.moveParts = new Button[2];
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
        if (SpecialEvents.kingIsChecked.Invoke(chessBoard.Board, kingPosition, layerLogic.gameManager.WhoPlays, false))
        {
            String kingSquare = kingPosition.Letter.ToString() + kingPosition.Number.ToString();

            (int kx, int ky) = kingSquare.FromVisualToProgrammingCoordinates();

            (int xc, int yc) = chessBoard.ThePieceGivingCheckCoordinates(kingPosition, layerLogic.gameManager.WhoPlays);

            frontLogic.frontBoard[kx, ky].BackColor =
                SpecialEvents.kingIsMate.Invoke(chessBoard.Board, kingPosition, chessBoard.Board[xc, yc].ASquare, true, layerLogic.gameManager.WhoPlays)
                ? Color.Red : Color.DarkOrange;
        }
    }
    private Square GetKingSquare()
    {
        return layerLogic.gameManager.WhoPlays == WhoseTurn.White ? layerLogic.gameManager.BlackKingPosition : layerLogic.gameManager.WhiteKingPosition;
    }
    private void MoveCounterReset()
    {
        layerLogic.moveCounter = 0;
    }
    private void ClearPreiviousPressedSquare()
    {
        frontLogic.moveParts[0].Image = null;
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
        ((Button)sender).Image = frontLogic.moveParts[0].Image;
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
            layerLogic.moveCounter = 0;
            frontLogic.moveParts = new Button[2];
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
            ((Button)sender).Image = frontLogic.moveParts[0].Image;
            chessBoard.Board[x, y].ApieceOccupySquare = true;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].ApieceOccupySquare = false;
            chessBoard.Board[layerLogic.coordinates[0].x, layerLogic.coordinates[0].y].Apiece = null;
            frontLogic.moveParts[0].Image = null;
            frontLogic.frontBoard[layerLogic.gameManager.WhoPlays == WhoseTurn.White ? x + 1 : x - 1, y].Image = null;
            layerLogic.moveCounter = 0;
            layerLogic.gameManager.WhoPlays = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? WhoseTurn.Black : WhoseTurn.White;
            ColorsRender();
            frontLogic.moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            layerLogic.gameManager.ChessBoard.Board[x, y].Apiece = layerLogic.currentBoardRelatedInfo.Apiece;
            layerLogic.gameManager.ChessBoard.Board[x, y].ASquare = layerLogic.currentBoardRelatedInfo.ASquare;
            SpecialEvents.pawnHasJustMovedTwice = () => (-1, -1);
            layerLogic.moveCounter = 0;
            LastBackBoardUpdate(x, y);
            return true;
        }
        return false;
    }
    private bool IsPawnPromoting(ChessBoard chessBoard, int xfromPosition, int yfromPosition, int xtoPosition, int ytoPosition)
        =>
        (xtoPosition == 0 || xtoPosition == 7) && (chessBoard.Board[xfromPosition, yfromPosition]?.Apiece?.Name == PieceName.PAWN);

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
            frontLogic.frontBoard[0, 2].Image = frontLogic.frontBoard[0, 4].Image;
            frontLogic.frontBoard[0, 4].Image = null;
            frontLogic.frontBoard[0, 3].Image = frontLogic.frontBoard[0, 0].Image;
            frontLogic.frontBoard[0, 0].Image = null;
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
            frontLogic.moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            layerLogic.moveCounter = 0;
            return true;
        }
        return false;
    }
    private bool WhiteLongCastlingProcess(ChessBoard chessBoard)
    {
        if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 1 && layerLogic.RooksMovingState[2])
        {
            frontLogic.frontBoard[7, 2].Image = frontLogic.frontBoard[7, 4].Image;
            frontLogic.frontBoard[7, 4].Image = null;
            frontLogic.frontBoard[7, 3].Image = frontLogic.frontBoard[7, 0].Image;
            frontLogic.frontBoard[7, 0].Image = null;
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
            frontLogic.moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            layerLogic.moveCounter = 0;
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
            frontLogic.frontBoard[0, 6].Image = frontLogic.frontBoard[0, 4].Image;
            frontLogic.frontBoard[0, 4].Image = null;
            frontLogic.frontBoard[0, 5].Image = frontLogic.frontBoard[0, 7].Image;
            frontLogic.frontBoard[0, 7].Image = null;
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
            frontLogic.moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            layerLogic.moveCounter = 0;
            return true;
        }
        return false;
    }
    private bool WhiteShortCastlingProcess(ChessBoard chessBoard)
    {
        if (layerLogic.boardRelatedInfoMove[1].ASquare.Number == 1 && layerLogic.RooksMovingState[3])
        {
            frontLogic.frontBoard[7, 6].Image = frontLogic.frontBoard[7, 4].Image;
            frontLogic.frontBoard[7, 4].Image = null;
            frontLogic.frontBoard[7, 5].Image = frontLogic.frontBoard[7, 7].Image;
            frontLogic.frontBoard[7, 7].Image = null;
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
            frontLogic.moveParts = new Button[2];
            layerLogic.boardRelatedInfoMove = new BoardRelatedInfo[2];
            layerLogic.gameManager.Move = new Square[2];
            layerLogic.coordinates = new (int x, int y)[2];
            layerLogic.moveCounter = 0;
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
        return chessBoard.Board.KingCanCastleShort(layerLogic.gameManager.WhoPlays);
    }
    private bool CanCutEnPass(ChessBoard chessBoard)
    {
        return chessBoard.Board.CanTakeEnPassant(layerLogic.gameManager.WhoPlays, layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare);
    }
    private bool IsThereNoObstacle(ChessBoard chessBoard)
    {
        return layerLogic.currentBoardRelatedInfo.Apiece.Name.ThereIsNoObstacle(layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare, chessBoard.Board, layerLogic.gameManager.WhoPlays);
    }
    private bool CanMoveChosenWay()
    {
        return layerLogic.currentBoardRelatedInfo.Apiece!.Name.CanPerfomeThisMove(layerLogic.boardRelatedInfoMove[0].ASquare, layerLogic.boardRelatedInfoMove[1].ASquare, layerLogic.gameManager.WhoPlays, layerLogic.currentBoardRelatedInfo.Apiece.Color);
    }
    private void FrontSquareColorUpdate()
    {
        frontLogic.moveParts[0].BackColor = frontLogic.squareColor;
    }
    private void FirstFrontBoardUpdate(object sender, int x, int y)
    {
        frontLogic. frontBoard[x, y] = ((Button)sender);
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
            ApieceOccupySquare = true,
            ASquare = chessBoard.Board[x, y].ASquare
        };
    }
    private bool AbsolutePinCase(int x, int y, BoardRelatedInfo[,] copiedBoard, int xfrom, int yfrom)
    {
        if (PieceIsPinned(copiedBoard, xfrom, yfrom, x, y))
        {
            frontLogic.frontBoard[xfrom, yfrom].BackColor = copiedBoard[xfrom, yfrom].ASquare.Color == SquareColor.WHITE ? Color.White : Color.DimGray;
            layerLogic.moveCounter = 0;
            return true;
        }
        return false;
    }
    private void CoordinatesStorage(int x, int y)
    {
        layerLogic.coordinates[layerLogic.moveCounter - 1] = (x, y);
    }
    private string SquarePressed(object sender) => ((Button)sender).Name;
    private void HalfMoveCounter()
    {
        layerLogic.moveCounter += 1;
    }
    private bool PieceIsPinned(BoardRelatedInfo[,] copiedBoard, int xfrom, int yfrom, int xto, int yto)
    {
        copiedBoard[xto, yto].Apiece = copiedBoard[xfrom, yfrom].Apiece;
        copiedBoard[xto, yto].ApieceOccupySquare = true;

        copiedBoard[xfrom, yfrom].Apiece = null;
        copiedBoard[xfrom, yfrom].ApieceOccupySquare = false;

        var kingPosition = copiedBoard[xto, yto].Apiece?.Name == PieceName.KING ? copiedBoard[xto, yto].ASquare : layerLogic.gameManager.WhoPlays == WhoseTurn.Black ? layerLogic.gameManager.BlackKingPosition : layerLogic.gameManager.WhiteKingPosition;

        if (SpecialEvents.kingIsChecked.Invoke(copiedBoard, kingPosition, layerLogic.gameManager.WhoPlays, true))
        {
            copiedBoard[xfrom, yfrom].Apiece = copiedBoard[xto, yto].Apiece;
            copiedBoard[xfrom, yfrom].ApieceOccupySquare = true;

            copiedBoard[xto, yto].Apiece = null;
            copiedBoard[xto, yto].ApieceOccupySquare = false;

            return true;
        }
        return false;
    }
    private void ReColoringKingsSquare()
    {
        var kings = layerLogic.chessBoard.GetKingsPositions();
        String whiteKingSquare = kings.whiteKing.Letter + kings.whiteKing.Number.ToString();
        String blackKingSquare = kings.blackKing.Letter + kings.blackKing.Number.ToString();
        (int wkx, int wky) = whiteKingSquare.FromVisualToProgrammingCoordinates();
        (int bkx, int bky) = blackKingSquare.FromVisualToProgrammingCoordinates();
        layerLogic.gameManager.WhiteKingPosition = kings.whiteKing;
        layerLogic.gameManager.BlackKingPosition = kings.blackKing;
        layerLogic.gameManager.WhiteKingPosition.Color = kings.whiteKing.Color;
        layerLogic.gameManager.BlackKingPosition.Color = kings.blackKing.Color;
        frontLogic.frontBoard[wkx, wky].BackColor = layerLogic.gameManager.WhiteKingPosition.Color == SquareColor.WHITE ? Color.White : Color.DimGray;
        frontLogic.frontBoard[bkx, bky].BackColor = layerLogic.gameManager.BlackKingPosition.Color == SquareColor.WHITE ? Color.White : Color.DimGray;
    }
    private void ColorsRender()
    {
        this.whoPlaysLabel.ForeColor = layerLogic.gameManager.WhoPlays == WhoseTurn.White ? Color.White : Color.Black;
        this.whoPlaysLabel.Text = layerLogic.gameManager.WhoPlays.ToString();
    }
    private void BoardColorsRefresh()
    {
        for (int i = 0; i < layerLogic.chessBoard.Board.GetLength(0); i++)
        {
            for (int j = 0; j < layerLogic.chessBoard.Board.GetLength(1); j++)
            {
                if ((i + j) % 2 == 0)
                {
                    layerLogic.chessBoard.Board[i, j].ASquare.Color = SquareColor.WHITE;
                    frontLogic.frontBoard[i, j].BackColor = Color.White;
                }
                else
                {
                    layerLogic.chessBoard.Board[i, j].ASquare.Color = SquareColor.BLACK;
                    frontLogic.frontBoard[i, j].BackColor = Color.DimGray;
                }

            }

        }
    }
    private void FrontBoardImagesFill()
    {
        frontLogic.frontBoard[0, 0] = this.a8;
        frontLogic.frontBoard[0, 1] = this.b8;
        frontLogic.frontBoard[0, 2] = this.c8;
        frontLogic.frontBoard[0, 3] = this.d8;
        frontLogic.frontBoard[0, 4] = this.e8;
        frontLogic.frontBoard[0, 5] = this.f8;
        frontLogic.frontBoard[0, 6] = this.g8;
        frontLogic.frontBoard[0, 7] = this.h8;
        frontLogic.frontBoard[1, 0] = this.a7;
        frontLogic.frontBoard[1, 1] = this.b7;
        frontLogic.frontBoard[1, 2] = this.c7;
        frontLogic.frontBoard[1, 3] = this.d7;
        frontLogic.frontBoard[1, 4] = this.e7;
        frontLogic.frontBoard[1, 5] = this.f7;
        frontLogic.frontBoard[1, 6] = this.g7;
        frontLogic.frontBoard[1, 7] = this.h7;
        frontLogic.frontBoard[2, 0] = this.a6;
        frontLogic.frontBoard[2, 1] = this.b6;
        frontLogic.frontBoard[2, 2] = this.c6;
        frontLogic.frontBoard[2, 3] = this.d6;
        frontLogic.frontBoard[2, 4] = this.e6;
        frontLogic.frontBoard[2, 5] = this.f6;
        frontLogic.frontBoard[2, 6] = this.g6;
        frontLogic.frontBoard[2, 7] = this.h6;
        frontLogic.frontBoard[3, 0] = this.a5;
        frontLogic.frontBoard[3, 1] = this.b5;
        frontLogic.frontBoard[3, 2] = this.c5;
        frontLogic.frontBoard[3, 3] = this.d5;
        frontLogic.frontBoard[3, 4] = this.e5;
        frontLogic.frontBoard[3, 5] = this.f5;
        frontLogic.frontBoard[3, 6] = this.g5;
        frontLogic.frontBoard[3, 7] = this.h5;
        frontLogic.frontBoard[4, 0] = this.a4;
        frontLogic.frontBoard[4, 1] = this.b4;
        frontLogic.frontBoard[4, 2] = this.c4;
        frontLogic.frontBoard[4, 3] = this.d4;
        frontLogic.frontBoard[4, 4] = this.e4;
        frontLogic.frontBoard[4, 5] = this.f4;
        frontLogic.frontBoard[4, 6] = this.g4;
        frontLogic.frontBoard[4, 7] = this.h4;
        frontLogic.frontBoard[5, 0] = this.a3;
        frontLogic.frontBoard[5, 1] = this.b3;
        frontLogic.frontBoard[5, 2] = this.c3;
        frontLogic.frontBoard[5, 3] = this.d3;
        frontLogic.frontBoard[5, 4] = this.e3;
        frontLogic.frontBoard[5, 5] = this.f3;
        frontLogic.frontBoard[5, 6] = this.g3;
        frontLogic.frontBoard[5, 7] = this.h3;
        frontLogic.frontBoard[6, 0] = this.a2;
        frontLogic.frontBoard[6, 1] = this.b2;
        frontLogic.frontBoard[6, 2] = this.c2;
        frontLogic.frontBoard[6, 3] = this.d2;
        frontLogic.frontBoard[6, 4] = this.e2;
        frontLogic.frontBoard[6, 5] = this.f2;
        frontLogic.frontBoard[6, 6] = this.g2;
        frontLogic.frontBoard[6, 7] = this.h2;
        frontLogic.frontBoard[7, 0] = this.a1;
        frontLogic.frontBoard[7, 1] = this.b1;
        frontLogic.frontBoard[7, 2] = this.c1;
        frontLogic.frontBoard[7, 3] = this.d1;
        frontLogic.frontBoard[7, 4] = this.e1;
        frontLogic.frontBoard[7, 5] = this.f1;
        frontLogic.frontBoard[7, 6] = this.g1;
        frontLogic.frontBoard[7, 7] = this.h1;
    }
    private T InstancesContructor<T>() where T : class, new() => new T();
    #endregion
}