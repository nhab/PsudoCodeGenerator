using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsudoCodeGen.Models
{
    public class MethodInfo
    {
        public string Name { get; }
        public string ReturnType { get; }
        public string Comments { get; }

        public MethodInfo(string name, string returnType, string comments)
        {
            Name = name;
            ReturnType = returnType;
            Comments = comments;
        }
    }
}
