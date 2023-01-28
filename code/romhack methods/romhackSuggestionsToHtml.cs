using System;
using System.IO;
using System.Data.SQLite;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        string globalPath = CPH.GetGlobalVar<string>("path");
        string path = Path.Combine(globalPath, "romhack.sqlite");
        string connectionString = $"Data Source={path};Version=3;";
        string htmlFile = Path.Combine(globalPath, "suggestions.html");
        if (!File.Exists(htmlFile))
        {
            File.Create(htmlFile);
        }

        StringBuilder html = new StringBuilder();
        html.Append("<!DOCTYPE html>");
        html.Append("<html>");
        html.Append("<head>");
        html.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
        html.Append("<title>Romhack Suggestions</title>");
        html.Append("</head>");
        html.Append("<body>");
        html.Append("<h1> Romhack Suggestions</h1>");
        html.Append("<table>");
        html.Append("<tr>");
        html.Append("<th>Date</th>");
        html.Append("<th>Name</th>");
        html.Append("<th>Author</th>");
        html.Append("<th>Type</th>");
        html.Append("<th>Exits</th>");
        html.Append("<th>Link</th>");
        html.Append("<th>Suggested by</th>");
        html.Append("<th>Popularity</th>");
        html.Append("</tr>");
        using (SQLiteConnection db = new SQLiteConnection(connectionString))
        {
            db.Open();
            using (SQLiteTransaction transaction = db.BeginTransaction())
            {
                string query = "SELECT * FROM suggestions ORDER BY weight DESC";
                using (SQLiteCommand cmd = new SQLiteCommand(query, db))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            html.Append("<tr>");
                            html.Append("<td>" + DateTime.Parse(reader["datetime"].ToString()).ToString("yyyy-MM-dd") + "</td>");
                            html.Append("<td>" + reader["name"] + "</td>");
                            html.Append("<td>" + reader["author"] + "</td>");
                            html.Append("<td>" + reader["type"] + "</td>");
                            html.Append("<td>" + reader["exits"] + "</td>");
                            html.Append(string.Format("<td><a href='{0}'>Link</a></td>", reader["link"]));
                            html.Append(string.Format("<td><a href=https://twitch.tv/'{0}'>{0}</a></td>", reader["suggester"]));
                            if (Convert.ToInt32(reader["weight"]) > 0)
                            {
                                html.Append("<td>" + reader["weight"] + "</td>");
                            }
                            else
                            {
                                html.Append("<td> </td>");
                            }

                            html.Append("</tr>");
                        }
                    }
                }

                transaction.Commit();
            }

            db.Close();
        }

        html.Append("</table>");
        html.Append("<p> Sorted by popularity");
        html.Append("<footer>");
        html.Append("Generated with &#10084; and <a href='https://github.com/synthie-cat/-romhack'>!romhack</a> by synthie_cat | Trans-Rights are human rights.");
        html.Append("</footer>");
        html.Append("</body>");
        html.Append("</html>");
        File.WriteAllText(htmlFile, html.ToString());
        return true;
    }
}