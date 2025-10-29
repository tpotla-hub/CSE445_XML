using System;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    public class Program
    {
        // REPLACE <your-username> with your GitHub username
        public static string xmlURL      = "https://github.com/tpotla-hub/CSE445_XML/blob/main/Hotels.xml ";
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

        // Q2.1
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

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlUrl);

                JObject hotelsRoot = new JObject();
                JObject hotelsObj  = new JObject();
                JArray hotelsArr   = new JArray();
                hotelsRoot["Hotels"] = hotelsObj;
                hotelsObj["Hotel"]   = hotelsArr;

                XmlNodeList hotelNodes = doc.SelectNodes("/Hotels/Hotel");
                foreach (XmlNode h in hotelNodes)
                {
                    JObject hObj = new JObject
                    {
                        ["Name"] = h.SelectSingleNode("Name")?.InnerText ?? ""
                    };

                    // Phones
                    XmlNodeList phones = h.SelectNodes("Phone");
                    if (phones.Count == 1)
                        hObj["Phone"] = phones[0].InnerText;
                    else if (phones.Count > 1)
                    {
                        JArray pArr = new JArray();
                        foreach (XmlNode p in phones) pArr.Add(p.InnerText);
                        hObj["Phone"] = pArr;
                    }

                    // Address
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

                        string na = a.Attributes?["NearestAirport"]?.Value;
                        if (!string.IsNullOrWhiteSpace(na))
                            aObj["_NearestAirport"] = na;

                        hObj["Address"] = aObj;
                    }

                    // Optional attribute Rating
                    string rating = h.Attributes?["Rating"]?.Value;
                    if (!string.IsNullOrWhiteSpace(rating))
                        hObj["_Rating"] = rating;

                    hotelsArr.Add(hObj);
                }

                return hotelsRoot.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                return $"Exception - {ex.Message}";
            }
        }
    }
}
