namespace ChessUIForm.FrontLib;

public class FrontLogic
{
    public Button[,] frontBoard { get; set; }
    public Button[] moveParts { get; set; }
    public Color squareColor { get; set; }
    public Func<int, int, Button> OnPawnPromotion { get; set; }
    public FrontLogic()
    {
        frontBoard = new Button[8, 8];
        moveParts = new Button[2];
    }
}
