public class Program
{
    public static async Task Main(string[] args)
    {
        var engine = new Engine(new DefaultGame());
        await engine.InitializeAsync();
    }
}