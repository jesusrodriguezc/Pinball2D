using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pinball.Utils
{
    public static class StringUtils
    {
        public static string GetTemplateName(Node node)
        {
            if (node == null) return null;

            string nodeName = node.Name;
            if (string.IsNullOrEmpty(nodeName))
            {
                return nodeName;
            }

            string baseNodeName = Regex.Replace(nodeName, "[0-9@]", "");

            return baseNodeName;

        }
    }
}
