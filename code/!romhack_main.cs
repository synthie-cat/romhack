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

public class CPHInline
{
    public bool Execute()
    {
        // Import Globals
        string romhackInput = CPH.GetGlobalVar<string>("romhack"); // Raw Input
        int currentExits = CPH.GetGlobalVar<int>("info.currentExits"); // Get current progress for restore
        int infoExits = CPH.GetGlobalVar<int>("info.Exits"); // Get exits of current hack for restore
        /* 
		Permissions 
		*/
        string currentUser = CPH.GetGlobalVar<string>("targetUser"); // Current user
        string broadcastUser = CPH.GetGlobalVar<string>("broadcastUser"); // Account that is set as broadcaster in Streamer.Bot for permissions check
        bool targetUserMod = CPH.GetGlobalVar<bool>("targetUserMod"); // See if current user is a moderator for permissions check
        /* 
		File Path
		*/
        string directory = CPH.GetGlobalVar<string>("path"); // Path to where you want your files
        /* 
		Processing the inputs for commands
		*/
        var(command, romhackSearch, romhackBackup, logTime, historyPath, suggestionsPath) = ProcessInput(romhackInput, directory);
        /* 
		Error dictionary
		*/
        Dictionary<string, string> errorMessages = new Dictionary<string, string>{{"http-error", "Error: HTTP Error occurred. Please try again."}, {"no-results", "Error: No results found. Make sure the Romhack exists and your spelling is correct."}, {"multiple-results", "Error: Multiple results found. Please write the complete name of the Romhack you are looking for."}};
        /* 
		Parse the Input
		*/
        RomhackInfo info = Parser.GetRomhackInfo(romhackSearch).GetAwaiter().GetResult();
        if (info.Error != null) // If there's an error we cannot proceed; tell the user what went wrong
        {
            CPH.SendMessage(errorMessages[info.Error]);
        }
        else // If no error continue
        {
            switch (command)
            {
                case "search": // Simple search that returns the hack
                    CPH.SendMessage($"{info.Name} is a hack by {info.Author} with {info.Exits} Exits. It is a {info.Type} hack.");
                    CPH.SendMessage($"Link: {info.Url}");
                    break;
                case "update":
                    if (currentUser == broadcastUser || targetUserMod)
                    { // Backup
                        CPH.ExecuteMethod("romhackBackup", "");
                        // Update Globals - I am using globals here to also use them in other parts of my overlay.
                        Dictionary<string, object> infoDict = new Dictionary<string, object>()
                        {{"Name", info.Name}, {"Author", info.Author}, {"Type", info.Type}, {"Url", info.Url}, {"Exits", info.Exits}};
                        foreach (var item in infoDict)
                        {
                            CPH.SetGlobalVar("info." + item.Key, item.Value, true);
                        }

                        CPH.SetGlobalVar("info.currentExits", 0, true); // Set counter to 0. Doing it here to allow restore older states
                        CPH.ExecuteMethod("romhackOverlayUpdate", "");
                    }
                    else
                    {
                        CPH.SendMessage("Permission denied.");
                    }

                    break;
                case "suggest":
                    if (File.Exists(suggestionsPath)) // Add the information of the suggestion to the table
                    {
                        File.AppendAllText(suggestionsPath, $"| {logTime} | {info.Name} | {info.Author} | {info.Exits} | {info.Type} | {info.Url} | {currentUser} | " + Environment.NewLine);
                        CPH.SendMessage($"Successfully added {info.Name} to the suggestions, {currentUser}");
                    }
                    else // Error if suggestions file does not exist.
                    {
                        CPH.SendMessage("Suggestions file does not exist. Please check that the file is in your bots folder and writeable.");
                    }

                    break;
                case "restore":
                    CPH.ExecuteMethod("romhackRestore", "");
                    break;
                case "setup":
                    if (!CPH.ObsIsStreaming() && currentUser == broadcastUser) // Permission check: Only broadcaster is allowed to use the setup.
                    {
                        CPH.ExecuteMethod("romhackSetup", "");
                    }
                    else if (CPH.ObsIsStreaming() && currentUser == broadcastUser) // Permission check: Allow only offline install.
                    {
                        CPH.SendMessage("For security the setup is only available when not streaming. Please try again later.");
                    }
                    else // Permission check: Not permitted.
                    {
                        CPH.SendMessage("Permission denied.");
                    }

                    break;
                default:
                    CPH.SendMessage("Use !romhack search [NAME] to search SMW Central for ROMHacks, or !romhack suggest [Name] to suggest a romhack.");
                    break;
            }
        }

        return true;
    }

    private (string command, string search, string backup, string logTime, string historyPath, string suggestionsPath) ProcessInput(string input, string directory)
    {
        // Split raw input into parts to make it more manageable
        string[] parts = input.Split(' ');
        // first argument becomes the command
        string command = parts[0].ToLower();
        // Rebuilding the search term
        string[] romhackSearchArray = parts.Skip(1).ToArray();
        string romhackSearch = string.Join(" ", romhackSearchArray);
        // Creation of house-keeping variables
        string logTime = DateTime.Now.ToString("dd.MM." + "HH:mm");
        string romhackBackup = "[" + logTime + "] " + input;
        string historyPath = Path.Combine(directory, "_history.txt"); // File that shows hacks you finished
        string suggestionsPath = Path.Combine(directory, "_suggestions.md"); // Hack suggestions file
        return (command, romhackSearch, romhackBackup, logTime, historyPath, suggestionsPath);
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
            String url = FindStartEnd(nameTd, "href=", ">");
            if (url == null)
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
            infos.Add(new RomhackInfo(name, url, exits, type, author, null));
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
    public String Url;
    public String Error;
    public RomhackInfo(string name, string url, int exits, string type, string author, String error)
    {
        this.Name = name;
        this.Exits = exits;
        this.Type = type;
        this.Author = author;
        this.Error = error;
        var match = Regex.Match(url, @"id=(\d+)");
        string id = match.Groups[1].Value;
        this.Url = "https://www.smwcentral.net/?p=section&a=details&id=" + id;
    }
}