using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

public class CPHInline
{
    public bool Execute()
    {
        string path = CPH.GetGlobalVar<string>("path");
        string connectionString = "Data Source=" + Path.Combine(path, CPH.GetGlobalVar<string>("dbFileName")) + ";Version=3;";
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");
        int currentdbId = CPH.GetGlobalVar<int>("info.dbId");
        string name = CPH.GetGlobalVar<string>("info.Name");
        DateTime currentTime = DateTime.Now;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        CPH.LogDebug(currentdbId.ToString());
        using (SQLiteConnection db = new SQLiteConnection(connectionString))
        {
            db.Open();
            string romhackIdQuery = "SELECT id FROM romhacks WHERE name = @name";
            using (SQLiteCommand cmd = new SQLiteCommand(romhackIdQuery, db))
            {
                cmd.Parameters.AddWithValue("@name", name);
                var romhackIdResult = cmd.ExecuteScalar();
                CPH.SetGlobalVar("info.dbId", romhackIdResult.ToString(), true);
                string insertQuery = "INSERT INTO history (romhack_id, current_exit, session_start) VALUES (@romhackId, @currentExits, @unixTime)";
                using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, db))
                {
                    insertCmd.Parameters.AddWithValue("@romhackId", romhackIdResult);
                    insertCmd.Parameters.AddWithValue("@unixTime", unixTime);
					insertCmd.Parameters.AddWithValue("@currentExits", currentExits);
                    insertCmd.ExecuteNonQuery();
                }
            }

            db.Close();
        }

        return true;
    }
}