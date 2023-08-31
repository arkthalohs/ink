﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Ink
{
    public class PluginManager
    {
        public PluginManager (List<string> pluginDirectories)
        {
            _plugins = new List<IPlugin> ();

            foreach (string pluginName in pluginDirectories) 
            {
                foreach (string file in Directory.GetFiles(pluginName, "*.dll"))
                {
                    foreach (Type type in Assembly.LoadFile(Path.GetFullPath(file)).GetExportedTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type)
                            && type.GetConstructor(new Type[0]) != null
                            && !type.IsGenericTypeDefinition
                            && !type.IsAbstract
                            && !type.IsInterface)
                        {
                            _plugins.Add((IPlugin)Activator.CreateInstance(type));
                        }
                    }
                }
            }
        }

        public string PreParse(string storyContent, ErrorHandler externalErrorHandler = null)
        {
            foreach (IPlugin plugin in _plugins)
            {
                plugin.PreParse(ref storyContent, externalErrorHandler);
            }

            return storyContent;
        }

        public Parsed.Story PostParse(Parsed.Story parsedStory, ErrorHandler externalErrorHandler = null)
        {
            foreach (IPlugin plugin in _plugins)
            {
                plugin.PostParse(ref parsedStory, externalErrorHandler);
            }

            return parsedStory;
        }

        public Runtime.Story PostExport(Parsed.Story parsedStory, Runtime.Story runtimeStory, ErrorHandler externalErrorHandler = null)
        {
            foreach (IPlugin plugin in _plugins)
            {
                plugin.PostExport(parsedStory, ref runtimeStory, externalErrorHandler);
            }

            return runtimeStory;
        }

        List<IPlugin> _plugins;
    }
}

