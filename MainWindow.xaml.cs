using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit;
using JustEditXml._CONTROLLER;
using JustEditXml.Contoller;
using System;
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
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace JustEditXml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FoldingManager foldingManager;
        private XmlFoldingStrategy foldingStrategy;
        public MainWindow()
        {
            InitializeComponent();
            InitAvalonEdit();
        }
        private void InitAvalonEdit()
        {
            foldingManager = FoldingManager.Install(XmlPreviewerBox.TextArea);
            foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, XmlPreviewerBox.Document);

            // Set a minimum width for the folding margin (make sure it's wide enough)
            //XmlPreviewerBox.TextArea.LeftMargins[0].Width = 16;  // Adjust width if necessary

            XmlPreviewerBox.TextChanged += (sender, e) =>
            {
                foldingStrategy.UpdateFoldings(foldingManager, XmlPreviewerBox.Document);
            };
        }
        //--- BUTTON Load an ee file into the Editor as XML ---
        private void Button_LoadFile(object sender, RoutedEventArgs e)
        {
            GameFileSelector gameFileSelector = new GameFileSelector();
            GameFileConverter gameFileConverter = new GameFileConverter();
            XmlFileEditorAvalonEdit xmlFileEditor = new XmlFileEditorAvalonEdit(this);
            //XmlFileEditorWPF xmlFileEditor = new XmlFileEditorWPF(this);

            //get the user to select a file from the Windows fileselect Dialog
            string filePath = gameFileSelector.SelectGameFile();

            //put the textcontent of the xml inside the Xml Preview Box (skip unpack for now), but delete old text first
            xmlFileEditor.ClearAllText();
            xmlFileEditor.OpenXml(filePath);

            // ---- (WPF stuff) ----
            // set the vertical size to something big so we dont get linebreaks 
            //XmlPreviewerBox.Document.PageWidth = 19200.00;

            // ---- (AvalonEdit Stuff) ----
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
