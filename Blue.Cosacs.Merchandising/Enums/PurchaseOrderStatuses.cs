namespace Blue.Cosacs.Merchandising.Enums
{
    public static class PurchaseOrderStatuses
    {
        public const string New = "New";
        public const string PartiallyReceived = "PartiallyReceived";
        public const string Completed = "Completed";
        public const string Expired = "Expired";
        public const string Cancelled = "Cancelled";
        /// <summary>
        /// Code Added by Abhijeet for Ashley CR 
        /// Create New Constant for Approved, UnApproved & Rejected for PO
        /// </summary>
        public const string Approved = "Approved";
        public const string Unapproved = "UnApproved";
        public const string Rejected = "Rejected";
        public const string PartiallyApproved = "PartiallyApproved";
    }
}
