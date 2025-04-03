namespace Blue.Cosacs.Merchandising.Infrastructure
{
    public interface IMerchandisingConfiguration
    {
        bool IsMaster { get; }

        string MasterAuthKey { get; }

        string MasterAuthPass { get; }

        string MasterServiceRoute { get; }
    }
}
