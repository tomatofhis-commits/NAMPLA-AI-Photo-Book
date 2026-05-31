using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Storage;

namespace Nanpla.Models
{
    public static class AlbumManager
    {
        private const string UnlockedImagesKey = "UnlockedImagesList";
        private static readonly string PhotoDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo");

        public static HashSet<string> GetUnlockedImages()
        {
            var serialized = Preferences.Default.Get(UnlockedImagesKey, string.Empty);
            if (string.IsNullOrEmpty(serialized))
            {
                return new HashSet<string>();
            }
            return new HashSet<string>(serialized.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public static void UnlockImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            var unlocked = GetUnlockedImages();
            if (!unlocked.Contains(fileName))
            {
                unlocked.Add(fileName);
                var serialized = string.Join(",", unlocked);
                Preferences.Default.Set(UnlockedImagesKey, serialized);
            }
        }

        public static List<string> GetAllImages()
        {
            try
            {
                // 1. 物理ディレクトリが存在する場合 (Windows開発・デバッグ環境など)
                if (Directory.Exists(PhotoDirPath))
                {
                    var list = Directory.GetFiles(PhotoDirPath, "*.jpg")
                                     .Select(Path.GetFileName)
                                     .Where(name => !string.IsNullOrEmpty(name) && name != "title.jpg" && !name.StartsWith("thumb_"))
                                     .Cast<string>()
                                     .OrderBy(x => x)
                                     .ToList();

                    // 自動的にファイル一覧インデックス (images_list.txt) を更新保存
                    try
                    {
                        var indexPath = Path.Combine(PhotoDirPath, "images_list.txt");
                        File.WriteAllLines(indexPath, list);
                    }
                    catch (Exception writeEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to write images_list.txt: {writeEx.Message}");
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to scan physical directory {PhotoDirPath}: {ex.Message}");
            }

            // 2. 物理ディレクトリがない場合 (Androidパッケージ実行時など)
            try
            {
                // MAUI の Asset 経由で images_list.txt から画像一覧をロード
                var task = FileSystem.OpenAppPackageFileAsync("photo/images_list.txt");
                using (var stream = task.GetAwaiter().GetResult())
                using (var reader = new StreamReader(stream))
                {
                    var list = new List<string>();
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            list.Add(line);
                        }
                    }
                    if (list.Count > 0)
                    {
                        return list.OrderBy(x => x).ToList();
                    }
                }
            }
            catch (Exception assetEx)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load images_list.txt from app package: {assetEx.Message}");
            }

            return new List<string> { "000.jpg" };
        }
    }
}
