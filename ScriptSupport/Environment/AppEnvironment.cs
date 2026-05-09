using System.IO;
using System.Reflection;

namespace ScriptSupport.Environment
{
    public sealed class AppEnvironment
    {
        public string ExeFilePath { get; } = Assembly.GetExecutingAssembly().Location;
        public string BaseDirectory { get; }
        public string ConfigFolderPath { get; }
        public string ErrorLogFilePath { get; }
        public string DataFolderPath { get; }
        public string RaresFolderPath { get; }
        public string RaresListDBPath { get; }
        public string RareCardDBPath { get; }
        public string GenesysFolderPath { get; }
        public string GenesysDBPath { get; }
        public string StampFolderPath { get; }
        public string KonamiIDFilePath { get; }
        public string HighLightFolderPath { get; }
        public string ScrapiyardFolderPath { get; }


        public AppEnvironment()
        {
            BaseDirectory = Path.GetDirectoryName(ExeFilePath)
                ?? throw new InvalidOperationException("The application's root directory cannot be determined.");
            ConfigFolderPath = System.IO.Path.Combine(BaseDirectory, "config");
            DataFolderPath = System.IO.Path.Combine(BaseDirectory, "data");
            ErrorLogFilePath = System.IO.Path.Combine(BaseDirectory, "ErrorLog.txt");
            RaresFolderPath = System.IO.Path.Combine(DataFolderPath, "Rares");
            RaresListDBPath = System.IO.Path.Combine(RaresFolderPath, "RaresListDB.cdb");
            RareCardDBPath = System.IO.Path.Combine(RaresFolderPath, "RareCardsDB.cdb");
            GenesysFolderPath = System.IO.Path.Combine(DataFolderPath, @"CardData\Genesys");
            GenesysDBPath = System.IO.Path.Combine(GenesysFolderPath, "GenesysCardsDB.cdb");
            StampFolderPath = System.IO.Path.Combine(DataFolderPath, "RareStamp");
            KonamiIDFilePath = System.IO.Path.Combine(DataFolderPath, $@"CardData\KonamiID\KonamiID.cdb");
            HighLightFolderPath = System.IO.Path.Combine(BaseDirectory, "HighLight");
            ScrapiyardFolderPath = System.IO.Path.Combine(DataFolderPath, "scrapiyard");
        }
    }
}
