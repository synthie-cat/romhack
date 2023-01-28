using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // The ol' switcheroo needs a Dictionary!
        Dictionary<string, Type> properties = new Dictionary<string, Type>()
        {{"Name", typeof(string)}, {"Author", typeof(string)}, {"Type", typeof(string)}, {"Url", typeof(string)}, {"Exits", typeof(int)}, {"currentExits", typeof(int)}};
        foreach (var property in properties)
        {	// Mind the type. Stores current strings / ints into a temporary variable, sets the current to the backup, backups the current. 
            if (property.Value == typeof(string))
            {
                var temp = CPH.GetGlobalVar<string>("info." + property.Key);
                CPH.SetGlobalVar("info." + property.Key, CPH.GetGlobalVar<string>("backupInfo" + property.Key), true);
                CPH.SetGlobalVar("backupInfo" + property.Key, temp, true);
            }
            else if (property.Value == typeof(int))
            {
                var temp = CPH.GetGlobalVar<int>("info." + property.Key);
                CPH.SetGlobalVar("info." + property.Key, CPH.GetGlobalVar<int>("backupInfo" + property.Key), true);
                CPH.SetGlobalVar("backupInfo" + property.Key, temp, true);
            }
            CPH.ExecuteMethod("romhackOverlayUpdate", ""); // Bring it to OBS
        }

        return true;
    }
}
