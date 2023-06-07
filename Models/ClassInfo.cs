using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsudoCodeGen.Models
{
    public class ClassInfo
    {
        public string Name { get; }
        public string Comments { get; }
        public List<PropertyInfo> Properties { get; }
        public List<MethodInfo> Methods { get; }

        public ClassInfo(string name, string comments, List<PropertyInfo> properties, List<MethodInfo> methods)
        {
            Name = name;
            Comments = comments;
            Properties = properties;
            Methods = methods;
        }
    }
}
