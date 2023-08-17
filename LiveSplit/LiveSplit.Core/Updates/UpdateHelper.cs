using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UpdateManager;

namespace LiveSplit.Updates
{
    public static class UpdateHelper
    {
        public static readonly Version Version = Version.Parse($"{ Git.LastTag }.{ Git.CommitsSinceLastTag }");
        public static string UserAgent => $"LiveSplit/{ Version }";

        public static readonly List<Type> AlreadyChecked = new List<Type>();

        public static void Update(Form form, Action closeAction, params IUpdateable[] updateables)
        {
            Task.Factory.StartNew(() =>
            {
                //No updates for this LiveSplit.
            });
        }
    }
}
