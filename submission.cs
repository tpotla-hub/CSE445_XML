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
            string validationError = string.Empty;
            bool foundError = false;

            try
            {
                XmlSchemaSet schemaCollection = new XmlSchemaSet();
                
                XmlReader xsdReaderStream = XmlReader.Create(xsdUrl);
                schemaCollection.Add(null, xsdReaderStream);
                xsdReaderStream.Close();

                XmlReaderSettings validationSettings = new XmlReaderSettings();
                validationSettings.ValidationType = ValidationType.Schema;
                validationSettings.Schemas = schemaCollection;
                validationSettings.DtdProcessing = DtdProcessing.Ignore;

                validationSettings.ValidationEventHandler += delegate(object sender, ValidationEventArgs validationArgs)
                {
                    if (!foundError)
                    {
                        validationError = validationArgs.Message;
                        foundError = true;
                    }
                };

                XmlReader xmlReaderStream = XmlReader.Create(xmlUrl, validationSettings);
                
                while (xmlReaderStream.Read())
                {
                    if (foundError)
                    {
                        break;
                    }
                }
                
                xmlReaderStream.Close();

                if (foundError)
                {
                    return validationError;
                }
                
                return "No Error";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = false;
                xmlDoc.Load(xmlUrl);

                string convertedJson = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented, false);

                XmlDocument testDeserialize = JsonConvert.DeserializeXmlNode(convertedJson);

                return convertedJson;
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
    }
}
