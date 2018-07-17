namespace Robot.Quik2Net.XlFormatParser
{
    public interface IXlTableDataFactory<out T>
    {
        T Blank { get; }
        T Skip  { get; }
        T Table (int rows, int cols);
        T Float (double value);
        T String(string value);
        T Bool  (bool value);
        T Error (int value);
        T Int   (int value);
    }
}