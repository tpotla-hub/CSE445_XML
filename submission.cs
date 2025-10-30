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
            // Q3(1): verify the correct XML
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            // Q3(2): verify the error XML (should NOT be "No Error")
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            // Q3(3): convert valid XML to JSON (handout’s shape)
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                // Load schema from URL
                var schemas = new XmlSchemaSet();
                using (var xsdReader = XmlReader.Create(xsdUrl))
                {
                    schemas.Add(null, xsdReader);
                }

                bool hasError = false;
                var sb = new System.Text.StringBuilder();

                var settings = new XmlReaderSettings
                {


        

      
                 
