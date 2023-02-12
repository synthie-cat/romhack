using System;
using System.IO;
using System.Data.SQLite;
using System.Text;

public class CPHInline
{
    public bool Execute()
    {
        string path = Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName"));
        string name = CPH.GetGlobalVar<string>("temp.Name");
        string currentUser = CPH.GetGlobalVar<string>("temp.CurrentUser");
        if (File.Exists(path))
        {
            try
            {
                using (SQLiteConnection db = new SQLiteConnection($"Data Source={path};Version=3;"))
                {
                    db.Open();
                    string checkNameQuery = "SELECT * FROM suggestions WHERE romhack_id = (SELECT id FROM romhacks WHERE name = @name)"; // Uses "romhacks" fkey to connect information to prevent redundancy
                    using (SQLiteCommand cmd = new SQLiteCommand(checkNameQuery, db))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        SQLiteDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows) // If a suggestion already exists we add a "viewer request" 
                        {
                            reader.Close(); // prevent db locks. Ran into that way to often when making this. 
                            cmd.CommandText = "UPDATE suggestions SET weight = weight + 1 WHERE romhack_id = (SELECT id FROM romhacks WHERE name = @name)";
							string spamUser = CPH.GetGlobalVar<string>("suggestion.spamProtectionUser"); // Store the last request to prevent users from spamming the same hack.
							string spamHack = CPH.GetGlobalVar<string>("suggestion.spamProtectionHack");
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (currentUser != spamUser && spamHack != name) // Check if new request. Prevents that the same suggestion gets in by the same user but allows for multiple suggestions.
                            {
                                CPH.SendMessage($"Suggestion for {name} already exists; adding a request.");
								CPH.SetGlobalVar("suggestion.spamProtectionUser", currentUser, true);
								CPH.SetGlobalVar("suggestion.spamProtectionHack", name, true);
							}
							else
							{
								CPH.SendMessage($"Spamming requests is not allowed, {currentUser}");
							}
                        }
                        else // ... we add a new entry with the user name
                        {
                            reader.Close();
                            cmd.CommandText = "INSERT INTO suggestions (romhack_id, suggested_by, date_suggested) VALUES ((SELECT id FROM romhacks WHERE name = @name), @suggested_by, datetime('now'))";
                            cmd.Parameters.AddWithValue("@suggested_by", currentUser);
                            cmd.ExecuteNonQuery();
                            CPH.SendMessage($"Successfully added {name} as new suggestion, {currentUser}");
                        }
                    }

                    db.Close();
                }
            }
            catch (SQLiteException e)
            {
                if (e.ErrorCode == (int)SQLiteErrorCode.Locked)
                {
                    Console.WriteLine("The database is locked");
                }
                else
                {
                    Console.WriteLine("An error occurred while accessing the database");
                }
            }
        }

        CPH.ExecuteMethod("romhackSuggestionsToHtml", "");
        return true;
    }
}