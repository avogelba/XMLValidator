using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using XMLValidator.Validator;
/// <summary>
/// XMLValidator Name space
/// </summary>
namespace XMLValidator
{
    #region CLASS
    /// <summary>
    /// Main Program
    /// </summary>
    class Program
    {
#region DEFINIONS
        //global used exit codes
        const int ERROR = 1;
        const int OK = 0;
#endregion
        #region MAIN
        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">command line parameters</param>
        /// <returns>0 if success, 1 if error occurred</returns>
        static int Main(string[] args)
        {
            string xmlFile = null;
            string xsdFile = null;
            string errors = "";
            bool status;

            #region CULTURE
            //Force US-English to fit Article
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            #endregion

            #region ARGS_HANDLING
            //args handling not done yet
            if (args.Length == 2)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i].EndsWith(".xsd"))
                    {
                        xsdFile = args[i];
                    }
                    else if (args[i].EndsWith(".xml")){
                        xmlFile = args[i];
                    }
                    else
                    {
                        printHelp(false);
                        Console.WriteLine($"ERROR: Wrong parameters entered.");
                        return ERROR;
                    }
                }
                if(string.IsNullOrEmpty(xmlFile) || string.IsNullOrEmpty(xsdFile))
                {
                    printHelp(false);
                    Console.WriteLine($"ERROR: Wrong parameters entered.");
                    return ERROR;
                }
            }
            else
            {
                printHelp();
                Console.WriteLine($"ERROR: Wrong parameters entered.");
                return ERROR;
            }
            #endregion

            #region FILECHECK
            //do the files exists?

            var oXMLFileName = xmlFile; //save name as entered by user for error message
            //add path if necessary
            if (File.Exists(Directory.GetCurrentDirectory() + @"\" + xmlFile) || !File.Exists(xmlFile))
            {
                xmlFile = Directory.GetCurrentDirectory() + @"\" + xmlFile;
            }

            var oXSDFileName = xsdFile; //save name as entered by user for error message
            //add path if necessary
            if (File.Exists(Directory.GetCurrentDirectory() + @"\" + xsdFile) || !File.Exists(xsdFile))
            {
                xsdFile = Directory.GetCurrentDirectory() + @"\" + xsdFile;
            }

            if (!File.Exists(xmlFile))
            {
                Console.WriteLine($"ERROR: File \"{oXMLFileName}\" does not exist");
                return ERROR;
            }

            if (!File.Exists(xsdFile))
            {

                Console.WriteLine($"ERROR: File \"{oXSDFileName}\" does not exist");
                return ERROR;
            }
            #endregion

            #region CONTENTCHECK
            //Is it really a XSD or XML, or maybe some text or binary file?
            status = XMLValidator.Validator.Validator.isXML(xmlFile, out errors);
            if (!status)
            {
                Console.WriteLine($"ERROR: given XML is not a XML: {errors}");
                return ERROR;
            }

            status = XMLValidator.Validator.Validator.isXML(xsdFile, out errors);
            if (!status)
            {
                Console.WriteLine($"ERROR: given XSD is not a XML: {errors}");
                return ERROR;
            }
            #endregion

            #region VALIDATION
            //finally check if they are valid
            status = XMLValidator.Validator.Validator.isWellFormedXML(xmlFile, out errors);
            if (!status)
            {
                Console.WriteLine($"ERROR: XML not well formed: {errors}");
                return ERROR;
            }

            status = XMLValidator.Validator.Validator.isWellFormedXML(xsdFile, out errors);
            if (!status)
            {
                Console.WriteLine($"ERROR: XSD not well formed: {errors}");
                return ERROR;
            }

            status = XMLValidator.Validator.Validator.isValidXMLAgainstXSD(xmlFile, xsdFile, out errors);
            if (!status)
            {
                Console.WriteLine($"ERROR: XML is not Valid against schema: {errors}");
                return ERROR;
            }
            #endregion

            return OK;
        }//main
        #endregion

        #region HELP
        /// <summary>
        /// Help Screen
        /// </summary>
        /// <param name="showHelp">show usage help or not</param>
        public static void printHelp(bool showHelp = true)
        {
            var myType = typeof(Program);
            var myName = myType.Namespace;
            var myVersion = Assembly.GetExecutingAssembly().GetName().Version;

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            Console.WriteLine($"{myName} - Version {myVersion}");
            if (showHelp)
            {
                Console.WriteLine($"Usage : {AppDomain.CurrentDomain.FriendlyName} <input.xml> <input.xsd>");
               
            }
        }//printHelp
        #endregion
    }//class
    #endregion
}//name space
