/*
 * SMW Central Romhack Parser for Streamer.bot
 * Authors: EvilAdmiralKivi (Parser) and synthie_cat
 * Published under MIT License
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Data.SQLite;
using System.Data;

public class CPHInline
{
    public bool Execute()
    {
        // var setups
        int id;
        string name = "";
        string author = "";
        string type = "";
        int exits = 0;
        string link_id = "";
        string cover_id = "";
        string errorString = "";
        // Import Globals
        string romhackInput = CPH.GetGlobalVar<string>("romhack"); // Raw Input
        string currentUser = CPH.GetGlobalVar<string>("targetUser"); // Current user
        string broadcastUser = CPH.GetGlobalVar<string>("broadcastUser"); // Account that is set as broadcaster in Streamer.Bot for permissions check
        bool targetUserMod = CPH.GetGlobalVar<bool>("targetUserMod"); // See if current user is a moderator for permissions check
        string path = CPH.GetGlobalVar<string>("path"); // Path to where you want your files
        // Processing the inputs for commands
        var(command, romhackSearch, db, elevatedPermission) = ProcessInput(romhackInput, path, broadcastUser, targetUserMod, currentUser);
        // From where do we have to parse?
        bool romhackIsInDb = false;
        bool parse = false;
		bool parseError = false;
        using (SQLiteConnection connection = new SQLiteConnection(db)) // Reworked to prefer (faster) DB requests to parsing from SMWC
        {
            connection.Open();
            var cmd = new SQLiteCommand("SELECT * FROM romhacks WHERE name LIKE @romhackSearch", connection);
            cmd.Parameters.AddWithValue("@romhackSearch", $"%{romhackSearch}%");
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    id = (Convert.ToInt32(reader["id"]));
                    name = (reader["name"].ToString());
                    author = (reader["author"].ToString());
                    type = (reader["type"].ToString());
                    exits = (Convert.ToInt32(reader["exits"]));
                    link_id = (reader["link_id"].ToString());
                    cover_id = (reader["cover_image_id"].ToString());
                    romhackIsInDb = true;
                    reader.Close();
                    break;
                }
            }

            connection.Close();
        }

        if (!romhackIsInDb) // Romhack is not currently in Database so we parse from SMWC 
        {
            string[] commandExecution = {"id", "manual", "m", "setup"};
            RomhackInfo info = Parser.GetRomhackInfo(romhackSearch).GetAwaiter().GetResult();
            if (commandExecution.Contains(command) && info.Error != null) // Extra options that will be handled differently so we... 
            {
            // ..do nothing
            }
            else if (info.Error != null) // Check if parsing was successfull
            {
                Dictionary<string, string> errorMessages = new Dictionary<string, string>{{"http-error", "Error: HTTP Error occurred. Please try again."}, {"no-results", "Error: No results found. Make sure the Romhack exists and your spelling is correct."}, {"multiple-results", "Error: Multiple results found. Please write the complete name of the Romhack you are looking for."}};
                errorString = errorMessages[info.Error];
                parseError = true;
            }
            else // We parse from SMWC and add everything straight to the database. That way every future search, update, suggestion etc. will be faster
            {
                name = info.Name;
                author = info.Author;
                type = info.Type;
                exits = info.Exits;
                link_id = info.id;
                CPH.SetGlobalVar("info.Id", link_id, true);
                CPH.ExecuteMethod("romhackImageLoader", "");
                cover_id = CPH.GetGlobalVar<string>("info.Image");
                string insertSql = "INSERT INTO romhacks (name, author, type, link_id, cover_image_id, exits, played) " + "VALUES (@name, @author, @type, @link_id, @cover_image_id, @exits, @played)";
                using (SQLiteConnection connection = new SQLiteConnection(db))
                {
                    connection.Open();
                    var cmd = new SQLiteCommand(insertSql, connection);
                    cmd.Parameters.AddWithValue("@name", info.Name);
                    cmd.Parameters.AddWithValue("@author", info.Author);
                    cmd.Parameters.AddWithValue("@type", info.Type);
                    cmd.Parameters.AddWithValue("@link_id", info.id);
                    cmd.Parameters.AddWithValue("@cover_image_id", cover_id);
                    cmd.Parameters.AddWithValue("@exits", info.Exits);
                    cmd.Parameters.AddWithValue("@played", 0);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        if (parseError && parse)
        {
            CPH.SendMessage(errorString);
        }
        else // If no error continue
        {
            switch (command)
            {
                case "update":
                case "u":
                    if (elevatedPermission) // mods and broadcaster can update
                    {
                        CPH.SetGlobalVar("info.Name", name, true);
                        CPH.SetGlobalVar("info.Author", author, true);
                        CPH.SetGlobalVar("info.Type", type, true);
                        CPH.SetGlobalVar("info.Exits", exits, true);
                        CPH.SetGlobalVar("info.currentExits", 0, true);
                        CPH.ExecuteMethod("romhackOverlayUpdate", "");
                    }
                    else
                    {
                        CPH.SendMessage("Access denied.");
                    }

                    break;
                case "suggest":
                case "s":
                    CPH.SetGlobalVar("temp.Name", name, true);
                    CPH.SetGlobalVar("temp.CurrentUser", currentUser, true);
                    CPH.ExecuteMethod("romhackSuggestionsToDB", "");
                    break;
                case "restore":
                case "r":
                    if (elevatedPermission)
                    {
                        CPH.SetGlobalVar("info.restoreName", name, true);
                        CPH.SetGlobalVar("info.Author", author, true);
                        CPH.SetGlobalVar("info.Type", type, true);
                        CPH.SetGlobalVar("info.Exits", exits, true);
                        CPH.ExecuteMethod("romhackRestore", "");
                    }
                    else
                    {
                        CPH.SendMessage("Access denied.");
                    }

                    break;
                case "manual":
                case "m":
                    if (elevatedPermission)
                    {
                        CPH.SetGlobalVar("manualUpdate", romhackSearch, true);
                        CPH.ExecuteMethod("romhackManualUpdate", "");
                    }
                    else
                    {
                        CPH.SendMessage("Access denied.");
                    }

                    break;
                case "id":
                    if (elevatedPermission)
                    {
                        CPH.SetGlobalVar("info.ParseById", romhackSearch, true);
                        CPH.ExecuteMethod("romhackParseById", "");
                    }
                    else
                    {
                        CPH.SendMessage("Access denied.");
                    }

                    break;
                case "setup":
                    if (!CPH.ObsIsStreaming() && elevatedPermission) // Permission check: Only broadcaster is allowed to use the setup.
                    {
                        CPH.ExecuteMethod("romhackSetup", "");
                    }
                    else if (CPH.ObsIsStreaming() && elevatedPermission) // Permission check: Allow only offline install.
                    {
                        CPH.SendMessage("For security the setup is only available when not streaming. Please try again later.");
                    }
                    else
                    {
                        CPH.SendMessage("Access denied.");
                    }

                    break;
                case "help":
                case "h":
                    CPH.SendMessage("Use !romhack search [NAME] to search SMW Central for ROMHacks, or !romhack suggest [Name] to suggest a romhack.");
                    break;
                default:
                    CPH.SendMessage($"{name} is a hack by {author} with {exits} Exits. It is a {type} hack.");
                    CPH.SendMessage($"Link: https://www.smwcentral.net/?p=section&a=details&id={link_id}");
                    break;
            }
        }

        return true;
    }

    private (string command, string search, string db, bool elevatedPermission) ProcessInput(string romhackInput, string path, string broadcastUser, bool targetUserMod, string currentUser)
    {
        // Database to store progress and info
        string db = "Data Source=" + Path.Combine(CPH.GetGlobalVar<string>("path"), CPH.GetGlobalVar<string>("dbFileName")) + ";Version=3;";
        // Split raw input into parts to make it more manageable
        string[] parts = romhackInput.Split(' ');
        // first argument becomes the command
        string command = parts[0].ToLower();
        // Rebuilding the search term
        string romhackSearch = "";
        int id;
        bool isNumeric = int.TryParse(string.Join("", parts), out id);
        string[] validCommands = new string[]{"update", "u", "suggest", "s", "restore", "r", "setup", "help", "h"};
        if (!validCommands.Contains(command) && !isNumeric)
        {
            command = "";
            romhackSearch = string.Join(" ", parts);
        }
        else if (isNumeric)
        {
            command = "id";
            romhackSearch = id.ToString();
        }
        else
        {
            string[] romhackSearchArray = parts.Skip(1).ToArray();
            romhackSearch = string.Join(" ", romhackSearchArray);
        }

        bool elevatedPermission = false;
        if (currentUser == broadcastUser || targetUserMod)
        {
            elevatedPermission = true;
        }

        return (command, romhackSearch, db, elevatedPermission);
    }
}

public class Parser
{
    static public async Task<RomhackInfo> GetRomhackInfo(String romhackName)
    {
        String url = "https://www.smwcentral.net/?p=section&s=smwhacks&f[name]=" + HttpUtility.UrlEncode(romhackName);
        String content = null;
        try
        {
            HttpClient client = new HttpClient();
            content = await client.GetStringAsync(url);
        }
        catch (Exception e)
        {
            return new RomhackInfo("-", "-", 0, "-", "-", "http-error");
        }

        List<RomhackInfo> infos = Parser.ParseResponse(content);
        if (infos.Count == 0)
            return new RomhackInfo("-", "-", 0, "-", "-", "no-results");
        else if (infos.Count == 1)
            return infos[0];
        String romhackName_Parsed = Parser.GetParsedRomhackName(romhackName);
        for (int i = 0; i < infos.Count; i++)
        {
            if (Parser.GetParsedRomhackName(infos[i].Name) == romhackName_Parsed)
                return infos[i];
        }

        return new RomhackInfo("-", "-", 0, "-", "-", "multiple-results");
    }

    static private String GetParsedRomhackName(String romhackName)
    {
        String romhackName_Raw = romhackName.ToLower();
        String letters_Allowed = "1234567890" + "qwertyuiop" + "asdfghjkl" + "zxcvbnm" + " ";
        String romhackName_Parsed = "";
        for (int i = 0; i < romhackName_Raw.Length; i++)
        {
            if (letters_Allowed.IndexOf(romhackName_Raw[i]) == -1)
                continue;
            romhackName_Parsed += romhackName_Raw[i];
        }

        return romhackName_Parsed;
    }

    static private String FindNext(ref String content, String next)
    {
        int nextIndex = content.IndexOf(next);
        if (nextIndex == -1)
            return null;
        String part = content.Substring(0, nextIndex + next.Length);
        content = content.Substring(nextIndex + next.Length);
        return part;
    }

    static private String FindNextStartEnd(ref String content, String start, String end)
    {
        int startIndex = content.IndexOf(start);
        if (startIndex == -1)
            return null;
        int endIndex = content.IndexOf(end, startIndex + start.Length);
        if (endIndex == -1)
            return null;
        String temp = content.Substring(startIndex + start.Length, endIndex - (startIndex + start.Length));
        content = content.Substring(endIndex + end.Length);
        return temp;
    }

    static private String FindStartEnd(String content, String start, String end)
    {
        int startIndex = content.IndexOf(start);
        if (startIndex == -1)
            return null;
        int endIndex = content.IndexOf(end, startIndex + start.Length);
        if (endIndex == -1)
            return null;
        return content.Substring(startIndex + start.Length, endIndex - (startIndex + start.Length));
    }

    static private List<RomhackInfo> ParseResponse(String html)
    {
        String next = null;
        String table = Parser.FindStartEnd(html, "<table class=\"list\">", "</table>");
        if (table == null)
            return null;
        String tbody = Parser.FindStartEnd(table, "<tbody>", "</tbody>");
        if (tbody == null)
            return null;
        List<RomhackInfo> infos = new List<RomhackInfo>();
        while (true)
        {
            String tr = Parser.FindNextStartEnd(ref tbody, "<tr>", "</tr>");
            if (tr == null)
                break;
            /* Name */
            String nameTd = Parser.FindNextStartEnd(ref tr, "<td class=\"text\">", "</td>");
            if (nameTd == null)
                break;
            String id = FindStartEnd(nameTd, "href=", ">");
            if (id == null)
                break;
            String name = Parser.FindStartEnd(nameTd, "<a ", "</a>");
            if (name == null)
                break;
            next = Parser.FindNext(ref name, ">");
            if (next == null)
                break;
            /* No */
            String noTd = Parser.FindNextStartEnd(ref tr, "<td>", "</td>");
            if (noTd == null)
                break;
            noTd = Parser.FindNextStartEnd(ref tr, "<td>", "</td>");
            if (noTd == null)
                break;
            /* Exits */
            String exitsTd = Parser.FindNextStartEnd(ref tr, "<td>", " exit(s)</td>");
            if (exitsTd == null)
                break;
            int exits = 0;
            try
            {
                exits = Int32.Parse(exitsTd);
            }
            catch (Exception ex)
            {
            // Do nothing
            }

            /* Type */
            String type = Parser.FindNextStartEnd(ref tr, "<td>", "</td>");
            if (type == null)
                break;
            /* Author */
            String authorTd = Parser.FindNextStartEnd(ref tr, "<td>", "</td>");
            if (authorTd == null)
                break;
            String author = Parser.FindStartEnd(authorTd, "<a ", "</a>");
            if (author == null)
                break;
            next = Parser.FindNext(ref author, ">");
            if (author == null)
                break;
            infos.Add(new RomhackInfo(name, id, exits, type, author, null));
        }

        return infos;
    }
}

public class RomhackInfo
{
    public String Name;
    public int Exits;
    public String Type;
    public String Author;
    public String id;
    public String Error;
    public RomhackInfo(string name, string id, int exits, string type, string author, String error)
    {
        this.Name = name;
        this.Exits = exits;
        this.Type = type;
        this.Author = author;
        this.Error = error;
        var match = Regex.Match(id, @"(\d+)");
        this.id = match.ToString();
    }
}