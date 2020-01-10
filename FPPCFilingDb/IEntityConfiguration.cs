using Microsoft.EntityFrameworkCore;

namespace FPPCFilingDb
{
    public interface IEntityConfiguration
    {
        void Configure( ModelBuilder builder );
    }

    public interface IEntityConfiguration<TEntity> : IEntityConfiguration
        where TEntity : class
    {
    }
}