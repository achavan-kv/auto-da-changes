using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Cosacs.Web.Controllers
{
    public class HierarchyController : Controller
    {
        //[HttpGet]
        //TODO: we need to check with Paulo to make sure that this endpoint isn't needed any more
        //public JsonResult Hierarchies()
        //{
        //    var retLevels = new List<Level>();
        //    var level1 = new List<Data>()
        //    {
        //        new Data() { Key = "PCE", Name = "Electrical" },
        //        new Data() { Key = "PCF", Name = "Furniture" },
        //        new Data() { Key = "PCO", Name = "Other" },
        //        new Data() { Key = "PCW", Name = "Workstation" },
        //        new Data() { Key = "PCDIS", Name = "Discount" },
        //    };
        //    var level2 = new List<Data>();
        //    var level3 = new List<Data>();

        //    var tree = Blue.Cosacs.Stock.ProductRelations.TreeWithDeparmentCode;
        //    foreach (var l in level1)
        //    {
        //        var level2Data = new List<Data>();
        //        if (tree.ContainsKey(l.Key))
        //        {
        //            level2Data = GetLevelData(l.Key, tree[l.Key]);
        //        }

        //        if (level2Data.Count > 0)
        //        {
        //            level2.AddRange(level2Data);

        //            foreach (var d in level2Data)
        //            {
        //                var level3Data = GetLevelData(d.Key, tree[l.Key].children[d.Key]);
        //                level3.AddRange(level3Data);
        //            }
        //            l.Code = tree[l.Key].code;
        //        }
        //    }

        //    ReplaceDepartmentCodesByHierarchyLevelCodes(level1);

        //    return Json(
        //        new List<object>()
        //        {
        //            new
        //            {
        //                Name = "Division",
        //                Data = level1
        //            },
        //            new
        //            {
        //                Name = "Department",
        //                Data = level2
        //            },
        //            new
        //            {
        //                Name = "Class",
        //                Data = level3
        //            },
        //        },
        //        JsonRequestBehavior.AllowGet);
        //}

        private void ReplaceDepartmentCodesByHierarchyLevelCodes(List<Data> level1)
        {
            for (int i = 0; i < level1.Count; i++)
            {
                level1[i].Key = level1[i].Code;
                level1[i].Code = string.Empty;
            }
        }

        private List<Data> GetLevelData(string nodeKey, TreeNode node)
        {
            var retData = new List<Data>();
            var name = node.Name;

            foreach (var c in node.Children)
            {
                retData.Add(new Data()
                {
                    Key = c.Key,
                    Name = c.Value.Name,
                });
            }

            return retData;
        }

        private class Level
        {
            public string Name { get; set; }
            public List<Data> Data { get; set; }
        }

        private class Data
        {
            public string Key { get; set; }
            public string Name { get; set; }

            [ScriptIgnore]
            public string Code { get; set; }
        }
    }

    public class TreeNode
    {
        public TreeNode()
        {
            Children = new Dictionary<string, TreeNode>();
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public IDictionary<string, TreeNode> Children { get; set; }
    }
}
