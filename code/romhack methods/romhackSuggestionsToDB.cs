using System;
using System.IO;
using System.Data.SQLite;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        string path = Path.Combine(CPH.GetGlobalVar<string>("path"), "romhack.sqlite");
        string name = CPH.GetGlobalVar<string>("temp.Name");
        string author = CPH.GetGlobalVar<string>("temp.Author");
        string type = CPH.GetGlobalVar<string>("temp.Type");
        int exits = CPH.GetGlobalVar<int>("temp.Exits");
        string Url = CPH.GetGlobalVar<string>("temp.Url");
        string currentUser = CPH.GetGlobalVar<string>("temp.CurrentUser");
        if (File.Exists(path))
        {
            using (SQLiteConnection db = new SQLiteConnection($"Data Source={path};Version=3;"))
            {
                db.Open();
                string checkNameQuery = "SELECT * FROM suggestions WHERE name = @name";
                using (SQLiteCommand cmd = new SQLiteCommand(checkNameQuery, db))
                {
                    cmd.CommandText = "UPDATE suggestions SET weight = weight + 1 WHERE name = @name;";
                    cmd.Parameters.AddWithValue("@name", name);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        CPH.SendMessage($"Suggestion for {name} already exists; adjusting the weight to reflect the multiple requests.");
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO suggestions (name, author, type, exits, link, suggester, datetime) VALUES (@name, @author, @type, @exits, @link, @suggester, datetime('now'))";
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@author", author);
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@exits", exits);
                        cmd.Parameters.AddWithValue("@link", Url);
                        cmd.Parameters.AddWithValue("@suggester", currentUser);
                        cmd.ExecuteNonQuery();
                        CPH.SendMessage($"Successfully added {name} to suggestions, {currentUser}");
                    }
                }

                db.Close();
            }
        }

        CPH.ExecuteMethod("romhackSuggestionsToHtml", "");
        return true;
    }
}