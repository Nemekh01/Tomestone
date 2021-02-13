using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Lumina.Data.Files;
using Cyalume = Lumina.Lumina;

namespace LuminaIconExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            var lumina = new Cyalume(args[0]);

            var start = int.Parse(args[1]);
            var end = int.Parse(args[2]);

            var outPath = new DirectoryInfo("out");

            if (!outPath.Exists)
                outPath.Create();

            for (var i = start; i < end; i++)
            {
                var icon = GetIcon(lumina, i);

                if (icon == null) 
                    continue;

                Console.WriteLine($"-> {i:D6}");
                var folder = outPath.CreateSubdirectory($"{i / 1000:D3}000");

                GetImage(icon).Save(Path.Combine(folder.FullName, $"{i:D6}.png"), ImageFormat.Png);

            }
        }

        private enum ClientLanguage
        {
            Japanese,
            English,
            German,
            French
        }

        /// <summary>
        /// Get a <see cref="TexFile"/> containing the icon with the given ID.
        /// </summary>
        /// <param name="iconId">The icon ID.</param>
        /// <returns>The <see cref="TexFile"/> containing the icon.</returns>
        private static TexFile GetIcon(Cyalume lumina, int iconId)
        {
            return GetIcon(lumina, ClientLanguage.English, iconId);
        }

        /// <summary>
        /// Get a <see cref="TexFile"/> containing the icon with the given ID, of the given language.
        /// </summary>
        /// <param name="iconLanguage">The requested language.</param>
        /// <param name="iconId">The icon ID.</param>
        /// <returns>The <see cref="TexFile"/> containing the icon.</returns>
        private static TexFile GetIcon(Cyalume lumina, ClientLanguage iconLanguage, int iconId)
        {
            var type = iconLanguage switch
            {
                ClientLanguage.Japanese => "ja/",
                ClientLanguage.English => "en/",
                ClientLanguage.German => "de/",
                ClientLanguage.French => "fr/",
                _ => throw new ArgumentOutOfRangeException(nameof(iconLanguage),
                    "Unknown Language: " + iconLanguage)
            };

            return GetIcon(lumina, type, iconId);
        }

        private const string IconFileFormat = "ui/icon/{0:D3}000/{1}{2:D6}.tex";

        /// <summary>
        /// Get a <see cref="TexFile"/> containing the icon with the given ID, of the given type.
        /// </summary>
        /// <param name="type">The type of the icon (e.g. 'hq' to get the HQ variant of an item icon).</param>
        /// <param name="iconId">The icon ID.</param>
        /// <returns>The <see cref="TexFile"/> containing the icon.</returns>
        private static TexFile GetIcon(Cyalume lumina, string type, int iconId)
        {
            type ??= string.Empty;
            if (type.Length > 0 && !type.EndsWith("/"))
                type += "/";

            var filePath = string.Format(IconFileFormat, iconId / 1000, type, iconId);
            var file = lumina.GetFile<TexFile>(filePath);

            if (file != default(TexFile) || type.Length <= 0) return file;

            // Couldn't get specific type, try for generic version.
            filePath = string.Format(IconFileFormat, iconId / 1000, string.Empty, iconId);
            file = lumina.GetFile<TexFile>(filePath);
            return file;
        }

        private static unsafe Image GetImage(TexFile tex)
        {
            // this is terrible please find something better or get rid of .net imaging altogether
            Image image;
            fixed (byte* p = tex.ImageData)
            {
                var ptr = (IntPtr)p;
                using var tempImage = new Bitmap(tex.Header.Width, tex.Header.Height, tex.Header.Width * 4, PixelFormat.Format32bppArgb, ptr);
                image = new Bitmap(tempImage);
            }

            return image;
        }
    }
}
