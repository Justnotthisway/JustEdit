using JustEditXml._MODEL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace JustEditXml._CONTROLLER
{
    internal abstract class XmlFileEditorBase
    {
        public XmlFileEditorBase()
        {

        }
        protected XDocument _document;
        protected string _FilePath;
        public BoundXmlNode RootNode { get; set; }

        //implement abstract methods in Child class with choosen UI Element
        abstract public void AddLine(string documentLine);
        abstract public void ClearAllText();
        abstract public void GeneratePropertyRows(string xmlFilePath, XDocument xmlDocument);

        //entry point for the class, trigger it with Button etc. to load a complete formatted xml document in desired UI element.
        public void OpenXml(string xmlFilePath)
        {
            _FilePath = xmlFilePath;
            try
            {
                //DEBUG with Watch  
                //List<XElement> xmlNodes = XDocument.Load(xmlFilePath).Descendants().ToList();

                XmlReaderSettings settings = new XmlReaderSettings
                {
                    IgnoreWhitespace = true // This will ignore whitespace and &#xA
                };

                // Create an XmlReader with the settings
                using (XmlReader reader = XmlReader.Create(xmlFilePath, settings))
                {
                    // Load the XDocument using the XmlReader
                    _document = XDocument.Load(reader);
                }

                // Clear the RichTextBox and set its text to the file contents
                this.ClearAllText();


                // loop over every node in the document recursively, keep track of depth for TABs
                int depth = 0;
                //foreach (var node in xmlDocument.Descendants())
                //{
                BuildXmlNode(_document.Root, depth);
                //}

                //UI -- generate the ui rows foreach node here
                GeneratePropertyRows(xmlFilePath, _document);

            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show($"could not open file, not an xml file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // IXmlPreviewerElement provides abstraction layer to disconnect logic from UI Framework. implement Interface Methods for choosen UI framework
        //BuildXmlNode builds a string that can be passed to the UI.
        public void BuildXmlNode(XElement node, int depth)
        {
            string documentLine = String.Empty;

            //indentation for deepness
            for (int i = 0; i < depth; i++)
            {
                documentLine += "\t";
            }

            documentLine = BuildXmlLineStart(documentLine, node);

            if (node.HasElements) // IF WE HAVE CHILD ELEMENTS WE GO DEEPER
            {
                this.AddLine(documentLine); //we add a <node type="opening"> here. only of it has elements will it be its own line.
                depth++;
                foreach (var childNode in node.Elements())
                {
                    BuildXmlNode(childNode, depth);   //RECURSIVE
                }
                depth--;

                string closingDocumentLine = String.Empty;
                for (int i = 0; i < depth; i++)     //add indentation for deepness
                {
                    closingDocumentLine += "\t";
                }

                closingDocumentLine = BuildXmlLineEnd(closingDocumentLine, node);
                this.AddLine(closingDocumentLine); //we add </closingnode> here
            }
            else //IF WE HAVE NO CHILD ELEMENTS. only save innerText if has no child nodes, saves memory.
            {
                string nodeValue = node.Value;
                documentLine += nodeValue;
                documentLine = BuildXmlLineEnd(documentLine, node);
                this.AddLine(documentLine); //we add </closingnode> here
            }
        }
        public string BuildXmlLineStart(string documentLine, XElement node)
        {
            //iterate over attributes: name, type --- e.g.: <value name=x type=x> inner text<value/>
            //Run nodeName = new Run()
            //{
            //    Foreground = Brushes.Violet
            //};

            documentLine += " <" + node.Name.ToString();

            string attributeName;
            string attributeValue;
            foreach (var attribute in node.Attributes())
            {
                // write down name in red
                if (attribute.Name == "name" || attribute.Name == "id")
                {
                    attributeName = $" {attribute.Name}=";
                    //{
                    //    Foreground = Brushes.Black // Red for name
                    //};

                    attributeValue = $"\"{attribute.Value}\"";
                    //{
                    //    Foreground = Brushes.Red // Red for name
                    //};
                }

                // write down type in blue
                else if (attribute.Name == "type")
                {
                    attributeName = $" {attribute.Name}=";
                    //{
                    //    Foreground = Brushes.Black // Red for name
                    //};

                    attributeValue = $"\"{attribute.Value}\"";
                    //{
                    //    Foreground = Brushes.Blue // Red for name
                    //};
                }

                // Default color for other attributes (if needed)
                else
                {
                    attributeName = $" {attribute.Name}=";
                    //{
                    //    Foreground = Brushes.Black // Red for name
                    //};

                    attributeValue = $"\"{attribute.Value}\"";
                    //{
                    //    Foreground = Brushes.Orange // Red for name
                    //};
                }

                //paragraph.Inlines.Add($"<{node.Name} {attributeRun}>{innerText}</{node.Name}>\n");
                documentLine += attributeName;
                documentLine += attributeValue;
            }

            documentLine += ">";
            if (node.HasElements)
            {
                return documentLine+="\n";
            }
            else
            {
                return documentLine;
            }
        }
        public string BuildXmlLineEnd(string closingDocumentLine, XElement node)
        {
            closingDocumentLine += "</";
            closingDocumentLine += node.Name.ToString();
            closingDocumentLine += ">";
            closingDocumentLine += "\n";

            return closingDocumentLine;
        }
        //UI Stuff --- generate Rows with: label-value | label-type | input | apply-button

        public int GetNodeDepth(XElement node)
        {
            int depth = 0;
            while (node.Parent != null)
            {
                depth++;
                node = node.Parent;
            }
            return depth;
        }

        public void ValdiateProperty(string propertyName, object value)
        {

        }
        public void SaveXml()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.Title = "Save an XML File";
                saveFileDialog.DefaultExt = "xml"; // Default file extension
                saveFileDialog.AddExtension = true; // Automatically add extension
                saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_FilePath)+"_mod";

                // Show the dialog and get result
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Save the XDocument to the selected path
                    _document.Save(saveFileDialog.FileName);
                    System.Windows.Forms.MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

    }

}
