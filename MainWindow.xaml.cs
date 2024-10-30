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

namespace JustEditXml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //--- BUTTON Load an ee file into the Editor as XML ---
        private void Button_LoadFile(object sender, RoutedEventArgs e)
        {
            GameFileSelector gameFileSelector = new GameFileSelector();
            GameFileConverter gameFileConverter = new GameFileConverter();
            XmlFileEditor xmlFileEditor = new XmlFileEditor(this);

            //get the user to select a file from the Windows fileselect Dialog
            string filePath = gameFileSelector.SelectGameFile();

            //put the textcontent of the xml inside the Xml Preview Box (skip unpack for now)
            xmlFileEditor.OpenXml(filePath, XmlPreviewerBox);

            // (WPF stuff) set the vertical size to something big so we dont get linebreaks 
            XmlPreviewerBox.Document.PageWidth = 19200.00;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
