using System.Collections.Generic;
using System.Data.SQLite;

internal static class ShortcutManagerHelpers
{
    private static string dbPath;

    // Kısayolları yükle
    public static List<ShortcutManager.ShortcutInfo> LoadShortcuts()
        {
            var shortcuts = new List<ShortcutManager.ShortcutInfo>();
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.Open();
                string selectCommand = "SELECT TargetPath, IconPath FROM Shortcuts;";
                using (var command = new SQLiteCommand(selectCommand, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        shortcuts.Add(new ShortcutManager.ShortcutInfo
                        {
                            TargetPath = reader.GetString(0),
                            IconPath = reader.GetString(1)
                        });
                    }
                }
            }
            return shortcuts;
        }

    // Kısayolu kaydet
    public static void SaveShortcut(ShortcutManager.ShortcutInfo shortcut)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            string insertCommand = "INSERT INTO Shortcuts (TargetPath, IconPath) VALUES (@TargetPath, @IconPath);";
            using (var command = new SQLiteCommand(insertCommand, connection))
            {
                command.Parameters.AddWithValue("@TargetPath", shortcut.TargetPath);
                command.Parameters.AddWithValue("@IconPath", shortcut.IconPath);
                command.ExecuteNonQuery();
            }
        }
    }
}