using System;
using System.IO;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;

namespace Projekat.Models
{
    public class Serijalizacija
    {
        public void Serijalizuj<T>(T objekat, string putanja)
        {
            string putanja1 = HostingEnvironment.MapPath(putanja);
            if (objekat == null)
            {
                return;
            }
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer xmlSerializer = new XmlSerializer(objekat.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    xmlSerializer.Serialize(ms, objekat);
                    ms.Position = 0;
                    xmlDocument.Load(ms);
                    
                    xmlDocument.Save(putanja1);
                    ms.Close();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public T Deserijalizuj<T>(string putanja)
        {
            putanja = HostingEnvironment.MapPath(putanja);
            if (string.IsNullOrEmpty(putanja))
            {
                return default(T);
            }

            T ret = default(T);

            try
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(putanja);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer xmlSerializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        ret = (T)xmlSerializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return ret;
        }
    }
}