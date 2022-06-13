using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;

namespace AspNetCore.DataProtection.RavenDB
{
    internal class RavenDBXmlRepository : IXmlRepository
    {
        private const string IdPrefix = "Keys";

        private static readonly XmlWriterSettings WriterSettings = new XmlWriterSettings
        {
            Indent = false,
            Async = false
        };

        private readonly ILogger<RavenDBXmlRepository> _logger;
        private readonly DocumentStoreInstance _store;

        public RavenDBXmlRepository(DocumentStoreInstance store, ILoggerFactory loggerFactory)
        {
            _store = store;
            _logger = loggerFactory.CreateLogger<RavenDBXmlRepository>();
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using var session = _store.OpenSession();
            return session.Query<XmlKey>()
                .ToList() //Load from server
                .Select(k => XElement.Parse(k.Xml))
                .ToList();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using var session = _store.OpenSession();
            var sb = new StringBuilder();
            using (var xWriter = XmlWriter.Create(sb, WriterSettings))
            {
                element.WriteTo(xWriter);
            }

            var key = new XmlKey($"{IdPrefix}/{friendlyName}", sb.ToString());
            session.Store(key);
            session.SaveChanges();
        }
    }
}