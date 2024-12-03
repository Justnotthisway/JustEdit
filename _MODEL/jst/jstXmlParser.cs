using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustEditXml._MODEL.jst
{
    internal class jstXmlParser
    {
        public jstXmlParser(string _filepath) 
        {
        FilePath = _filepath;
        }
        string FilePath;
        int LineCount = 0;
        int Processedlines = 0;
        float ProgressPercentage = 0;

        jstXmlNode ParseLine(string xmlLineText)
        {
            jstXmlNode node = new jstXmlNode();

            for (int i = 0; i < xmlLineText.Count(); i++)
            {
                switch (xmlLineText[i])
                {
                    case '<':
                        //make new Opening Node, go to Attributes next OR this is beginning of the closing tag
                        break;
                    case '>':
                        //attributes are finished here, innertext is next OR node is finished if the last char was a '/'
                        if (xmlLineText[i - 1] == '/')//this means the node is closing here
                        {

                        }
                        else //this means we have finished the attributes
                        {

                        }
                        break;
                    case '/':
                        //the node is closed here, process the next char '>' as well, then end node.
                        break;
                    case '\n':
                        //indent, signals end of node, NOT RELIABLE
                        break;
                    case '\t':
                        // indent, signals recursive depth, NOT RELIABLE
                        break;
                    default:
                        break;
                }
            }
            return node;
        }

        jstXmlNode BuildNewNode(string name, Dictionary<string,string> attributes, string innertext)
        {
            jstXmlNode node = new jstXmlNode
            {
                TagName = name,
                Attributes = attributes,
                InnerText = innertext
            };
            return node;
        }

        string BuildString(jstXmlNode node)
        {
            switch (node.lineType)
            {
                case XmlLineType.OpeningNode:
                    return $"<{node.TagName}{node.GetAttributesAsString()}>";
                case XmlLineType.ClosingNode:
                    return $"</{node.TagName}>";
                default: //case selfenclosed Node
                    return $"<{node.TagName}{node.GetAttributesAsString()}>{node.InnerText}</{node.TagName}>";
            }
        }

    }
}
