namespace Blue.Cosacs.Merchandising.Mappers
{
    using Blue.Admin;
    using Blue.Cosacs.Merchandising.Models;

    public interface IProductMapper
    {
        ProductViewModel CreateViewModel(Product product, UserSession user);
        RepossessedProductViewModel CreateRepossessedViewModel(Product product = null);
        SetViewModel CreateSetViewModel(SetModel set);
        ComboViewModel CreateComboViewModel(ComboModel combo);
    }
}