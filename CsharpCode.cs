using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PsudoCodeGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PropertyInfo = PsudoCodeGen.Models.PropertyInfo;
using MethodInfo = PsudoCodeGen.Models.MethodInfo;

namespace PsudoCodeGen
{
    /// <summary>
    ///  Main method of the class is GetStringReport
    ///  Other methods help this method
    /// </summary>
    public class CsharpCode
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="csCodeFilePath"></param>
        /// <returns></returns>
        public static string GetStringReport(string csCodeFilePath)
        {
            var classes = Extract(csCodeFilePath);
            List<string> lines = GenerateReport(classes);
            return PrintListToString(lines);
        }
        #region helper functions
        public static Dictionary<string, ClassInfo> Extract(string csCodeFilePath)
        {
            string code                   = File.ReadAllText(csCodeFilePath);
            SyntaxTree syntaxTree         = CSharpSyntaxTree.ParseText(code);
            CSharpCompilation compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { syntaxTree });
            SyntaxNode root               = syntaxTree.GetRoot();

            Dictionary<string, ClassInfo> classes = new Dictionary<string, ClassInfo>();
            foreach (var node in root.DescendantNodes())
            {
                if (node is ClassDeclarationSyntax classNode)
                {
                    string className = classNode.Identifier.ValueText;
                    string classComments = classNode.GetLeadingTrivia().ToString();

                    List<PropertyInfo> properties = new List<PropertyInfo>();
                    foreach (var propertyNode in classNode.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                    {
                        string propertyName = propertyNode.Identifier.ValueText;
                        string propertyComments = propertyNode.GetLeadingTrivia().ToString();
                        string propertyType = propertyNode.Type.ToString();

                        properties.Add(new PropertyInfo(propertyName, propertyType, propertyComments));
                    }

                    List<MethodInfo> methods = new List<MethodInfo>();
                    foreach (var methodNode in classNode.DescendantNodes().OfType<MethodDeclarationSyntax>())
                    {
                        string methodName = methodNode.Identifier.ValueText;
                        string methodComments = methodNode.GetLeadingTrivia().ToString();
                        string returnType = methodNode.ReturnType.ToString();

                        methods.Add(new MethodInfo(methodName, returnType, methodComments));
                    }

                    classes.Add(className, new ClassInfo(className, classComments, properties, methods));
                }
               
            }
            return classes;
        }
        public static List<string> GenerateReport(Dictionary<string, ClassInfo> classes,bool showType=true)
        {
            List<string> lines =new List<string>() ;
            string s = "";
            foreach (var classInfo in classes.Values)
            {
                s=(showType)?$"Class:":"";
                lines.Add(s+"{classInfo.Name}" );

                if (!String.IsNullOrWhiteSpace(classInfo.Comments))
                {
                    s = (showType) ? $"Comments:" : "";
                    lines.Add(s + "{classInfo.Comments}");
                }
                //s = (showType) ? "Properties:" :"";
                //lines.Add(s);

                foreach (var property in classInfo.Properties)
                {
                    lines.Add( $"       Property: {property.Name} ({property.Type})");
                    if (!String.IsNullOrWhiteSpace(property.Comments))
                        lines.Add( $"       Property Comments: {property.Comments}");
                }

                //lines.Add("Methods:");
                foreach (var method in classInfo.Methods)
                {
                    lines.Add($"        Method : {method.Name} ({method.ReturnType})");
                    lines.Add($"        Method Comments: {method.Comments}");
                }
            }
            return lines;
        }
        public static string PrintListToString(List<string> lines)
        {
            string s = "";
            foreach(string line in lines)
            {
                s += line + "\r\n";
            }
            return s;
        }
        #endregion
    }
}
