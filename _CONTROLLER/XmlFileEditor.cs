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

                // loop over every node in the document
                foreach (var node in xmlDocument.Descendants())
                {

                    //one paragraph per node or line
                    var paragraph = new Paragraph
                    {
                        FontSize = 12,
                        LineHeight = 14 // Set the desired line height
                    };

                    //get innter text for node
                    string innerText = node.Value;

                    //iterate over attributes: name, type --- e.g.: <value name=x type=x> inner text<value/>

                    Run nodeNameRun = new Run()
                    {
                        Foreground = Brushes.Violet
                    };

                    paragraph.Inlines.Add(" <" + node.Name.ToString());

                    Run attributeRun;
                    Run attributeValueRun;
                    foreach (var attribute in node.Attributes())
                    {
                        // write down name in red
                        if (attribute.Name == "name" || attribute.Name == "id")
                        {
                            attributeRun = new Run($" {attribute.Name}=")
                            {
                                Foreground = Brushes.Black // Red for name
                            };

                            attributeValueRun = new Run($"\"{attribute.Value}\"")
                            {
                                Foreground = Brushes.Red // Red for name
                            };
                        }

                        // write down type in blue
                        else if (attribute.Name == "type")
                        {
                            attributeRun = new Run($" {attribute.Name}=")
                            {
                                Foreground = Brushes.Black // Red for name
                            };

                            attributeValueRun = new Run($"\"{attribute.Value}\"")
                            {
                                Foreground = Brushes.Blue // Red for name
                            };
                        }

                        // Default color for other attributes (if needed)
                        else
                        {
                            attributeRun = new Run($" {attribute.Name}=")
                            {
                                Foreground = Brushes.Black // Red for name
                            };

                            attributeValueRun = new Run($"\"{attribute.Value}\"")
                            {
                                Foreground = Brushes.Orange // Red for name
                            };
                        }

                        //paragraph.Inlines.Add($"<{node.Name} {attributeRun}>{innerText}</{node.Name}>\n");
                        paragraph.Inlines.Add(attributeRun);
                        paragraph.Inlines.Add(attributeValueRun);
                    }
                    // write down inner text in green
                    Run valueRun = new Run(innerText)
                    {
                        Foreground = Brushes.Green
                    };

                    paragraph.Inlines.Add(">");

                    if (!node.HasElements)
                    {
                        paragraph.Inlines.Add(valueRun);
                    }

                    paragraph.Inlines.Add("</");
                    paragraph.Inlines.Add(node.Name.ToString());
                    paragraph.Inlines.Add(">");



                    // Add the paragraph to the document
                    XmlPreviewerBox.Document.Blocks.Add(paragraph);


                    //OTHER OPTION, MAKE IT SINGLE EDITABLE LINES NOT TEXTBOX
                    //var rowPanel = new StackPanel
                    //{
                    //    Orientation = Orientation.Horizontal,
                    //    Margin = new Thickness(0, 0, 0, 0),
                    //    Height = UI_Height
                    //};

                    //var inputField = new TextBox
                    //{
                    //    Text = node.ToString(),
                    //    Width = 300,
                    //    Height = UI_Height
                    //};
                    //rowPanel.Children.Add(inputField);
                }







                //UI -- generate the ui rows foreach node here
                GeneratePropertyRows(xmlFilePath, xmlDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"could not open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
