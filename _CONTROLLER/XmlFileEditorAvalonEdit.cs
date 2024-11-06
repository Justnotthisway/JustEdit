using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace JustEditXml._CONTROLLER
{
    internal class XmlFileEditorAvalonEdit : XmlFileEditorBase
    {
        MainWindow MainWindow { get; set; }
        List<string> XmlLines = new List<string>();
        List<StackPanel> StackPanels = new List<StackPanel>();
        public XmlFileEditorAvalonEdit(MainWindow _mainWindow)
        {
            MainWindow = _mainWindow;
        }

        private int fontSize = 12;
        private int lineHeight = 12;
        //used to left view with input boxes
        private int UI_Height = 16;
        public override void AddLine(string documentLine)
        {
            MainWindow.XmlPreviewerBox.AppendText(documentLine);
        }

        public override void ClearAllText()
        {
            MainWindow.XmlPreviewerBox.Clear();
        }

        public override void GeneratePropertyRows(string xmlFilePath, XDocument xmlDocument)
        {
            MainWindow.StackPanelContainer.Children.Clear();
            MainWindow.StackPanelContainer.Children.Add(BuildUiFromXml(xmlDocument.Root));
        }
        public UIElement BuildUiFromXml(XElement node)
        {
            var expander = new Expander()
            {
                Header = BuildHeaderRow(node,0),
                IsExpanded = true,
                Margin= new Thickness(20,0,0,0)
            };
            var stackPanel = new StackPanel();
            expander.Content = stackPanel;
            if (node.HasElements) //build an expander here
            {
                foreach (var child in node.Elements())
                {
                    stackPanel.Children.Add(BuildUiFromXml(child));
                }
            }
            else //return a stackpanel here.
            {
                stackPanel = BuildEditableRow(node,0);
                return stackPanel;
            }
            return expander;
        }
        public StackPanel BuildHeaderRow(XElement node,int index)
        {
            //var extender = new Expander();
            var rowPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 1, 0, 1),
                Height = UI_Height
            };

            var lineIndex = new TextBlock
            {
                Text = $"{index} ",
                TextAlignment = TextAlignment.Right,
                Width = 48,
                Height = UI_Height,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            string nameForNode = node
                .Elements()
                .Where(e => e.Attribute("name")?.Value == "name")
                .Select(e => e.Value)
                .FirstOrDefault();

            string hashForNode = node
                .Elements()
                .Where(e => e.Attribute("name")?.Value == "_class_hash")
                .Select(e => e.Value)
                .FirstOrDefault();

            var namedXmlLabel = new TextBlock
            {
                Text = $"    {nameForNode ?? "null"}      ",
                Width = 200,
                Height = UI_Height,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 0),
            };

            var rawXmlLabel = new TextBlock
            {
                Text =$"hash:   {hashForNode ?? "null"}",
                Width = 200,
                Height = UI_Height,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            var ButtonSwapComponent = new Button
            {
                Content = "Swap Component",
                Width = 150,
                Height = UI_Height,
                Margin = new Thickness(20, 0, 0, 0)
            };
            //rowPanel.Children.Add(lineIndex);
            rowPanel.Children.Add(namedXmlLabel);
            rowPanel.Children.Add(rawXmlLabel);
            rowPanel.Children.Add(ButtonSwapComponent);
            return rowPanel;
        }

        public StackPanel BuildEditableRow(XElement node, int index)
        {
            var rowPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 1, 0, 1),
                Height = UI_Height
            };

            string indents = string.Empty;
            
            var lineIndex = new TextBlock
            {
                Text = $"{index}: ",
                TextAlignment = TextAlignment.Right,
                Width = 48,
                Height = UI_Height,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };
            
            string labelValue = node.Attribute("name")?.Value ?? node.Attribute("id")?.Value;
            // Create label for NAME of property
            var nameLabel = new TextBlock
            {
                Text = $"prop:    {labelValue}",
                Width = 300,
                Height = UI_Height,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };
           
            // Create label for DATATYPE of property
            var typeLabel = new TextBlock
            {
                Text = $"({node.Attribute("type")?.Value})",
                Width = 70,
                Height = UI_Height,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };
            // Create a TextBox to edit property
            var inputField = new TextBox
            {
                Text = node.Value,
                Width = 300,
                Height = UI_Height,
                Margin = new Thickness(0, 0, 0, 0),
            };
           
            // Create an Apply button
            var applyButton = new Button
            {
                Content = "Apply",
                Width = 50,
                Height = UI_Height,
                Margin = new Thickness(0, 0, 0, 0)
            };
            var applyButtonBorder = new Border { CornerRadius = new CornerRadius(25.0), Child = applyButton };

            //rowPanel.Children.Add(lineIndex);
            rowPanel.Children.Add(nameLabel);
            rowPanel.Children.Add(typeLabel);
            rowPanel.Children.Add(inputField);
            rowPanel.Children.Add(applyButtonBorder);

            applyButton.Click += (s, e) =>
            {
                // Handle button click (e.g., save changes)
                MessageBox.Show($"Applied: {inputField.Text}");
            };
            return rowPanel;
        }
    }
}
