using System;

public class CPHInline
{
    public bool Execute()
    {
        // Put everything into a new global. Helps us restore later
        CPH.SetGlobalVar("backupInfoName", CPH.GetGlobalVar<string>("info.Name"), true);
        CPH.SetGlobalVar("backupInfoAuthor", CPH.GetGlobalVar<string>("info.Author"), true);
        CPH.SetGlobalVar("backupInfoType", CPH.GetGlobalVar<string>("info.Type"), true);
        CPH.SetGlobalVar("backupInfoUrl", CPH.GetGlobalVar<string>("info.Url"), true);
        CPH.SetGlobalVar("backupInfoExits", CPH.GetGlobalVar<int>("info.Exits"), true);
        CPH.SetGlobalVar("backupInfoCurrentExits", CPH.GetGlobalVar<int>("info.currentExits"), true);
        return true;
    }
}
