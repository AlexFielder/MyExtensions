using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using MyExtensions;

namespace MyExtensions
{
    public class MyIVPluginLoader<T>
    {
        private CompositionContainer _Container;
        public static readonly ILog log = LogManager.GetLogger(typeof(MyExtensionsServer));

        [ImportMany]
        public IEnumerable<T> Plugins
        {
            get;

            set;
        }

        public MyIVPluginLoader(string path)
        {
            try
            {
                DirectoryCatalog directoryCatalog = new DirectoryCatalog(path);

                var catalog = new AggregateCatalog(directoryCatalog);

                _Container = new CompositionContainer(catalog);

                _Container.ComposeParts(this);
            }
            catch (System.Exception e)
            {
                if (e is System.Reflection.ReflectionTypeLoadException)
                {
                    //not sure if this will work or not:
                    var typeLoadException = e as System.Reflection.ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    foreach (var item in loaderExceptions)
                    {
                        log.Error(item.Message, item);
                    }
                }
                else
                {
                    log.Error(e.Message, e);
                }

                //log.Error(e.ToString());
            }
        }
    }
}
