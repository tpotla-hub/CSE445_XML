using System;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;

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
                    while (xmlReader.Read()) { }
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

        
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument doc = new XmlDocument { PreserveWhitespace = false };
                using (XmlReader xr = XmlReader.Create(xmlUrl, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
                {
                    doc.Load(xr);
                }

                JObject root = new JObject();
                JObject hotelsObj = new JObject();
                JArray hotelArr = new JArray();
                root["Hotels"] = hotelsObj;
                hotelsObj["Hotel"] = hotelArr;

                XmlNodeList hotels = doc.SelectNodes("/Hotels/Hotel");
                if (hotels != null)
                {
                    foreach (XmlNode h in hotels)
                    {
                        JObject hObj = new JObject();

                        
                        hObj["Name"] = h.SelectSingleNode("Name")?.InnerText ?? "";

                        
                        XmlNodeList phones = h.SelectNodes("Phone");
                        if (phones != null && phones.Count > 0)
                        {
                            if (phones.Count == 1)
                            {
                                hObj["Phone"] = phones[0].InnerText;
                            }
                            else
                            {
                                JArray pArr = new JArray();
                                foreach (XmlNode p in phones) pArr.Add(p.InnerText);
                                hObj["Phone"] = pArr;
                            }
                        }

                        
                        XmlNode a = h.SelectSingleNode("Address");
                        if (a != null)
                        {
                            JObject aObj = new JObject
                            {
                                ["Number"] = a.SelectSingleNode("Number")?.InnerText ?? "",
                                ["Street"] = a.SelectSingleNode("Street")?.InnerText ?? "",
                                ["City"]   = a.SelectSingleNode("City")?.InnerText ?? "",
                                ["State"]  = a.SelectSingleNode("State")?.InnerText ?? "",
                                ["Zip"]    = a.SelectSingleNode("Zip")?.InnerText ?? ""
                            };

                            string nearest = a.Attributes?["NearestAirport"]?.Value;
                            if (!string.IsNullOrWhiteSpace(nearest))
                                aObj["_NearestAirport"] = nearest;

                            hObj["Address"] = aObj;
                        }

                        
                        string rating = h.Attributes?["Rating"]?.Value;
                        if (!string.IsNullOrWhiteSpace(rating))
                            hObj["_Rating"] = rating;

                        hotelArr.Add(hObj);
                    }
                }

                return root.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                return $"Exception - {ex.Message}";
            }
        }
    }
}
