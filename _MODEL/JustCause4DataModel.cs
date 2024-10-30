using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustEditXml._MODEL
{
    internal static class JustCause4DataModel
    {
        //collection of more user readable names for all properties in the .epe files.
        static readonly Dictionary<string, string> ReadablePropertyNames;

        //collection of MIN and MAX values of all properties where the valid ranges are known.
        static readonly Dictionary<string, List<float>> ValidateablePropertyRanges;
    }
}
