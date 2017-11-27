using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
/// <summary>
/// XMLValidator.Validator Name space
/// </summary>
namespace XMLValidator.Validator
{
    #region CLASS
    /// <summary>
    /// Class Validator
    /// </summary>
    class Validator
    {
        #region isXML
        /// <summary>
        /// Checks if file is a XML
        /// </summary>
        /// <param name="xmlFile">filename</param>
        /// <param name="error">output string in case of errors</param>
        /// <returns>true: if it is a XML</returns>
        public static bool isXML(string xmlFile, out string error)
        {
            error = "";
            try
            {
                using (FileStream input = new FileStream(xmlFile, FileMode.Open))
                {
                    try
                    {
                        var doc = XDocument.Load(input);
                    }
                    catch (Exception exFS)
                    {
                        error += "\nException:\n" + exFS.Message;
                        return false;
                    }
                }
            }
            catch (Exception exU)
            {
                error+= "\nException:\n" + exU.Message;
            }
            
            return true;
        }//isXML
        #endregion

        #region isWellFormedXML
        /// <summary>
        /// Checks if XML is well formed
        /// </summary>
        /// <param name="xmlFile">input XML File</param>
        /// <param name="errorString">output string for possible errors</param>
        /// <returns>true: if there where no errors and file is well formed</returns>
        public static bool isWellFormedXML(string xmlFile, out string errorString)
        {
            //ignore DTD
            errorString = "";
            bool noError = true;
            var xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
            try
            {
                using (var reader = XmlReader.Create(new System.IO.StreamReader(xmlFile), xmlReaderSettings))
                {
                    try
                    {
                        var document = new XmlDocument();
                        document.Load(reader);
                    }
                    catch (Exception exU)
                    {
                        errorString += "\nException:\n" + exU.Message;
                        noError = false;
                    }
                }
            }
            catch (XmlException exX)
            {
                errorString += "\nException:\n" + exX.Message;
                //Load failed ->not well formed
                //return false;
                noError = false;
            }
            catch (Exception exC)
            {
                errorString += "\nException:\n" + exC.Message;
                //Some other error -> maybe well formed after all
                //return false;
                noError = false;
            }
            return noError;
        }//isWellFormedXML
        #endregion 

        #region isValidXMLAgainstXSD
        /// <summary>
        /// Checks if XML is valid against XSD
        /// </summary>
        /// <param name="xmlFile">XML File</param>
        /// <param name="xsdFile">XSD File</param>
        /// <param name="errorString">output string for errors</param>
        /// <returns>true: if file is Valid</returns>
        public static bool isValidXMLAgainstXSD(string xmlFile, string xsdFile, out string errorString)
        {
            return isValidXMLAgainstXSD(xmlFile, xsdFile, null, out errorString);
        }
        
        /// <summary>
        /// Checks if XML is valid against XSD
        /// </summary>
        /// <param name="xmlFile">XML File</param>
        /// <param name="xsdFile">XSD File</param>
        /// <param name="nameSpace">NameSpace to use</param>
        /// <param name="errorString">output string for errors</param>
        /// <returns>true: if file is Valid</returns>
        public static bool isValidXMLAgainstXSD(string xmlFile, string xsdFile, String nameSpace, out string errorString)
        {
            var xDoc = XDocument.Load(xmlFile);
            var xSchema = new XmlSchemaSet();
            errorString = "";
            bool noError  = true;
            //Build schema
            try
            {
                xSchema.Add(nameSpace, xsdFile);
            }
            catch (XmlSchemaException exX)
            {
                errorString += "\nException:\n" + exX.Message;
                //return false;
                noError = false;
            }
            catch (ArgumentNullException exA)
            {
                errorString += "\nException:\n" + exA.Message;
                //return false;
                noError = false;
            }
            catch (Exception exC) //FileNotFoundException and XmlException can also occur, they are not shown as possible exception for XmlSchemaSet.Add() see https://msdn.microsoft.com/en-us/library/s74fh1h1(v=vs.110).aspx
            {
                errorString += "\nException:\n" + exC.Message;
                //return false;
                noError = false;
            }

            //Validate with schema
            try
            {
                xDoc.Validate(xSchema, null);
            }
            catch (XmlSchemaValidationException exB)
            {
                errorString += "\nException:\n" + exB.Message;
                noError = false;
                //return false;
            }
            catch (Exception exD)
            {
                errorString += "\nException:\n" + exD.Message;
                //return false;
                noError = false;
            }
            
            return noError;
        }//isValidXMLAgainstXSD
#endregion

    }//class
    #endregion
}//name space
