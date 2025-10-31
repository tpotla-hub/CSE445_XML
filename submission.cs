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
            // (1) Valid XML => exact required message
            string msg = Verification(xmlURL, xsdURL);
            Console.WriteLine(msg);

            // (2) Error XML => list ALL errors
            msg = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(msg);

            // (3) XML -> JSON (must be deserializable by Json.NET)
            msg = Xml2Json(xmlURL);
            Console.WriteLine(msg);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                var schemas = new XmlSchemaSet();
                using (var xsdReader = XmlReader.Create(xsdUrl))
                {
                    schemas.Add(null, xsdReader);
                }

                bool hasAnyError = false;
                var all = new System.Text.StringBuilder();

                var settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas,
                    DtdProcessing = DtdProcessing.Ignore
                };

                settings.ValidationEventHandler += (sender, e) =>
                {
                    hasAnyError = true;
                    all.AppendLine(e.Message);
                };

                using (var xr = XmlReader.Create(xmlUrl, settings))
                {
                    while (xr.Read()) { /* consume */ }
                }

                // EXACT success text per assignment PDF
                return hasAnyError ? all.ToString().TrimEnd() : "No errors are found.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                var doc = new XmlDocument { PreserveWhitespace = false };
                doc.Load(xmlUrl);

                // Keep root; this form round-trips with DeserializeXmlNode
                string jsonText = JsonConvert.SerializeXmlNode(doc, Formatting.Indented, false);

                // Validate JSON is deserializable by Json.NET as required
                _ = JsonConvert.DeserializeXmlNode(jsonText);

                return jsonText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

       
