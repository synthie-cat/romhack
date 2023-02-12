using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

public class CPHInline
{
    public bool Execute()
    {
        string connectionString = "Data Source=" + Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName")) + ";Version=3;";
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits");
        int currentdbId = CPH.GetGlobalVar<int>("info.dbId");
        string name = CPH.GetGlobalVar<string>("info.Name");
		DateTime currentTime = DateTime.Now;
        long unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        CPH.LogDebug(currentdbId.ToString());
        using (SQLiteConnection db = new SQLiteConnection(connectionString))
        {
            db.Open();
            string romhackIdQuery = "SELECT id FROM history WHERE romhack_id = @romhack_id AND current_exit = @exit AND session_end IS NULL;";
            using (SQLiteCommand cmd = new SQLiteCommand(romhackIdQuery, db))
            {
                cmd.Parameters.AddWithValue("@romhack_id", currentdbId);
                cmd.Parameters.AddWithValue("@exit", currentExits);
                var historyId = cmd.ExecuteScalar();
                if (historyId != null)
                {
                    // ID found, session_end update
                    string insertQuery = "UPDATE history SET session_end = @unixTime WHERE id = @id";
                    using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, db))
                    {
                        insertCmd.Parameters.AddWithValue("@id", historyId);
						insertCmd.Parameters.AddWithValue("@unixTime", unixTime);
                        insertCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    CPH.SendMessage("No open session was found.");
                }
            }

            db.Close();
        }

        return true;
    }
}