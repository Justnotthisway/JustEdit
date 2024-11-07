using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit;
namespace JustEditXml._MODEL
{
    public class BoundXmlNode
    {
        public BoundXmlNode(XElement xElement)
        {
            Identity = System.Threading.Interlocked.Increment(ref Identity);
            XElement = xElement;
            if (!xElement.HasElements)
            {
                Raw = xElement.ToString();
            }
            else
            {
                string raw = $"<{xElement.Name}";
                foreach (var attribute in xElement.Attributes())
                {
                    raw += $" {attribute.Name}=\"{attribute.Value}\"";
                }
                raw += ">";

                Raw = raw;
            }
        }
        public static int Identity;
        string Raw;
        XElement XElement;
        UIElement UIElement;
        ICSharpCode.AvalonEdit.Document.DocumentLine AvalonEditorLine;
        public List<BoundXmlNode> Children = new List<BoundXmlNode>();
    }
}
