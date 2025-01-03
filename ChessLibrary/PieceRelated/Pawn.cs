using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using System.Reflection.Metadata.Ecma335;

namespace ChessLibrary.PieceRelated;

public class Pawn : Piece, IMove, IPawn
{
    public bool PawnIntentsToMoveTwice { get; set; }
    public bool IsOnInitialSquare { get; set; }
    public PieceInfo? pieceinfo { get; set; }

    public Pawn(PieceInfo? pieceinfo)
    {
        this.IsOnInitialSquare = true;
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
}
