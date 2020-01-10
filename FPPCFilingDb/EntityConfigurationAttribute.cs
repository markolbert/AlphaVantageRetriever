using System;

namespace FPPCFilingDb
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class EntityConfigurationAttribute : Attribute
    {
        private readonly Type _configType;

        public EntityConfigurationAttribute( Type configType )
        {
            _configType = configType ?? throw new NullReferenceException( nameof( configType ) );

            if( !typeof( IEntityConfiguration ).IsAssignableFrom( configType ) )
                throw new ArgumentException(
                    $"Database entity configuration type is not {nameof( IEntityConfiguration )}" );
        }

        public IEntityConfiguration GetConfigurator() =>
            (IEntityConfiguration) Activator.CreateInstance( _configType );
    }
}