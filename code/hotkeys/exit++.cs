using System;
using System.IO;

public class CPHInline
{
    public bool Execute()
    {
        int infoExits = CPH.GetGlobalVar<int>("info.Exits");
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");
        string romhack = CPH.GetGlobalVar<string>("info.Name");
        CPH.ExecuteMethod("romhackEndSession", "");
        if (currentExits >= infoExits)
        {
            currentExits = infoExits;
            CPH.SetGlobalVar("info.currentExits", currentExits, true);
            CPH.ExecuteMethod("romhackEndHack", "");
            CPH.ObsSetGdiText("[O]Romhack Info", "info.Exits", "DONE!");
        }
        else
        {
            currentExits++;
            CPH.SetGlobalVar("info.currentExits", currentExits, true);
            CPH.ExecuteMethod("romhackStartSession", "");
            CPH.ObsSetGdiText("[O]Romhack Info", "info.Exits", $"Exits: {currentExits.ToString()}/{infoExits.ToString()}");
        }

        CPH.ExecuteMethod("romhackHistoryToHTML", "");
        return true;
    }
}