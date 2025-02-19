using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.HellpingMethods;

namespace ChessLibrary.PieceRelated;

public class Pawn : Piece, IMove
{
    public bool PawnIntentsToMoveTwice { get; set; }
    public bool IsOnInitialSquare { get; set; }
    public PieceInfo? pieceinfo { get; set; }

    public Pawn(PieceInfo? pieceinfo)
    {
        this.pieceinfo = pieceinfo;
    }

    public bool Movement(Square from, Square to)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);
        
        if (pieceinfo == PieceInfo.WHITE)
        {
            if (pseudoCoorTo.xTo >= pseudoCoorFrom.xFrom)
                return false;
        }
        else if(pieceinfo == PieceInfo.BLACK)
        {
            if (pseudoCoorTo.xTo <= pseudoCoorFrom.xFrom)
                return false;
        }

        int xDistance = Math.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo);
        int yDistance = Math.Abs(pseudoCoorFrom.yFrom - pseudoCoorTo.yTo);
        
        if(yDistance > 1)
            return false;
        if (xDistance > 2)
            return false;
        if (xDistance == 2 && IsOnInitialSquare)
        {
            SpecialEvents.pawnHasJustMovedTwice = () => (pseudoCoorTo.xTo, pseudoCoorTo.yTo); 
            this.IsOnInitialSquare = false;
            return true;
        }
        if (xDistance == 1)
        {
            this.IsOnInitialSquare = false;
            return true;
        }

        return false;
    }

    public static bool PawnThreatensKing(PieceInfo pieceInfo, int kx, int ky, Piece _piece, BoardRelatedInfo[,] chessBoard, Square pawnTo)
    {
        (int px, int py) pawnToCoordinates = (pawnTo.Letter + pawnTo.Number.ToString()).FromVisualToProgrammingCoordinates();
        chessBoard[kx, ky].Apiece = _piece;
        if (pieceInfo == PieceInfo.WHITE)
        {
            if (pawnToCoordinates.px - 1 >= 0 && pawnToCoordinates.py - 1 >= 0 && chessBoard[pawnToCoordinates.px - 1, pawnToCoordinates.py - 1].Apiece?.Name == PieceName.KING)
            {
                chessBoard[kx, ky].Apiece = null;
                return true;
            }    
            if (pawnToCoordinates.px - 1 >= 0 && pawnToCoordinates.py + 1 <= 7 && chessBoard[pawnToCoordinates.px - 1, pawnToCoordinates.py + 1].Apiece?.Name == PieceName.KING)
            {
                chessBoard[kx, ky].Apiece = null;
                return true;
            }
            chessBoard[kx, ky].Apiece = null;
            return false;
        }
        else if (pieceInfo == PieceInfo.BLACK)
        {
            if (pawnToCoordinates.px + 1 <= 7 && pawnToCoordinates.py - 1 >= 0 && chessBoard[pawnToCoordinates.px + 1, pawnToCoordinates.py - 1].Apiece?.Name == PieceName.KING)
            {
                chessBoard[kx, ky].Apiece = null;
                return true;
            }
            if (pawnToCoordinates.px + 1 <= 7 && pawnToCoordinates.py + 1 <= 7 && chessBoard[pawnToCoordinates.px + 1, pawnToCoordinates.py + 1].Apiece?.Name == PieceName.KING)
            {
                chessBoard[kx, ky].Apiece = null;
                return true;
            }
            chessBoard[kx, ky].Apiece = null;
            return false;
        }
        else
        {
            chessBoard[kx, ky].Apiece = null;
            return false;
        }
    }
}
