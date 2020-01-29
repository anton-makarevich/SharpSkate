using System;
using System.Collections.Generic;
using System.Linq;
using PreCommitHooks.Checks;

namespace PreCommitHooks
{
    internal static class Program
    {
        private static List<IPreCommitCheck> _checks = new List<IPreCommitCheck>
        {
            new SecureKeysCheck()
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