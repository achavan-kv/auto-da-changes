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
    public partial class CustomerIdFromStoreCard : Artemis.Runtime.Data.Command
    {
        private const string ConnectionStringName = "Default";
        private const string ProcedureName = "[CustomerIdFromStoreCard]";

	    public CustomerIdFromStoreCard() : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure) {}

	    
	    public CustomerIdFromStoreCard(System.Int64? cardnumber) : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure)
	    {
		    this.cardnumber = cardnumber;

	    }
	    

	    

protected override void Initialize()
{
	base.Initialize();
	
	
	AddInParameter("cardnumber", DbType.Int64);
	
}

#region Parameters

public System.Int64? cardnumber
{
	
	set { this[0] = value; }
	
}

#endregion
    }
}

namespace Blue.Cosacs
{
	public partial class CustomerIdFromStoreCard
	{
		public object Execute()
		{
			return base.ExecuteScalar();
		}
		
		public object Execute(System.Int64? cardnumber)
		{
			this.cardnumber = cardnumber;

			var _result = Execute();
			
			return _result;
		}
		
	}
}
