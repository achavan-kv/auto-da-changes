//using StructureMap;
//using System;
//using System.Collections.Generic;
//using System.Web.Http.Dependencies;
//using System.Linq;

//namespace Blue.Cosacs.SalesManagement.Api
//{
//    public class DependencyResolver : IDependencyResolver
//    {
//        protected IContainer container;

//        public DependencyResolver(IContainer container)
//        {
//            if (container == null)
//            {
//                throw new ArgumentNullException("container");
//            }
//            this.container = container;
//        }

//        public object GetService(Type serviceType)
//        {
//            try
//            {
//                return container.GetInstance(serviceType);
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }

//        public IEnumerable<object> GetServices(Type serviceType)
//        {
//            try
//            {
//                var t = container.GetAllInstances(serviceType);

//                var y = t.OfType<object>().ToList();

//                return y;
//            }
//            catch (Exception)
//            {
//                return new List<object>();
//            }
//        }

//        public IDependencyScope BeginScope()
//        {
//            var child = container.GetNestedContainer();
//            return new DependencyResolver(child);
//        }

//        public void Dispose()
//        {
//            container.Dispose();
//        }
//    }
//}