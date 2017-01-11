using ServiceStack.OrmLite;

namespace oze.data
{
    public class OzeConnectionFactory: OrmLiteConnectionFactory, IOzeConnectionFactory
    {
        public OzeConnectionFactory(string s) : base(s) { }
        public OzeConnectionFactory(string s, IOrmLiteDialectProvider provider) : base(s, provider) { }
    }
}