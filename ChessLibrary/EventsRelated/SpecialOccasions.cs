using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.EventsRelated;

public static class SpecialOccasions
{
    public static bool CanTakeEnPassant(this BoardRelatedInfo[,] board, WhoseTurn whoPlays, Square from, Square to, bool pawnHasJustMoveTwice)
    {
        if(pawnHasJustMoveTwice == false)
            return false;
        
        from.InternalCoordinatesOperation(to, out (int xFrom, int yFrom) pseudoCoorFrom, out (int xTo, int yTo) pseudoCoorTo);
        PieceName? piece = board[pseudoCoorFrom.xFrom, pseudoCoorFrom.yFrom].Apiece?.Name;
        if (pawnHasJustMoveTwice && whoPlays == WhoseTurn.White)
        {
            if (pseudoCoorFrom.xFrom == 3 && pseudoCoorTo.xTo == 2 && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
            {
                if (piece == PieceName.PAWN && board[3, pseudoCoorTo.yTo].Apiece?.Name == PieceName.PAWN && board[3, pseudoCoorTo.yTo].Apiece?.Color == PieceInfo.BLACK)
                {
                    board[3, pseudoCoorTo.yTo].Apiece = null;
                    board[3, pseudoCoorTo.yTo].ApieceOccupySqsuare = false;
                    return true;
                }
            }
        }
        else if (pawnHasJustMoveTwice && whoPlays == WhoseTurn.Black)
        {
            if (pseudoCoorFrom.xFrom == 4 && pseudoCoorTo.xTo == 5 && Math.Abs(pseudoCoorTo.yTo - pseudoCoorFrom.yFrom) == 1)
            {
                if (piece == PieceName.PAWN && board[4, pseudoCoorTo.yTo].Apiece?.Name == PieceName.PAWN && board[4, pseudoCoorTo.yTo].Apiece?.Color == PieceInfo.WHITE)
                {
                    board[4, pseudoCoorTo.yTo].Apiece = null;
                    board[4, pseudoCoorTo.yTo].ApieceOccupySqsuare = false;
                    return true;
                }
            }
        }
        return false;
    }
}
