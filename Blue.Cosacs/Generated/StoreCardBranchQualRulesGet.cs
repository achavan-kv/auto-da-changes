﻿

////------------------------------------------------------------------------------
//// <auto-generated>
////     This code was generated by Artemis Code Generator.
////
////     Changes to this file may cause incorrect behavior and will be lost if
////     the code is regenerated.
//// </auto-generated>
////------------------------------------------------------------------------------




//using System;
//using System.Collections.Generic;
//using System.Data;

//namespace Blue.Cosacs
//{
//    public partial class StoreCardBranchQualRulesGet : Artemis.Runtime.Data.Command
//    {
//        private const string ConnectionStringName = "Default";
//        private const string ProcedureName = "[StoreCardBranchQualRulesGet]";

//        public StoreCardBranchQualRulesGet() : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure) { }


//        public StoreCardBranchQualRulesGet(System.Int16? branchNo)
//            : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure)
//        {
//            this.branchNo = branchNo;

//        }




//        protected override void Initialize()
//        {
//            base.Initialize();


//            AddInParameter("branchNo", DbType.Int16);

//        }

//        #region Parameters

//        public System.Int16? branchNo
//        {

//            set { this[0] = value; }

//        }

//        #endregion
//    }
//}

//namespace Blue.Cosacs
//{

//    public partial class StoreCardBranchQualRulesGet
//    {


//        [Serializable]
//        public abstract partial class Record
//        {

//            public abstract System.Int32? MinApplicationScore
//            {
//                get;
//            }

//            public abstract System.Int32? MinBehaviouralScore
//            {
//                get;
//            }

//            public abstract System.Int32? MinMthsAcctHist
//            {
//                get;
//            }

//            public abstract System.Int32? MaxCurrMthsInArrs
//            {
//                get;
//            }

//            public abstract System.Int32? MaxPrevMthsInArrs
//            {
//                get;
//            }

//            public abstract System.Decimal? MinAvailRFLimit
//            {
//                get;
//            }

//        }

//        class Online : Record
//        {
//            public IDataRecord Record;


//            public override System.Int32? MinApplicationScore
//            {
//                get
//                {
//                    if (Record.IsDBNull(0))
//                        return null;
//                    else
//                        return (System.Int32)Record[0];
//                }
//            }

//            public override System.Int32? MinBehaviouralScore
//            {
//                get
//                {
//                    if (Record.IsDBNull(1))
//                        return null;
//                    else
//                        return (System.Int32)Record[1];
//                }
//            }

//            public override System.Int32? MinMthsAcctHist
//            {
//                get
//                {
//                    if (Record.IsDBNull(2))
//                        return null;
//                    else
//                        return (System.Int32)Record[2];
//                }
//            }

//            public override System.Int32? MaxCurrMthsInArrs
//            {
//                get
//                {
//                    if (Record.IsDBNull(3))
//                        return null;
//                    else
//                        return (System.Int32)Record[3];
//                }
//            }

//            public override System.Int32? MaxPrevMthsInArrs
//            {
//                get
//                {
//                    if (Record.IsDBNull(4))
//                        return null;
//                    else
//                        return (System.Int32)Record[4];
//                }
//            }

//            public override System.Decimal? MinAvailRFLimit
//            {
//                get
//                {
//                    if (Record.IsDBNull(5))
//                        return null;
//                    else
//                        return (System.Decimal)Record[5];
//                }
//            }

//        }

//        public class Reader : IDisposable, IEnumerable<Record>
//        {
//            public Reader(IDataReader reader)
//            {
//                this.reader = reader;
//            }

//            private IDataReader reader;

//            public void Dispose()
//            {
//                reader.Dispose();
//            }

//            public IEnumerator<Record> GetEnumerator()
//            {
//                var ro = new Online();

//                using (reader)
//                    while (reader.Read())
//                    {
//                        ro.Record = reader;
//                        yield return ro;
//                    }
//            }

//            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//            {
//                return GetEnumerator();
//            }
//        }

//        public Reader Execute()
//        {
//            return new Reader(ExecuteReader());
//        }

//        public Reader Execute(System.Int16? branchNo)
//        {
//            this.branchNo = branchNo;

//            Reader _result = Execute();

//            return _result;
//        }



//    }
//}
