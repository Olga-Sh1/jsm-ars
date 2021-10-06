using BFSettings;
using JSMBase.Medicine.DAL;
using JSMBase.Medicine.RWServices.Excel;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    class SimpleModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ExcelParser>().ToSelf();
            Bind<ExcelReader>().ToSelf();
            Bind<SettingsSaverService>().ToSelf();
        }
    }
}
