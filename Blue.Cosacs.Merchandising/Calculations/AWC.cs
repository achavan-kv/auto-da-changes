namespace Blue.Cosacs.Merchandising.Calculations
{
    public static class AWC
    {
        public static decimal CalculateAWC(decimal averageWeightedCost, decimal price, int stockOnHand, int newQuantity, decimal repoCostPercent = 0)
        {
            var repoCostPercentFactor = 1 - (repoCostPercent / 100);

            if (stockOnHand + newQuantity <= 0)
            {
                return averageWeightedCost;
            }

            var oldAWCTotal = averageWeightedCost * ((stockOnHand <= 0) ? 0 : stockOnHand); //stockOnHand < 0 but total goes to +
            var newAWCTotal = (price * repoCostPercentFactor) * ((stockOnHand <= 0) ? (newQuantity + stockOnHand) : newQuantity); // Repo percent is the cost price plus repo markup. Want to remove so divide.
            var totalStock = stockOnHand + newQuantity;
            return (oldAWCTotal + newAWCTotal) / totalStock;
        }
    }
}
