using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.RulesRelated;

namespace ChessLibrary.PieceRelated;

public class King : Piece, IMove
{
    public King()
    {
        if (Color == PieceInfo.WHITE && SpecialEvents.whiteKingIsChecked == null)
            SpecialEvents.whiteKingIsChecked = WhiteKingIsChecked;
        if (Color == PieceInfo.BLACK && SpecialEvents.blackKingIsChecked == null)
            SpecialEvents.blackKingIsChecked = BlackKingIsChecked;
    }
    ~King() 
    {
        if (Color == PieceInfo.WHITE)
            SpecialEvents.whiteKingIsChecked -= WhiteKingIsChecked;
        if (Color == PieceInfo.BLACK)
            SpecialEvents.blackKingIsChecked -= BlackKingIsChecked;
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
    private bool WhiteKingIsChecked(ChessBoard chessBoard, Square whiteKingPosition, WhoseTurn turn)
    {
        String kingSquare = whiteKingPosition.Letter.ToString() + whiteKingPosition.Number.ToString();
        (int kx, int ky) = kingSquare.FromRealToProgrammingCoordinates();
        bool canPerformMove = false;
        bool thereAreNoObstacles = false;
        for (int x = 0; x < chessBoard.Board.GetLength(0); x++)
        {
            for (int y = 0; y < chessBoard.Board.GetLength(1); y++)
            {
                Piece? piece = chessBoard.Board[x, y]?.Apiece;

                if (chessBoard.Board[x, y].Apiece is null || piece!.Color == PieceInfo.WHITE || turn == WhoseTurn.White || piece.Name == PieceName.PAWN) 
                    continue;
                
                PieceName pieceName = piece.Name;
                Square rSquare = chessBoard.Board[x, y].ASquare;
                canPerformMove = pieceName.CanPerfomeThisMove(rSquare, chessBoard.Board[kx, ky].ASquare, WhoseTurn.Black);
                thereAreNoObstacles = pieceName.ThereIsNoObstacle(rSquare, chessBoard.Board[kx, ky].ASquare, chessBoard, WhoseTurn.Black);
                if (canPerformMove && thereAreNoObstacles)
                    return true;
            }
        }
        return false;
    }
    private bool BlackKingIsChecked(ChessBoard chessBoard, Square blackKingPosition, WhoseTurn turn)
    {
        return false;
    }
}
