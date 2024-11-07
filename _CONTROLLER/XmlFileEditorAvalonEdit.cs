using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit.Document;
using System;
using ICSharpCode.AvalonEdit.Folding;
using JustEditXml._MODEL;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using System.Windows.Ink;


namespace JustEditXml._CONTROLLER
{
    internal class XmlFileEditorAvalonEdit : XmlFileEditorBase
    {
        MainWindow MainWindow { get; set; }
        object[,] XLookupTable { get; set; }
        public XmlFileEditorAvalonEdit(MainWindow _mainWindow)
        {
            MainWindow = _mainWindow;
            //UI_Height = 18;
            RowHeight = this.GetAvalonLineHeightInPixels();
        }

        private int fontSize = 14;
        private int lineHeight = 12;
        public double RowHeight { get; set; }
        public double RowHeightCorrectionHeader = -4;
        public double RowHeightCorrectionEditable = 0;
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
            int line = 0;
            MainWindow.StackPanelPropertyContainer.Children.Clear();
            MainWindow.StackPanelPropertyContainer.Children.Add(BuildUiFromXml(xmlDocument.Root, ref line));
        }
        public UIElement BuildUiFromXml(XElement node, ref int lineIndex, int expanderDepth = 0)
        {
            expanderDepth++;
            lineIndex++;

            IdStackPanel header = BuildHeaderRow(node, lineIndex);
            var expander = new Expander()
            {
                Header = header,
                IsExpanded = true,
            };

            var stackPanel = new IdStackPanel();
            stackPanel.PanelId = lineIndex;
            expander.Content = stackPanel;
            expander.BorderThickness = new Thickness(2);
            expander.Margin = new Thickness(25, 0, 0, 0);
            expander.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d3d4d5"));
            expander.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#adaeae"));
            //expander.Style = (Style)MainWindow.FindResource("RebelDropStyle");
            expander.Collapsed += (s, e) =>
            {
                Expander collapsedExpander = s as Expander;
                if (collapsedExpander != null && e.OriginalSource == collapsedExpander)
                {
                    // Get the StackPanel from the Expander's content
                    IdStackPanel panel = collapsedExpander.Content as IdStackPanel;
                    if (panel != null)
                    {
                        CollapseNodeAtLine(panel.PanelId, MainWindow.FoldingManager, true);
                    }
                }
            };

            expander.Expanded += (s, e) =>
            {
                Expander expandedExpander = s as Expander;
                if (expandedExpander != null && e.OriginalSource == expandedExpander)
                {
                    // Get the StackPanel from the Expander's content
                    IdStackPanel panel = expandedExpander.Content as IdStackPanel;
                    if (panel != null)
                    {
                        CollapseNodeAtLine(panel.PanelId, MainWindow.FoldingManager, false);
                    }
                }
            };

            if (node.HasElements) //build an expander here
            {
                foreach (var child in node.Elements())
                {
                    stackPanel.Children.Add(BuildUiFromXml(child, ref lineIndex));
                }
                stackPanel.Children.Add(BuildEmptyRow(node, lineIndex));
                lineIndex++;
            }
            else //return a stackpanel here.
            {
                stackPanel = BuildEditableRow(node, lineIndex);
                //IdStackPanels.Add(stackPanel);
                return stackPanel;
            }
            return expander;
        }
        public IdStackPanel BuildHeaderRow(XElement node, int rowIndex)
        {
            //var extender = new Expander();
            var rowPanel = new IdStackPanel()
            {
                PanelId = rowIndex,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(2, 0, 2, 0),
                Height = RowHeight + RowHeightCorrectionHeader,
                Width = 1920
            };
            rowPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffb907"));
            var lineIndex = new TextBlock
            {
                Text = $"{rowIndex} ",
                TextAlignment = TextAlignment.Right,
                Width = 48,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };
            string nameForNode = String.Empty;
            string hashForNode = String.Empty;

            bool NodehasNameAttribute = node.Attributes().Any(attributes => attributes.Name.ToString() == "name");
            bool hasChildNodeWithAttributeName = node.Elements().Any(attribute => attribute.Name.ToString() != "name");
            bool AttributeNameIsRoot = node.Elements()
                    .Where(e => e.Attribute("name")?.Value == "name")
                    .Select(e => e.Value.ToString())
                    .FirstOrDefault() == "root";
            if (NodehasNameAttribute)
            {
                nameForNode = node.Attribute("name").Value;
                foreach (var attribute in node.Attributes())
                {
                    hashForNode += $" {attribute.Name}=\"{attribute.Value}\"";
                }
            }
            else if (hasChildNodeWithAttributeName && !AttributeNameIsRoot)
            {
                nameForNode = node
                    .Elements()
                    .Where(e => e.Attribute("name")?.Value == "name")
                    .Select(e => e.Value.ToString())
                    .FirstOrDefault();

                hashForNode = node
                    .Elements()
                    .Where(e => e.Attribute("name")?.Value == "_class_hash")
                    .Select(e => e.Value.ToString())
                    .FirstOrDefault();
            }
            else
            {
                nameForNode = node.Name.ToString();

                foreach (var attribute in node.Attributes())
                {
                    hashForNode += $" {attribute.Name}=\"{attribute.Value}\"";
                }
            }


            var namedXmlLabel = new TextBlock
            {
                Text = $"    {nameForNode ?? ""}      ",
                Width = 240,
                Height = RowHeight - 2,
                FontWeight = FontWeights.Bold,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            var rawXmlLabel = new TextBlock
            {
                Text = $"{hashForNode ?? ""}",
                Width = 160,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            var ButtonSwapComponent = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 0, 0),
                BorderThickness = new Thickness(0),
                BorderBrush = null,
                FontSize = fontSize,
                Content = "Swap Component",
                Width = 150,
                Height = RowHeight - 2,
                Margin = new Thickness(5, 0, 5, 0)
            };
            rowPanel.Children.Add(lineIndex);
            rowPanel.Children.Add(namedXmlLabel);
            rowPanel.Children.Add(rawXmlLabel);
            rowPanel.Children.Add(ButtonSwapComponent);
            return rowPanel;
        }

