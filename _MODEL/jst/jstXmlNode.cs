using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JustEditXml._MODEL
{
    public class jstXmlNode
    {
        public jstXmlNode() { }

        public XmlLineType lineType { get; set; } //XmlLineType is an enum to differentiate between Openingline, ClosingLine and Selfenclosed
        public int LinePosition { get; set; }
        public string InnerText { get; set; }
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes; //C++ equivalent is std::map
        public List<jstXmlNode> Children = new List<jstXmlNode>(); //C++ equivalent is std::vector
        public jstXmlNode OpeningNodeRef { get; set; }
        public jstXmlNode ClosingNodeRef { get; set; }
        
        public string GetAttributesAsString()
        {
            string allAttributes = string.Empty;
            foreach (var attribute in this.Attributes)
            {
                allAttributes = allAttributes + $" {attribute.Key}=\"{attribute.Value}\"";
            }
            return allAttributes;
        }
    }
    
    public enum XmlLineType
    {
        OpeningNode,
        SelfEnclosedNode,
        ClosingNode
    }
    public enum XmlCharacter
    {
        startbracket, endbracket, enddash, newline, whitespace
    }

    public enum XmlParseState
    {
        None, inOpeningNodeName, inAttributes, inInnerText, inClosingNodeName,
    }
    
}
