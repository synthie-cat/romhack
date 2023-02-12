using System;
using System.Net;
using HtmlAgilityPack;
using System.Xml;
using System.IO;


public class CPHInline
{
    public bool Execute()
    {
		string id = CPH.GetGlobalVar<string>("info.ParseById");
        string url = "https://www.smwcentral.net/?p=section&a=details&id=" + id;
        var web = new HtmlWeb();
        var doc = web.Load(url);
		// Name
        var nameNode = doc.DocumentNode.SelectSingleNode("//td[text()='Name:']/following-sibling::td[1]/a");
        if (nameNode != null)
        {
            string name = nameNode.InnerText;
            CPH.LogDebug("Name: " + name);
        }

        // Author
        var authorNode = doc.DocumentNode.SelectSingleNode("//td[text()='Author:']/following-sibling::td[1]/span/a");
        if (authorNode != null)
        {
            string author = authorNode.InnerText;
            CPH.LogDebug("Author: " + author);
        }

        // Length
        var lengthNode = doc.DocumentNode.SelectSingleNode("//td[text()='Length:']/following-sibling::td[1]");
        if (lengthNode != null)
        {
            string lengthString = lengthNode.InnerText;
            int length = int.Parse(lengthString.Split(' ')[0]);
            CPH.LogDebug("Length: " + length.ToString());
        }

        // Type
        var typeNode = doc.DocumentNode.SelectSingleNode("//td[text()='Type:']/following-sibling::td[1]");
        if (typeNode != null)
        {
            string type = typeNode.InnerText;
            CPH.LogDebug("Type: " + type);
        }

        return true;
    }
}