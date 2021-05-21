using Shared_CS;
using System;
using System.Threading.Tasks;

namespace Server_CS
{
    public class MyCalculator : ICalculator
    {
        public TimeResult GetTime()
        {
            return new TimeResult { Time = DateTime.Now };
        }

        ValueTask<MultiplyResult> ICalculator.MultiplyAsync(MultiplyRequest request)
        {
            var result = new MultiplyResult { Result = request.X * request.Y };
            return new ValueTask<MultiplyResult>(result);
        }
    }
}