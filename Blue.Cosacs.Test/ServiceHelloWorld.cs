using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace Blue.Cosacs.Test
{
    public partial class HelloWorldService : IService<HelloWorldRequest>, IService<HelloWorldRequest2>
    {
        public object Execute(HelloWorldRequest request)
        {
            return new HelloWorldResponse { EchoText = request.Text };
        }

        public object Execute(HelloWorldRequest2 request)
        {
            return new HelloWorldResponse { EchoText = request.Text };
        }
    }

    public class HelloWorldRequest 
    {
        public string Text;
    }

    public class HelloWorldResponse 
    {
        public string EchoText;
    }

    public class HelloWorldRequest2 { public string Text; }
}
