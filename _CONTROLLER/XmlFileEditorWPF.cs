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
    internal class XmlFileEditorWPF : XmlFileEditorBase
    {
        MainWindow MainWindow { get; set; }
        List<string> DEBUG_Lines = new List<string>();
        public XmlFileEditorWPF(MainWindow _mainWindow)
        {
            MainWindow = _mainWindow;
        }

        private int fontSize = 12;
        private int lineHeight = 28;
        //used to left view with input boxes
        private int UI_Height = 28;

        public override void AddLine(string documentLine)
        {
            MainWindow.XmlPreviewerBox.AppendText(documentLine);
            DEBUG_Lines.Add(documentLine);
        }

        public override void ClearAllText()
        {
            //MainWindow.XmlPreviewerBox.Document.Blocks.Clear();
        }

        public override void GeneratePropertyRows(string xmlFilePath, XDocument xmlDocument)
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
                MainWindow.StackPanelContainer.Children.Add(rowPanel);
            }
        }
    }
}
