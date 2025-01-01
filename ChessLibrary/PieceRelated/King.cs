﻿using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.RulesRelated;

namespace ChessLibrary.PieceRelated;

public class King : Piece, IMove
{
    private bool _doubleCheck = false;
    public King()
    {
        if (SpecialEvents.whiteKingIsChecked != null && SpecialEvents.whiteKingIsMate != null)
            return;
        
        if (SpecialEvents.whiteKingIsChecked == null)
            SpecialEvents.whiteKingIsChecked = KingIsChecked;

        if (SpecialEvents.whiteKingIsMate == null)
            SpecialEvents.whiteKingIsMate = KingIsMate;
    }
    
    public bool Movement(Square from, Square to)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        if (MathF.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo) != 1 && MathF.Abs(pseudoCoorFrom.yFrom - pseudoCoorTo.yTo) != 1)
             return false;

        return true;
    }
    private bool KingIsChecked(ChessBoard chessBoard, Square whiteKingPosition, WhoseTurn turn)
    {
        String kingSquare = whiteKingPosition.Letter.ToString() + whiteKingPosition.Number.ToString();
        (int kx, int ky) = kingSquare.FromRealToProgrammingCoordinates();
        bool canPerformMove = false;
        bool thereAreNoObstacles = false;
        int howManyChecks = 0;
        var colorToPass = turn == WhoseTurn.White ? PieceInfo.BLACK : PieceInfo.WHITE;
        for (int x = 0; x < chessBoard.Board.GetLength(0); x++)
        {
            for (int y = 0; y < chessBoard.Board.GetLength(1); y++)
            {
                Piece? piece = chessBoard.Board[x, y]?.Apiece;
                if (piece is null || piece.Color == colorToPass) 
                    continue;
                
                PieceName pieceName = piece.Name;
                Square pSquare = chessBoard.Board[x, y].ASquare;
                canPerformMove = pieceName.CanPerfomeThisMove(pSquare, chessBoard.Board[kx, ky].ASquare, turn);
                thereAreNoObstacles = pieceName.ThereIsNoObstacle(pSquare, chessBoard.Board[kx, ky].ASquare, chessBoard, turn);
                if (canPerformMove && thereAreNoObstacles)
                    howManyChecks++;
            }
        }
        switch (howManyChecks)
        {
            case 0: return false;
            case 1: return true;
            case 2: _doubleCheck = true; return true;
            default: return false;
        }
    }
    private bool KingIsMate(ChessBoard chessBoard, Square threatedKingPosition, Square attackingPiecePosition, bool isChecked, WhoseTurn turn)
    {
        if (!isChecked)
            return false;
        
        bool kingCannotMove = KingCannotMove(chessBoard, threatedKingPosition, turn);
        
        if (kingCannotMove && _doubleCheck)
            return true;
        if (!kingCannotMove)
            return false;
        if(!AttackingPieceCannotBeCaptured(chessBoard, attackingPiecePosition, turn))
            return false;
        if(!BetweenAttackingPieceAndKingNopieceCanBlock(chessBoard, threatedKingPosition, attackingPiecePosition, turn))
            return false;
        
        return true;
    }
    private bool KingCannotMove(ChessBoard chessBoard, Square threatedKingPosition, WhoseTurn turn)
    {
        String kpos = threatedKingPosition.Letter + threatedKingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromRealToProgrammingCoordinates();
     
        if((kx + 1 <= 7) && !chessBoard.Board[kx + 1, ky].ApieceOccupySquare && !KingIsChecked(chessBoard, chessBoard.Board[kx + 1, ky].ASquare, turn))
            return false;
        if((kx - 1 >= 0) && !chessBoard.Board[kx - 1, ky].ApieceOccupySquare && !KingIsChecked(chessBoard, chessBoard.Board[kx - 1, ky].ASquare, turn))
            return false;
        if((ky + 1 <= 7) && !chessBoard.Board[kx, ky + 1].ApieceOccupySquare && !KingIsChecked(chessBoard, chessBoard.Board[kx, ky + 1].ASquare, turn))
            return false;
        if((ky - 1 >= 0) && !chessBoard.Board[kx, ky - 1].ApieceOccupySquare && !KingIsChecked(chessBoard, chessBoard.Board[kx, ky - 1].ASquare, turn))
            return false;
        if((kx + 1 <= 7) && (ky + 1 <= 7) && !chessBoard.Board[kx + 1, ky + 1].ApieceOccupySquare && !KingIsChecked(chessBoard, chessBoard.Board[kx + 1, ky + 1].ASquare, turn))
            return false;
        if((kx - 1 >= 0) && (ky - 1 >= 0) && !chessBoard.Board[kx - 1, ky - 1].ApieceOccupySquare && !KingIsChecked(chessBoard, chessBoard.Board[kx - 1, ky - 1].ASquare, turn))
            return false;

        return true;
    }
    private bool AttackingPieceCannotBeCaptured(ChessBoard chessBoard, Square attackingPiecePosition, WhoseTurn turn)
    {
        String apos = attackingPiecePosition.Letter + attackingPiecePosition.Number.ToString();
        (int ax, int ay) = apos.FromRealToProgrammingCoordinates();
        for (int i = 0; i < chessBoard.Board.GetLength(0); i++)
        {
            for (int j = 0; j < chessBoard.Board.GetLength(1); j++)
            {
                if (chessBoard.Board[i, j].Apiece == null)
                   continue;
                if (ax == i && ay == j)
                    continue;
                PieceName pieceName = chessBoard.Board[i, j].Apiece!.Name;
                if (pieceName.CanPerfomeThisMove(chessBoard.Board[i, j].ASquare, attackingPiecePosition, turn))
                    if (pieceName.ThereIsNoObstacle(chessBoard.Board[i, j].ASquare, attackingPiecePosition, chessBoard, turn))
                        return false;
            }
        }
        return true;
    }
    private bool BetweenAttackingPieceAndKingNopieceCanBlock(ChessBoard chessBoard, Square kingPosition, Square attackingPiecePosition, WhoseTurn turn)
    {
        String kpos = kingPosition.Letter + kingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromRealToProgrammingCoordinates();
        String apos = attackingPiecePosition.Letter + attackingPiecePosition.Number.ToString();
        (int ax, int ay) = apos.FromRealToProgrammingCoordinates();

        var board = chessBoard.Board;
        PieceName pieceName = board[ax, ay].Apiece!.Name;

        if (pieceName == PieceName.KNIGHT)
            return true;
        else if (pieceName == PieceName.KING)
            return true;
        else if (pieceName == PieceName.PAWN)
        {
            if ((ay + 1 <= 7 && chessBoard.Board[ax + 1, ay + 1].Apiece?.Name == PieceName.KING)
                ||
                (ay - 1 >= 0 && chessBoard.Board[ax + 1, ay - 1].Apiece?.Name == PieceName.KING))
                return true;
        }
        else if(pieceName == PieceName.ROOK || pieceName == PieceName.QUEEN)
        {
            bool goStraightX = ax - kx > 0;
            bool goStraightY = ay - ky > 0;
            bool xEqual = kx - ax == 0;
            bool yEqual = ky - ay == 0;
            bool notGoStraightX = ax - kx < 0;
            bool notGoStraightY = ay - ky < 0;
            if (xEqual)
            {
                if (goStraightY)
                {
                    for (int y = ky + 1; y < ay; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            if (board[x, y].Apiece == null)
                                continue;
                            if (board[x, y].Apiece!.Name.ThereIsNoObstacle(board[x, y].ASquare, board[kx, y].ASquare, chessBoard, turn))
                                return true;
                        }
                    }
                }
                else if (notGoStraightY)
                {
                    for (int y = ky - 1; y > ay; y--)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            if (board[x, y].Apiece == null)
                                continue;
                            if (board[x, y].Apiece!.Name.ThereIsNoObstacle(board[x, y].ASquare, board[kx, y].ASquare, chessBoard, turn))
                                return true;
                        }
                    }
                }
            }
            else if (yEqual)
            {
                if (goStraightX)
                {
                    for (int x = kx + 1; x < ax; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if (board[x, y].Apiece == null)
                                continue;
                            if (board[x, y].Apiece!.Name.ThereIsNoObstacle(board[x, y].ASquare, board[x, ky].ASquare, chessBoard, turn))
                                return true;
                        }
                    }
                }
                else if (notGoStraightX)
                {
                    for (int x = kx - 1; x > ax; x--)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if (board[x, y].Apiece == null)
                                continue;
                            if (board[x, y].Apiece!.Name.ThereIsNoObstacle(board[x, y].ASquare, board[x, ky].ASquare, chessBoard, turn))
                                return true;
                        }
                    }
                }
            }
            else if (goStraightX && goStraightY)
            {
                for (int x = kx + 1; x < ax; x++)
                {
                    for (int y = ky + 1; y < ay; y++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (board[i, j].Apiece == null)
                                    continue;
                                if (board[i, j].Apiece!.Name.ThereIsNoObstacle(board[i, j].ASquare, board[x, y].ASquare, chessBoard, turn))
                                    return true;
                            }
                        }
                    }
                }
            }
            //else if (goStraightX && notGoStraightY)
            //{
            //    int x = xfromResult + 1;
            //    for (int y = yfromResult - 1; y > ytoResult; y--)
            //    {
            //        if (x < 8 && chessBoard.Board[x, y].ApieceOccupySquare)
            //            return false;
            //        x++;
            //    }
            //}
            //else if (notGoStraightX && goStraightY)
            //{
            //    int x = xfromResult - 1;
            //    for (int y = yfromResult + 1; y < ytoResult; y++)
            //    {
            //        if (x >= 0 && chessBoard.Board[x, y].ApieceOccupySquare)
            //            return false;
            //        x--;
            //    }
            //}
            //else if (notGoStraightX && notGoStraightY)
            //{
            //    int y = yfromResult - 1;
            //    for (int x = xfromResult - 1; x > xtoResult; x--)
            //    {
            //        if (y >= 0 && chessBoard.Board[x, y].ApieceOccupySquare)
            //            return false;
            //        y--;
            //    }
            //}
        }
        else if (pieceName == PieceName.BISHOP || pieceName == PieceName.QUEEN)
        {
            //if (kx - ky == ax - ay)
            //    return NoPieceCanBlockQueenOrBishop(chessBoard, turn, kx, ky, ax, ay, ay + 1 < );
            //else if(-(kx-ky) == ax - ay)
            //    return NoPieceCanBlockQueenOrBishop(chessBoard, turn, kx, ky, ax, ay, ay + 1 )


        }
        return true;
    }
    private bool QueenOrRookCannotBlock(ChessBoard chessBoard, WhoseTurn turn, int kx, int ky, int ax, BoardRelatedInfo[,] board, int j)
    {
        Square square = board[ax, j].ASquare;
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (board[x, y].ApieceOccupySquare && board[x, y].Apiece!.Color == board[kx, ky].Apiece!.Color)
                {
                    if
                    (board[x, y].Apiece!.Name.CanPerfomeThisMove(board[x, y].ASquare, square, turn)
                    &&
                    board[x, y].Apiece!.Name.ThereIsNoObstacle(board[x, y].ASquare, square, chessBoard, turn))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private bool QueenOrBishopCannotBlock(ChessBoard chessBoard, WhoseTurn turn, int kx, int ky, int ax, int ay)
    {
        var board = chessBoard.Board;
        Square square = board[ax, ay].ASquare;
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                if (board[x, y].ApieceOccupySquare && board[x, y].Apiece!.Color == board[kx, ky].Apiece!.Color)
                {
                    if 
                    (board[x, y].Apiece!.Name.CanPerfomeThisMove(board[x, y].ASquare, square, turn)
                    &&
                    board[x, y].Apiece!.Name.ThereIsNoObstacle(board[x, y].ASquare, square, chessBoard, turn))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    private bool NoPieceCanBlockQueenOrBishop(ChessBoard chessBoard, WhoseTurn turn, int kx, int ky, int ax, int ay, bool condition)
    {
        for (int j = condition ? ay + 1 : ay - 1; condition ? j < ky : j > ky; j = condition ? j++ : j--)
            return QueenOrBishopCannotBlock(chessBoard, turn, kx, ky, ax, j);

        return true;
    }
}
