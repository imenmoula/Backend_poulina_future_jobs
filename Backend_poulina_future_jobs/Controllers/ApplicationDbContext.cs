
internal class ApplicationDbContext
{
    public object OffresEmploi { get; internal set; }
    public object Candidatures { get; internal set; }

    internal object Set<T>()
    {
        throw new NotImplementedException();
    }
}