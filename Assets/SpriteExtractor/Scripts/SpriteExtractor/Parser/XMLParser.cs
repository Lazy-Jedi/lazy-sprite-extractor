using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JellyFish.Serializables;
using UnityEngine;

namespace JellyFish.Parser
{
    public static class XMLParser
    {
        #region VARIABLES

        private static string RootName = "TextureAtlas";
        private static string ChildName = "SubTexture";
        private static string Name = "name";
        private static string X = "x";
        private static string Y = "y";
        private static string Width = "width";
        private static string Height = "height";

        #endregion

        #region METHODS

        public static List<SubTexture> ParseXml(TextAsset xmlAsset)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlAsset.text);

            XmlElement root = xml.DocumentElement;

            if (root == null || root.Name != RootName)
            {
                return null;
            }

            List<SubTexture> subTextures = root.ChildNodes.Cast<XmlNode>()
                                               .Where(childNode => childNode.Name == ChildName)
                                               .Select(childNode => new SubTexture
                                               {
                                                   name = childNode.Attributes[Name].Value,
                                                   x = Convert.ToInt32(childNode.Attributes[X].Value),
                                                   y = Convert.ToInt32(childNode.Attributes[Y].Value),
                                                   width = Convert.ToInt32(childNode.Attributes[Width].Value),
                                                   height = Convert.ToInt32(childNode.Attributes[Height].Value)
                                               }).ToList();

            return subTextures;
        }

        public static List<SubTexture> ParseXml(string xmlString)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            XmlElement root = xml.DocumentElement;

            if (root == null || root.Name != RootName)
            {
                return null;
            }

            List<SubTexture> subTextures = root.ChildNodes.Cast<XmlNode>()
                                               .Where(childNode => childNode.Name == ChildName)
                                               .Select(childNode => new SubTexture
                                               {
                                                   name = childNode.Attributes[Name].Value,
                                                   x = Convert.ToInt32(childNode.Attributes[X].Value),
                                                   y = Convert.ToInt32(childNode.Attributes[Y].Value),
                                                   width = Convert.ToInt32(childNode.Attributes[Width].Value),
                                                   height = Convert.ToInt32(childNode.Attributes[Height].Value)
                                               }).ToList();

            return subTextures;
        }

        #endregion
    }
}