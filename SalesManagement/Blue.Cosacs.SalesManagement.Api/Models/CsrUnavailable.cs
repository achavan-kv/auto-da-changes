using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue.Cosacs.SalesManagement.Api.Models
{
    public partial class CsrUnavailable
    {
        public short Id { get; set; }
        public int SalesPersonId { get; set; }
        public DateTime BeggingUnavailable { get; set; }
        public DateTime EndUnavailable { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        public Blue.Cosacs.SalesManagement.CsrUnavailable ToEntitySet()
        {
            return new SalesManagement.CsrUnavailable
            {
                BeggingUnavailable = this.BeggingUnavailable,
                CreatedBy = this.CreatedBy,
                CreatedOn = this.CreatedOn,
                SalesPersonId = this.SalesPersonId,
                EndUnavailable = this.EndUnavailable,
                Id = this.Id
            };
        }
    }
}