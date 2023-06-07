namespace BrowserDriver.Browsers
{

    public interface IBrowserDetector
    {
        string Name { get; }
        string AppFolder { get; }
        string AppExecutable { get; }

        bool Installed { get; }
    }

}