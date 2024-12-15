using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.RulesRelated;

public static class Constraints
{
    public static bool CanPerfomeThisMove(this PieceName pieceName,Square from, Square to, WhoseTurn whoPlays)
    {
        switch (pieceName)
        {
            case PieceName.PAWN: return new Pawn(whoPlays).Movement(from, to);
            case PieceName.KNIGHT: return new Knight().Movement(from, to);
            case PieceName.BISHOP: return new Bishop().Movement(from, to);
            case PieceName.ROOK: return new Rook().Movement(from, to);
            case PieceName.QUEEN: return new Queen().Movement(from, to);
            case PieceName.KING: return new King().Movement(from, to);  
            default: return true;
        }
    }
    public static bool ThereIsNoObstacle(this PieceName pieceName, Square from, Square to, ChessBoard chessBoard, WhoseTurn whoPlays)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        PieceInfo? pieceColor = chessBoard.Board[pseudoCoorTo.xTo, pseudoCoorTo.yTo].Apiece?.Color;

        if ((pieceColor == PieceInfo.WHITE && whoPlays == WhoseTurn.White) || (pieceColor == PieceInfo.BLACK && whoPlays == WhoseTurn.Black)) 
            return false;
        
        if (pieceColor is null)
            return true;

        if (pieceName == PieceName.KNIGHT)
            return true;
        

        return true;
    }
}
