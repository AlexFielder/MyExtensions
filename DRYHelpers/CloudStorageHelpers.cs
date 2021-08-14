using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using log4net;
using MoreLinq;
using System.IO;

namespace DRYHelpers
{
    public class CloudStorageHelpers
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CloudStorageHelpers));
        /// <summary>
        /// Gets the OneDrive folder path wherever it is located.
        /// </summary>
        /// <returns></returns>
        public static string getOneDriveFolderPath()
        {
            dynamic value1 = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\SkyDrive", "UserFolder", null);
            dynamic path1 = value1 as string;
            if (path1 != null && System.IO.Directory.Exists(path1))
                return path1;
            dynamic value2 = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\SkyDrive", "UserFolder", null);
            dynamic path2 = value2 as string;
            if (path2 != null && System.IO.Directory.Exists(path2))
                return path2;
            dynamic value3 = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\OneDrive", "UserFolder", null);
            dynamic path3 = value3 as string;
            if (path3 != null && System.IO.Directory.Exists(path3))
                return path3;
            return null;
        }

        /// <summary>
        /// Gets the Dropbox folder path wherever it is located.
        /// </summary>
        /// <returns></returns>
        public static string getDropBoxFolderPath()
        {
            dynamic infoPath = "Dropbox\\info.json";
            //Dim Appdata As String = Environment.SpecialFolder.LocalApplicationData
            string jsonPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), infoPath);
            //Dim jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath)
            //debug only
            //ProcessFiles.UpdateStatusBar(jsonPath)
            log.Info("Possible Dropbox LocalApplicationData path= " + jsonPath);
            if (!System.IO.File.Exists(jsonPath))
            {
                jsonPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), infoPath);
                //jsonPath = Path.Combine(Environment.GetEnvironmentVariable("AppData"), infoPath)
            }
            log.Info("Possible Dropbox ApplicationData path= " + jsonPath);
            //debug only
            //ProcessFiles.UpdateStatusBar(jsonPath)

            if (!System.IO.File.Exists(jsonPath))
            {
                throw new Exception("Dropbox could not be found!");
            }
            string dropboxPath = File.ReadAllText(jsonPath).Split('\"')[5].Replace(@"\\", @"\");
            return dropboxPath;
        }
    }
}
