using System;
using System.IO;
using System.Data.SQLite;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        string connectionString = "Data Source=" + Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName")) + ";Version=3;";
        string htmlFile = Path.Combine(CPH.GetGlobalVar<string>("path"), "suggestions.html");
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
        html.Append("<th>Requests</th>");
        html.Append("</tr>");
        using (SQLiteConnection db = new SQLiteConnection(connectionString))
        {
            db.Open();
            using (SQLiteTransaction transaction = db.BeginTransaction())
            {
                string query = "SELECT rh.name, rh.author, rh.type, rh.exits, rh.link_id, s.suggested_by, s.date_suggested, s.weight FROM romhacks rh JOIN suggestions s ON rh.id = s.romhack_id ORDER BY s.weight DESC";
                using (SQLiteCommand cmd = new SQLiteCommand(query, db))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            html.Append("<tr>");
                            html.Append("<td>" + DateTime.Parse(reader["date_suggested"].ToString()).ToString("yyyy-MM-dd") + "</td>");
                            html.Append("<td>" + reader["name"] + "</td>");
                            html.Append("<td>" + reader["author"] + "</td>");
                            html.Append("<td>" + reader["type"] + "</td>");
                            html.Append("<td>" + reader["exits"] + "</td>");
                            html.Append(string.Format("<td><a href=https://www.smwcentral.net/?p=section&a=details&id={0}>Link</a></td>", reader["link_id"]));
                            html.Append(string.Format("<td><a href=https://twitch.tv/{0}>{0}</a></td>", reader["suggested_by"]));
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
						reader.Close();
                    }
                }

                transaction.Commit();
            }

            db.Close();
        }

        html.Append("</table>");
        html.Append("<p> Sorted by Requests and Date.</p>");
        html.Append("<footer>");
        html.Append("Generated with &#10084; and <a href='https://github.com/synthie-cat/-romhack'>!romhack</a> by synthie_cat | Trans-Rights are human rights.");
        html.Append("</footer>");
        html.Append("</body>");
        html.Append("</html>");
        File.WriteAllText(htmlFile, html.ToString());
        return true;
    }
}