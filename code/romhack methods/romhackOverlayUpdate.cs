using System;

public class CPHInline
{
    public bool Execute()
    {
        // Get Globals
        string name = CPH.GetGlobalVar<string>("info.Name");
        string author = CPH.GetGlobalVar<string>("info.Author");
        string type = CPH.GetGlobalVar<string>("info.Type");
        int exits = CPH.GetGlobalVar<int>("info.Exits");
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");

        // Update OBS
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Name", name);
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Author", $"By: {author}");
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Exits", $"Exits: {currentExits.ToString()} / {exits.ToString()}");
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Type", type);
        CPH.ExecuteMethod("romhackImageLoader", "");
        CPH.ExecuteMethod("romhackAdditionalUpdates", "");

        return true;
    }
}