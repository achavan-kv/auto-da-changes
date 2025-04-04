﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Artemis Code Generator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------




using System;
using System.Collections.Generic;
using System.Data;

namespace Blue.Cosacs
{
    public partial class StoreCardRateLoadAll : Artemis.Runtime.Data.Command
    {
        private const string ConnectionStringName = "Default";
        private const string ProcedureName = "[StoreCardRateLoadAll]";

        public StoreCardRateLoadAll() : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure) { }





        protected override void Initialize()
        {
            base.Initialize();


        }

        #region Parameters

        #endregion
    }
}

namespace Blue.Cosacs
{

    public partial class StoreCardRateLoadAll
    {


        [Serializable]
        public abstract partial class Record
        {

            public abstract System.Int32? Id
            {
                get;
            }

            public abstract System.Int32? Version
            {
                get;
            }

            public abstract System.Boolean? IsDeleted
            {
                get;
            }

            public abstract System.String NAME
            {
                get;
            }

            public abstract System.Boolean? ratefixed
            {
                get;
            }

        }

        class Online : Record
        {
            public IDataRecord Record;


            public override System.Int32? Id
            {
                get
                {
                    if (Record.IsDBNull(0))
                        return null;
                    else
                        return (System.Int32)Record[0];
                }
            }

            public override System.Int32? Version
            {
                get
                {
                    if (Record.IsDBNull(1))
                        return null;
                    else
                        return (System.Int32)Record[1];
                }
            }

            public override System.Boolean? IsDeleted
            {
                get
                {
                    if (Record.IsDBNull(2))
                        return null;
                    else
                        return (System.Boolean)Record[2];
                }
            }

            public override System.String NAME
            {
                get
                {
                    if (Record.IsDBNull(3))
                        return null;
                    else
                        return (System.String)Record[3];
                }
            }

            public override System.Boolean? ratefixed
            {
                get
                {
                    if (Record.IsDBNull(4))
                        return null;
                    else
                        return (System.Boolean)Record[4];
                }
            }

        }

        public class Reader : IDisposable, IEnumerable<Record>
        {
            public Reader(IDataReader reader)
            {
                this.reader = reader;
            }

            private IDataReader reader;

            public void Dispose()
            {
                reader.Dispose();
            }

            public IEnumerator<Record> GetEnumerator()
            {
                var ro = new Online();

                using (reader)
                    while (reader.Read())
                    {
                        ro.Record = reader;
                        yield return ro;
                    }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public Reader Execute()
        {
            return new Reader(ExecuteReader());
        }



    }
}
