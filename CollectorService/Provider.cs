using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectorService
{
    class Provider
    {
        Random rnd;
        Stack<int> buffer;

        public Provider () {
            rnd = new Random();
            buffer = new Stack<int>();
            buffer.Push(rnd.Next(1, 20000));
            Console.WriteLine("Buffer inited");
            Task.Run(() => GenerateElement());
        }

        public int Get()
        {
            if(buffer.Count != 0)
            {
                return buffer.Pop();
            }
            else
            {
                return 0;
            }
        }

        private async Task GenerateElement()
        {
            while(true)
            {
                buffer.Push(rnd.Next(1, 20000));
                Console.WriteLine("Creating a new element");
                await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(1000)));
            }
        } 
    }
}
