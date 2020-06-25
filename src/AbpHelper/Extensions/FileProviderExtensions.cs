﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Volo.Abp;

namespace EasyAbp.AbpHelper.Extensions
{
    public static class FileProviderExtensions
    {
        public static IEnumerable<(string, IFileInfo)> GetFilesRecursively(this IFileProvider fileProvider, string subpath)
        {
            subpath = subpath.EnsureEndsWith('/');
            var contents = fileProvider.GetDirectoryContents(subpath);
            foreach (var content in contents)
            {
                if (content.IsDirectory)
                {
                    string path = subpath + content.Name;
                    foreach (var file in GetFilesRecursively(fileProvider, path))
                    {
                        yield return file;
                    }
                }
                else
                {
                    yield return (subpath + content.Name, content);
                }
            }
        }
    }
}