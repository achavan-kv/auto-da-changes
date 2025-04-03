using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Stock
{
    [Obsolete("You should not use this class")]
    public static class ProductRelations
    {
        [Obsolete("You should not use this field")]
        public static Dictionary<string, TreeNode> Tree;
        [Obsolete("You should not use this field")]
        public static Dictionary<string, TreeNode> TreeWithDeparmentCode;
        [Obsolete("You should not use this field")]
        public static Dictionary<string, string> Levels;

        static ProductRelations()
        {
            // read tree data
            List<StockItemViewRelations> rows;
            using (var scope = Context.Read())
            {
                rows = (from s in scope.Context.StockItemViewRelations
                        select s)
                       .ToList();
            }

            // create tree
            Tree = CreateTree(rows);
            TreeWithDeparmentCode = CreateTree(rows, true);

            // create levels
            Levels = new Dictionary<string, string>() 
            {
                { "Level_1", "Department" },
                { "Level_2", "Category" },
                { "Level_3", "Class" }
            };
        }

        [Obsolete("You should not use this class")]
        private static Dictionary<string, TreeNode> CreateTree(List<StockItemViewRelations> rows, bool returnDepartmentCode = false)
        {
            var level1 = new Dictionary<string, TreeNode>(); // level1 or tree root

            foreach (var row in rows)
            {
                // level 1
                if (!level1.ContainsKey(row.Department))
                {
                    if (returnDepartmentCode)
                    {
                        level1.Add(row.Department,
                            new TreeNode
                            {
                                name = row.DepartmentName,
                                code = row.DepartmentCode,
                            });
                    }
                    else
                    {
                        level1.Add(row.Department, new TreeNode { name = row.DepartmentName });
                    }
                }

                var level2 = level1[row.Department].children;

                // level 2
                if (!level2.ContainsKey(row.Category.ToString()))
                {
                    level2.Add(row.Category.ToString(), new TreeNode {name = row.CategoryName});
                }

                var level3 = level2[row.Category.ToString()].children;

                // level 3
                if (!level3.ContainsKey(row.Class))
                {
                    level3.Add(row.Class, new TreeNode {name = row.ClassName});
                }
            }

            return level1;
        }

        [Obsolete("You should not use this class")]
        public class TreeNode
        {
            public TreeNode()
            {
                children = new Dictionary<string, TreeNode>();
            }

            public string name;
            public string code;
            public IDictionary<string, TreeNode> children;
        }

    }

    public partial class StockItemViewRelations
    {
        public string _departmentName;
        public string DepartmentName
        {
            get
            {
                if (_departmentName == null)
                {
                    _departmentName = Department ?? string.Empty;
                }
                return _departmentName;
            }
        }
    }
}
