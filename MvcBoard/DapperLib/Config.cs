using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace MvcBoard.DapperLib
{
    public class Config
    {
        private static string GetConnStrFromFile(string name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"D:\DBConnCfg\ConnStr.xml");



            XmlNode connStrNode = xmlDoc.DocumentElement.SelectSingleNode("ConnectionString/Item[@name='" + name.ToLower() + "']");



            if (connStrNode != null)
            {
                return connStrNode.Attributes["value"].Value;
            }
            else
            {
                return "";
            }
        }



        public static string DBConnStrTest()
        {
            return GetConnStrFromFile("yiguha_test");
        }
    }
}
