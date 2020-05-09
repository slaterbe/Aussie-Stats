using Xunit;
using Amazon.Lambda.TestUtilities;

namespace EtlEnqueue.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestToUpperFunction()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var upperCase = function.FunctionHandler("hello world", context);

            Assert.Equal("Enqueue Successful", upperCase);
        }
    }
}
