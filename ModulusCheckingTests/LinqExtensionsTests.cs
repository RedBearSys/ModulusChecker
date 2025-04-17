using System;
using System.Collections.Generic;
using ModulusChecking;
using Xunit;

namespace ModulusCheckingTests
{
    public class LinqExtensionsTests
    {
        [Fact]
        public void CanCallForSecondItemInEnumerable()
        {
            var target = new List<int>{1, 2, 3, 4, 5};
            Assert.Equal(2,target.Second());
        }

        [Fact]
        public void ThrowsExceptionAsExpectedWithSingleItemList()
        {
            var target = new List<int> {1};
            try
            {
                target.Second();
            }
            catch (ArgumentException)
            {
                //this is expected
                return;
            }
            Assert.Fail("should have thrown by now");
        }
    }
}
