using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace JustEditXml._CONTROLLER
{
    internal class GameFileSelector
    {
        public GameFileSelector() { }
        public string SelectGameFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "all files (*.*)|*.*";

            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}
