namespace Keebee.AAT.GenerateThumbnails
{
    internal class Program
    {
        private static void Main()
        {
            var engine = new Engine();
            engine.GenerateThumbnails(overwrite: false, deleteOrphans: true);
        }
    }
}
