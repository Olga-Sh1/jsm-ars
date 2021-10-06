using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    /// <summary>Сервис для временной папки</summary>
    public class TempFolderService
    {
        #region Временная папка
        public void CreateTemp()
        {
            string tmp = TempDir;
            try
            {
                if (!System.IO.Directory.Exists(tmp)) System.IO.Directory.CreateDirectory(tmp);
            }
            catch { }
        }
        public void CleanTemp()
        {
            var tmp = TempDir;
            try
            {
                if (System.IO.Directory.Exists(tmp))
                {
                    var subs = System.IO.Directory.GetDirectories(tmp);
                    foreach (var sub in subs)
                        System.IO.Directory.Delete(sub, true);
                }
            }
            catch { }
        }

        public string TempDir
        {
            get
            {
                string dir = System.IO.Path.GetTempPath();
                string tmp_d = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().GetName().Name);
                return System.IO.Path.Combine(dir, tmp_d);
            }
        }

        #endregion
    }
}
