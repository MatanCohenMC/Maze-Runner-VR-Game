public class GameLevel
{
    public string Name { get; private set; }
    public int Rows { get; private set; }
    public int Cols { get; private set; }

    public GameLevel(string i_Name, int i_Rows, int i_Cols)
    {
        Name = i_Name;
        Rows = i_Rows;
        Cols = i_Cols;
    }
}