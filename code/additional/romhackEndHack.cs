using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

public class CPHInline
{
    public bool Execute()
    {
        string path = CPH.GetGlobalVar<string>("path");
        string connectionString = "Data Source=" + Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName")) + ";Version=3;";
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");
        int currentdbId = CPH.GetGlobalVar<int>("info.dbId");
        string name = CPH.GetGlobalVar<string>("info.Name");
        DateTime currentTime = DateTime.Now;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        using (SQLiteConnection db = new SQLiteConnection(connectionString))
        {
            db.Open();
            string latestSessionEndQuery = "SELECT session_end FROM history WHERE romhack_id = @romhackId AND date_beaten IS NULL ORDER BY session_end DESC LIMIT 1;";
            using (SQLiteCommand cmd = new SQLiteCommand(latestSessionEndQuery, db))
            {
                cmd.Parameters.AddWithValue("@romhackId", currentdbId);
                long latestSessionEndTime = (long)cmd.ExecuteScalar();
                string updateDateBeatenQuery = "UPDATE history SET date_beaten = @currentTime WHERE romhack_id = @romhackId AND session_end = @sessionEnd";
                using (SQLiteCommand updateCmd = new SQLiteCommand(updateDateBeatenQuery, db))
                {
                    updateCmd.Parameters.AddWithValue("@romhackId", currentdbId);
                    updateCmd.Parameters.AddWithValue("@sessionEnd", latestSessionEndTime);
                    updateCmd.Parameters.AddWithValue("@currentTime", unixTime);
                    updateCmd.ExecuteNonQuery();
                }
            }

            db.Close();
        }

        return true;
    }
}