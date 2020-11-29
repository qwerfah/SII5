using SII5.Helpers;
using SII5.Measures;
using SII5.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SII5.ViewModels
{
    public partial class ApplicationViewModel : Observable
    {
        private void GenerateRecommendations()
        {
            SetRateForAllUsers();

            List<User> users = Users.ToList();
            users.Sort(delegate (User user1, User user2)
            {
                if (user1 == null && user2 == null) return 0;
                if (user1 == null) return 1;
                if (user2 == null) return -1;

                if (user1.Rate == user2.Rate) return 0;
                if (user1.Rate > user2.Rate) return -1;
                return 1;
            });

            users = users.Take(UsersCount).ToList();

            var recommendations = new List<Node>();

            foreach (User user in users)
            {
                var diff = user.Favourite.Except(CurrentUser.Favourite);
                recommendations.AddRange(diff);
            }

            Recommendations = new ObservableCollection<Node>(
                recommendations.Distinct().Except(CurrentUser.NotShow));

            AddRecommendationsToItems();
        }

        private void SetRateForAllUsers()
        {
            foreach (User user in Users)
            {
                user.Rate = user.Favourite.Intersect(CurrentUser.Favourite).Count();
            }
        }

        private void AddRecommendationsToItems()
        {
            RecommendationsItems = new ObservableCollection<TreeViewItem>();

            foreach (Node node in Recommendations)
            {
                var item = new TreeViewItem();
                var menu = new ContextMenu();

                menu.Items.Add(new MenuItem
                {
                    Header = "Больше не предлагать",
                    Command = AddToNotShowCommand,
                    CommandParameter = item,
                });
                menu.Items.Add(new MenuItem
                {
                    Header = "Добавить в сохраненные",
                    Command = AddToFavouriteCommand,
                    CommandParameter = item,
                });

                item.ContextMenu = menu;
                item.Header = node.ToString();
                Tree.AddNodeToTreeView(item, node);
                RecommendationsItems.Add(item);
            }
        }

        private void GenerateRecsForSingleNode(Node node)
        {
            List<Node> nodes = new List<Node>();

            MemoryTree.ToList(nodes);
            nodes = nodes.Except(CurrentUser.NotShow).Except(new[] { node }).ToList();

            foreach (Node n in nodes)
            {
                DistanceCalculator calculator = new DistanceCalculator(node, n);
                n.Distance = calculator.CalculateDistance((MeasureType)MeasureTypeIndex);
            }

            nodes.Sort(delegate (Node node1, Node node2)
            {
                if (node1 == null && node2 == null) return 0;
                if (node1 == null) return -1;
                if (node2 == null) return 1;

                if (node1.Distance == node2.Distance) return 0;
                if (node1.Distance > node2.Distance) return 1;
                return -1;
            });

            Recommendations = new ObservableCollection<Node>(nodes.Take(RecsLength));

            AddRecommendationsToItems();
        }

        private void GenerateRecsForNodeArray()
        {
            List<Node> nodes = new List<Node>();

            MemoryTree.ToList(nodes);
            nodes = nodes.Except(CurrentUser.NotShow).Except(CurrentUser.Favourite).ToList();

            foreach (Node node in nodes)
            {
                node.Distance = 0.0;

                foreach (Node n in CurrentUser.Favourite)
                {
                    DistanceCalculator calculator = new DistanceCalculator(node, n);
                    node.Distance += calculator.CalculateDistance((MeasureType)MeasureTypeIndex);
                }

                node.Distance /= (double)CurrentUser.Favourite.Count();
            }

            nodes.Sort(delegate (Node node1, Node node2)
            {
                if (node1 == null && node2 == null) return 0;
                if (node1 == null) return -1;
                if (node2 == null) return 1;

                if (node1.Distance == node2.Distance) return 0;
                if (node1.Distance > node2.Distance) return 1;
                return -1;
            });

            Recommendations = new ObservableCollection<Node>(nodes.Take(UsersCount));

            AddRecommendationsToItems();
        }
    }
}
