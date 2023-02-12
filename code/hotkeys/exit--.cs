using System;
using System.IO;

public class CPHInline
{
    public bool Execute()
    {
        int infoExits = CPH.GetGlobalVar<int>("info.Exits");
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");
        currentExits--;
        if (currentExits <= -1)
        {
            currentExits = 0;
            CPH.ObsSetGdiText("[O]Romhack Info", "info.Exits", $"Exits: {currentExits.ToString()}/{infoExits.ToString()}");
            
        }
        else
        {
            CPH.ObsSetGdiText("[O]Romhack Info", "info.Exits", $"Exits: {currentExits.ToString()}/{infoExits.ToString()}");
        }

        CPH.SetGlobalVar("info.currentExits", currentExits, true);
        return true;
    }
}