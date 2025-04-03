using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
		public class SyncDataList
	{
		public string ServiceCode { get; set; }
		public string Code { get; set; }
		public bool IsInsertRecord { get; set; }
		public bool IsEODRecords { get; set; }
		public string Method { get; set; }
	}
}

