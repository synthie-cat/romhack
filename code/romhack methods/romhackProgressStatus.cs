using System;
using System.Data.SQLite;
using System.IO;
using System.Data;

public class CPHInline
{
    public bool Execute()
    {
        string name = CPH.GetGlobalVar<string>("info.Name");
        string path = @"D:\tutorial\";
        string dbPath = Path.Combine(path + "romhack_structure_test.db");
        string db = $"Data Source={dbPath};Version=3;";
        int romhackId = 0;
        using (var connection = new SQLiteConnection(db))
        {
            connection.Open();
            var cmd = new SQLiteCommand($"SELECT id, name FROM romhacks WHERE name LIKE '%{name}%'", connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = Convert.ToInt32(reader["id"]);
                var romhackName = reader["name"].ToString();
                // CPH.LogDebug("Found match for romhack name: " + romhackName);
                name = romhackName;
                romhackId = id;
            }
        }

        using (var connection = new SQLiteConnection(db))
        {
            connection.Open();
            var totalTimeDB = new SQLiteCommand($"SELECT SUM(strftime('%s', session_end) - strftime('%s', session_start)) / 3600 AS total_time FROM history WHERE romhack_id = {romhackId}", connection);
            var dateBeatenDB = new SQLiteCommand($"SELECT date_beaten FROM history WHERE romhack_id = {romhackId} AND date_beaten IS NOT NULL;", connection);
            var totalTimeDBResult = totalTimeDB.ExecuteScalar();
            var totalTime = totalTimeDBResult != null ? totalTimeDBResult.ToString() : "";
            var dateBeatenDBResult = dateBeatenDB.ExecuteScalar();
            var dateBeaten = dateBeatenDBResult != null ? dateBeatenDBResult.ToString() : "";
            CPH.LogDebug(dateBeaten);
            if (dateBeaten != "")
            {
                CPH.SendMessage($"Romhack {name} was beaten on {dateBeaten} after {totalTime} hours");
            }
            else if (totalTime != "" && dateBeaten == "")
            {
                CPH.SendMessage($"Romhack {name} was not yet beaten. It's being played for {totalTime} hours");
            }
            else
            {
                CPH.SendMessage($"Romhack {name} has not been started.");
            }
        }

        return true;
    }
}