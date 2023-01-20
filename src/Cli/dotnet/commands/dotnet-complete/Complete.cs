// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine.Completions;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.Tools;
using static System.Array;
using System.Collections.Generic;

namespace Microsoft.DotNet.Cli
{
    internal static class Complete
    {
        private static CompletionItem ToCompletionItem(string s) => new CompletionItem(s);

        public static IEnumerable<CompletionItem> TargetFrameworksFromProjectFile(CompletionContext _)
        {
            try
            {
                return GetMSBuildProject()?.GetTargetFrameworks().Select(tf => tf.GetShortFolderName()).Select(ToCompletionItem) ?? Empty<CompletionItem>();
            }
            catch (Exception)
            {
                return Empty<CompletionItem>();
            }
        }

        private static void Report(Exception e) =>
            Reporter.Verbose.WriteLine($"Exception occurred while getting completions: {e}");

        public static IEnumerable<CompletionItem> RunTimesFromProjectFile(CompletionContext _)
        {
            try
            {
                return GetMSBuildProject()?.GetRuntimeIdentifiers().Select(ToCompletionItem) ?? Empty<CompletionItem>();
            }
            catch (Exception)
            {
                return Empty<CompletionItem>();
            }
        }

        public static IEnumerable<CompletionItem> ProjectReferencesFromProjectFile(CompletionContext _)
        {
            try
            {
                return GetMSBuildProject()?.GetProjectToProjectReferences().Select(r => ToCompletionItem(r.Include)) ?? Empty<CompletionItem>();
            }
            catch (Exception)
            {
                return Empty<CompletionItem>();
            }
        }

        public static IEnumerable<CompletionItem> ConfigurationsFromProjectFileOrDefaults(CompletionContext _)
        {
            try
            {
                return (GetMSBuildProject()?.GetConfigurations() ?? new[] { "Debug", "Release" }).Select(ToCompletionItem);
            }
            catch (Exception)
            {
                return Empty<CompletionItem>();
            }
        }

        private static MsbuildProject GetMSBuildProject()
        {
            try
            {
                return MsbuildProject.FromFileOrDirectory(
                    new ProjectCollection(),
                    Directory.GetCurrentDirectory(), interactive: false);
            }
            catch (Exception e)
            {
                Report(e);
                return null;
            }
        }
    }
}
