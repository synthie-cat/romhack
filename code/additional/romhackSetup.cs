using System;
using System.IO;
using System.Data.SQLite;

public class CPHInline
{
    public bool Execute()
    {
        // Helpers
        string path = Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName"));
        bool ObsConnected = CPH.ObsIsConnected();
        if (ObsConnected)
        {
            if (File.Exists(path))
            {
				// Create Scene
                CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\"[O] Romhack Info\"}", 0);
                // Setup example data
                string[] inputNames = {"info.Name", "info.Author", "info.Exits", "info.Type"};
                string[] textValues = {"Evil Doopu World", "By: EvilAdmiralKivi", "Exits: 0/6", "Kaizo: Intermediate"};
                int positionY = 0;
                // Create Text Sources
                for (int i = 0; i < inputNames.Length; i++)
                {
                    CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"[O] Romhack Info\",\"inputName\":\"" + inputNames[i] + "\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"font\":{\"face\":\"Impact\",\"flags\":0,\"size\":64,\"style\":\"Medium\"},\"outline\":true,\"outline_color\":4278190080,\"outline_size\":5,\"read_from_file\":false,\"text\":\"" + textValues[i] + "\"},\"sceneItemEnabled\":true}", 0);
                    CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"[O] Romhack Info\",\"sceneItemId\":" + (i + 1) + ",\"sceneItemTransform\":{\"alignment\":5,\"cropBottom\":0,\"cropLeft\":0,\"cropRight\":0,\"cropTop\":0,\"height\":5,\"positionX\":5,\"positionY\":" + positionY + ",\"rotation\":0,\"scaleX\":1,\"scaleY\":1,\"sourceHeight\":74,\"sourceWidth\":406,\"width\":406}}", 0);
                    positionY += 74;
                }
				// Create "Cover" Overalay - First image from the SMW Central site. Creating it as Browser Source so you can adjust it to a different picture (if desired) without needing to download an image.
                CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"[O] Romhack Info\",\"inputName\":\"info.Cover\",\"inputKind\":\"browser_source\",\"inputSettings\":{\"inputSettings\":{\"height\":220,\"url\":\"https://dl.smwcentral.net/image/94575.png\",\"width\":250}},\"sceneItemEnabled\":false}", 0);
                CPH.SendMessage("Overlay successfully created.");
            }
            else
            {
                CPH.SendMessage("DB File not found. Please check your settings for the database path.");
            }
        }
        else
        {
            CPH.SendMessage("OBS is not connected. Please check your OBS Websocket connection.");
        }

        return true;
    }
}