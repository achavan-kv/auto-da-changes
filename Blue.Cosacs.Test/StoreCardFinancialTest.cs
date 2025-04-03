using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Blue.Cosacs.Shared;
using Blue.Cosacs.StoreCardUtil;

namespace Blue.Cosacs.Test

{
   [TestFixture]
    public class StoreCardFinancialTest
    {

       List<fintranswithBalancesVW> fintransBalances;

         [SetUp]
        public void Setup()
        {
            fintransBalances = new List<fintranswithBalancesVW>
            {
                new fintranswithBalancesVW{ 
                datetrans = new DateTime(2011, 02, 02),
                transvalue = 100
                }, 
                
                new fintranswithBalancesVW{ 
                datetrans = new DateTime(2011, 03, 01),
                transvalue = 100
                },

            new fintranswithBalancesVW
            {
                datetrans = new DateTime(2011, 02, 05),
                transvalue = -32
            },

            new fintranswithBalancesVW
            {
                datetrans = new DateTime(2011, 02, 07),
                transvalue = 1999
            },

             new fintranswithBalancesVW
            {
                datetrans = new DateTime(2011, 02, 27),
                transvalue = 222
            },

             new fintranswithBalancesVW{ 
                datetrans = new DateTime(2011, 01, 01),
                transvalue = 1020
                },

            new fintranswithBalancesVW
            {
                datetrans = new DateTime(2011, 01, 05),
                transvalue = -324
            },

            new fintranswithBalancesVW
            {
                datetrans = new DateTime(2011, 01, 07),
                transvalue = 1349
            },

             new fintranswithBalancesVW
            {
                datetrans = new DateTime(2011, 01, 31),
                transvalue = 2322
            }
            
            };

         }

       [Test]
         public void WeightedAverage()
         {
             var x = StoreCardWeighting.WeightedAverage(fintransBalances, new DateTime(2011, 02, 01), new DateTime(2011, 03, 01).AddSeconds(-1));
            Assert.AreEqual(6022.5,x);
         }
       
    }
}
