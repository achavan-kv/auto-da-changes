using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using STL.DAL;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace Blue.Cosacs.Test
{
    [TestFixture]
    public class AccountLockingTest
    {
        //[Test]
        //public void TelephoneActionScreenConcurrencyTest()
        //{
        //    sync = new Semaphore(0, maximumCount: 10);
        //    var accts = new[] {
        //        "700000001159",
        //        "700000001295",
        //        "700000001947",
        //        "700000001948",
        //        "700000001949",
        //        "700000002584",
        //        "700000002585",
        //        "700000002922",
        //        "700000002923",
        //        "700000002924",
        //    };
        //    const int concurrency = 5;
        //    var countFails = 0;
        //    var countTotal = 1000;

        //    for (var i = 0; i < countTotal; i++)
        //    {
        //        var r = new Dictionary<int, string>();
        //        var acct = accts[i % accts.Length];
        //        var ts = new Thread[concurrency];
        //        for (var j = 0; j < ts.Length; j++)
        //            ts[j] = Thread(acct, j, r);
        //        //var t1 = Thread(acct, 1, r);
        //        //var t2 = Thread(acct, 2, r);

        //        TruncateLocking();
        //        sync.Release(ts.Length);

        //        foreach (var t in ts)
        //            t.Join();

        //        if (Count() > 1)
        //            //Assert.Fail();
        //            countFails++;
        //        //Assert.AreEqual(1, Count(), "Ooopps.. same account locked more than once.");
        //    }

        //    Console.WriteLine("{0:p} lock failures", ((float)countFails) / countTotal);
        //}

        private int Count(string table = "AccountLocking")
        {
            using (var c = new SqlConnection(Connections.Default))
            {
                c.Open();
                return Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM [" + table + "]", c).ExecuteScalar());
            }
        }

        private Thread Thread(string acct, int user, IDictionary<int,string> result)
        {
            var t = new Thread(x => {
                var acctno = AccountLockingFindandLockForCaller(acct, user);
                //Console.WriteLine(System.Threading.Thread.CurrentThread.Name + ": " + acctno);
                lock(result)
                    result.Add(user,acctno);
            });
            t.Name = user.ToString();
            t.Start();
            return t;
        }

        private System.Threading.Semaphore sync;

        private void TruncateLocking()
        {
            using (var c = new SqlConnection(Connections.Default))
            {
                c.Open();
                new SqlCommand("TRUNCATE TABLE AccountLocking", c).ExecuteNonQuery();
                new SqlCommand("TRUNCATE TABLE CustomerLocking", c).ExecuteNonQuery();
            }
        }

        private string AccountLockingFindandLockForCaller(string acctlist, int user)
        {
            using (var c = new SqlConnection(Connections.Default))
            {
                c.Open();
                using (var tx = c.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var p = new DAccountParms
                    {
                        conn = c,
                        trans = tx,
                        AcctList = acctlist,
                        user = user,
                        RunDate = DateTime.Now
                    };
                    sync.WaitOne();
                    new DAccount().AccountLockingFindandLockForCaller(ref p);

                    tx.Commit();
                    return p.Acctno;
                }
            }
        }
    }
}
