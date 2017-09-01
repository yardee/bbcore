﻿using Lib.DiskCache;
using Lib.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Lib.TSCompiler
{
    public class TSFileAdditionalInfo
    {
        public IFileCache Owner { get; set; }
        public IDiskCache DiskCache { get; set; }
        public List<int> LastCompilationCacheIds { get; set; }
        public TSFileAdditionalInfo DtsLink { get; set; }
        public TSFileAdditionalInfo JsLink { get; set; }
        public SourceMap MapLink { get; set; }

        List<TSProject> _moduleImports;

        public IReadOnlyList<TSProject> ModuleImports { get => _moduleImports; }

        private TSFileAdditionalInfo()
        {
        }

        public void StartCompiling()
        {
            _moduleImports = new List<TSProject>();
            _localImports = new List<TSFileAdditionalInfo>();
            _diag = null;
        }

        public void ImportingModule(TSProject name)
        {
            _moduleImports.Add(name);
        }

        List<TSFileAdditionalInfo> _localImports;
        List<Diag> _diag;

        public IReadOnlyList<TSFileAdditionalInfo> LocalImports { get => _localImports; }
        public string ImportedAsModule { get; internal set; }

        public void ImportingLocal(TSFileAdditionalInfo name)
        {
            _localImports.Add(name);
        }

        public void ReportDiag(bool isError, int code, string text, int startLine, int startCharacter, int endLine, int endCharacter)
        {
            if (_diag == null) _diag = new List<Diag>();
            _diag.Add(new Diag(isError, code, text, startLine, startCharacter, endLine, endCharacter));
        }

        public static TSFileAdditionalInfo Get(IFileCache file, IDiskCache diskCache)
        {
            if (file == null) return null;
            if (file.AdditionalInfo == null)
                file.AdditionalInfo = new TSFileAdditionalInfo { Owner = file, DiskCache = diskCache };
            return (TSFileAdditionalInfo)file.AdditionalInfo;
        }

        public bool NeedsCompilation()
        {
            if (JsLink == null || MapLink == null || _moduleImports == null || _localImports == null || LastCompilationCacheIds == null)
            {
                return true;
            }
            if (Enumerable.SequenceEqual(LastCompilationCacheIds, BuildLastCompilationCacheIds()))
            {
                return false;
            }
            return true;
        }

        public IEnumerable<int> BuildLastCompilationCacheIds()
        {
            yield return Owner.ChangeId;
            if (_moduleImports != null) foreach (var module in _moduleImports)
                {
                    yield return module.InterfaceChangeId;
                }
            if (_localImports != null) foreach (var local in _localImports)
                {
                    if (local.DtsLink != null)
                    {
                        yield return local.DtsLink.Owner.ChangeId;
                    }
                    else
                    {
                        yield return local.Owner.ChangeId;
                    }
                }
        }

        public void RememberLastCompilationCacheIds()
        {
            LastCompilationCacheIds = BuildLastCompilationCacheIds().ToList();
        }

        private class Diag
        {
            private bool isError;
            private int code;
            private string text;
            private int startLine;
            private int startCharacter;
            private int endLine;
            private int endCharacter;

            public Diag(bool isError, int code, string text, int startLine, int startCharacter, int endLine, int endCharacter)
            {
                this.isError = isError;
                this.code = code;
                this.text = text;
                this.startLine = startLine;
                this.startCharacter = startCharacter;
                this.endLine = endLine;
                this.endCharacter = endCharacter;
            }
        }
    }
}