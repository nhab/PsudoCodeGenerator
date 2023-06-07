using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsudoCodeGen.Models
{
    public class PropertyInfo
    {
        public string Name { get; }
        public string Type { get; }
        public string Comments { get; }

        public PropertyInfo(string name, string type, string comments)
        {
            Name = name;
            Type = type;
            Comments = comments;
        }
    }
}
