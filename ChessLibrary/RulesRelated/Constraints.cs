using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;
using System.Reflection.Metadata.Ecma335;

namespace ChessLibrary.RulesRelated;

public static class Constraints
{
    public static bool CanPerfomeThisMove(this PieceName pieceName, Square from, Square to, WhoseTurn whoPlays)
    {
        switch (pieceName)
        {
            case PieceName.PAWN: return new Pawn(whoPlays).Movement(from, to);
            case PieceName.KNIGHT: return new Knight().Movement(from, to);
            case PieceName.BISHOP: return new Bishop().Movement(from, to);
            case PieceName.ROOK: return new Rook().Movement(from, to);
            case PieceName.QUEEN: return new Queen().Movement(from, to);
            case PieceName.KING: return new King().Movement(from, to);
            default: return false;
        }
    }
    public static bool ThereIsNoObstacle(this PieceName pieceName, Square from, Square to, ChessBoard chessBoard, WhoseTurn whoPlays)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        PieceInfo pieceColorInit = chessBoard.Board[pseudoCoorFrom.xFrom, pseudoCoorFrom.yFrom].Apiece.Color;
        PieceInfo? pieceColorFinal = chessBoard.Board[pseudoCoorTo.xTo, pseudoCoorTo.yTo].Apiece?.Color;

        if (pieceColorInit == pieceColorFinal)
            return false;

        if (pieceName == PieceName.KNIGHT)
            return true;

        if (pieceName == PieceName.PAWN)
        {
            if (whoPlays == WhoseTurn.White && pieceColorInit == PieceInfo.WHITE && pieceColorFinal == PieceInfo.BLACK)
            {
                if (pseudoCoorTo.xTo == pseudoCoorFrom.xFrom - 1 && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
                    return true;
            }
            else if (whoPlays == WhoseTurn.Black && pieceColorInit == PieceInfo.BLACK && pieceColorFinal == PieceInfo.WHITE)
            {
                if (pseudoCoorTo.xTo == pseudoCoorFrom.xFrom + 1 && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
                    return true;
            }
        }

        return chessBoard.HasNoObstacles(whoPlays, pseudoCoorFrom, pseudoCoorTo);
    }

    private static bool HasNoObstacles(this ChessBoard chessBoard, WhoseTurn whoPlays, (int xFrom, int yFrom) pseudoCoorFrom, (int xTo, int yTo) pseudoCoorTo)
    {
        bool goStraightX = pseudoCoorTo.xTo - pseudoCoorFrom.xFrom >= 0;
        bool goStraightY = pseudoCoorTo.yTo - pseudoCoorFrom.yFrom >= 0;
        int xtoResult = goStraightX ? pseudoCoorTo.xTo : pseudoCoorTo.xTo;
        int ytoResult = goStraightY ? pseudoCoorTo.yTo : pseudoCoorTo.yTo;
        int xfromResult = goStraightX ? pseudoCoorFrom.xFrom : pseudoCoorFrom.xFrom;
        int yfromResult = goStraightY ? pseudoCoorFrom.yFrom : pseudoCoorFrom.yFrom;
        if (pseudoCoorTo.xTo == pseudoCoorFrom.xFrom)
        {
            if (goStraightY)
            {
                for (int y = pseudoCoorFrom.yFrom + 1; y < pseudoCoorTo.yTo; y++)
                {
                    if (chessBoard.Board[pseudoCoorFrom.xFrom, y].ApieceOccupySqsuare)
                        return false;
                }
            }
            else if (!goStraightY)
            {
                for (int y = pseudoCoorFrom.yFrom - 1; y > pseudoCoorTo.yTo; y--)
                {
                    if (chessBoard.Board[pseudoCoorFrom.xFrom, y].ApieceOccupySqsuare)
                        return false;
                }
            }
        }
        else if (pseudoCoorTo.yTo == pseudoCoorFrom.yFrom)
        {
            if (goStraightX)
            {
                for (int x = pseudoCoorFrom.xFrom + 1; x < pseudoCoorTo.xTo; x++)
                {
                    if (chessBoard.Board[x, pseudoCoorFrom.yFrom].ApieceOccupySqsuare)
                        return false;
                }
            }
            else if (!goStraightX)
            {
                for (int x = pseudoCoorFrom.xFrom - 1; x > pseudoCoorTo.xTo; x--)
                {
                    if (chessBoard.Board[x, pseudoCoorFrom.yFrom].ApieceOccupySqsuare)
                        return false;
                }
            }
        }
        else if (goStraightX && goStraightY)
        {
            int y = yfromResult + 1;
            for (int x = xfromResult + 1; x < xtoResult; x++)
            {
                if (chessBoard.Board[x, y].ApieceOccupySqsuare)
                    return false;
                y++;
            }
        }
        else if (goStraightX && !goStraightY)
        {
            int x = xfromResult + 1;
            for (int y = yfromResult - 1; y > ytoResult; y--)
            {
                if (chessBoard.Board[x, y].ApieceOccupySqsuare)
                    return false;
                x++;
            }
        }
        else if (!goStraightX && goStraightY)
        {
            int x = xfromResult - 1;
            for (int y = yfromResult + 1; y < ytoResult; y++)
            {
                if (chessBoard.Board[x, y].ApieceOccupySqsuare)
                    return false;
                x--;
            }
        }
        else if (!goStraightX && !goStraightY)
        {
            int y = yfromResult - 1;
            for (int x = xfromResult - 1; x > xtoResult; x--)
            {
                if (chessBoard.Board[x, y].ApieceOccupySqsuare)
                    return false;
                y--;
            }
        }

        return true;
    }
}
