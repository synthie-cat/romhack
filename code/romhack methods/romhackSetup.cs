using System;
using System.IO;

public class CPHInline
{
    public bool Execute()
    {
        string path = CPH.GetGlobalVar<string>("path");
        string historyPath = Path.Combine(path, "_history.txt");
        string suggestionsPath = Path.Combine(path, "_history.txt");
        CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\"[O] Romhack Info\"}", 0);
        string[] inputNames = { "info.Name", "info.Author", "info.Exits", "info.Type" };
        string[] textValues = { "Evil Doopu World", "By: EvilAdmiralKivi", "Exits: 0/6", "Kaizo: Intermediate" };
        CPH.SetGlobalVar("info.Name", "Evil Doopu World", true);
        CPH.SetGlobalVar("info.Author", "By: EvilAdmiralKivi", true);
        CPH.SetGlobalVar("info.currentExits", 0, true);
        CPH.SetGlobalVar("info.Exits", 6, true);
        CPH.SetGlobalVar("info.Type", "Kaizo: Intermediate", true);
        int positionY = 0;
        for (int i = 0; i < inputNames.Length; i++)
        {
            CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"[O] Romhack Info\",\"inputName\":\"" + inputNames[i] + "\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"font\":{\"face\":\"Impact\",\"flags\":0,\"size\":64,\"style\":\"Medium\"},\"outline\":true,\"outline_color\":4278190080,\"outline_size\":5,\"read_from_file\":false,\"text\":\"" + textValues[i] + "\"},\"sceneItemEnabled\":true}", 0);
            CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"[O] Romhack Info\",\"sceneItemId\":" + (i + 1) + ",\"sceneItemTransform\":{\"alignment\":5,\"cropBottom\":0,\"cropLeft\":0,\"cropRight\":0,\"cropTop\":0,\"height\":5,\"positionX\":5,\"positionY\":" + positionY + ",\"rotation\":0,\"scaleX\":1,\"scaleY\":1,\"sourceHeight\":74,\"sourceWidth\":406,\"width\":406}}", 0);
            positionY += 74;
        }

        CPH.SendMessage("Overlay successfully created. Check your OBS.");
        if (!File.Exists(historyPath) && !File.Exists(suggestionsPath))
        {
            File.Create(historyPath).Close();
            File.Create(suggestionsPath).Close();
            string tableHeader = "| Date | Romhack Name | Creator | Exits | Type | Link | Requester |\n" + "|---|---|---|---:|---|---|---|";
            File.AppendAllText(suggestionsPath, tableHeader + Environment.NewLine);
            CPH.SendMessage("Successfully created backup data and suggestions table.");
        }
        else
        {
            CPH.SendMessage("Potential Error: history.txt or suggestions.md already exist. Please check your Streamer.Bot folder.");
        }

        CPH.SendMessage("Overlay Setup complete.");
        return true;
    }
}