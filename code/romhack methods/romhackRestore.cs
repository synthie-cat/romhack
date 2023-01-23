using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        Dictionary<string, Type> properties = new Dictionary<string, Type>()
        {{"Name", typeof(string)}, {"Author", typeof(string)}, {"Type", typeof(string)}, {"Url", typeof(string)}, {"Exits", typeof(int)}, {"currentExits", typeof(int)}};
        foreach (var property in properties)
        {
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
            CPH.ExecuteMethod("romhackOverlayUpdate", "");
        }

        return true;
    }
}
