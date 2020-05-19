using System;
using System.Collections.Generic;
using System.Linq;
using PreCommitHooks.Checks;
using ProjectHelpers.Services;

namespace PreCommitHooks
{
    internal static class Program
    {
        private static List<IPreCommitCheck> _checks = new List<IPreCommitCheck>
        {
            new SecureKeysCheck(new ProjectFilesService())
        };
        private static void Main()
        {
            if (_checks.Any(check => !check.CanCommit()))
            {
                Console.WriteLine("1");
                return;
            }

            Console.WriteLine("0");
        }
    }
}