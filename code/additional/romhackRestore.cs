using System;
using System.IO;
using System.Data.SQLite;

public class CPHInline
{
    public bool Execute()
    {
        string romhackName = CPH.GetGlobalVar<string>("info.restoreName");
        string db = Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName"));
        CPH.SendMessage(db);
        int romhackId = 0;
        int currentExit = 0;
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + db + ";Version=3;"))
        {
            connection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT id FROM romhacks WHERE name = @name", connection))
            {
                cmd.Parameters.AddWithValue("@name", romhackName);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    romhackId = Convert.ToInt32(result);
                }
            }

            if (romhackId != 0)
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT current_exit FROM history WHERE romhack_id = @romhack_id ORDER BY current_exit DESC LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@romhack_id", romhackId);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentExit = Convert.ToInt32(result);
                        CPH.SetGlobalVar("info.Name", romhackName, true);
                        CPH.SetGlobalVar("info.currentExits", currentExit, true);
                        CPH.SendMessage($"Restoring {romhackName} to {currentExit} Exits.");
                        CPH.ExecuteMethod("romhackUpdateOverlay", "");
                    }
                    else
                    {
                        CPH.SendMessage($"No restore information for {romhackName} found.");
                    }
                }
            }
        }

        return true;
    }
}