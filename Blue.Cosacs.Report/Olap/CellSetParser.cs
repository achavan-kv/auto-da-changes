using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AnalysisServices.AdomdClient;

namespace Blue.Cosacs.Report.Olap
{
    public class GenericCellSetParser : ICellSetParser<List<List<String>>>
    {
        public const string StructureMapNamed = "Generic";

        private Indexer indexer;
        
        private class Indexer
        {
            private Func<int[]> indexerArray;

            public Indexer(bool measuresOnColumns)
            {
                //According on which axis the measures are, we will then have to read horizontally or vertically.
                if (measuresOnColumns)
                {
                    indexerArray = () => new int[] { x, y };
                }
                else
                {
                    indexerArray = () => new int[] { y, x };
                }
            }

            public int x
            {
                get;
                set;
            }

            public int y
            {
                get;
                set;
            }

            public int[] GetIndexer()
            {
                return indexerArray();
            }
        }

        private bool UseRows;

        public List<List<string>> Parse(CellSet set)
        {

            if (set.Cells.Count == 0)
            {
                return null;
            }

            var axis = GetAxis(set);
            var columnNames = GetColumnNames(axis);
            var returnValue = new List<List<string>>((set.Cells.Count / columnNames.Count) + 1);

            returnValue.Add(columnNames);

            foreach (var py in axis.Dimensions.Positions)
            {
                var row = new List<string>(columnNames.Count);
                foreach (var mem in py.Members)
                {
                    row.Add(mem.Caption);
                }

                for (indexer.x = 0; indexer.x <= axis.Measures.Positions.Count - 1; indexer.x++)
                {
                    row.Add(set[indexer.GetIndexer()].FormattedValue);
                }
                ++indexer.y;

                returnValue.Add(row);
            }

            return returnValue;
        }

        /// <summary>
        /// Get a list of string with friendly columns names based on the axis metadata
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private List<string> GetColumnNames(AxisCollection axis)
        {
            return axis.Dimensions.Positions[0].Members
                .OfType<Member>()
                //the names came in this format [Dimension].[Attribute].[Attribute| members | allmembers | children | etc ]
                //it is converted to an array, get only 2 positions and put the attribute between parentheses 
                .Select(p => string.Join("", p.LevelName.Split('.').Take(2)).Replace("][", " (").Replace("]", ")").Substring(1))
                .Union(
                    axis.Measures.Positions.OfType<Position>()
                   .SelectMany(p => p.Members.OfType<Member>().Where(member =>
                   {
                       try
                       {
                           //if the member has children it will be ignored
                           return member.GetChildren().Count == 0;
                       }
                       catch (Exception)
                       {
                           //it may happen that we have dimensions mixed with measures. In that case the member will have no children and it will even throw an error if we try to read then.
                           return true;
                       }
                   }).Take(1) //only the first result is needed
                    .ToList())
                   .Select(p => p.Caption))
                .ToList();
        }

        private struct AxisCollection
        {
            /// <summary>
            /// the axis that contain the measures
            /// </summary>
            public Axis Measures
            {
                get;
                set;
            }

            /// <summary>
            /// the axis that contains the dimensions
            /// </summary>
            public Axis Dimensions
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Determine which axis hold the measures
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private AxisCollection GetAxis(CellSet set)
        {
            var returnValue = new AxisCollection();
            var allAxis = set.Axes.OfType<Axis>().ToList();
            const string MeasuresHierarchyName = "[Measures]";

            returnValue.Measures = allAxis.First(axis => axis.Set.Hierarchies.OfType<Hierarchy>().Any(p => p.UniqueName == MeasuresHierarchyName));
            returnValue.Dimensions = allAxis.First(axis => axis != returnValue.Measures);

            indexer = new Indexer(returnValue.Measures == allAxis.First());

            return returnValue;
        }
    }
}