using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortcutManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Form yüklendiğinde kayıtlı kısayolları yükle
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadShortcuts();
        }

        // Kısayolları veritabanından yükle ve UI'ya ekle
        private void LoadShortcuts()
        {
            List<string> shortcuts = ShortcutDatabase.LoadShortcuts();

            foreach (var targetPath in shortcuts)
            {
                if (File.Exists(targetPath))
                {
                    Icon icon = Icon.ExtractAssociatedIcon(targetPath);
                    AddShortcutToPanel(icon, targetPath);
                }
            }
        }

        // Kısayolu FlowLayoutPanel'e ekle
        private void AddShortcutToPanel(Icon icon, string targetPath)
        {
            Button shortcutButton = new Button();
            shortcutButton.Size = new Size(100, 100);

            // Dosya adını uzantısından ayırarak sadece ismini al
            shortcutButton.Text = Path.GetFileNameWithoutExtension(targetPath);
            shortcutButton.Image = icon.ToBitmap();
            shortcutButton.TextImageRelation = TextImageRelation.ImageAboveText;

            // Kısayola tıklandığında uygulamayı başlat
            shortcutButton.Click += (sender, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(targetPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Uygulama başlatılamadı: {ex.Message}");
                }
            };

            Button deleteButton = new Button();
            deleteButton.Size = new Size(30, 30);
            deleteButton.Text = "X";
            deleteButton.BackColor = Color.Red;
            deleteButton.ForeColor = Color.White;

            deleteButton.Click += async (sender, e) =>
            {
                var result = MessageBox.Show("Kısayolu silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await DeleteShortcutAsync(targetPath);
                        MessageBox.Show("Kısayol başarıyla silindi.");
                        flowLayoutPanel1.Controls.Remove(shortcutButton);
                        flowLayoutPanel1.Controls.Remove(deleteButton);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata: {ex.Message}");
                    }
                }
            };

            deleteButton.Location = new Point(shortcutButton.Width - deleteButton.Width, 0);
            flowLayoutPanel1.Controls.Add(shortcutButton);
            flowLayoutPanel1.Controls.Add(deleteButton);
        }

        public static async Task DeleteShortcutAsync(string targetPath)
        {
            await Task.Run(() => ShortcutDatabase.DeleteShortcut(targetPath));
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Uygulama ve Kısayol Dosyaları (*.exe;*.lnk)|*.exe;*.lnk|Tüm Dosyalar (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    if (File.Exists(filePath))
                    {
                        ShortcutDatabase.SaveShortcut(filePath);
                        Icon icon = Icon.ExtractAssociatedIcon(filePath);
                        AddShortcutToPanel(icon, filePath);
                    }
                }
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Bu metot kullanılmıyor, isterseniz kaldırabilirsiniz.
        }
    }

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
}