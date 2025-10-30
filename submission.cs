using System;
using System.Text;
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

           
            result = Xml2Json("Hotels.xml");
            Console.WriteLine(result);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                // Load the XSD
                XmlSchemaSet schemas = new XmlSchemaSet();
                using (XmlReader xsdReader = XmlReader.Create(xsdUrl, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
                {
                    schemas.Add(null, xsdReader);
                }

                bool hasError = false;
                StringBuilder sb = new StringBuilder();

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas,
                    DtdProcessing = DtdProcessing.Prohibit
                };
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += (sender, e) =>
                {
                    hasError = true;
                    sb.AppendLine($"{e.Severity} - {e.Message}");
                };

                using (XmlReader xmlReader = XmlReader.Create(xmlUrl, settings))
                {
                    while (xmlReader.Read()) { /* iterate to trigger validation */ }
                }

                return hasError ? sb.ToString().Trim() : "No Error";
            }
            catch (XmlException xe)
            {
               
                return $"XMLException - {xe.Message}";
            }
            catch (Exception ex)
            {
                return $"Exception - {ex.Message}";
            }
        }

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                
                XmlDocument doc = new XmlDocument { PreserveWhitespace = false };
                using (XmlReader xr = XmlReader.Create(xmlUrl, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
                {
                    doc.Load(xr);
                }

                
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"Hotels\":{\"Hotel\":[");

                XmlNodeList hotels = doc.SelectNodes("/Hotels/Hotel");
                if (hotels != null)
                {
                    for (int i = 0; i < hotels.Count; i++)
                    {
                        XmlNode h = hotels[i];
                        if (i > 0) sb.Append(',');

                     
                        sb.Append('{');

                       
                        string name = h.SelectSingleNode("Name")?.InnerText ?? "";
                        sb.Append("\"Name\":").Append(JsonConvert.ToString(name)).Append(',');

                        
                        sb.Append("\"Phone\":[");
                        XmlNodeList phones = h.SelectNodes("Phone");
                        if (phones != null)
                        {
                            for (int p = 0; p < phones.Count; p++)
                            {
                                if (p > 0) sb.Append(',');
                                string ph = phones[p]?.InnerText ?? "";
                                sb.Append(JsonConvert.ToString(ph));
                            }
                        }
                        sb.Append("],");

                       
                        XmlNode a = h.SelectSingleNode("Address");
                        sb.Append("\"Address\":{");
                        if (a != null)
                        {
                            string number = a.SelectSingleNode("Number")?.InnerText ?? "";
                            string street = a.SelectSingleNode("Street")?.InnerText ?? "";
                            string city   = a.SelectSingleNode("City")?.InnerText ?? "";
                            string state  = a.SelectSingleNode("State")?.InnerText ?? "";
                            string zip    = a.SelectSingleNode("Zip")?.InnerText ?? "";

                            sb.Append("\"Number\":").Append(JsonConvert.ToString(number)).Append(',');
                            sb.Append("\"Street\":").Append(JsonConvert.ToString(street)).Append(',');
                            sb.Append("\"City\":").Append(JsonConvert.ToString(city)).Append(',');
                            sb.Append("\"State\":").Append(JsonConvert.ToString(state)).Append(',');
                            sb.Append("\"Zip\":").Append(JsonConvert.ToString(zip));

                            // Optional _NearestAirport attribute
                            string nearest = a.Attributes?["NearestAirport"]?.Value;
                            if (!string.IsNullOrWhiteSpace(nearest))
                            {
                                sb.Append(',').Append("\"_NearestAirport\":").Append(JsonConvert.ToString(nearest));
                            }
                        }
                        else
                        {
                            
                            sb.Append("\"Number\":\"\",\"Street\":\"\",\"City\":\"\",\"State\":\"\",\"Zip\":\"\"");
                        }
                        sb.Append('}');

                        
                        string rating = h.Attributes?["Rating"]?.Value;
                        if (!string.IsNullOrWhiteSpace(rating))
                        {
                            sb.Append(',').Append("\"_Rating\":").Append(JsonConvert.ToString(rating));
                        }

                        sb.Append('}'); // end Hotel
                    }
                }

                sb.Append("]}}");
                string jsonText = sb.ToString();

               
                JsonConvert.DeserializeXmlNode(jsonText);

                return jsonText;
            }
            catch (Exception ex)
            {
                return $"Exception - {ex.Message}";
            }
        }
    }
}

        
       