        public IdStackPanel BuildEditableRow(XElement node, int rowIndex)
        {
            var rowPanel = new IdStackPanel
            {
                PanelId = rowIndex,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 0),
                Height = RowHeight + RowHeightCorrectionEditable
            };

            string indents = string.Empty;

            var lineIndex = new TextBlock
            {
                Text = $"{rowIndex}: ",
                TextAlignment = TextAlignment.Right,
                Width = 48,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            string labelValue = node.Attribute("name")?.Value ?? node.Attribute("id")?.Value;
            // Create label for NAME of property
            var nameLabel = new TextBlock
            {
                Text = $"prop:    {labelValue}",
                Width = 200,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            // Create label for DATATYPE of property
            var typeLabel = new TextBlock
            {
                Text = $"({node.Attribute("type")?.Value})",
                Width = 70,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };
            // Create a TextBox to edit property
            var inputField = new TextBox
            {
                AcceptsReturn = false, // Ensure it does not accept multiple lines
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Text = node.Value,
                Width = 300,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };

            // Create an Apply button
            var applyButton = new Button
            {
                Content = "Apply",
                Width = 50,
                Height = RowHeight - 3,
                //Margin = new Thickness(0, 0, 0, 0)

                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0, 0, 0, 0),
                BorderThickness = new Thickness(0),
                BorderBrush = null,
                FontSize = 12,
                Margin = new Thickness(5, 0, 5, 0)
            };

            rowPanel.Children.Add(lineIndex);
            rowPanel.Children.Add(nameLabel);
            rowPanel.Children.Add(typeLabel);
            rowPanel.Children.Add(inputField);
            rowPanel.Children.Add(applyButton);

            applyButton.Click += (s, e) =>
            {
                // Handle button click (save changes, change color of row to indicate change)
                node.Value = inputField.Text;
                rowPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#178600"));
                int d = 0;
                //TODO change to a more performant implementation that only updates changes rows instead of all
                ClearAllText();
                BuildXmlNode(_document.Root, d);
            };
            return rowPanel;
        }

        public IdStackPanel BuildEmptyRow(XElement node, int rowIndex)
        {
            var rowPanel = new IdStackPanel
            {
                PanelId = rowIndex,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(26, 0, 0, 0),
                Height = RowHeight + RowHeightCorrectionEditable
            };

            var lineIndex = new TextBlock
            {
                Text = $"{rowIndex}: <  />",
                TextAlignment = TextAlignment.Right,
                Height = RowHeight - 2,
                FontSize = fontSize,
                Margin = new Thickness(0, 0, 0, 0),
            };
            rowPanel.Children.Add(lineIndex);
            return rowPanel;
        }
        public void UpdateXLookupTable()
        {
            XLookupTable = new object[2, MainWindow.XmlPreviewerBox.Document.Lines.Count];
            for (int i = 0; i < XLookupTable.GetLength(1); i++)
            {
                XLookupTable[2, i] = MainWindow.XmlPreviewerBox.Document.Lines[i];
            }
        }
        public void CollapseNodeAtLine(int lineIndex, FoldingManager foldingManager, bool doCollapseFlg)
        {
            foreach (FoldingSection fold in foldingManager.AllFoldings)
            {

                TextLocation foldLocation = MainWindow.XmlPreviewerBox.Document.GetLocation(fold.StartOffset);
                TextLocation lineLocation = MainWindow.XmlPreviewerBox.Document.GetLocation(MainWindow.XmlPreviewerBox.Document.GetLineByNumber(lineIndex).Offset);
                if (foldLocation.Line == lineLocation.Line && foldLocation.Line != 0)
                {
                    fold.IsFolded = doCollapseFlg; // collapses AND expandes, depends on BOOL

                    break;
                }
            }
        }
        private double GetAvalonLineHeightInPixels()
        {
            // Accessing the TextView from the TextEditor
            var textView = MainWindow.XmlPreviewerBox.TextArea.TextView;

            // Calculate the height of a single line
            double defaultLineHeight = textView.DefaultLineHeight; // Default line height based on font

            // Total height of a single line
            double singleLineHeight = defaultLineHeight;

            return singleLineHeight;
        }
    }
}
