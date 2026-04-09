using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using Xunit;

namespace HonoursUnitTests
{
    public class MinimalTest : TestContext
    {
        [Fact]
        public void CanInstantiateTestNavigationManager()
        {
            var nav = new TestNavigationManager();
            Assert.NotNull(nav);
        }
    }
}
