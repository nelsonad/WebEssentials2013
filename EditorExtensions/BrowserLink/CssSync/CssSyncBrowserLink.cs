﻿using Microsoft.VisualStudio.Web.BrowserLink;
using System.ComponentModel.Composition;
using System.IO;

namespace MadsKristensen.EditorExtensions
{
    [Export(typeof(BrowserLinkExtensionFactory))]
    [BrowserLinkFactoryName("CssSync")] // Not needed in final version of VS2013
    public class CssSyncFactory : BrowserLinkExtensionFactory
    {
        private static CssSync _extension;

        public override BrowserLinkExtension CreateInstance(BrowserLinkConnection connection)
        {
            // Instantiate the extension as a singleton
            if (_extension == null)
            {
                _extension = new CssSync();
            }

            return _extension;
        }

        public override string Script
        {
            get
            {
                using (Stream stream = GetType().Assembly.GetManifestResourceStream("MadsKristensen.EditorExtensions.BrowserLink.CssSync.CssSyncBrowserLink.js"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    public class CssSync : BrowserLinkExtension
    {
        private FileSystemWatcher _fsw;

        public override void OnConnected(BrowserLinkConnection connection)
        {
            if (_fsw == null)
            {
                string path = connection.Project.Properties.Item("FullPath").Value.ToString();

                // Check if the FullPath is a file and if so, get the directory. This is for Website Project compat.
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                
                Watch(path);
            }
        }

        public override void OnDisconnecting(BrowserLinkConnection connection)
        {
            if (_fsw != null)
            {
                _fsw.Changed -= RefreshStyles;
                _fsw.Created -= RefreshStyles;
                _fsw.Deleted -= RefreshStyles;
                _fsw.Dispose();
                _fsw = null;
            }
        }

        private void Watch(string path)
        {            
            _fsw = new FileSystemWatcher(path, "*.css");
            _fsw.Changed += RefreshStyles;
            _fsw.Created += RefreshStyles;
            _fsw.Deleted += RefreshStyles;
            _fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.DirectoryName;
            _fsw.IncludeSubdirectories = true;
            _fsw.EnableRaisingEvents = true;
        }

        private void RefreshStyles(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith(".css"))
            {
                Clients.CallAll("refresh");
            }
        }
    }
}