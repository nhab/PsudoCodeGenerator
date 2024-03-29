﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PsudoCodeGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PropertyInfo = PsudoCodeGen.Models.PropertyInfo;
using MethodInfo = PsudoCodeGen.Models.MethodInfo;
using Microsoft.CodeAnalysis.VisualBasic;

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
        /// <param name="CodeFilePath"></param>
        /// <returns></returns>
        public static string GetStringReport(string CodeFilePath)
        {
            var classes = Extract(CodeFilePath);
            List<string> lines = GenerateReport(classes,true);
            return PrintListToString(lines);
        }
        #region helper functions
        public static Dictionary<string, ClassInfo> Extract(string CodeFilePath)
        {
            string code              = File.ReadAllText(CodeFilePath);
            SyntaxTree syntaxTree    = null;
          
            if(CodeFilePath.EndsWith("cs"))
             {
                 syntaxTree =  CSharpSyntaxTree.ParseText(code);
                 CSharpCompilation compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { syntaxTree });
            }
            else
            {
                if (CodeFilePath.EndsWith("vb"))
                {
                    syntaxTree = VisualBasicSyntaxTree.ParseText(code);
                    VisualBasicCompilation compilation= VisualBasicCompilation.Create("MyCompilation", syntaxTrees: new[] { syntaxTree });
                }
            }
            SyntaxNode root = syntaxTree.GetRoot();

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
        public static List<string> GenerateReport(Dictionary<string, ClassInfo> classes,bool showTheType=true)
        {
            List<string> lines =new List<string>() ;
            string s = "";
            foreach (var classInfo in classes.Values)
            {
                s=(showTheType)?$"Class:":"";
                lines.Add(s+$"{classInfo.Name}" );

                if (!String.IsNullOrWhiteSpace(classInfo.Comments))
                {
                    s = (showTheType && !string.IsNullOrEmpty( classInfo.Comments)) ? $"Comments:" : "";
                    lines.Add(s + $"{classInfo.Comments}");
                }

                foreach (var property in classInfo.Properties)
                {
                    s = (showTheType) ? "Property :" : "";
                    lines.Add( $"       "+s+$"{property.Name} ({property.Type})");

                    if (!String.IsNullOrWhiteSpace(property.Comments))
                    {
                        s = (showTheType && !string.IsNullOrEmpty( property.Comments)) ? "Property Comments: " : "";
                        lines.Add($"       "+s+$"{property.Comments}");
                    }
                }

                foreach (var method in classInfo.Methods)
                {
                    s = (showTheType) ? "Method : " : "";
                    lines.Add($"        "+s+$"{method.Name} ({method.ReturnType})");
                    
                    s = (showTheType && !String.IsNullOrWhiteSpace( method.Comments)) ? "Method Comments: " : "";
                    lines.Add($"        "+s+$" {method.Comments}");
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
