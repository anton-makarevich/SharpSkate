using System;
using System.IO;
using System.Text;
using FluentAssertions;
using KeysProvider;
using KeysProvider.Services;
using NSubstitute;
using Xunit;

namespace KeysProviderTests
{
    public class ProgramTests
    {
        private readonly StringBuilder _testStringBuilder;
        private readonly ISecretKeysService _secretKeysService;

        public ProgramTests()
        {
            _testStringBuilder = new StringBuilder();
            var testStringWriter = new StringWriter(_testStringBuilder);
            Console.SetOut(testStringWriter);

            _secretKeysService = Substitute.For<ISecretKeysService>();
            Program.SetServices(_secretKeysService);
        }
        
        [Fact]
        public void PrintsBadArgumentsMessage_WhenArgsAreEmpty()
        {
            Program.Main(new string[0]);

            var output = _testStringBuilder.ToString();
            output.Trim().Should().Be(Program.BadArgumentsMessage.Trim());
        }
        
        [Fact]
        public void PrintsBadArgumentsMessage_WhenArgIsUnknown()
        {
            Program.Main(new[]{"-w"});

            var output = _testStringBuilder.ToString();
            output.Trim().Should().Be(Program.BadArgumentsMessage.Trim());
        }
        
        [Fact]
        public void PrintsHelpMessage_WhenHelpArgIsPassed()
        {
            Program.Main(new[]{"-h"});

            var output = _testStringBuilder.ToString();
            output.Trim().Should().Be(Program.HelpMessage.Trim());
        }
        
        [Fact]
        public void CallsSetSecrets_WhenSetArgIsPassed()
        {
            Program.Main(new[]{"-d"});

            _secretKeysService.Received().SetSecrets();
        }
        
        [Fact]
        public void CallsRemoveSecrets_WhenRemoveArgIsPassed()
        {
            Program.Main(new[]{"-u"});

            _secretKeysService.Received().RemoveSecrets();
        }
    }
}