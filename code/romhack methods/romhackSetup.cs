using System;
using System.IO;

public class CPHInline
{
    public bool Execute()
    {
        // Helpers
        string path = CPH.GetGlobalVar<string>("path");
        string historyPath = Path.Combine(path, "_history.txt");
        string suggestionsPath = Path.Combine(path, "_suggestions.md");
        string suggestionsTableHeader = "| Date | Romhack Name | Creator | Exits | Type | Link | Requester |\n" + "|---|---|---|---:|---|---|---|";
        // Create Scene
        CPH.ObsSendRaw("CreateScene", "{\"sceneName\":\"[O] Romhack Info\"}", 0);
        // Setup example data. Needs refactoring but meh
        string[] inputNames = { "info.Name", "info.Author", "info.Exits", "info.Type" };
        string[] textValues = { "Evil Doopu World", "By: EvilAdmiralKivi", "Exits: 0/6", "Kaizo: Intermediate" };
        CPH.SetGlobalVar("info.Name", "Evil Doopu World", true);
        CPH.SetGlobalVar("info.Author", "By: EvilAdmiralKivi", true);
        CPH.SetGlobalVar("info.currentExits", 0, true);
        CPH.SetGlobalVar("info.Exits", 6, true);
        CPH.SetGlobalVar("info.Type", "Kaizo: Intermediate", true);
        int positionY = 0;
        // Create Text Sources
        for (int i = 0; i < inputNames.Length; i++)
        {
            CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"[O] Romhack Info\",\"inputName\":\"" + inputNames[i] + "\",\"inputKind\":\"text_gdiplus_v2\",\"inputSettings\":{\"font\":{\"face\":\"Impact\",\"flags\":0,\"size\":64,\"style\":\"Medium\"},\"outline\":true,\"outline_color\":4278190080,\"outline_size\":5,\"read_from_file\":false,\"text\":\"" + textValues[i] + "\"},\"sceneItemEnabled\":true}", 0);
            CPH.ObsSendRaw("SetSceneItemTransform", "{\"sceneName\":\"[O] Romhack Info\",\"sceneItemId\":" + (i + 1) + ",\"sceneItemTransform\":{\"alignment\":5,\"cropBottom\":0,\"cropLeft\":0,\"cropRight\":0,\"cropTop\":0,\"height\":5,\"positionX\":5,\"positionY\":" + positionY + ",\"rotation\":0,\"scaleX\":1,\"scaleY\":1,\"sourceHeight\":74,\"sourceWidth\":406,\"width\":406}}", 0);
            positionY += 74;
        }
        CPH.ObsSendRaw("CreateInput", "{\"sceneName\":\"[O] Romhack Info\",\"inputName\":\"info.Cover\",\"inputKind\":\"browser_source\",\"inputSettings\":{\"inputSettings\":{\"height\":220,\"url\":\"https://dl.smwcentral.net/image/94575.png\",\"width\":250}},\"sceneItemEnabled\":false}", 0);
        CPH.SendMessage("Overlay successfully created.");
        // Create additional files
        /* if (!File.Exists(historyPath) && !File.Exists(suggestionsPath)) // Create both files
        {
            try
            {
                File.Create(historyPath).Close();
                File.Create(suggestionsPath).Close();
                File.AppendAllText(suggestionsPath, suggestionsTableHeader + Environment.NewLine);
                CPH.SendMessage("Successfully created backup data and suggestions table.");
            }
            catch (UnauthorizedAccessException) // Error Check
            {
                CPH.SendMessage("Error: Could not write files. You path might be wrong; check and re-run the setup.");
            }
        }
        else // Mostly if somebody deleted one of the files and re-runs the setup. Does not override anything.
        {
            if (!File.Exists(historyPath))
            {
                CPH.SendMessage("Error: history.txt is missing. Attempting to re-generate.");
                try
                {
                    File.Create(historyPath).Close();
                    CPH.SendMessage("Successfully re-generated _history.txt.");
                }
                catch (UnauthorizedAccessException)
                {
                    CPH.SendMessage("Error: history.txt could not be generated. Check if the file is writeable.");
                }
            }

            if (!File.Exists(suggestionsPath))
            {
                CPH.SendMessage("Error: suggestions.md is missing. Attempting to re-generate.");
                try
                {
                    File.Create(suggestionsPath).Close();
                    CPH.SendMessage("Successfully re-generated _suggestions.md.");
                    File.AppendAllText(suggestionsPath, suggestionsTableHeader + Environment.NewLine);
                }
                catch (UnauthorizedAccessException)
                {
                    CPH.SendMessage("Error: suggestions.md could not be generated. Check if the file is writeable.");
                }
            }
        } */

        CPH.SendMessage("Overlay Setup complete.");
        return true;
    }
}