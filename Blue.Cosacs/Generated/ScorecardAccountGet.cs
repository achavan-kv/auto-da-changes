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
    public partial class ScorecardAccountGet : Artemis.Runtime.Data.Command
    {
        private const string ConnectionStringName = "Default";
        private const string ProcedureName = "[ScorecardAccountGet]";

	    public ScorecardAccountGet() : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure) {}

	    
	    public ScorecardAccountGet(System.String custid) : base(ConnectionStringName, ProcedureName, CommandType.StoredProcedure)
	    {
		    this.custid = custid;

	    }
	    

	    

protected override void Initialize()
{
	base.Initialize();
	
	
	AddInParameter("custid", DbType.String);
	
}

#region Parameters

public System.String custid
{
	
	set { this[0] = value; }
	
}

#endregion
    }
}

namespace Blue.Cosacs
{
	public partial class ScorecardAccountGet
	{
		public object Execute()
		{
			return base.ExecuteScalar();
		}
		
		public object Execute(System.String custid)
		{
			this.custid = custid;

			var _result = Execute();
			
			return _result;
		}
		
	}
}
