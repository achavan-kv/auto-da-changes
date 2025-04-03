/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using Ninject.Modules;
using Unicomer.Cosacs.Repository.Interfaces;

namespace Unicomer.Cosacs.Repository.DependencyInjections
{
    public class RepositoryNinjectModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IDbLogggerRepository>().To<Implementations.DbLogggerRepository>();
            this.Bind<IPaymentRepository>().To<Implementations.PaymentRepository>();
            this.Bind<ICustomerRepository>().To<Implementations.CustomerRepository>();
            this.Bind<IStoreRepository>().To<Implementations.StoreRepository>();
            this.Bind<IAccountRepository>().To<Implementations.AccountRepository>();
            this.Bind<IProductsRepository>().To<Implementations.ProductsRepository>();
            this.Bind<ISalesOrderRepository>().To<Implementations.SalesOrderRepository>();
            this.Bind<ISelectProductRepository>().To<Implementations.SelectProductRepository>();
            this.Bind<IProcessDeliveryRepository>().To<Implementations.ProcessDeliveryRepository>();
            this.Bind<IRepaymentRepository>().To<Implementations.RepaymentRepository>();
        }
    }
}
