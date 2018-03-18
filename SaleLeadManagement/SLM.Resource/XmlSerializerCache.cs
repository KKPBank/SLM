using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace SLM.Resource
{
    public  class XmlSerializerCache
    {
        private static object sync = new Object();
        private static readonly Dictionary<string, XmlSerializer> Cache = new Dictionary<string, XmlSerializer>();

        public static XmlSerializer Create(Type type, XmlRootAttribute root)
        {
            lock (sync)
            {
                var key = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", type, root.ElementName);

                if (!Cache.ContainsKey(key))
                {
                    Cache.Add(key, new XmlSerializer(type, root));
                }

                return Cache[key];
            }
        }
    }
}
