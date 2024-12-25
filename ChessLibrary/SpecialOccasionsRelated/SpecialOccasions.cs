using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.SpecialOccasionsRelated;

public static class SpecialOccasions
{
    public static bool CanTakeEnPassant(this BoardRelatedInfo[,] board, WhoseTurn whoPlays, Square from, Square to)
    {
        from.InternalCoordinatesOperation(to, out (int xFrom, int yFrom) pseudoCoorFrom, out (int xTo, int yTo) pseudoCoorTo);
        
        (int? xpass, int? ypass)? specialCoor = SpecialEvents.pawnHasJustMovedTwice?.Invoke();
        
        if (specialCoor?.xpass == null || 
            specialCoor?.ypass == null || 
            specialCoor?.ypass != pseudoCoorTo.yTo || 
            Math.Abs((int)specialCoor?.ypass - pseudoCoorFrom.yFrom) != 1 ||
            Math.Abs((int)specialCoor?.xpass - pseudoCoorFrom.xFrom) != 0)
            return false;
        
        PieceName? piece = board[pseudoCoorFrom.xFrom, pseudoCoorFrom.yFrom].Apiece?.Name;
        if (SpecialEvents.pawnHasJustMovedTwice != null && whoPlays == WhoseTurn.White)
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
        else if (SpecialEvents.pawnHasJustMovedTwice != null && whoPlays == WhoseTurn.Black)
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
