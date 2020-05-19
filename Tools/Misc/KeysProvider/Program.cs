using System;
using KeysProvider.Services;
using ProjectHelpers.Services;
using Microsoft.Extensions.Configuration;

namespace KeysProvider
{
    public static class Program
    {
        private static ISecretKeysService? _secretKeysService;
        public const string HelpMessage = "-d put secret values\r" +
                                          "-r restore secret values";
        public const string  BadArgumentsMessage = "Wrong arguments, please type -h for help";
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            _secretKeysService ??= new SecretKeysService(new ProjectFilesService(), config);
            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "-d":
                        _secretKeysService?.SetSecrets();
                        break;
                    case "-u":
                        _secretKeysService.RemoveSecrets();
                        break;
                    case "-h":
                        Console.WriteLine(HelpMessage);
                        break;
                    default:
                        Console.WriteLine(BadArgumentsMessage);
                        break;
                }
            }
            else
            {
                Console.WriteLine(BadArgumentsMessage);
            }
        }

        public static void SetServices(ISecretKeysService secretKeysService)
        {
            _secretKeysService = secretKeysService;
        }
    }
}