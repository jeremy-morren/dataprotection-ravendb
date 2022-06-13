using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace AspNetCore.DataProtection.RavenDB
{
    /// <summary>
    ///     Helper wrapper around <see cref="IDocumentStore" />
    ///     for DI to avoid interfering with any other
    ///     instances of <see cref="IDocumentStore" />
    /// </summary>
    internal class DocumentStoreInstance
    {
        private readonly IDocumentStore _store;

        public DocumentStoreInstance(IDocumentStore store)
        {
            _store = store;
        }

        public IDocumentSession OpenSession()
        {
            return _store.OpenSession();
        }
    }
}