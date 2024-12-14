using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;

namespace ChessLibrary.PieceRelated;

public class Pawn : Piece, IMove, IPawn
{
    public bool IsOnInitialSquare { get; set; }
    public WhoseTurn WhoPlays { get; set; }

    public Pawn(WhoseTurn whoseTurn)
    {
        this.IsOnInitialSquare = true;
        this.WhoPlays = whoseTurn;
    }

    public bool Movement(Square from, Square to)
    {
        (int xFrom, int yFrom) pseudoCoorFrom;
        (int xTo, int yTo) pseudoCoorTo;
        from.InternalCoordinatesOperation(to, out pseudoCoorFrom, out pseudoCoorTo);

        if (WhoPlays == WhoseTurn.White)
        {
            if (pseudoCoorTo.xTo >= pseudoCoorFrom.xFrom)
                return false;
        }
        else
        {
            if (pseudoCoorTo.xTo <= pseudoCoorFrom.xFrom)
                return false;
        }
        
        int xDistance = Math.Abs(pseudoCoorFrom.xFrom - pseudoCoorTo.xTo);
        
        if (xDistance > 2)
            return false;
        if (xDistance == 2 && IsOnInitialSquare)
        {
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
