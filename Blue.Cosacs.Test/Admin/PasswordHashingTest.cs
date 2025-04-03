
using System;
using System.Text;
using NUnit.Framework;
namespace Blue.Cosacs.Test.Admin
{
    [TestFixture]
    public class PasswordHashingTest
    {
        [Test]
        public void HashSimple()
        {
            var algo = new Blue.Admin.BCryptPasswordHashingAlgorithm();
            const string pass = "Testing12345VeryLong";
            var hash = algo.HashPassword(pass);
            var result = algo.Verify(pass, hash);
            Console.WriteLine("Hash: " + hash);
            Console.WriteLine("Hash length: " + hash.Length);
            Assert.IsTrue(result);
        }

        [Test]
        public void HashLargePasswords()
        {
            var algo = new Blue.Admin.BCryptPasswordHashingAlgorithm();
            var r = new Random();
            var rand = r.Next(100);

            var s = new StringBuilder();
            for (int i = 0; i < rand; i++)
            {
                var c = (char)r.Next(126 + 33) - 33;
                s.Append(c);
            }

            var hash = algo.HashPassword(s.ToString());
            var result = algo.Verify(s.ToString(), hash);
            Console.WriteLine("Hash: " + hash);
            Console.WriteLine("Hash length: " + hash.Length);
            Assert.IsTrue(result);
        }
    }
}
