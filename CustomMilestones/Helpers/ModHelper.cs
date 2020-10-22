using ColossalFramework.Plugins;
using ICities;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CustomMilestones.Helpers
{
    public class ModHelper
    {
        public static string GetModVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            StringBuilder stringBuilder = new StringBuilder();
            if (version.Major > 1)
            {
                stringBuilder.Append(version.Major);
                if (version.Minor > 0)
                {
                    stringBuilder.Append(".");
                    stringBuilder.Append(version.Minor);
                }
            }
            return stringBuilder.ToString();
        }

        public static string GetModPath()
        {
            string path = string.Empty;
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                try
                {
                    IUserMod[] mods = plugin.GetInstances<IUserMod>();
                    if (mods.FirstOrDefault() is CustomMilestonesMod)
                    {
                        path = plugin.modPath;
                    }
                }
                catch { }
            }
            return path;
        }
    }
}
