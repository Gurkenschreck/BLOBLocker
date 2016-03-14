using CryptoPool.Entities.Models.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CryptoPool.WebApp.App_Start
{
    public static class TaskConfig
    {
        public static void StartBackgroundService(HttpApplicationState application, CancellationToken cancellationToken)
        {
            var task = Task.Factory.StartNew(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                using (CryptoPoolContext context = new CryptoPoolContext())
                {
                    SystemConfiguration configChangedItem;
                    bool firstRun = true;

                    do
                    {
                        configChangedItem = context.SystemConfigurations.FirstOrDefault(p => p.Key == "ConfigChanged");
                        bool configChanged = firstRun ? firstRun : bool.Parse(configChangedItem.Value);

                        if (configChanged)
                        {
                            lock (application)
                            {
                                foreach (SystemConfiguration conf in context.SystemConfigurations)
                                {
                                    application[conf.Key] = conf.Value;
                                }
                            }
                            configChangedItem.Value = "false";
                            context.SaveChangesAsync();
                        }
                        firstRun = false;
                        Thread.Sleep(30000);
                    } while (!cancellationToken.IsCancellationRequested);
                }
            }, cancellationToken);
        }
    }

}