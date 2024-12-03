using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustEditXml.Contoller
{
    public class GameFileConverter
    {
        public GameFileConverter(string filepath)
        {
            FilePath = filepath;
            FileExtension = Path.GetExtension(filepath);
        }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }

        public void UnpackToXml()
        {
            if (FileExtension == avalancheFileTypes[avalancheFileType.epe])
            {
                // use propertyContainer.exe
            }
            if (FileExtension == avalancheFileTypes[avalancheFileType.ee])
            {
                //use streamedArchive.exe
            }
        }

        public void RepackToDropzone()
        {

        }

        public enum avalancheFileType
        {
            ee, epe, wtunec, bl, nl, blo
        }
        public static readonly Dictionary<avalancheFileType, string> avalancheFileTypes = new Dictionary<avalancheFileType, string>
        {
            {avalancheFileType.epe, ".epe" },
            {avalancheFileType.ee, ".ee" }
        };
    }
}
