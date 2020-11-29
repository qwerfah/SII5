using SII5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SII5.Search
{
    public static class ParametricSearch
    {
        /// <summary> Параметрический поиск. </summary>
        /// <param name="searchParams"> Параметры поиска. </param>
        public static IEnumerable<Node> Search(SearchParams searchParams)
        {
            List<Node> nodes = new List<Node>();
            searchParams.Tree.ToList(nodes);

            foreach (Node node in nodes)
            {
                node.Points = ChargePoints(node, searchParams);
            }

            nodes.Sort(delegate (Node node1, Node node2)
            {
                if (node1 == null && node2 == null) return 0;
                if (node1 == null) return -1;
                if (node2 == null) return 1;

                if (node1.Points == node2.Points) return 0;
                if (node1.Points > node2.Points) return -1;
                return 1;
            });

            return nodes.Take(searchParams.Count);
        }

        /// <summary> Функция начисления очков за соответствие параметрам поиска. </summary>
        /// <param name="node"> Узел, для которого вычисляются очки. </param>
        /// <param name="searchParams"> Параметры поиска. </param>
        private static int ChargePoints(Node node, SearchParams searchParams)
        {
            int points = 0;

            points += searchParams.SpeedInterval.IsInInterval(node.Params.MaxSpeed) ? 1 : 0;
            points += searchParams.CapacityInterval.IsInInterval(node.Params.MaxStorageCapacity) ? 1 : 0;
            points += searchParams.YearInterval.IsInInterval(node.Params.ReleaseYear) ? 1 : 0;
            points += searchParams.CostInterval.IsInInterval(node.Params.AverageCost) ? 1 : 0;
            points += (!searchParams.ApplicationType.HasValue ? true :
                searchParams.ApplicationType == node.Params.IsGeneralPurpose) ? 1 : 0;
            points += (!searchParams.MemoryType.HasValue ? true :
                node.Params.MemoryTypes.Contains(searchParams.MemoryType.Value)) ? 1 : 0;

            return points;
        }
    }
}
