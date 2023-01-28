using System;
using System.Net;
using HtmlAgilityPack;
using System.Xml;
using System.IO;


public class CPHInline
{
    public bool Execute()
    {
        string url = CPH.GetGlobalVar<string>("info.Url");
        CPH.LogDebug("URL:" + url);
        var web = new HtmlWeb();
        var doc = web.Load(url);
        var scriptNode = doc.DocumentNode.SelectSingleNode("//script[contains(text(), 'SMWCentral.Slideshow')]");
        if (scriptNode != null)
        {
            var rawImageUrls = scriptNode.InnerText;
            string rawImageTrimmedFront = rawImageUrls.Replace("SMWCentral.Slideshow.onLoad = [{element: \"slideshow\", images: [", "");
            string imageTrimmed = rawImageTrimmedFront.Replace("]}];", "");
            string[] imageUrls = imageTrimmed.Split(new string[] { ", " }, StringSplitOptions.None);
            string firstImageCleanup = imageUrls[0].Replace("window.SMWCentral = window.SMWCentral || {};", "");
            firstImageCleanup = firstImageCleanup.Replace("SMWCentral.Slideshow = SMWCentral.Slideshow || {};", "");
            firstImageCleanup = firstImageCleanup.Replace("'", "");
            firstImageCleanup = firstImageCleanup.Trim().Replace(" ", "");
            string firstImageUrl = "https:" + firstImageCleanup;
            CPH.LogDebug(imageUrls[0]);
            CPH.ObsSetBrowserSource("[O]Romhack Info", "info.Cover", firstImageUrl, 0);
        }

        return true;
    }
}