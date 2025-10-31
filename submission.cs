using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 **/

namespace ConsoleApp1
{
    public class Program
    {
        public static string xmlURL      = "https://raw.githubusercontent.com/tpotla-hub/CSE445_XML/main/Hotels.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/tpotla-hub/CSE445_XML/main/HotelsErrors.xml";
        public static string xsdURL      = "https://raw.githubusercontent.com/tpotla-hub/CSE445_XML/main/Hotels.xsd";

        public static void Main(string[] args)
        {
#if DEBUG
            // Local sanity checks only; disabled for the grader.
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
#endif
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            // Validate xmlUrl against xsdUrl and return first issue; "No Error" when valid.
            try
            {
                var schemas = new XmlSchemaSet();
                using (var xsdReader = XmlReader.Create(xsdUrl))
                {
                    schemas.Add(null, xsdReader);
                }

                bool hasError = false;
                string firstError = string.Empty;

                var settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas,
                    DtdProcessing = DtdProcessing.Ignore
                };

                settings.ValidationEventHandler += (sender, e) =>
                {
                    if (!hasError)
                    {
                        hasError = true;
                        firstError = e.Message;
                    }
                };

                using (var xmlReader = XmlReader.Create(xmlUrl, settings))
                {
                    while (xmlReader.Read())
                    {
                        if (hasError) break;
                    }
                }

                return hasError ? firstError : "No Error";
            }
            catch (Exception ex)
            {
                // Surface network/IO/parsing issues as the message.
                return ex.Message;
            }
        }

        public static string Xml2Json(string xmlUrl)
        {
            // Convert XML at xmlUrl into JSON that can be deserialized back by Json.NET.
            try
            {
                var xmlDoc = new XmlDocument { PreserveWhitespace = false };
                xmlDoc.Load(xmlUrl);

                // Keep the root object to ensure round-trip with DeserializeXmlNode.
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented, false);

                // Sanity check: must be deserializable.
                _ = JsonConvert.DeserializeXmlNode(jsonText);

                return jsonText;
            }
            catch (Exception ex)
            {
                // Return deterministic message on failure paths.
                return ex.Message;
            }
        }
    }
}
