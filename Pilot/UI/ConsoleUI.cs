namespace Pilot.UI
{
    public class ConsoleUI
    {
        // Output
        public void Print(string? text) => Console.WriteLine(text);

        // Input
        public string Read() => Console.ReadLine()!;
    }
}