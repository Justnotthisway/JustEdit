using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit;
using JustEditXml._CONTROLLER;
using JustEditXml.Contoller;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;

namespace JustEditXml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[] args = Environment.GetCommandLineArgs();
        public FoldingManager FoldingManager;
        private XmlFoldingStrategy foldingStrategy;
        private XmlFileEditorAvalonEdit _XmlFileEditor;
        public MainWindow()
        {
            InitializeComponent();
            InitAvalonEdit();
            if (true) // check if commandline args are a filepath
            {
                _XmlFileEditor = new XmlFileEditorAvalonEdit(this);
                //_XmlFileEditor.OpenXml(args[0]);
            }
        }
        private void InitAvalonEdit()
        {
            //XmlPreviewerBox.Width = 1920;
            XmlPreviewerBox.FontSize = 17;
            FoldingManager = FoldingManager.Install(XmlPreviewerBox.TextArea);
            foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(FoldingManager, XmlPreviewerBox.Document);

            // Set a minimum width for the folding margin (make sure it's wide enough)
            //XmlPreviewerBox.TextArea.LeftMargins[0].Width = 16;  // Adjust width if necessary

            XmlPreviewerBox.TextChanged += (sender, e) =>
            {
                foldingStrategy.UpdateFoldings(FoldingManager, XmlPreviewerBox.Document);
            };
        }
        //--- BUTTON Load an ee file into the Editor as XML ---
        private void Button_LoadFile(object sender, RoutedEventArgs e)
        {
            GameFileSelector gameFileSelector = new GameFileSelector();
            //GameFileConverter gameFileConverter = new GameFileConverter(filePath);
            _XmlFileEditor = new XmlFileEditorAvalonEdit(this);
            //XmlFileEditorWPF xmlFileEditor = new XmlFileEditorWPF(this);

            //get the user to select a file from the Windows fileselect Dialog, count the total lines of text.
            string filePath = gameFileSelector.SelectGameFile();
            int lineCount = File.ReadLines(filePath).Count();
            // reset laoding bar Progress to 0;
            this.ProgressBar.Value = 0;


            //display loading in UI. Dispatcher.Invoke will force a UI update.
            Dispatcher.Invoke(() =>
            {
                currentFileLabel.Text = $"Loading... {Path.GetFileName(filePath)}";
                currentFileLineCount.Text = $"Lines: {lineCount}";
            });

            //put the textcontent of the xml inside the Xml Preview Box (skip unpack for now), but delete old text first
            _XmlFileEditor.ClearAllText();
            _XmlFileEditor.OpenXml(filePath);

            currentFileLabel.Text = $"Loaded: {Path.GetFileName(filePath)}";
            
            // ---- (WPF stuff) ----
            // set the vertical size to something big so we dont get linebreaks 
            //XmlPreviewerBox.Document.PageWidth = 19200.00;

            // ---- (AvalonEdit Stuff) ----
        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource == ScrollViewerMiddle)
            {
                // Synchronize AvalonEdit's vertical scroll position with ScrollViewer's scroll position
                if (e.VerticalChange > 0)
                {
                    XmlPreviewerBox.LineDown();
                    XmlPreviewerBox.LineDown();
                    //XmlPreviewerBox.ScrollToVerticalOffset(XmlPreviewerBox.VerticalOffset + e.VerticalChange);
                }
                if (e.VerticalChange < 0)
                {
                    XmlPreviewerBox.LineUp();
                    XmlPreviewerBox.LineUp();
                    //XmlPreviewerBox.ScrollToVerticalOffset(XmlPreviewerBox.VerticalOffset + e.VerticalChange);
                }
            }

        }
        private void MyScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Prevent default scrolling behavior
            e.Handled = true;

            // Get the ScrollViewer
            var scrollViewer = sender as ScrollViewer;

            if (e.Delta > 0) // Scrolling up
            {
                // Scroll up exactly TWO AvalonEdit Lines
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - XmlPreviewerBox.TextArea.TextView.DefaultLineHeight * 2);
            }
            else if (e.Delta < 0) // Scrolling down
            {
                // Scroll down exactly TWO AvalonEdit Lines
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + XmlPreviewerBox.TextArea.TextView.DefaultLineHeight * 2);
            }
        }

        private void Button_SaveFile(object sender, RoutedEventArgs e)
        {
            _XmlFileEditor.SaveXml();
        }
    }
}
