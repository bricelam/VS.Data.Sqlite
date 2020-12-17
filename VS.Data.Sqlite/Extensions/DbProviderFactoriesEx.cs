using System.Reflection;

namespace System.Data.Common
{
    class DbProviderFactoriesEx
    {
        public static void RegisterFactory(string providerInvariantName, Type providerFactoryClass)
        {
            var providerTable = DbProviderFactories.GetFactoryClasses();
            providerTable.Rows
                .Add(new object[] { null, null, providerInvariantName, providerFactoryClass.AssemblyQualifiedName });
            typeof(DbProviderFactories)
                .GetField("_providerTable", BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, providerTable);
        }
    }
}
