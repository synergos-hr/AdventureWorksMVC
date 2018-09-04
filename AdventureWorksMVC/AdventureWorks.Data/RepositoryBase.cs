using NLog;

namespace AdventureWorks.Data
{
    public abstract class RepositoryBase
    {
        protected AppDbContext Context = new AppDbContext();

        protected readonly Logger Log = LogManager.GetCurrentClassLogger();
    }
}
