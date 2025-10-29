using System;
using System.Net;
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
            
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

           
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                
                XmlSchemaSet schemas = new XmlSchemaSet();
                using (XmlReader xsdReader = XmlReader.Create(xsdUrl))
                {
                    schemas.Add(null, xsdReader);
                }

               
                bool hasError = false;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas,
                    DtdProcessing = DtdProcessing.Ignore
                };
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += (sender, e) =>
                {
                    hasError = true;
                   
                    IXmlLineInfo li = sender as IXmlLineInfo;
                    if (li != null && li.HasLineInfo())
                    {
                        sb.AppendLine($"Line {li.LineNumber}, Pos {li.LinePosition}: {e.Severity} - {e.Message}");
                    }
                    else
                    {
                        sb.AppendLine($"{e.Severity} - {e.Message}");
                    }
                };

               
                using (XmlReader xmlReader = XmlReader.Create(xmlUrl, settings))
                {
                    while (xmlReader.Read()) { /* just walk */ }
                }

                return hasError ? sb.ToString().Trim() : "No Error";
            }
            catch (XmlException xe)
            {
               
                return $"XMLException - {xe.Message}";
            }
            catch (WebException we)
            {
                return $"WebException - {we.Message}";
            }
            catch (Exception ex)
            {
                return $"Exception - {ex.Message}";
            }
        }

        
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument doc = new XmlDocument
                {
                    PreserveWhitespace = false
                };
                using (XmlReader xr = XmlReader.Create(xmlUrl, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                {
                    doc.Load(xr);
                }

                
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

                
                var back = JsonConvert.DeserializeXmlNode(jsonText);

                return jsonText;
            }
            catch (Exception ex)
            {
                return $"Exception - {ex.Message}";
            }
        }
    }
}
