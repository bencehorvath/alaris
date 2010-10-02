using System;
using System.Xml;
using Alaris.API;

namespace Alaris.Config
{
    /// <summary>
    ///   XML settings class, used for config files.
    /// </summary>
    public sealed class XmlSettings : IAlarisComponent
    {
        private readonly XmlDocument _xmlDocument = new XmlDocument();
        private readonly string _documentPath = string.Empty;
        private readonly string _rootNode = "";
        private readonly Guid _guid;

        ///<summary>
        ///  Create a new instance of XmlSettings.
        ///</summary>
        ///<param name = "document">XML document to load.</param>
        ///<param name = "root">The xml document's root node.</param>
        public XmlSettings(string document, string root)
        {

            _guid = Guid.NewGuid();

            try
            {
                _xmlDocument.Load(document);
                _documentPath = document;
                _rootNode = root;
            }
            catch
            {
                _xmlDocument.LoadXml("<settings></settings>");
            }
        }

        /// <summary>
        ///   Gets a setting from the xml file.
        /// </summary>
        /// <param name = "xPath">The xpath code.</param>
        /// <param name = "defaultValue">default value to return.</param>
        /// <returns></returns>
        public int GetSetting(string xPath, int defaultValue)
        {
            return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue)));
        }


        /// <summary>
        ///   Creates or edits a setting.
        /// </summary>
        /// <param name = "xPath">Xpath formula.</param>
        /// <param name = "value">value to set</param>
        public void PutSetting(string xPath, int value)
        {
            PutSetting(xPath, Convert.ToString(value));
        }

        /// <summary>
        ///   Gets a setting from the xml file.
        /// </summary>
        /// <param name = "xPath">The xpath code.</param>
        /// <param name = "defaultValue">default value to return.</param>
        /// <returns></returns>
        public string GetSetting(string xPath, string defaultValue)
        {
            var xmlNode = _xmlDocument.SelectSingleNode(_rootNode + "/" + xPath);
            
            return xmlNode != null ? xmlNode.InnerText : defaultValue;
        }

        /// <summary>
        ///   Creates or edits a setting.
        /// </summary>
        /// <param name = "xPath">Xpath formula.</param>
        /// <param name = "value">value to set</param>
        public void PutSetting(string xPath, string value)
        {
            var xmlNode = _xmlDocument.SelectSingleNode(_rootNode + "/" + xPath) ??
                          CreateMissingNode(_rootNode + "/" + xPath);
            xmlNode.InnerText = value;
            _xmlDocument.Save(_documentPath);
        }


        private XmlNode CreateMissingNode(string xPath)
        {
            var xPathSections = xPath.Split('/');
            var currentXPath = "";
            var currentNode = _xmlDocument.SelectSingleNode(_rootNode);
            foreach (var xPathSection in xPathSections)
            {
                currentXPath += xPathSection;
                var testNode = _xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null)
                {
                    currentNode.InnerXml += "<" +
                                            xPathSection + "></" +
                                            xPathSection + ">";
                }
                currentNode = _xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }

        public Guid GetGuid()
        {
            return _guid;
        }
    }
}