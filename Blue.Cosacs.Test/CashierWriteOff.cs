using System;
using System.Collections.Generic;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Extensions;
using NUnit.Framework;
using Blue.Cosacs.Repositories;
using System.Linq;
using Blue.Cosacs.Cashier;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    class CashierWriteOffTest
    {
        DateTime sat;
        DateTime sun;
        DateTime wed;
        DateTime fri;
     

        [SetUp]
        public void Init()
        {
            sat = new DateTime(2012, 02, 25);
            sun = new DateTime(2012, 02, 26);
            fri = new DateTime(2012, 02, 24);
            wed = new DateTime(2012, 02, 22);
        }

        public CashierWriteOff SetUp(string limit, int shortage, int overage, DateTime current, DateTime? lastwriteoff = null, string account = "123456")
        {

            var cparams = new Dictionary<string, CountryMaintenance> { 
                                                                          {"WriteOffAccount", new CountryMaintenance { Value = "123456789"}},
                                                                          {"MaxTimeLimit", new CountryMaintenance { Value = limit}},
                                                                          {"CashierMaxShortage", new CountryMaintenance { Value = shortage.ToString()}},
                                                                          {"CashierMaxOverage", new CountryMaintenance { Value = overage.ToString()}}};
            return new CashierWriteOff(cparams, lastwriteoff, current);



        }

        [Test]
        public void InitalizeWriteOffTest()
        {
            var writeoff = SetUp(CashierWriteLimits.DayUpper, 100, 100, sat);
            Assert.AreEqual(sat.AddDays(-1), writeoff.InitalizeWriteOff(sat));

            writeoff = SetUp(CashierWriteLimits.DayUpper, 100, 100, wed);
            Assert.AreEqual(wed.AddDays(-1), writeoff.InitalizeWriteOff(wed));

            writeoff = SetUp(CashierWriteLimits.WeekSunUpper, 100, 100, sun);
            Assert.AreEqual(sun.AddDays(-7), writeoff.InitalizeWriteOff(sun));

            writeoff = SetUp(CashierWriteLimits.WeekSunUpper, 100, 100, wed);
            Assert.AreEqual(wed.AddDays(-10), writeoff.InitalizeWriteOff(wed));

            writeoff = SetUp(CashierWriteLimits.WeekSatUpper, 100, 100, sat);
            Assert.AreEqual(sat.AddDays(-7), writeoff.InitalizeWriteOff(sat));

            writeoff = SetUp(CashierWriteLimits.WeekSatUpper, 100, 100, sun);
            Assert.AreEqual(sun.AddDays(-8), writeoff.InitalizeWriteOff(sun));

        }

        [Test]
        public void CalculateEndDate()
        {
            var writeoff = SetUp(CashierWriteLimits.DayUpper, 100, 100, sat);
            Assert.AreEqual(sat, writeoff.CalculateEndDate(sat));

            writeoff = SetUp(CashierWriteLimits.DayUpper, 100, 100, wed);
            Assert.AreEqual(wed, writeoff.CalculateEndDate(wed));

            writeoff = SetUp(CashierWriteLimits.WeekSunUpper, 100, 100, sun);
            Assert.AreEqual(sun, writeoff.CalculateEndDate(sun));

            writeoff = SetUp(CashierWriteLimits.WeekSunUpper, 100, 100, wed);
            Assert.AreEqual(sun.AddDays(-7), writeoff.CalculateEndDate(wed));

            writeoff = SetUp(CashierWriteLimits.WeekSatUpper, 100, 100, sat);
            Assert.AreEqual(sat, writeoff.CalculateEndDate(sat));

            writeoff = SetUp(CashierWriteLimits.WeekSatUpper, 100, 100, wed);
            Assert.AreEqual(sat.AddDays(-7), writeoff.CalculateEndDate(wed));
        }

        [Test]
        public void Intervals()
        {

            //daily

            Assert.AreEqual(0, CashierWriteOff.Intervals(true,sat,sat).Count());
            Assert.AreEqual(3, CashierWriteOff.Intervals(true, wed,sat).Count());
            Assert.AreEqual(4, CashierWriteOff.Intervals(true, wed, sun).Count());

            //weekly - End dates are always on the correct day.
            Assert.AreEqual(1, CashierWriteOff.Intervals(false, wed, sat).Count());
            var x = CashierWriteOff.Intervals(false, wed, sat).FirstOrDefault();
            Assert.AreEqual(wed,x.start);
            Assert.AreEqual(sat, x.end);

            Assert.AreEqual(1, CashierWriteOff.Intervals(false, sat.AddDays(-1), sat).Count());
            x = CashierWriteOff.Intervals(false, sat.AddDays(-1), sat).FirstOrDefault();
            Assert.AreEqual(sat.AddDays(-1), x.start);
            Assert.AreEqual(sat, x.end);

            Assert.AreEqual(2, CashierWriteOff.Intervals(false, sat.AddDays(-8), sat).Count());
            x = CashierWriteOff.Intervals(false, sat.AddDays(-8), sat).OrderBy(i => i.end).FirstOrDefault();
            Assert.AreEqual(sat.AddDays(-8), x.start);
            Assert.AreEqual(sat.AddDays(-7), x.end);

            Assert.AreEqual(1, CashierWriteOff.Intervals(false, wed, sun).Count());
            x = CashierWriteOff.Intervals(false, wed, sun).FirstOrDefault();
            Assert.AreEqual(wed, x.start);
            Assert.AreEqual(sun, x.end);

            Assert.AreEqual(1, CashierWriteOff.Intervals(false, sun.AddDays(-1), sun).Count());
            x = CashierWriteOff.Intervals(false, sun.AddDays(-1), sun).FirstOrDefault();
            Assert.AreEqual(sun.AddDays(-1), x.start);
            Assert.AreEqual(sun, x.end);

            Assert.AreEqual(2, CashierWriteOff.Intervals(false, sun.AddDays(-8), sun).Count());
            x = CashierWriteOff.Intervals(false, sun.AddDays(-8), sun).OrderBy(i => i.end).FirstOrDefault();
            Assert.AreEqual(sun.AddDays(-8), x.start);
            Assert.AreEqual(sun.AddDays(-7), x.end);

        }

      

       
        [Test]
        public void Calculate()
        {
            var acctno1 = "acctno1";
            var acctno2 = "acctno2";

            var test1 = new List<CashierTotalWriteOffView>
            {
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sun.AddDays(-21), difference = 10  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sun.AddDays(-21), difference = -10  },
                new CashierTotalWriteOffView { acctno = acctno2, empeeno = 4, datetrans = sun.AddDays(-21), difference = 15  },
                new CashierTotalWriteOffView { acctno = acctno2, empeeno = 4, datetrans = sun.AddDays(-21), difference = -15 },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 4, datetrans = sun.AddDays(-21), difference = 11  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 4, datetrans = sun.AddDays(-21), difference = -11  },
                new CashierTotalWriteOffView { acctno = acctno2, empeeno = 5, datetrans = sun.AddDays(-21), difference = 10  },
                new CashierTotalWriteOffView { acctno = acctno2, empeeno = 5, datetrans = sun.AddDays(-21), difference = -10  },
               
            };


            var test3 = new List<CashierTotalWriteOffView>
            {
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-11), difference = 20  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-21), difference = 29  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 3, datetrans = sat.AddDays(-21), difference = -9  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-15), difference = 20  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-21), difference = -40 }
            };

            var test2 = new List<CashierTotalWriteOffView>
            {
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-14), difference = 20  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-14), difference = -69  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 3, datetrans = sat.AddDays(-14), difference = -9  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-21), difference = 0  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 5, datetrans = sat.AddDays(-21), difference = -40 },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 4, datetrans = sat.AddDays(-21), difference = 0  },
                new CashierTotalWriteOffView { acctno = acctno1, empeeno = 4, datetrans = sat.AddDays(-21), difference = 41 }
            };

            var writeoff = SetUp(CashierWriteLimits.WeekSatUpper, 50, 50, sat, sat.AddDays(-21));
            Assert.True(writeoff.GetAccounts(test1).Count() == 0); // Grouping

            writeoff = SetUp(CashierWriteLimits.Day, 50, 50, sat, sat.AddDays(-21));
            Assert.True(writeoff.GetAccounts(test1).Count() == 0); // Grouping

            writeoff = SetUp(CashierWriteLimits.WeekSat, 50, 50, sat.AddDays(7), sat.AddDays(-21));
            Assert.True(writeoff.GetAccounts(test3).Count() == 5); // Date Grouping
          


            writeoff = SetUp(CashierWriteLimits.WeekSat, 50, 50, sat.AddDays(7), sat.AddDays(-21));
           Assert.True(writeoff.GetAccounts(test3).Count() == 5); // Date Grouping
            //Console.WriteLine(writeoff.GetAccounts(test3).Count());

            writeoff = SetUp(CashierWriteLimits.WeekSun, 1, 1, sun.AddDays(7), sun.AddDays(-21));
            Assert.True(writeoff.GetAccounts(test3).Count() == 0); // Min Max

            writeoff = SetUp(CashierWriteLimits.Day, 1, 1, sun.AddDays(7), sun.AddDays(-21));
            Assert.True(writeoff.GetAccounts(test3).Count() == 0); // Min Max


        }
    }
}


