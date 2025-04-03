using System;
using System.Collections.Generic;
using NUnit.Framework;
using STL.DAL.Licensing;
using System.Threading;
using System.Security.Cryptography;
using Blue.Licensing.Common;
using System.Linq;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    public class LicenseTest
    {
        List<Machine> machines;
        DLicense dLicense;
        License license;
        Interval interval = new Interval(0, 0, 10);
        RegistrationResult result = new RegistrationResult();

        [TestFixtureSetUp]
        public void SetUp()
        {
            license = new Blue.Licensing.Generator.LicenseGenerationService().Generate(new Blue.Licensing.Common.Payload()
            {
                ApplicationName = "Test Application",
                MaximumActiveCount = 10,
                RegistrationIntervalTimeSpan = "00:00:10",
                CustomerApplicationId = "Y",
                ExpiryDate = DateTime.Today.AddDays(10.0f) // t"01:01:3000"
            });

            dLicense = new DLicense();
            dLicense.ClearLicenseAudit();
            machines = new List<Machine>();

            for (var i = 0; i < 11; i++)
                machines.Add(
                    new Machine
                    {
                        MachineId = GenerateMac(),
                        UserId = 12345 + i,
                        CustomerApplicationId = "Y",
                        MachineName= "Test Machine " + i
                    });
        }

        [Test]
        public void BasicRegisterTest()
        {
            dLicense.ClearLicenseAudit();

            //foreach (var machine in machines.Skip(1))
            for (var i = 1; i < machines.Count; i++)
            {
                Assert.True((result = dLicense.Register(machines[i], license)).Registered, StatusMsg(result));
            }

            Assert.False(dLicense.Register(machines[0], license).Registered);
        }

        [Test]
        public void RegisterToLimitFailUnregisterSucceedTest()
        {
            dLicense.ClearLicenseAudit();

            //foreach (var machine in machines.Skip(1))
            for (var i = 1; i < machines.Count; i++)
            {
                Assert.True((result = dLicense.Register(machines[i], license)).Registered, StatusMsg(result));
            }

            Assert.False((result = dLicense.Register(machines[0], license)).Registered, StatusMsg(result));

            Assert.False((result = dLicense.UnRegister(machines[machines.Count - 1])).Registered, StatusMsg(result));

            Assert.True((result = dLicense.Register(machines[0], license)).Registered, StatusMsg(result));
        }

        [Test]
        public void ReRegisterInsideIntervalTest()
        {
            dLicense.ClearLicenseAudit();

            //foreach (var machine in machines.Skip(1))
            for (var i = 1; i < machines.Count; i++)
            {
                Assert.True((result = dLicense.Register(machines[i], license)).Registered, StatusMsg(result));
            }

            //foreach (var machine in machines.Skip(1))
            for (var i = 1; i < machines.Count; i++)
            {
                Assert.True((result = dLicense.Register(machines[i], license)).Registered, StatusMsg(result));
            }

            //foreach (var machine in machines.Skip(1))
            for (var i = 1; i < machines.Count; i++)
            {
                Assert.True((result = dLicense.Register(machines[i], license)).Registered, StatusMsg(result));
            }
        }

        readonly Random random = new Random();
        string GenerateMac()
        {
            var mac = "";

            for (int i = 0; i < 2; i++)
                mac += random.Next(100000, 999999);

            return mac;
        }

        static string StatusMsg(RegistrationResult result)
        {
            return result.Machine.MachineId + " : " + result.StatusMsg;
        }

        [Test]
        public void GeneratePublicPrivateKeyPair()
        {
            const int PROVIDER_RSA_FULL = 1;
            const string CONTAINER_NAME = "KeyContainer";
            CspParameters cspParams;
            cspParams = new CspParameters(PROVIDER_RSA_FULL);
            cspParams.KeyContainerName = CONTAINER_NAME;
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            var rsa = new RSACryptoServiceProvider(cspParams);

            rsa.PersistKeyInCsp = false;

            string publicPrivateKeyXML = rsa.ToXmlString(true);
            string publicOnlyKeyXML = rsa.ToXmlString(false);
            Console.WriteLine("Public + Private:\n{0}", publicPrivateKeyXML);
            Console.WriteLine("Public Only:\n{0}", publicOnlyKeyXML);
            // do stuff with keys...
        }

      //  [Test] to do put back. 
        public void GenerateLicenseAndValidate()
        {
            var payload = new Blue.Licensing.Common.Payload()
            {
                ApplicationName = "Test Application",
                MaximumActiveCount = 3,
                RegistrationIntervalTimeSpan = "00:00:30",
                CustomerApplicationId = "Y",
                ExpiryDate = DateTime.Today.AddDays(10.0f) // t"01:01:3000"
            };

            var license = new Blue.Licensing.Generator.LicenseGenerationService().Generate(payload);
            Console.WriteLine(license.ToXml());
            var s = license.ToBase64();
            Console.WriteLine(s);

            var licenseAfterRead = new Licensing.Common.LicenseValidationService().Validate(s);
            Console.WriteLine(licenseAfterRead.Payload.ExpiryDate);
            // tamper with the Signature

            var charToReplace = license.Signature.Skip(15).First();
            var replaceWithChar = (char)(charToReplace + 1 % 0xFF);             // fix build change + to -
            license.Signature = license.Signature.Replace(charToReplace, replaceWithChar);
            s = license.ToBase64();

            try
            {
                new Licensing.Common.LicenseValidationService().Validate(s);
                Assert.Fail("Should have failed because we tampered with the signature.");
            }
            catch (Licensing.Common.LicenseValidationException)
            {
                // everything OK
            }

            // tamper with the Data
            license.Payload.MaximumActiveCount++;
            s = license.ToBase64();

            try
            {
                new Licensing.Common.LicenseValidationService().Validate(s);
                Assert.Fail("Should have failed because we tampered with the data.");
            }
            catch (Licensing.Common.LicenseValidationException)
            {
                // everything OK
            }
        }
    }
}
