using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared.Services.StoreCard
{
	partial class SearchRequest  
	{
        public DateTime StartDate {get; set;}
        public DateTime EndDate { get; set; }
        public string AcctNo { get; set; }
        public long StoreCardNo { get; set; }
        public string LastName { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
        public string Branch { get; set; }
        public bool Holder { get; set; }
	}
	
	partial class SearchResponse 
	{
        public List<View_StoreCard> View_StoreCard { get; set; }
	}
}
