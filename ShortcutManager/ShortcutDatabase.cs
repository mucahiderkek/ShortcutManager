using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class ShortcutDatabase
{
    private static string databasePath = "shortcuts.txt";

    public static void SaveShortcut(string targetPath)
    {
        using (StreamWriter writer = new StreamWriter(databasePath, true))
        {
            writer.WriteLine(targetPath);
        }
    }

    public static List<string> LoadShortcuts()
    {
        List<string> shortcuts = new List<string>();
        if (File.Exists(databasePath))
        {
            using (StreamReader reader = new StreamReader(databasePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    shortcuts.Add(line);
                }
            }
        }
        return shortcuts;
    }

    public static void DeleteShortcut(string targetPath)
    {
        var shortcuts = LoadShortcuts();
        shortcuts.Remove(targetPath);
        File.WriteAllLines(databasePath, shortcuts);
    }
}