using System.Diagnostics.CodeAnalysis;

namespace AspNetCore.DataProtection.RavenDB
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    internal class XmlKey
    {
        public XmlKey(string id, string xml)
        {
            Id = id;
            Xml = xml;
        }

        public string Id { get; }
        public string Xml { get; }
    }
}