using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace JustEditXml._MODEL
{
    internal static class XmlLabeler
    {
        public enum NamingRule
        {
            WARNING,
            NOTE,
            hasName,
            childHasName,
            RawNode
        }
        public static NamingRule DecideHeaderLabels(XElement _node)
        {
            NamingRule rule = new NamingRule();


            if (_node.Attribute("WARNING") != null) //1. Rule, allways display WARNING="DO NOT TOUCH" if exist 
            {
                rule = NamingRule.WARNING;
            }
            else if (_node.Attribute("NOTE") !=null) //2. Rule, allways display NOTE="Here you can touch" if exist
            {
                if (_node.Attribute("NOTE").Value == "HERE YOU CAN TOUCH, BUT BE GENTLE")
                    rule = NamingRule.NOTE;
            }
            else if (_node.Elements().Any(e => e.Attribute("name")?.Value == "name")) //4. Rule, use "name" Attribute of child, if parent has none.
            {
                rule = NamingRule.childHasName;
            }
            else if (_node.Attribute("name") != null ) //3. Rule, use name as Label, if attribute is in node
            {
                rule = NamingRule.hasName;
            }
            else //fallback case display raw node
            {
                rule = NamingRule.RawNode;
            }

            return rule;
        }
        public static TextBlock GetPrimaryHeaderLabel(XElement node, NamingRule rule)
        {
            var PrimaryLabel = new TextBlock();

            switch (rule)
            {
                case NamingRule.WARNING:
                    PrimaryLabel.Text = $"{node.Attribute("WARNING")?.Name}: {node.Attribute("WARNING")?.Value}";
                    break;
                case NamingRule.NOTE:
                    PrimaryLabel.Text = $"{node.Attribute("NOTE")?.Name}: {node.Attribute("NOTE")?.Value}";
                    break;
                case NamingRule.hasName:
                    PrimaryLabel.Text = $"{node.Attribute("name")?.Value}";
                    break;
                case NamingRule.childHasName:
                    PrimaryLabel.Text = node
                        .Elements()
                        .Where(e => e.Attribute("name")?.Value == "name")
                        .Select(e => e.Value)
                        .FirstOrDefault();
                    break;
                case NamingRule.RawNode:
                    PrimaryLabel.Text = $"<{node.Name}>";
                    break;

                default:
                    break;
            }
            return PrimaryLabel;
        }
        public static TextBlock GetSecondaryHeaderLabel(XElement node, NamingRule rule)
        {
            var SecoundaryLabel = new TextBlock();
            switch (rule)
            {
                case NamingRule.WARNING:
                    SecoundaryLabel.Text = String.Empty;
                    break;
                case NamingRule.NOTE:
                    SecoundaryLabel.Text = String.Empty;
                    break;
                case NamingRule.hasName:
                    SecoundaryLabel.Text = $"type: {node.Attribute("type")?.Value}";
                    break;
                case NamingRule.childHasName:
                    SecoundaryLabel.Text = node
                    .Elements()
                    .Where(e => e.Attribute("name")?.Value == "_class_hash")
                    .Select(e => e.Value.ToString())
                    .FirstOrDefault();
                    break;
                case NamingRule.RawNode:
                    string nodeString = String.Empty;
                    foreach (var attribute in node.Attributes())
                    {
                        nodeString += $" {attribute.Name}=\"{attribute.Value}\"";
                    }
                    SecoundaryLabel.Text = nodeString;
                    break;

                default:
                    break;
            }
            return SecoundaryLabel;

        }
    }
}
