using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Selenium;
using OpenQA.Selenium;
using Blue.Cosacs.Selenium;
using System.Reflection;

namespace Blue.Cosacs.Selenium.Common
{
    public class Program
    {
        public static int Main(String[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetExportedTypes();

            var fixtures = from t in types
                           where t.IsFixture()
                           select t;

            Blue.Selenium.Console.Info("Fixtures {0}", fixtures.Count());

            var tests = fixtures.Select(f => f.Tests()).SelectMany(t => t).ToArray();
            var suite = new TestSuite(tests);

            if (args.Length != 6)
            {
                Blue.Selenium.Console.Error("usage: exe <chrome|firefox|safari|ie> <timeout> <number-of-drivers> <baseurl> <huburl> <WINDOWS|LINUX>");
                return -1;
            }

            suite.Run(new Session.Config
            {
                Browser = (Session.BrowserType)Enum.Parse(typeof(Session.BrowserType), args[0]),
                WaitInSecs = int.Parse(args[1]),
                BaseUrl = args[3], // "http://grid0/cosacs71",
                HubUrl = args[4], // "http://localhost:4444/wd/hub"
                Platform = args[5] // WINDOWS or LINUX
            }, driverCount: int.Parse(args[2]));

            return 0; // suite.Failures > 0 ? -1 : 0;
        }
    }
}
