﻿using System;
using System.IO;
using Lib.DiskCache;
using Lib.Watcher;
using Lib.Utils;
using Lib.ToolsDir;
using Lib.TSCompiler;

namespace bb
{
    class Program
    {
        static void Main(string[] args)
        {
            var composition = new Lib.Composition.Composition();
            composition.InitTools("2.4.2");
            composition.InitDiskCache();
            composition.AddProject("c:/Research/bobril/bbExamples/clickingSpree");
            composition.StartWebServer(null);
            composition.InitInteractiveMode();
            composition.WaitForStop();
            /*
            var t = new Lib.Test.CompilerTests();
            t.IfaceChangingLocalLibRecompilesBothFiles();
            */
            /*
            var bbdir = PathUtils.Join(PathUtils.Normalize(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)), ".bbcore");
            var tools = new ToolsDir(PathUtils.Join(bbdir, "tools"));
            tools.InstallTypeScriptVersion("2.4.2");
            Console.WriteLine(tools.GetTypeScriptVersion());
            var dc = new DiskCache(new NativeFsAbstraction(), () => new ModulesLinksOsWatcher());
            dc.AddRoot(tools.TypeScriptLibDir);
            var sampleProjectDir = PathUtils.Normalize(new DirectoryInfo("c:/Research/WebBobwai").FullName);
            dc.AddRoot(sampleProjectDir);
            var ctx = new BuildCtx(new TSCompilerPool(tools));
            var dirCache = dc.TryGetItem(sampleProjectDir) as IDirectoryCache;
            var proj = TSProject.Get(dirCache, dc);
            proj.Build(ctx);
            */
            /*
            var compiler = new TSCompiler(tools);
            compiler.DiskCache = dc;
            compiler.createProgram(sampleProjectDir, new[] { "index.ts", "example.ts" });
            compiler.compileProgram();
            compiler.emitProgram();
            */
            //List(dc, dc.Root());
            //Console.ReadLine();
        }

        static void List(DiskCache cache, IDirectoryCache directoryCache)
        {
            cache.UpdateIfNeeded(directoryCache);
            foreach (var item in directoryCache)
            {
                if (item.IsDirectory)
                {
                    Console.WriteLine("D:" + item.FullPath);
                    List(cache, (IDirectoryCache)item);
                }
                else
                {
                    Console.WriteLine("F:" + item.FullPath + " " + ((IFileCache)item).Length);
                }
            }
        }
    }
}