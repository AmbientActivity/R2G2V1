namespace Keebee.AAT.Backup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var engine = new Engine();
            engine.DoBackup();
        }
    }
}