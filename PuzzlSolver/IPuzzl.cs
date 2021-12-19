namespace AdventOfCode
{
    public interface IPuzzle
    {
        int Solve();
        long SolveNext();

        int FirstResult {get;}
        long SecondResult {get;}
    }
}