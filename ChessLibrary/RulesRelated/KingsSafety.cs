using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.RulesRelated;

public class KingsSafety
{
    public KingsSafety()
    {
        if (SpecialEvents.kingIsChecked != null && SpecialEvents.kingIsMate != null)
            return;
        
        if(SpecialEvents.kingIsChecked == null)
            SpecialEvents.kingIsChecked += KingIsChecked;

        if (SpecialEvents.kingIsMate == null)
            SpecialEvents.kingIsMate += KingIsMate;
    }
    ~KingsSafety()
    {
        if (SpecialEvents.kingIsChecked.GetInvocationList().Length > 1)
            SpecialEvents.kingIsChecked -= KingIsChecked;
        if (SpecialEvents.kingIsMate.GetInvocationList().Length > 1)
            SpecialEvents.kingIsMate -= KingIsMate!;
    }
    private bool _doubleCheck = false;
    private bool KingIsChecked(ChessBoard chessBoard, Square kingPosition, WhoseTurn turn)
    {
        String kingSquare = kingPosition.Letter.ToString() + kingPosition.Number.ToString();
        (int kx, int ky) = kingSquare.FromRealToProgrammingCoordinates();
        
        bool canPerformMove = false;
        bool thereAreNoObstacles = false;
        int howManyChecks = 0;
        var colorToPass = turn == WhoseTurn.White ? PieceInfo.BLACK : PieceInfo.WHITE;
        for (int x = 0; x < chessBoard.Board.GetLength(0); x++)
        {
            for (int y = 0; y < chessBoard.Board.GetLength(1); y++)
            {
                Piece? piece = chessBoard.Board[x, y]?.Apiece;

                if (piece is null || (piece.Color == colorToPass))
                    continue;

                PieceName pieceName = piece!.Name;
                Square pSquare = chessBoard.Board[x, y].ASquare;
                canPerformMove = pieceName.CanPerfomeThisMove(pSquare, chessBoard.Board[kx, ky].ASquare, turn, chessBoard.Board[kx, ky].Apiece?.Color);
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
    private bool KingIsMate(ChessBoard chessBoard, Square threatedKingPosition, Square attackingPiecePosition, bool isChecked, WhoseTurn turn)
    {
        if (!isChecked)
            return false;
        bool kingCannotMove = IllegalKingMovement(chessBoard, threatedKingPosition, turn);
        if (kingCannotMove && _doubleCheck)
            return true;
        if (!kingCannotMove)
            return false;
        if (!AttackingPieceCannotBeCaptured(chessBoard, attackingPiecePosition, turn))
            return false;
        if (!BetweenAttackingPieceAndKingNopieceCanBlock(chessBoard, threatedKingPosition, attackingPiecePosition, turn))
            return false;

        return true;
    }
    private bool IllegalKingMovement(ChessBoard chessBoard, Square kingPosition, WhoseTurn turn)
    {
        PieceInfo sameColorAsKing = turn == WhoseTurn.White ? PieceInfo.BLACK : PieceInfo.WHITE;  
        bool[,] kingBoard = new bool[3, 3];
        
        String kpos = kingPosition.Letter + kingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromRealToProgrammingCoordinates();
        var board = chessBoard.Board;
        if(KingIsChecked(chessBoard, board[kx, ky].ASquare, turn))
            kingBoard[1,1] = false;
        if (kx + 1 <= 7)
        {
            if ((board[kx + 1, ky].ApieceOccupySquare && board[kx + 1, ky].Apiece?.Color == sameColorAsKing) || (KingIsChecked(chessBoard, board[kx + 1, ky].ASquare, turn)))
            {
                kingBoard[2, 1] = false;
            }
            else
                return true;
        }
       
        if (kx - 1 >= 0)
        {
            if ((board[kx - 1, ky].ApieceOccupySquare && board[kx - 1, ky].Apiece?.Color == sameColorAsKing))
                kingBoard[0, 1] = false;
            else if (KingIsChecked(chessBoard, board[kx - 1, ky].ASquare, turn))
                return true;
        }

        if (ky + 1 <= 7)
        {
            if ((board[kx, ky + 1].ApieceOccupySquare && board[kx, ky + 1].Apiece?.Color == sameColorAsKing))
                kingBoard[1, 2] = false;
            else if(KingIsChecked(chessBoard, board[kx, ky + 1].ASquare, turn))
                return true;
        }
        if (ky - 1 >= 0)
        {
            if ((board[kx, ky - 1].ApieceOccupySquare && board[kx, ky - 1].Apiece?.Color == sameColorAsKing))
                kingBoard[1, 0] = false;
            else if(KingIsChecked(chessBoard, board[kx, ky - 1].ASquare, turn))
                return true;
        }
        if (kx + 1 <= 7 && ky + 1 <= 7)
        {
            if ((board[kx + 1, ky + 1].ApieceOccupySquare && board[kx + 1, ky + 1].Apiece?.Color == sameColorAsKing))
                kingBoard[2, 2] = false;
            else if(KingIsChecked(chessBoard, board[kx + 1, ky + 1].ASquare, turn))
                return true;
        }
        if (kx + 1 <= 7 && ky - 1 >= 0)
        {
            if ((board[kx + 1, ky - 1].ApieceOccupySquare && board[kx + 1, ky - 1].Apiece?.Color == sameColorAsKing))
                kingBoard[2, 0] = false;
            else if(KingIsChecked(chessBoard, board[kx + 1, ky - 1].ASquare, turn))
                return true;
        }
        if (kx - 1 >= 0 && ky + 1 <= 7)
        {
            if (board[kx - 1, ky + 1].ApieceOccupySquare && board[kx - 1, ky + 1].Apiece?.Color == sameColorAsKing)
                kingBoard[0, 2] = false;
            else if(KingIsChecked(chessBoard, board[kx - 1, ky + 1].ASquare, turn))
                return true;
        }
        if (kx - 1 >= 0 && ky - 1 >= 0)
        {
            if ((board[kx - 1, ky - 1].ApieceOccupySquare && board[kx - 1, ky - 1].Apiece?.Color == sameColorAsKing))
                kingBoard[0, 0] = false;
            else if(KingIsChecked(chessBoard, board[kx - 1, ky - 1].ASquare, turn))
                return true;
        }
        return false;
    }
    
    private bool AttackingPieceCannotBeCaptured(ChessBoard chessBoard, Square attackingPiecePosition, WhoseTurn turn)
    {
        String apos = attackingPiecePosition.Letter + attackingPiecePosition.Number.ToString();
        (int ax, int ay) = apos.FromRealToProgrammingCoordinates();
        for (int i = 0; i < chessBoard.Board.GetLength(0); i++)
        {
            for (int j = 0; j < chessBoard.Board.GetLength(1); j++)
            {
                if (chessBoard.Board[i, j].Apiece == null
                    ||
                    //(turn == WhoseTurn.Black && chessBoard.Board[i, j].Apiece!.Color == PieceInfo.WHITE)
                    //||
                    //(turn == WhoseTurn.White && chessBoard.Board[i, j].Apiece!.Color == PieceInfo.BLACK)
                    //||
                    (chessBoard.Board[i, j].Apiece!.Name == PieceName.QUEEN))
                    continue;

                PieceName pieceName = chessBoard.Board[i, j].Apiece!.Name;

                if (pieceName.CanPerfomeThisMove(chessBoard.Board[i, j].ASquare, attackingPiecePosition, turn, chessBoard.Board[i, j].Apiece!.Color))
                    if (pieceName.ThereIsNoObstacle(chessBoard.Board[i, j].ASquare, attackingPiecePosition, chessBoard, turn))
                        return false;
            }
        }
        return true;
    }
    private bool BetweenAttackingPieceAndKingNopieceCanBlock(ChessBoard chessBoard, Square kingPosition, Square attackingPiecePosition, WhoseTurn turn)
    {
        String kpos = kingPosition.Letter + kingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromRealToProgrammingCoordinates();
        String apos = attackingPiecePosition.Letter + attackingPiecePosition.Number.ToString();
        (int ax, int ay) = apos.FromRealToProgrammingCoordinates();

        var board = chessBoard.Board;
        PieceName pieceName = board[ax, ay].Apiece!.Name;

        if (pieceName == PieceName.KNIGHT)
            return true;
        else if (pieceName == PieceName.KING)
            return true;
        else if (pieceName == PieceName.PAWN)
        {
            if ((ay + 1 <= 7 && chessBoard.Board[ax + 1, ay + 1].Apiece?.Name == PieceName.KING)
                ||
                (ay - 1 >= 0 && chessBoard.Board[ax + 1, ay - 1].Apiece?.Name == PieceName.KING))
                return true;
        }
        else if (pieceName == PieceName.ROOK || pieceName == PieceName.QUEEN || pieceName == PieceName.BISHOP)
        {
            PieceInfo pieceInfo = turn == WhoseTurn.White ? PieceInfo.WHITE : PieceInfo.BLACK;

            bool goStraightX = kx - ax > 0;
            bool goStraightY = ky - ay > 0;
            bool xEqual = kx - ax == 0;
            bool yEqual = ky - ay == 0;
            bool notGoStraightX = kx - ax < 0;
            bool notGoStraightY = ky - ay < 0;

            if (xEqual)
            {
                if (goStraightY)
                {
                    int x = 0;
                    for (int y = ay + 1; y < ky; y++)
                    {
                        if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                            return false;
                        x++;
                    }
                }
                else if (notGoStraightY)
                {
                    int x = 0;
                    for (int y = ay - 1; y > ky; y--)
                    {
                        if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                            return false;
                        x++;
                    }
                }
            }
            else if (yEqual)
            {
                if (goStraightX)
                {
                    int y = 0;
                    for (int x = ax + 1; x < kx; x++)
                    {
                        if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                            return false;
                        y++;
                    }
                }
                else if (notGoStraightX)
                {
                    int y = 0;
                    for (int x = ax - 1; x > kx; x--)
                    {
                        if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                            return false;
                        y++;
                    }
                }
            }
            else if (goStraightX && goStraightY)
            {
                int y = ay + 1;
                for (int x = ax + 1; x < kx; x++)
                {
                    if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                        return false;
                    y++;
                }
            }
            else if (goStraightX && notGoStraightY)
            {
                int y = ay - 1;
                for (int x = ax + 1; x < kx; x++)
                {
                    if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                        return false;
                    y--;
                }
            }
            else if (notGoStraightX && goStraightY)
            {
                int y = ay + 1;
                for (int x = ax - 1; x > kx; x--)
                {
                    if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                        return false;
                    y++;
                }
            }
            else if (notGoStraightX && notGoStraightY)
            {
                int y = ay - 1;
                for (int x = ax - 1; x > kx; x--)
                {

                    if (ApieceCanAccessSquare(chessBoard, turn, board, pieceInfo, x, y, ax, ay))
                        return false;
                    y--;
                }
            }
        }
        return true;
    }
    private bool ApieceCanAccessSquare(ChessBoard chessBoard, WhoseTurn turn, BoardRelatedInfo[,] board, PieceInfo pieceInfo, int x, int y, int ax, int ay)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j].Apiece == null
                    ||
                    board[i, j].Apiece!.Color == pieceInfo
                    ||
                    board[i, j].Apiece!.Name == PieceName.KING
                    ||
                    (i == ax && j == ay)
                    )
                    continue;

                if (
                    board[i, j].Apiece!.Name.CanPerfomeThisMove(board[i, j].ASquare, board[x, y].ASquare, turn, board[i, j].Apiece!.Color)
                    &&
                    board[i, j].Apiece!.Name.ThereIsNoObstacle(board[i, j].ASquare, board[x, y].ASquare, chessBoard, turn)
                    )
                    return true;
            }
        }
        return false;
    }
}
