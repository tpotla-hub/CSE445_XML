using System;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;


namespace ConsoleApp1
{
    public class Program
    {
        public static string xmlURL      = "https://github.com/tpotla-hub/CSE445_XML/blob/main/Hotels.xml";
        public static string xmlErrorURL = "https://github.com/tpotla-hub/CSE445_XML/blob/main/HotelsErrors.xml";
        public static string xsdURL      = "https://github.com/tpotla-hub/CSE445_XML/blob/main/Hotels.xsd";

        public static void Main(string[] args)
        {
           
            string msg = Verification(xmlURL, xsdURL);
            Console.WriteLine(msg);

            msg = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(msg);

            msg = Xml2Json(xmlURL);
            Console.WriteLine(msg);
        }

       
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                bool seenError = false;
                string first = string.Empty;

                var set = new XmlSchemaSet();
                using (var xsd = XmlReader.Create(xsdUrl))
                {
                    set.Add(null, xsd);
                }

                var cfg = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = set,
                    DtdProcessing = DtdProcessing.Ignore
                };

                cfg.ValidationEventHandler += (s, e) =>
                {
                    if (!seenError)
                    {
                        seenError = true;
                        first = e.Message;
                    }
                };

                using (var xr = XmlReader.Create(xmlUrl, cfg))
                {
                    while (xr.Read())
                    {
                        if (seenError) break;
                    }
                }

                return seenError ? first : "No Error";
            }
            catch (Exception ex)
            {
                
                return ex.Message;
            }
        }

        
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                var dom = new XmlDocument { PreserveWhitespace = false };
                dom.Load(xmlUrl);

                
                string json = JsonConvert.SerializeXmlNode(dom, Formatting.Indented, false);

                
                _ = JsonConvert.DeserializeXmlNode(json);

                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

       
