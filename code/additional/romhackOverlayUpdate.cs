using System;
using System.IO;
using System.Data.SQLite;

public class CPHInline
{
    public bool Execute()
    {
        // Get Globals
        string path = CPH.GetGlobalVar<string>("path");
        string connectionString = "Data Source=" + Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName")) + ";Version=3;";
        string name = CPH.GetGlobalVar<string>("info.Name");
        string author = CPH.GetGlobalVar<string>("info.Author");
        string type = CPH.GetGlobalVar<string>("info.Type");
        int exits = CPH.GetGlobalVar<int>("info.Exits");
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");
        // Some time shinanigans
		DateTime currentTime = DateTime.Now;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

        // Update OBS
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Name", name);
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Author", $"By: {author}");
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Exits", $"Exits: {currentExits.ToString()} / {exits.ToString()}");
        CPH.ObsSetGdiText("[O]Romhack Info", "info.Type", type);
        CPH.ExecuteMethod("romhackImageLoader", "");
        // DB start new session
        using (SQLiteConnection db = new SQLiteConnection(connectionString))
        {
            db.Open();
            string romhackIdQuery = "SELECT id FROM romhacks WHERE name = @name";
            using (SQLiteCommand cmd = new SQLiteCommand(romhackIdQuery, db))
            {
                cmd.Parameters.AddWithValue("@name", name);
                var romhackIdResult = cmd.ExecuteScalar();
                CPH.SetGlobalVar("info.dbId", romhackIdResult.ToString(), true);
                string insertQuery = "INSERT INTO history (romhack_id, current_exit, session_start) VALUES (@romhackId, 0, @unixTime)";
                using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, db))
                {
                    insertCmd.Parameters.AddWithValue("@romhackId", romhackIdResult);
					insertCmd.Parameters.AddWithValue("@unixTime", unixTime);
                    insertCmd.ExecuteNonQuery();
                }
            }

            db.Close();
        }
		CPH.SendMessage($"Overlay updated to {name}");
        CPH.ExecuteMethod("romhackAdditionalUpdates", "");
        return true;
    }
}