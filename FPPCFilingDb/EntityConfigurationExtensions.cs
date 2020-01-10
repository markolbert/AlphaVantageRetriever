using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace J4JSoftware.FppcFiling
{
    public static class EntityConfigurationExtensions
    {
        public static void ConfigureEntities( this ModelBuilder modelBuilder, Assembly assemblyToScan )
        {
            if( modelBuilder == null || assemblyToScan == null )
                return;

            // scan current assembly for types decorated with EntityConfigurationAttribute
            foreach( var entityType in assemblyToScan.DefinedTypes
                .Where( t => ((MemberInfo) t).GetCustomAttribute<EntityConfigurationAttribute>() != null ) )
            {
                entityType.GetCustomAttribute<EntityConfigurationAttribute>()
                    .GetConfigurator()
                    .Configure( modelBuilder );
            }
        }
    }
}