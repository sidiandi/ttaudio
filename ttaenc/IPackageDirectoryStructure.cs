namespace ttaenc
{
    public interface IPackageDirectoryStructure
    {
        Package Package
        {
            get;
        }

        string GmeFile { get; }
        string HtmlFile { get; }
        string HtmlMediaDirectory { get; }
    }
}