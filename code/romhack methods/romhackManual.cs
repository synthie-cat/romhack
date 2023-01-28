using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        CPH.ExecuteMethod("romhackBackup", ""); // Generate Backup
        string manualInput = CPH.GetGlobalVar<string>("manualUpdate"); // Fetch the manual update data
        string[] value = manualInput.Split(','); // Split data into values. Values are: Name, Author, Type, Exits, URL
        try
        {
            int exits = int.Parse(value[3]); // Try parsing exit numbers into an int to count it later.
        }
        catch (FormatException e) // 
        {
            CPH.SendMessage("Error: " + e.Message);
        }

        CPH.SetGlobalVar("info.Name", value[0], true);
        CPH.SetGlobalVar("info.Author", value[1], true);
        CPH.SetGlobalVar("info.Type", value[2], true);
        CPH.SetGlobalVar("info.Exits", value[3], true);
        CPH.SetGlobalVar("info.Url", "Not yet released.", true);
        CPH.SetGlobalVar("info.currentExits", 0, true); // Set counter to 0. Doing it here to allow restore older states
        CPH.ExecuteMethod("romhackOverlayUpdate", ""); // update to the manual values.
        return true;
    }
}