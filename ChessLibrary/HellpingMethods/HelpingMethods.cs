using ChessLibrary.BoardRelated;
using ChessLibrary.EventsRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.PieceRelated;
using System.Diagnostics;

namespace ChessLibrary.HellpingMethods;

public static class HelpingMethods
{
    public static BoardRelatedInfo[,] ChessBoardFill(this BoardRelatedInfo[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (i == 7)
                {
                    if (j == 0 || j == 7)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Rook { Color = PieceInfo.WHITE, Name = PieceName.ROOK },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 1 || j == 6)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Knight { Color = PieceInfo.WHITE, Name = PieceName.KNIGHT },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 2 || j == 5)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Bishop { Color = PieceInfo.WHITE, Name = PieceName.BISHOP },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 3)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Queen { Color = PieceInfo.WHITE, Name = PieceName.QUEEN },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 4)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new King { Color = PieceInfo.WHITE, Name = PieceName.KING },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                }
                else if (i == 6)
                    board[i, j] = new BoardRelatedInfo
                    {
                        ApieceOccupySquare = true,
                        Apiece = new Pawn(PieceInfo.WHITE) { Color = PieceInfo.WHITE, Name = PieceName.PAWN, IsOnInitialSquare = true, PawnIntentsToMoveTwice = false },
                        ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                    };
                else if (i == 1)
                    board[i, j] = new BoardRelatedInfo
                    {
                        ApieceOccupySquare = true,
                        Apiece = new Pawn(PieceInfo.BLACK) { Color = PieceInfo.BLACK, Name = PieceName.PAWN, IsOnInitialSquare = true, PawnIntentsToMoveTwice = false },
                        ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                    };
                else if (i == 0)
                {
                    if (j == 0 || j == 7)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Rook { Color = PieceInfo.BLACK, Name = PieceName.ROOK },
                            ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 1 || j == 6)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Knight { Color = PieceInfo.BLACK, Name = PieceName.KNIGHT },
                            ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 2 || j == 5)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Bishop { Color = PieceInfo.BLACK, Name = PieceName.BISHOP },
                            ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 3)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new Queen { Color = PieceInfo.BLACK, Name = PieceName.QUEEN },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 4)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            ApieceOccupySquare = true,
                            Apiece = new King { Color = PieceInfo.BLACK, Name = PieceName.KING },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                }
                else
                {
                    board[i, j] = new BoardRelatedInfo() { ApieceOccupySquare = false, Apiece = null , ASquare = new Square() { Letter = j.ColumnConverter(), Number = i.RowConverter(), Color = (i + j) % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK } };
                }
            }
        }
        return board;
    }
    public static void InternalCoordinatesOperation(this Square from, Square to, out (int xFrom, int yFrom) pseudoCoorFrom, out (int xTo, int yTo) pseudoCoorTo)
    {
        pseudoCoorFrom = (from.Letter + from.Number.ToString()).FromRealToProgrammingCoordinates();
        pseudoCoorTo = (to.Letter + to.Number.ToString()).FromRealToProgrammingCoordinates();
    }
    public static char ColumnConverter(this int y)
    {
        switch (y)
        {
            case 0: return 'a';
            case 1: return 'b';
            case 2: return 'c';
            case 3: return 'd';
            case 4: return 'e';
            case 5: return 'f';
            case 6: return 'g';
            case 7: return 'h';
            default: return '\0';
        }
    }
    public static int RowConverter(this int x)
    {
        switch (x)
        {
            case 0: return 8;
            case 1: return 7;
            case 2: return 6;
            case 3: return 5;
            case 4: return 4;
            case 5: return 3;
            case 6: return 2;
            case 7: return 1;
            default: return 0;
        }
    }
    public static (int x, int y) FromRealToProgrammingCoordinates(this String real)
    {
        int y = -1;
        switch (real[0])
        {
            case 'a': y = 0; break; 
            case 'b': y = 1; break; 
            case 'c': y = 2; break; 
            case 'd': y = 3; break; 
            case 'e': y = 4; break; 
            case 'f': y = 5; break; 
            case 'g': y = 6; break; 
            case 'h': y = 7; break; 
        }
        int x = -1;
        switch (int.Parse(real[1].ToString()))
        {
            case 1: x = 7; break;
            case 2: x = 6; break;
            case 3: x = 5; break;
            case 4: x = 4; break;
            case 5: x = 3; break;
            case 6: x = 2; break;
            case 7: x = 1; break;
            case 8: x = 0; break;
        }
        return (x, y);
    }
    public static (int xc, int yc) ThePieceGivingCheckCoordinates(this ChessBoard chessBoard, Square kingPosition, WhoseTurn turn)
    {
        BoardRelatedInfo[,] board = (BoardRelatedInfo[,])chessBoard.Board.Clone();
        PieceInfo pieceColorToAvoid = turn == WhoseTurn.White ? PieceInfo.BLACK : PieceInfo.WHITE;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var checkingCandidateSquare = board[i, j].ApieceOccupySquare;
                var checkingCandidatePiece = board[i, j].Apiece;
                chessBoard.Board[i, j].Apiece = null;
                if (checkingCandidatePiece?.Color != pieceColorToAvoid &&
                    SpecialEvents.kingIsChecked.Invoke(chessBoard, kingPosition, turn, false) == false)
                {
                    chessBoard.Board[i, j].Apiece = checkingCandidatePiece;
                    chessBoard.Board[i, j].ApieceOccupySquare = checkingCandidateSquare;
                    return (i, j);
                }
                chessBoard.Board[i, j].Apiece = checkingCandidatePiece;
                chessBoard.Board[i, j].ApieceOccupySquare = checkingCandidateSquare;
            }
        }
        return (-1, -1);
    }
    public static (Square whiteKing, Square blackKing) GetKingsPositions(this ChessBoard chessBoard)
    {
        Square white = new Square();
        Square black = new Square();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece?.Name == PieceName.KING)
                {
                    if (GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece.Color == PieceInfo.WHITE)
                        white = GamingProcess.Instance.ChessBoard.Board[i, j]?.ASquare;
                    if (GamingProcess.Instance.ChessBoard.Board[i, j]?.Apiece.Color == PieceInfo.BLACK)
                        black = GamingProcess.Instance.ChessBoard.Board[i, j]?.ASquare;
                }
        return (white!, black!);
    }
}
