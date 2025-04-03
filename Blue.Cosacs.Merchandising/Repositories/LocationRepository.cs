namespace Blue.Cosacs.Merchandising.Repositories
{
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Event;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Events;
    using System.Collections.Generic;
    using System.Linq;

    public interface ILocationRepository
    {
        List<LocationModel> Get(bool warehouseOnly = false);

        List<LocationModel> Get(string fascia);

        List<LocationModel> Get(List<int> ids, bool warehouseOnly = false);

        Dictionary<int, string> GetDictionary(bool warehouseOnly = false);

        List<LocationListItemViewModel> GetList(bool warehouseOnly = false);

        LocationModel Get(int id);

        int LocateResource(string locationId);

        Location LocateBySalesId(string salesId);

        bool IsLocationIdDuplicate(LocationModel model);

        bool IsSalesIdDuplicate(LocationModel model);

        LocationModel Save(LocationModel model);

        IEnumerable<string> GetStoreTypes();

    }

    public class LocationRepository : ILocationRepository
    {
        private readonly IEventStore _audit;

        public LocationRepository(IEventStore audit)
        {
            _audit = audit;
        }

        private IQueryable<Location> GetLocations(bool warehouseOnly = false, IEnumerable<int> ids = null)
        {
            using (var scope = Context.Read())
            {
                var locations = scope.Context.Location.Select(l => l);
                if (ids != null)
                {
                    locations = locations.Where(l => ids.Contains(l.Id));
                }
                if (warehouseOnly)
                {
                    locations = locations.Where(l => l.Warehouse);
                }
                return locations;
            }
        }

        public List<LocationModel> Get(bool warehouseOnly = false)
        {
            return GetLocations(warehouseOnly).Select(Mapper.Map<LocationModel>).ToList();
        }

        public List<LocationModel> Get(string fascia)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Location.Where(l => l.Fascia == fascia).Select(Mapper.Map<LocationModel>).ToList();
            }
        }

        public List<LocationModel> Get(List<int> ids, bool warehouseOnly = false)
        {
            return GetLocations(warehouseOnly, ids).Select(Mapper.Map<LocationModel>).ToList();
        }

        public Dictionary<int, string> GetDictionary(bool warehouseOnly = false)
        {
            return GetLocations(warehouseOnly).ToDictionary(l => l.Id, l => l.Name);
        }

        public List<LocationListItemViewModel> GetList(bool warehouseOnly = false)
        {
            return GetLocations(warehouseOnly)
                   .Select(s => new LocationListItemViewModel { LocationId = s.Id, LocationName = s.Name }).ToList();
        }


        public LocationModel Get(int id)
        {
            using (var scope = Context.Read())
            {
                var location = scope.Context.Location.Single(l => l.Id == id);
                return Mapper.Map<LocationModel>(location);
            }
        }

        public int LocateResource(string locationId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Location.Where(l => l.LocationId == locationId).Select(l => l.Id).SingleOrDefault();
            }
        }

        public Location LocateBySalesId(string salesId)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Location.FirstOrDefault(l => l.SalesId == salesId);
            }
        }

        public bool IsLocationIdDuplicate(LocationModel model)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Location.Any(l => l.LocationId == model.LocationId && (model.Id == 0 || model.Id != l.Id));
            }
        }
        public bool IsSalesIdDuplicate(LocationModel model)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Location.Any(l => l.SalesId == model.SalesId && (model.Id == 0 || model.Id != l.Id));
            }
        }

        public LocationModel Save(LocationModel model)
        {
            using (var scope = Context.Write())
            {
                var location = scope.Context.Location.Find(model.Id);
                string eventType;

                if (location == null)
                {
                    location = new Location();
                    scope.Context.Location.Add(location);
                    eventType = LocationEvents.Create;
                }
                else
                {
                    eventType = LocationEvents.Edit;
                }


                Mapper.Map(model, location);
                scope.Context.SaveChanges();

                // Hack to insert into dbo.Branch
                var workPhone = model.Contacts.FirstOrDefault(s => s.Key == "WorkPhone");
                scope.Context.UpdateLocationBranch(new UpdateLocationBranchInput
                {
                    LocationId = location.Id,
                    WorkPhone = workPhone == null ? string.Empty : workPhone.Value
                });


                _audit.LogAsync(location, eventType, EventCategories.Merchandising);
                scope.Complete();
                return Mapper.Map<LocationModel>(location);
            }
        }

        public IEnumerable<string> GetStoreTypes()
        {
            using (var scope = Context.Read())
            {
                return scope.Context.Location.Select(s => s.StoreType).Distinct().ToList();
            }

        }
    }
}