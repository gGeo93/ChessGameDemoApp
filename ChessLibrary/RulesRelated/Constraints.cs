using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.RulesRelated;

public static class Constraints
{
    public static bool CanPerfomeThisMove(this PieceName pieceName, Square from, Square to, WhoseTurn whoPlays, PieceInfo? pieceInfo)
    {
        switch (pieceName)
        {
            case PieceName.PAWN: return PawnInfo(from, to, whoPlays, pieceInfo);
            case PieceName.KNIGHT: return new Knight().Movement(from, to);
            case PieceName.BISHOP: return new Bishop().Movement(from, to);
            case PieceName.ROOK: return new Rook().Movement(from, to);
            case PieceName.QUEEN: return new Queen().Movement(from, to);
            case PieceName.KING: return new King().Movement(from, to);
            default: return false;
        }
    }
    private static bool PawnInfo(Square from, Square to, WhoseTurn whoPlays, PieceInfo? pieceInfo)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);
        
        Pawn pawn = new Pawn(pieceInfo);
        if(pseudoCoorFrom.xFrom == 6 && pseudoCoorTo.xTo == 4 && whoPlays == WhoseTurn.White)
            pawn.IsOnInitialSquare = true;
        if (pseudoCoorFrom.xFrom == 1 && pseudoCoorTo.xTo == 3 && whoPlays == WhoseTurn.Black)
            pawn.IsOnInitialSquare = true;
        return pawn.Movement(from, to);
    }
    public static bool ThereIsNoObstacle(this PieceName pieceName, Square from, Square to, BoardRelatedInfo[,] chessBoard, WhoseTurn whoPlays)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        PieceInfo? pieceColorInit = chessBoard[pseudoCoorFrom.xFrom, pseudoCoorFrom.yFrom].Apiece?.Color;
        PieceInfo? pieceColorFinal = chessBoard[pseudoCoorTo.xTo, pseudoCoorTo.yTo].Apiece?.Color;

        if (pieceColorInit == pieceColorFinal)
            return false;

        if (pieceName == PieceName.KNIGHT)
            return true;

        if (pieceName == PieceName.PAWN)
        {
            if (pieceColorFinal != null && pieceColorInit != pieceColorFinal && (pseudoCoorFrom.yFrom == pseudoCoorTo.yTo))
                return false;
            if (whoPlays == WhoseTurn.White && pieceColorInit == PieceInfo.WHITE && pieceColorFinal == PieceInfo.BLACK)
            {
                if (pseudoCoorTo.xTo == pseudoCoorFrom.xFrom + 1 && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
                    return true;
            }
            else if (whoPlays == WhoseTurn.Black && pieceColorInit == PieceInfo.BLACK && pieceColorFinal == PieceInfo.WHITE)
            {
                if (pseudoCoorTo.xTo == pseudoCoorFrom.xFrom + 1 && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
                    return true;
            }
            else if (pieceColorFinal == null)
            {
                if ((pseudoCoorTo.xTo - pseudoCoorFrom.xFrom == 1 || pseudoCoorTo.xTo - pseudoCoorFrom.xFrom == -1) && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
                    return false;
            }
        }

        return chessBoard.HasNoObstacles(whoPlays, pseudoCoorFrom, pseudoCoorTo);
    }
    private static bool HasNoObstacles(this BoardRelatedInfo[,] chessBoard, WhoseTurn whoPlays, (int xFrom, int yFrom) pseudoCoorFrom, (int xTo, int yTo) pseudoCoorTo)
    {
        bool goStraightX = pseudoCoorTo.xTo - pseudoCoorFrom.xFrom > 0;
        bool goStraightY = pseudoCoorTo.yTo - pseudoCoorFrom.yFrom > 0;
        bool xEqual = pseudoCoorTo.xTo - pseudoCoorFrom.xFrom == 0;
        bool yEqual = pseudoCoorTo.yTo - pseudoCoorFrom.yFrom == 0;
        bool notGoStraightX = pseudoCoorTo.xTo - pseudoCoorFrom.xFrom < 0;
        bool notGoStraightY = pseudoCoorTo.yTo - pseudoCoorFrom.yFrom < 0;

        int xtoResult = pseudoCoorTo.xTo;
        int ytoResult = pseudoCoorTo.yTo;
        int xfromResult = pseudoCoorFrom.xFrom;
        int yfromResult = pseudoCoorFrom.yFrom;
        if (xEqual)
        {
            if (goStraightY)
            {
                for (int y = pseudoCoorFrom.yFrom + 1; y < pseudoCoorTo.yTo; y++)
                {
                    if (chessBoard[pseudoCoorFrom.xFrom, y].ApieceOccupySquare)
                        return false;
                }
            }
            else if (notGoStraightY)
            {
                for (int y = pseudoCoorFrom.yFrom - 1; y > pseudoCoorTo.yTo; y--)
                {
                    if (chessBoard[pseudoCoorFrom.xFrom, y].ApieceOccupySquare)
                        return false;
                }
            }
        }
        else if (yEqual)
        {
            if (goStraightX)
            {
                for (int x = pseudoCoorFrom.xFrom + 1; x < pseudoCoorTo.xTo; x++)
                {
                    if (chessBoard[x, pseudoCoorFrom.yFrom].ApieceOccupySquare)
                        return false;
                }
            }
            else if (notGoStraightX)
            {
                for (int x = pseudoCoorFrom.xFrom - 1; x > pseudoCoorTo.xTo; x--)
                {
                    if (chessBoard[x, pseudoCoorFrom.yFrom].ApieceOccupySquare)
                        return false;
                }
            }
        }
        else if (goStraightX && goStraightY)
        {
            int y = yfromResult + 1;
            for (int x = xfromResult + 1; x < xtoResult; x++)
            {
                 if (y < 8 && chessBoard[x, y].ApieceOccupySquare)
                    return false;
                y++;
            }
        }
        else if (goStraightX && notGoStraightY)
        {
            int x = xfromResult + 1;
            for (int y = yfromResult - 1; y > ytoResult; y--)
            {
                if (x < 8 && chessBoard[x, y].ApieceOccupySquare)
                    return false;
                x++;
            }
        }
        else if (notGoStraightX && goStraightY)
        {
            int x = xfromResult - 1;
            for (int y = yfromResult + 1; y < ytoResult; y++)
            {
                if (x >= 0 && chessBoard[x, y].ApieceOccupySquare)
                    return false;
                x--;
            }
        }
        else if (notGoStraightX && notGoStraightY)
        {
            int y = yfromResult - 1;
            for (int x = xfromResult - 1; x > xtoResult; x--)
            {
                if (y >= 0 && chessBoard[x, y].ApieceOccupySquare)
                    return false;
                y--;
            }
        }
        return true;
    }
}
