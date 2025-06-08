
using Backend_poulina_future_jobs.Models;

internal class ApplicationDbContext
{
    public object OffresEmploi { get; internal set; }
    public object Candidatures { get; internal set; }

    internal object Set<T>()
    {
        throw new NotImplementedException();
    }

    public static implicit operator ApplicationDbContext(AppDbContext v)
    {
        throw new NotImplementedException();
    }
}