using SII5.Helpers;
using SII5.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace SII5.ViewModels
{
    /// <summary> ViewModel для MainView. </summary>
    public partial class ApplicationViewModel : Observable
    {
        private Tree _memoryTree;
        private ObservableCollection<TreeViewItem> _memoryTreeViewItems;

        private ObservableCollection<User> _users;
        private ObservableCollection<ListViewItem> _userItems;

        private User _currentUser;

        private ObservableCollection<TreeViewItem> _favouriteItems;
        private ObservableCollection<TreeViewItem> _notShowItems;

        private ObservableCollection<Node> _recommendations;
        private ObservableCollection<TreeViewItem> _recommendationsItems;

        /// <summary> Дерево типов памяти. </summary>
        public Tree MemoryTree
        {
            get => _memoryTree;
            set => Set(ref _memoryTree, value);
        }
        /// <summary> TreeView для отображения дерева типов памяти. </summary>
        public ObservableCollection<TreeViewItem> MemoryTreeViewItems
        {
            get => _memoryTreeViewItems;
            set => Set(ref _memoryTreeViewItems, value);
        }
        /// <summary> Список пользователей системы. </summary>
        public ObservableCollection<User> Users
        {
            get => _users;
            set => Set(ref _users, value);
        }
        /// <summary> Список пользователей системы (ListView). </summary>
        public ObservableCollection<ListViewItem> UserItems
        {
            get => _userItems;
            set => Set(ref _userItems, value);
        }
        /// <summary> Текущий выбранный пользователь. </summary>
        public User CurrentUser
        {
            get => _currentUser;
            set => Set(ref _currentUser, value);
        }
        /// <summary> Список сохраненных элементов текущего пользователя. </summary>
        public ObservableCollection<TreeViewItem> FavouriteItems
        {
            get => _favouriteItems;
            set => Set(ref _favouriteItems, value);
        }
        /// <summary> Список элементов текущего пользователя, помеченных как "больше не придлагать" (ListView). </summary>
        public ObservableCollection<TreeViewItem> NotShowItems
        {
            get => _notShowItems;
            set => Set(ref _notShowItems, value);
        }
        /// <summary> Список рекомендаций текущего пользователя. </summary>
        public ObservableCollection<Node> Recommendations
        {
            get => _recommendations;
            set => Set(ref _recommendations, value);
        }
        /// <summary> Список рекомендаций текущего пользователя (ListView). </summary>
        public ObservableCollection<TreeViewItem> RecommendationsItems
        {
            get => _recommendationsItems;
            set => Set(ref _recommendationsItems, value);
        }

        private string _filename = "";
        private string _userName = "";
        private string _currentUserName = "";

        private int _usersCount = 5;
        private int _recsLength = 10;

        private double _minSpeed = 100;
        private double _maxSpeed = 1000;

        private double _minCapacity = 100;
        private double _maxCapacity = 1000;

        private int _minReleaseYear = 2000;
        private int _maxReleaseYear = 2010;

        private double _minCost = 10.0;
        private double _maxCost = 100.0;

        /// <summary> Путь к JSON-файлу с деревом. </summary>
        public string Filename
        {
            get => _filename;
            set => Set(ref _filename, value);
        }
        /// <summary> Имя пользователя (поле ввода). </summary>
        public string UserName
        {
            get => _userName;
            set => Set(ref _userName, value);
        }
        /// <summary> Имя текущего пользователя системы. </summary>
        public string CurrentUserName
        {
            get => _currentUserName;
            set => Set(ref _currentUserName, value);
        }
        /// <summary> Число пользователей при подборе рекомендаций. </summary>
        public int UsersCount
        {
            get => _usersCount;
            set => Set(ref _usersCount, value);
        }
        /// <summary> Максимальная длина рекомендаций. </summary>
        public int RecsLength
        {
            get => _recsLength;
            set => Set(ref _recsLength, value);
        }

        public double MinSpeed
        {
            get => _minSpeed;
            set => Set(ref _minSpeed, value);
        }

        public double MaxSpeed
        {
            get => _maxSpeed;
            set => Set(ref _maxSpeed, value);
        }

        public double MinCapacity
        {
            get => _minCapacity;
            set => Set(ref _minCapacity, value);
        }

        public double MaxCapacity
        {
            get => _maxCapacity;
            set => Set(ref _maxCapacity, value);
        }

        public int MinReleaseYear
        {
            get => _minReleaseYear;
            set => Set(ref _minReleaseYear, value);
        }

        public int MaxReleaseYear
        {
            get => _maxReleaseYear;
            set => Set(ref _maxReleaseYear, value);
        }

        public double MinCost
        {
            get => _minCost;
            set => Set(ref _minCost, value);
        }

        public double MaxCost
        {
            get => _maxCost;
            set => Set(ref _maxCost, value);
        }

        private int _measureTypeIndex = 0;
        private int _application = 0;
        private int _memoryType = 0;

        /// <summary> Индекс выбранной меры близости. </summary>
        public int MeasureTypeIndex
        {
            get => _measureTypeIndex;
            set => Set(ref _measureTypeIndex, value);
        }

        /// <summary> Назначение памяти. </summary>
        public int Application
        {
            get => _application;
            set => Set(ref _application, value);
        }

        /// <summary> Тип памяти. </summary>
        public int MemoryType
        {
            get => (int)_memoryType;
            set => Set(ref _memoryType, value);
        }

        public ApplicationViewModel()
        {
            Users = new ObservableCollection<User>();
            
            UserItems = new ObservableCollection<ListViewItem>();
            FavouriteItems = new ObservableCollection<TreeViewItem>();
            RecommendationsItems = new ObservableCollection<TreeViewItem>();

            LoadTreeFromFile();

            var user = new User("user1");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR3-800"),
                MemoryTree.GetNode("DDR3-1066"),
                MemoryTree.GetNode("DDR3-1333"),
                MemoryTree.GetNode("DDR3-1600"),
            };
            AddUser(user);

            user = new User("user2");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR3-1066"),
                MemoryTree.GetNode("DDR3-1333"),
                MemoryTree.GetNode("DDR3-1600"),
                MemoryTree.GetNode("DDR3-1866"),
                MemoryTree.GetNode("DDR3-2133"),
            };
            AddUser(user);

            user = new User("user3");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR4-1600"),
                MemoryTree.GetNode("DDR4-1866"),
                MemoryTree.GetNode("DDR3-1866"),
                MemoryTree.GetNode("DDR3-2133"),
            };
            AddUser(user);

            user = new User("user4");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR4-1600"),
                MemoryTree.GetNode("DDR4-1866"),
                MemoryTree.GetNode("DDR3-1600"),
            };
            AddUser(user);

            user = new User("user5");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR4-1600"),
                MemoryTree.GetNode("DDR4-1866"),
                MemoryTree.GetNode("DDR3-1066"),
            };
            AddUser(user);

            user = new User("user6");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR4-1600"),
            };
            AddUser(user);

            user = new User("user7");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DDR4-2666"),
                MemoryTree.GetNode("DDR4-2933"),
            };
            AddUser(user);

            user = new User("user8");
            user.Favourite = new List<Node>
            {
                MemoryTree.GetNode("DRAM"),
                MemoryTree.GetNode("DDR3-1066"),
            };
            AddUser(user);
        }
    }
}
