namespace Blue.Cosacs.Merchandising.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    public interface IBrandRepository
    {
        Brand Get(int id);

        List<Brand> GetAll();

        Brand GetByCode(string code);

        Brand Save(Brand brand);
      //  IEnumerable<Brand> Save(IEnumerable<Brand> brand);
    }

    public class BrandRepository : IBrandRepository
    {
        public Brand Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Brand.FirstOrDefault(b => b.Id == id);
            }
        }

        public List<Brand> GetAll()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Brand.ToList();
            }
        }

        public Brand GetByCode(string code)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Brand.FirstOrDefault(b => b.BrandCode == code);
            }
        }

        public Brand Save(Brand brand)
        {
            using (var scope = Context.Write())
            {
                var existing = this.GetByCode(brand.BrandCode);

                if (existing == null)
                {
                    scope.Context.Brand.Add(brand);
                }
                else
                {
                    Mapper.Map(brand, existing);
                    brand = existing;
                }

                scope.Context.SaveChanges();
                scope.Complete();

                return brand;
            }
        }

        //public IEnumerable<Brand> Save(IEnumerable<Brand> brand)
        //{
        //    using (var scope = Context.Write())
        //    {
        //        var existingBrands = scope.Context.Brand.ToList();

        //        existingBrands.ForEach(e =>
        //        {
        //            e.BrandName = brand.Where(b => b.BrandCode == e.BrandCode).Select(s => s.BrandName).FirstOrDefault() ?? e.BrandName;
        //        });

        //        var newbrands = brand.ToList().Where(b => !existingBrands.Any(e => e.BrandCode.ToUpper() == b.BrandCode.ToUpper())).ToList();
        //        existingBrands.AddRange(newbrands);

        //        scope.Context.SaveChanges();
        //        scope.Complete();

        //        return existingBrands;
        //    }
        //}
    }
}