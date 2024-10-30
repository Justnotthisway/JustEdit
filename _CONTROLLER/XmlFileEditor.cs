using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Media;


namespace JustEditXml._CONTROLLER
{
    internal class XmlFileEditor
    {
        public MainWindow _MainWindow { get; set; }
        public XmlFileEditor(MainWindow mainWindow)
        {
            _MainWindow = mainWindow;
        }

        private int UI_Height = 28;

        public void OpenXml(string xmlFilePath, RichTextBox XmlPreviewerBox)
        {

            //DEBUG with Watch 
            List<XElement> xmlNodes = XDocument.Load(xmlFilePath).Descendants().ToList();

            try
            {
                // Read all text from the file
                XDocument xmlDocument = XDocument.Load(xmlFilePath);

                // Clear the RichTextBox and set its text to the file contents
                XmlPreviewerBox.Document.Blocks.Clear();


                // loop over every node in the document recursively, keep track of depth for TABs
                int depth = 0;
                foreach (var node in xmlDocument.Descendants())
                {
                    WriteXmlNode_RECURSIVE(node, depth, XmlPreviewerBox);
                }

                //UI -- generate the ui rows foreach node here
                GeneratePropertyRows(xmlFilePath, xmlDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"could not open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void WriteXmlNode_RECURSIVE(XElement node, int depth, RichTextBox XmlPreviewerBox)
        {
            var paragraph = new Paragraph
            {
                FontSize = 12,
                LineHeight = 14 // Set the desired line height
            };
            //indentation for deepness
            for (int i = 0; i < depth; i++)
            {
                paragraph.Inlines.Add("\t");
            }

            WriteXmlLineStart(paragraph, node, XmlPreviewerBox);

            if (node.HasElements) // IF WE HAVE CHILD ELEMENTS WE GO DEEPER
            {
                depth++;
                foreach (var childNode in node.Elements())
                {
                    WriteXmlNode_RECURSIVE(childNode, depth, XmlPreviewerBox);
                }
                depth--;
                //WriteXmlLineEnd(paragraph, node, XmlPreviewerBox);
                var closingParagraph = new Paragraph
                {
                    FontSize = 12,
                    LineHeight = 14 // Set the desired line height
                };
                for (int i = 0; i < depth; i++)
                {
                    closingParagraph.Inlines.Add("\t");
                }
                WriteXmlLineEnd(closingParagraph, node, XmlPreviewerBox); // Add the closing tag
            }
            else //IF WE HAVE NO CHILD ELEMENTS BE COME BACK UP.  only save innerText if has no child nodes, saves memory.
            {
                
                //depth--;
                string innerText = node.Value;
                // write down inner text in green
                Run nodeValue = new Run(innerText)
                {
                    Foreground = Brushes.Green
                };

                paragraph.Inlines.Add(nodeValue);
                
            }

            if (!node.HasElements)
            {
                WriteXmlLineEnd(paragraph, node, XmlPreviewerBox);
            }



            // Add the paragraph to the document

            
        }
        public void WriteXmlLineStart(Paragraph paragraph, XElement node, RichTextBox XmlPreviewerBox)
        {
            //iterate over attributes: name, type --- e.g.: <value name=x type=x> inner text<value/>
            Run nodeName = new Run()
            {
                Foreground = Brushes.Violet
            };

            paragraph.Inlines.Add(" <" + node.Name.ToString());

            Run attributeName;
            Run attributeValue;
            foreach (var attribute in node.Attributes())
            {
                // write down name in red
                if (attribute.Name == "name" || attribute.Name == "id")
                {
                    attributeName = new Run($" {attribute.Name}=")
                    {
                        Foreground = Brushes.Black // Red for name
                    };

                    attributeValue = new Run($"\"{attribute.Value}\"")
                    {
                        Foreground = Brushes.Red // Red for name
                    };
                }

                // write down type in blue
                else if (attribute.Name == "type")
                {
                    attributeName = new Run($" {attribute.Name}=")
                    {
                        Foreground = Brushes.Black // Red for name
                    };

                    attributeValue = new Run($"\"{attribute.Value}\"")
                    {
                        Foreground = Brushes.Blue // Red for name
                    };
                }

                // Default color for other attributes (if needed)
                else
                {
                    attributeName = new Run($" {attribute.Name}=")
                    {
                        Foreground = Brushes.Black // Red for name
                    };

                    attributeValue = new Run($"\"{attribute.Value}\"")
                    {
                        Foreground = Brushes.Orange // Red for name
                    };
                }

                //paragraph.Inlines.Add($"<{node.Name} {attributeRun}>{innerText}</{node.Name}>\n");
                paragraph.Inlines.Add(attributeName);
                paragraph.Inlines.Add(attributeValue);
            }

            paragraph.Inlines.Add(">");
            if (node.HasElements)
            {
                XmlPreviewerBox.Document.Blocks.Add(paragraph);
            }
        }
        public void WriteXmlLineEnd(Paragraph paragraph, XElement node, RichTextBox XmlPreviewerBox)
        {
            

            paragraph.Inlines.Add("</");
            paragraph.Inlines.Add(node.Name.ToString());
            paragraph.Inlines.Add(">");


            XmlPreviewerBox.Document.Blocks.Add(paragraph);
        }
        //UI Stuff --- generate Rows with: label-value | label-type | input | apply-button
        public void GeneratePropertyRows(string xmlFilePath, XDocument xmlDocument)
        {



            foreach (var node in xmlDocument.Descendants())
            {
                // Create a horizontal stack panel for each row
                var rowPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 1, 0, 1),
                    Height = UI_Height
                };

                if (node.Name != "value")
                {
                    var nameLabel = new Label
                    {
                        Content = node.ToString(),
                        Width = 1024,
                        Height = UI_Height
                    };
                    rowPanel.Children.Add(nameLabel);
                }
                //create labeled row only if its a value element
                if (node.Name == "value")
                {
                    string labelValue = node.Attribute("name")?.Value ?? node.Attribute("id")?.Value;
                    // Create label for NAME of property
                    var nameLabel = new Label
                    {
                        Content = $"property:    {labelValue}",
                        Width = 256,
                        Height = UI_Height
                    };
                    rowPanel.Children.Add(nameLabel);

                    // Create label for DATATYPE of property
                    var typeLabel = new Label
                    {
                        Content = $"({node.Attribute("type")?.Value})",
                        Width = 128,
                        Height = UI_Height
                    };
                    rowPanel.Children.Add(typeLabel);

                    // Create a TextBox to edit property
                    var inputField = new TextBox { Text = node.Value, Width = 300, Height = UI_Height };
                    rowPanel.Children.Add(inputField);

                    // Create an Apply button
                    var applyButton = new Button { Content = "Apply", Width = 50, Height = UI_Height, };
                    var applyButtonBorder = new Border { CornerRadius = new CornerRadius(25.0), Child = applyButton };
                    rowPanel.Children.Add(applyButtonBorder);

                    applyButton.Click += (s, e) =>
                    {
                        // Handle button click (e.g., save changes)
                        MessageBox.Show($"Applied: {inputField.Text}");
                    };
                }





                // Add the row to the StackPanel in the window
                _MainWindow.StackPanelContainer.Children.Add(rowPanel);
            }
        }
        public void ValdiateProperty(string propertyName, object value)
        {

        }
        public void SaveXml()
        {

        }
    }
}
