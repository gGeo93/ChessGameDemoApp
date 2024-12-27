using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.RulesRelated;

namespace ChessLibrary.PieceRelated;

public class King : Piece, IMove
{
    private bool _doubleCheck = false;
    public King()
    {
        if (SpecialEvents.whiteKingIsChecked != null && SpecialEvents.whiteKingIsMate != null)
            return;
        
        if (Color == PieceInfo.WHITE && SpecialEvents.whiteKingIsChecked == null)
            SpecialEvents.whiteKingIsChecked = WhiteKingIsChecked;

        if (Color == PieceInfo.WHITE && SpecialEvents.whiteKingIsMate == null)
            SpecialEvents.whiteKingIsMate = WhiteKingIsMate;
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
        int howManyChecks = 0;
        for (int x = 0; x < chessBoard.Board.GetLength(0); x++)
        {
            for (int y = 0; y < chessBoard.Board.GetLength(1); y++)
            {
                Piece? piece = chessBoard.Board[x, y]?.Apiece;

                if (piece is null || piece!.Color == PieceInfo.WHITE) 
                    continue;
                
                PieceName pieceName = piece.Name;
                Square pSquare = chessBoard.Board[x, y].ASquare;
                canPerformMove = pieceName.CanPerfomeThisMove(pSquare, chessBoard.Board[kx, ky].ASquare, turn);
                thereAreNoObstacles = pieceName.ThereIsNoObstacle(pSquare, chessBoard.Board[kx, ky].ASquare, chessBoard, turn);
                if (canPerformMove && thereAreNoObstacles)
                    howManyChecks++;
            }
        }
        switch (howManyChecks)
        {
            case 0: return false;
            case 1: return true;
            case 2: _doubleCheck = true; return true;
            default: return false;
        }
    }
    private bool WhiteKingIsMate(ChessBoard chessBoard, Square whiteKingPosition, Square attackingPiecePosition, bool isChecked)
    {
        if (!isChecked)
            return false;
        
        bool kingCannotMove = KingCannotMove(chessBoard, whiteKingPosition);
        
        if (kingCannotMove && _doubleCheck)
            return true;
        if (!kingCannotMove)
            return false;
        if(!AttackingPieceCannotBeCaptured())
            return false;
        if(!BetweenAttackingPieceAndKingNopieceCanBlock())
            return false;
        
        return true;
    }
    private bool KingCannotMove(ChessBoard chessBoard, Square whiteKingPosition)
    {
        String kpos = whiteKingPosition.Letter + whiteKingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromRealToProgrammingCoordinates();
     
        if((kx + 1 <= 7) && !chessBoard.Board[kx + 1, ky].ApieceOccupySquare && !WhiteKingIsChecked(chessBoard, chessBoard.Board[kx + 1, ky].ASquare, WhoseTurn.White))
            return false;
        if((kx - 1 >= 0) && !chessBoard.Board[kx - 1, ky].ApieceOccupySquare && !WhiteKingIsChecked(chessBoard, chessBoard.Board[kx - 1, ky].ASquare, WhoseTurn.White))
            return false;
        if((ky + 1 <= 7) && !chessBoard.Board[kx, ky + 1].ApieceOccupySquare && !WhiteKingIsChecked(chessBoard, chessBoard.Board[kx, ky + 1].ASquare, WhoseTurn.White))
            return false;
        if((ky - 1 >= 0) && !chessBoard.Board[kx, ky - 1].ApieceOccupySquare && !WhiteKingIsChecked(chessBoard, chessBoard.Board[kx, ky - 1].ASquare, WhoseTurn.White))
            return false;
        if((kx + 1 <= 7) && (ky + 1 <= 7) && !chessBoard.Board[kx + 1, ky + 1].ApieceOccupySquare && !WhiteKingIsChecked(chessBoard, chessBoard.Board[kx + 1, ky + 1].ASquare, WhoseTurn.White))
            return false;
        if((kx - 1 >= 0) && (ky - 1 >= 0) && !chessBoard.Board[kx - 1, ky - 1].ApieceOccupySquare && !WhiteKingIsChecked(chessBoard, chessBoard.Board[kx - 1, ky - 1].ASquare, WhoseTurn.White))
            return false;

        return true;
    }
    private bool AttackingPieceCannotBeCaptured()
    {
        return true;
    }
    private bool BetweenAttackingPieceAndKingNopieceCanBlock()
    {
        return true;
    }
    private bool BlackKingIsChecked(ChessBoard chessBoard, Square blackKingPosition, WhoseTurn turn)
    {
        return false;
    }
}
