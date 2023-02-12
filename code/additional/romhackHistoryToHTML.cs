
using System;
using System.Data.SQLite;
using System.IO;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        string db = Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName"));
        string htmlPath = Path.Combine(CPH.GetGlobalVar<string>("path"), "progress.html");
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={db};Version=3;"))
        {
            connection.Open();
            StringBuilder html = new StringBuilder();
            html.Append("<html>");
            html.Append("<head>");
            html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            html.Append("<title>Romhack Progress</title>");
            html.Append("</head>");
            html.Append("<body>");
            html.Append("<h1>Romhack Progress</h1>");
            html.Append("<table>");
            string selectQuery = @"SELECT r.id AS id,
				r.name AS romhack_name,
				h.romhack_id,
				h.current_exit,
				h.date_beaten,
				h.session_end - h.session_start AS clear_time_seconds,
				SUM(h.session_end - h.session_start) OVER (PARTITION BY h.romhack_id) AS total_time
				FROM romhacks r
				JOIN history h ON r.id = h.romhack_id
				ORDER BY r.name, h.current_exit DESC;";
            using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    string currentName = "";
                    int totalTime = 0;
                    while (reader.Read())
                    {
                        if (currentName != reader["romhack_name"].ToString())
                        {
                            currentName = reader["romhack_name"].ToString();
                            html.Append("<tr><th colspan=4>" + currentName + "</th></tr>");
                            html.Append("<tr><th>Date Beaten</th><th>Exit</th><th>Clear Time</th><th>Total Time</th></tr>");
                        }

                        html.Append("<tr>");
                        if (!reader.IsDBNull(reader.GetOrdinal("date_beaten")))
                        {
                            DateTime dateBeaten = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                            dateBeaten = dateBeaten.AddSeconds(Convert.ToDouble(reader["date_beaten"])).ToLocalTime();
                            string formattedDateBeaten = dateBeaten.ToString("yyyy-MM-dd HH:mm:ss");
                            html.Append("<td>" + formattedDateBeaten + "</td>");
                        }
                        else
                        {
                            html.Append("<td> </td>");
                        }

                        html.Append("<td>" + reader["current_exit"] + "</td>");
                        if (!reader.IsDBNull(reader.GetOrdinal("clear_time_seconds")))
                        {
                            int clearTimeSeconds = Convert.ToInt32(reader["clear_time_seconds"]);
                            hours = clearTimeSeconds / 3600;
                            minutes = (clearTimeSeconds % 3600) / 60;
                            seconds = clearTimeSeconds % 60;
                        }
                        else
                        {
                            html.Append("<td> In Progress </td>");
                        }

                        html.Append("<td>" + string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds) + "</td>");
                        if (!reader.IsDBNull(reader.GetOrdinal("date_beaten")))
                        {
                            int beatenTimeSeconds = Convert.ToInt32(reader["total_time"]);
                            hours = beatenTimeSeconds / 3600;
                            minutes = (beatenTimeSeconds % 3600) / 60;
                            seconds = beatenTimeSeconds % 60;
                            html.Append("<td>" + string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds) + "</td>");
                        }
                        else
                        {
                            html.Append("<td>" + " " + "</td>");
                        }

                        html.Append("</tr>");
                    }
                }
            }

            html.Append("<footer>");
            html.Append("Generated with &#10084; and <a href='https://github.com/synthie-cat/-romhack'>!romhack</a> by synthie_cat | Trans-Rights are human rights.");
            html.Append("</footer>");
            html.Append("</body>");
            html.Append("</html>");
            if (!File.Exists(htmlPath))
            {
                File.Create(htmlPath).Close();
            }

            File.WriteAllText(htmlPath, html.ToString());
            return true;
        }
    }
}