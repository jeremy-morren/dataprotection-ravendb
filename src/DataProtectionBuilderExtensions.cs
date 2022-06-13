using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;

[assembly: InternalsVisibleTo("AspNetCore.DataProtection.RavenDB.Tests")]
namespace AspNetCore.DataProtection.RavenDB
{
    public static class DataProtectionBuilderExtensions
    {
        /// <summary>
        /// Configures the data protection system to persist keys to RavenDB
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.DataProtection.IDataProtectionBuilder" />.</param>
        /// <param name="urls">RavenDB URL(s)</param>
        /// <param name="database">The RavenDB Database in which to store keys</param>
        /// <param name="certificate">Optional RavenDB client certificate used to connect to database</param>
        /// <returns>A reference to the <see cref="T:Microsoft.AspNetCore.DataProtection.IDataProtectionBuilder" /> after this operation has completed.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="urls"/> is null or empty
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="database"/> is null or empty
        /// </exception>
        /// <remarks>Keys are stored in a <c>Keys</c> collection where <c>DocumentId</c> is the key friendly name</remarks>
        public static IDataProtectionBuilder PersistKeysToRavenDB(this IDataProtectionBuilder builder,
            string[] urls,
            string database,
            X509Certificate2? certificate)
        {
            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("RavenDB database is required");
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (urls == null || urls.Length == 0)
                throw new ArgumentException("RavenDB url(s) are required");
            return builder.PersistKeysToRavenDB(() => new DocumentStore
                {
                    Urls = urls,
                    Database = database,
                    Certificate = certificate
                }
                .SetConventions()
                .Initialize());
        }

        internal static IDataProtectionBuilder PersistKeysToRavenDB(this IDataProtectionBuilder builder,
            Func<IDocumentStore> createStore)
        {
            builder.Services.AddSingleton(_ => new DocumentStoreInstance(createStore()))
                .AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
                {
                    var loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                    var store = services.GetRequiredService<DocumentStoreInstance>();
                    return new ConfigureOptions<KeyManagementOptions>(options => 
                        options.XmlRepository = new RavenDBXmlRepository(store, loggerFactory));
                });
            return builder;
        }

        internal static IDocumentStore SetConventions(this IDocumentStore store)
        {
            store.Conventions.FindCollectionName = t =>
                t == typeof(XmlKey) ? "Keys" : DocumentConventions.DefaultGetCollectionName(t);
            return store;
        }
    }
}