using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Linq;
using Blue.Cosacs.Shared;
using STL.Common.Constants.Categories;

namespace Blue.Cosacs.Repositories
{
    public class TechnicianRepository
    {
        public IEnumerable<DropDownItem> LoadAllTechnician()
        {
            var context = Context.Create();

            var technicians = context.SR_Technician
                                .Where(r => !r.Deleted)
                                .Select(r => new
                                {
                                    Id = r.TechnicianId,
                                    Name = String.Format("{0} {1}", r.FirstName.Trim(), r.LastName.Trim()).Trim()
                                })
                                .ToList();

            var length = technicians.Any() ? technicians.Max(t => t.Id).ToString().Length : 1;
            var format = Enumerable.Repeat("0", length).Stringify(separator:"");

            return technicians
                    .OrderBy(x => x.Name)
                    .Select(x => new DropDownItem
                    {
                        ValueMember = x.Id,
                        DisplayMember = String.Format("{0}: {1}", x.Id.ToString(format), x.Name)
                    });
        }

        public List<ZonedTechniciansResult> LoadZoneWithTechnician()
        {
            var context = Context.Create();

            var technicians = LoadAllTechnician();

            var zones = context.Code
                        .Where(c => c.category == CAT.ServiceZone)
                        .OrderBy(c => c.sortorder)
                        .ThenBy(c => c.codedescript)
                        .GroupJoin(context.SR_Zone,  c => c.code, z => z.Code, (c, group) => new
                        {
                            ZoneCode      = c.code,
                            ZoneName      = c.codedescript,
                            TechnicianIds = group.Select(s => s.TechnicianId)
                        })
                        .ToList();

            var result = zones.Select(z => new ZonedTechniciansResult
            {
                ZoneCode = z.ZoneCode,
                ZoneName = z.ZoneName,
                Technicians = z.TechnicianIds.Join(technicians, ids => ids, t => t.ValueMember, (ids, t) => new DropDownItem 
                {
                    ValueMember = t.ValueMember,
                    DisplayMember = t.DisplayMember
                }).ToList()
            })
            .ToList();

            //Prepend 'ALL' zone with all the Technicians
            result.Insert(0, new ZonedTechniciansResult
            {
                ZoneCode = "0",
                ZoneName = "ALL",
                Technicians = technicians.ToList()
            });

            return result;
        }
    }
}
