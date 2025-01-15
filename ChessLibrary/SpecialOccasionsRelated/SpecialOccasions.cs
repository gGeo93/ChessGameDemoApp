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
                    board[3, pseudoCoorTo.yTo].ApieceOccupySquare = false;
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
                    board[4, pseudoCoorTo.yTo].ApieceOccupySquare = false;
                    return true;
                }
            }
        }
        return false;
    }
    public static bool KingCanCastleShort(this ChessBoard chessBoard, WhoseTurn whoPlays)
    {
        var whoseKingIsThreatendSquare =
            whoPlays == WhoseTurn.White ?
            new Square { Color = SquareColor.BLACK, Letter = 'e', Number = 1 }
            :
            new Square { Color = SquareColor.WHITE, Letter = 'e', Number = 8 };
        
        if (whoPlays == WhoseTurn.White &&
            (chessBoard.Board[7, 5].ApieceOccupySquare
            //|| chessBoard.Board[7, 6].Apiece?.Name == PieceName.KING
            || chessBoard.Board[7, 7].Apiece?.Name != PieceName.ROOK
            || chessBoard.Board[7, 7].Apiece?.Color != PieceInfo.WHITE
            ))
            return false;
        if (whoPlays == WhoseTurn.Black &&
           (chessBoard.Board[0, 5].ApieceOccupySquare
           //|| chessBoard.Board[0, 6].ApieceOccupySquare
           || chessBoard.Board[0, 7].Apiece?.Name != PieceName.ROOK
           || chessBoard.Board[0, 7].Apiece?.Color != PieceInfo.BLACK
           ))
            return false;
        if (SpecialEvents.kingIsChecked.Invoke(chessBoard, whoseKingIsThreatendSquare, whoPlays, true))
            return false;
        if (whoPlays == WhoseTurn.White && SpecialEvents.BlackKingHasMoved.Invoke())
            return false;
        if (whoPlays == WhoseTurn.Black && SpecialEvents.WhiteKingHasMoved.Invoke())
            return false;

        return true;
    }
    public static bool KingCanCastleLong(this ChessBoard chessBoard, WhoseTurn whoPlays)
    {
        var whoseKingIsThreatendSquare =
            whoPlays == WhoseTurn.White ?
            new Square { Color = SquareColor.BLACK, Letter = 'e', Number = 1 }
            :
            new Square { Color = SquareColor.WHITE, Letter = 'e', Number = 8 };

        if (whoPlays == WhoseTurn.White &&
            (chessBoard.Board[7, 3].ApieceOccupySquare
            //|| chessBoard.Board[7, 2].ApieceOccupySquare
            || chessBoard.Board[7, 1].ApieceOccupySquare
            || chessBoard.Board[7, 0].Apiece?.Name != PieceName.ROOK
            || chessBoard.Board[7, 0].Apiece?.Color != PieceInfo.WHITE
            ))
            return false;
        if (whoPlays == WhoseTurn.Black &&
           (chessBoard.Board[0, 3].ApieceOccupySquare
           //|| chessBoard.Board[0, 2].ApieceOccupySquare
           || chessBoard.Board[0, 1].ApieceOccupySquare
           || chessBoard.Board[0, 7].Apiece?.Name != PieceName.ROOK
           || chessBoard.Board[0, 7].Apiece?.Color != PieceInfo.BLACK
           ))
            return false;
        if (SpecialEvents.kingIsChecked.Invoke(chessBoard, whoseKingIsThreatendSquare, whoPlays, true))
            return false;
        if (whoPlays == WhoseTurn.White && SpecialEvents.BlackKingHasMoved.Invoke())
            return false;
        if (whoPlays == WhoseTurn.Black && SpecialEvents.WhiteKingHasMoved.Invoke())
            return false;

        return true;
    }
    public static bool[] RooksCheck(this ChessBoard chessBoard,bool[] rooksMovingStates)
    {
        if (chessBoard.Board[0, 0].Apiece?.Name != PieceName.ROOK)
            rooksMovingStates[0] = false;
        if (chessBoard.Board[0, 7].Apiece?.Name != PieceName.ROOK)
            rooksMovingStates[1] = false;
        if (chessBoard.Board[7, 0].Apiece?.Name != PieceName.ROOK)
            rooksMovingStates[2] = false;
        if (chessBoard.Board[7, 7].Apiece?.Name != PieceName.ROOK)
            rooksMovingStates[3] = false;
        return rooksMovingStates;
    }
}
