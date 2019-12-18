using System.Collections.Generic;
using System.Linq;
using Sanet.SmartSkating.Utils;
using Xunit;

namespace Sanet.SmartSkating.Tests.Utils
{
    public class ListExtensionsTests
    {
        [Fact]
        public void TakesLastElementsOfTheList()
        {
            var sut = new List<int> {3, 5, 7, 2, 4, 8, 3, 1, 5};

            var result = sut.TakeLast(3).ToArray();
            
            Assert.Equal(new []{3, 1, 5},result);
        }
    }
}