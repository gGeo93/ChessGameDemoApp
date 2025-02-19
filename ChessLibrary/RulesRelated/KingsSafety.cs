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
        if (SpecialEvents.kingIsChecked != null && SpecialEvents.kingIsMate != null && 
            SpecialEvents.WhiteKingHasMoved != null && SpecialEvents.BlackKingHasMoved != null)
            return;
        
        if (SpecialEvents.WhiteKingHasMoved == null)
            SpecialEvents.WhiteKingHasMoved = () => false;
       
        if (SpecialEvents.BlackKingHasMoved == null)
            SpecialEvents.BlackKingHasMoved = () => false;

        if (SpecialEvents.kingIsChecked == null)
            SpecialEvents.kingIsChecked = KingIsChecked;

        if (SpecialEvents.kingIsMate == null)
            SpecialEvents.kingIsMate = KingIsMate;
    }
    ~KingsSafety()
    {
        if (SpecialEvents.kingIsChecked.GetInvocationList().Length > 0)
            SpecialEvents.kingIsChecked -= KingIsChecked;
        if (SpecialEvents.kingIsMate.GetInvocationList().Length > 0)
            SpecialEvents.kingIsMate -= KingIsMate!;
    }
    private bool _doubleCheck = false;
    private bool KingIsChecked(BoardRelatedInfo[,] board, Square kingPosition, WhoseTurn turn, bool checkForPin = false)
    {
        String kingSquare = kingPosition.Letter.ToString() + kingPosition.Number.ToString();
        (int kx, int ky) = kingSquare.FromVisualToProgrammingCoordinates();
        var _piece = board[kx, ky].Apiece;
        bool canPerformMove = false;
        bool thereAreNoObstacles = false;
        int howManyChecks = 0;
        
        var colorToPass = turn == WhoseTurn.White ? PieceInfo.BLACK : PieceInfo.WHITE;
        board[kx, ky].Apiece = null;
        if (checkForPin)
            colorToPass = turn == WhoseTurn.White ? PieceInfo.WHITE : PieceInfo.BLACK;

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                Piece? piece = board[x, y]?.Apiece;

                if (piece is null || (piece.Color == colorToPass))
                    continue;

                PieceName pieceName = piece!.Name;
                Square pSquare = board[x, y].ASquare;
                canPerformMove = pieceName.CanPerfomeThisMove(pSquare, board[kx, ky].ASquare, turn, board[kx, ky].Apiece?.Color);
                thereAreNoObstacles = pieceName.ThereIsNoObstacle(pSquare, board[kx, ky].ASquare, board, turn);
                if ((canPerformMove && thereAreNoObstacles) || (pieceName == PieceName.PAWN && Pawn.PawnThreatensKing(piece.Color, kx, ky, _piece, board, pSquare)))
                    howManyChecks++;
            }
        }
        board[kx, ky].Apiece = _piece;
        switch (howManyChecks)
        {
            case 0: return false;
            case 1: return true;
            case 2: _doubleCheck = true; return true;
            default: return false;
        }
    }
    private bool KingIsMate(BoardRelatedInfo[,] board, Square threatedKingPosition, Square attackingPiecePosition, bool isChecked, WhoseTurn turn)
    {
        if (!isChecked)
            return false;
        bool kingCanMove = KingMovementIsLegal(board, threatedKingPosition, turn);
        if (!kingCanMove && _doubleCheck)
            return true;
        if (kingCanMove)
            return false;
        if (!AttackingPieceCannotBeCaptured(board, attackingPiecePosition, turn))
            return false;
        if (!BetweenAttackingPieceAndKingNopieceCanBlock(board, threatedKingPosition, attackingPiecePosition, turn))
            return false;

        return true;
    }
    private bool KingMovementIsLegal(BoardRelatedInfo[,] chessBoard, Square kingPosition, WhoseTurn turn)
    {
        PieceInfo sameColorAsKing = turn == WhoseTurn.White ? PieceInfo.BLACK : PieceInfo.WHITE;
        bool[,] kingBoard = new bool[3, 3];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                kingBoard[i, j] = true;
            }
        }
        String kpos = kingPosition.Letter + kingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromVisualToProgrammingCoordinates();
        var board = chessBoard;
        kingBoard[1, 1] = false;
        if (kx + 1 <= 7)
        {
            if ((board[kx + 1, ky].ApieceOccupySquare && board[kx + 1, ky].Apiece?.Color == sameColorAsKing))
                kingBoard[2, 1] = false;
            if (KingIsChecked(chessBoard, board[kx + 1, ky].ASquare, turn))
                kingBoard[2, 1] = false;
        }
        else
            kingBoard[2, 1] = false;
        if (kx - 1 >= 0)
        {
            if ((board[kx - 1, ky].ApieceOccupySquare && board[kx - 1, ky].Apiece?.Color == sameColorAsKing))
                kingBoard[0, 1] = false;
            if (KingIsChecked(chessBoard, board[kx - 1, ky].ASquare, turn))
                kingBoard[0, 1] = false;
        }
        else
            kingBoard[0, 1] = false;
        if (ky + 1 <= 7)
        {
            if ((board[kx, ky + 1].ApieceOccupySquare && board[kx, ky + 1].Apiece?.Color == sameColorAsKing))
                kingBoard[1, 2] = false;
            if (KingIsChecked(chessBoard, board[kx, ky + 1].ASquare, turn))
                kingBoard[1, 2] = false;
        }
        else
            kingBoard[1, 2] = false;
        if (ky - 1 >= 0)
        {
            if ((board[kx, ky - 1].ApieceOccupySquare && board[kx, ky - 1].Apiece?.Color == sameColorAsKing))
                kingBoard[1, 0] = false;
            if (KingIsChecked(chessBoard, board[kx, ky - 1].ASquare, turn))
                kingBoard[1, 0] = false;
        }
        else
            kingBoard[1, 0] = false;
        if (kx + 1 <= 7 && ky + 1 <= 7)
        {
            if ((board[kx + 1, ky + 1].ApieceOccupySquare && board[kx + 1, ky + 1].Apiece?.Color == sameColorAsKing))
                kingBoard[2, 2] = false;
            if (KingIsChecked(chessBoard, board[kx + 1, ky + 1].ASquare, turn))
                kingBoard[2, 2] = false;
        }
        else
            kingBoard[2, 2] = false;
        if (kx + 1 <= 7 && ky - 1 >= 0)
        {
            if ((board[kx + 1, ky - 1].ApieceOccupySquare && board[kx + 1, ky - 1].Apiece?.Color == sameColorAsKing))
                kingBoard[2, 0] = false;
            else if (KingIsChecked(chessBoard, board[kx + 1, ky - 1].ASquare, turn))
                kingBoard[2, 0] = false;
        }
        else
            kingBoard[2, 0] = false;
        if (kx - 1 >= 0 && ky + 1 <= 7)
        {
            if (board[kx - 1, ky + 1].ApieceOccupySquare && board[kx - 1, ky + 1].Apiece?.Color == sameColorAsKing)
                kingBoard[0, 2] = false;
            if (KingIsChecked(chessBoard, board[kx - 1, ky + 1].ASquare, turn))
                kingBoard[0, 2] = false;
        }
        else
            kingBoard[0, 2] = false;
        if (kx - 1 >= 0 && ky - 1 >= 0)
        {
            if ((board[kx - 1, ky - 1].ApieceOccupySquare && board[kx - 1, ky - 1].Apiece?.Color == sameColorAsKing))
                kingBoard[0, 0] = false;
            if (KingIsChecked(chessBoard, board[kx - 1, ky - 1].ASquare, turn))
                kingBoard[0, 0] = false;
        }
        else
            kingBoard[0, 0] = false;

        foreach (bool k in kingBoard)
            if (k)
                return true;

        return false;
    }

    private bool AttackingPieceCannotBeCaptured(BoardRelatedInfo[,] chessBoard, Square attackingPiecePosition, WhoseTurn turn)
    {
        String apos = attackingPiecePosition.Letter + attackingPiecePosition.Number.ToString();
        (int ax, int ay) = apos.FromVisualToProgrammingCoordinates();
        for (int i = 0; i < chessBoard.GetLength(0); i++)
        {
            for (int j = 0; j < chessBoard.GetLength(1); j++)
            {
                if (chessBoard[i, j].Apiece == null
                    ||
                    chessBoard[i, j].Apiece!.Name == PieceName.KING)
                    continue;

                PieceName pieceName = chessBoard[i, j].Apiece!.Name;

                if (pieceName.CanPerfomeThisMove(chessBoard[i, j].ASquare, attackingPiecePosition, turn, chessBoard[i, j].Apiece!.Color))
                    if (pieceName.ThereIsNoObstacle(chessBoard[i, j].ASquare, attackingPiecePosition, chessBoard, turn))
                        return false;
            }
        }
        return true;
    }
    private bool BetweenAttackingPieceAndKingNopieceCanBlock(BoardRelatedInfo[,] chessBoard, Square kingPosition, Square attackingPiecePosition, WhoseTurn turn)
    {
        String kpos = kingPosition.Letter + kingPosition.Number.ToString();
        (int kx, int ky) = kpos.FromVisualToProgrammingCoordinates();
        String apos = attackingPiecePosition.Letter + attackingPiecePosition.Number.ToString();
        (int ax, int ay) = apos.FromVisualToProgrammingCoordinates();

        var board = chessBoard;
        PieceName pieceName = board[ax, ay].Apiece!.Name;

        if (pieceName == PieceName.KNIGHT)
            return true;
        else if (pieceName == PieceName.KING)
            return true;
        else if (pieceName == PieceName.PAWN)
        {
            if ((ay + 1 <= 7 && chessBoard[ax + 1, ay + 1].Apiece?.Name == PieceName.KING)
                ||
                (ay - 1 >= 0 && chessBoard[ax + 1, ay - 1].Apiece?.Name == PieceName.KING))
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

                    if (ApieceCanAccessSquare(chessBoard , turn, board, pieceInfo, x, y, ax, ay))
                        return false;
                    y--;
                }
            }
        }
        return true;
    }
    private bool ApieceCanAccessSquare(BoardRelatedInfo[,] chessBoard, WhoseTurn turn, BoardRelatedInfo[,] board, PieceInfo pieceInfo, int x, int y, int ax, int ay)
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
