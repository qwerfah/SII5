using Microsoft.Win32;
using SII5.Helpers;
using SII5.Models;
using SII5.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SII5.ViewModels
{
    public partial class ApplicationViewModel : Observable
    {
        private ICommand _openFileDialogCommand;
        private ICommand _loadTreeCommand;
        private ICommand _addUserCommand;
        private ICommand _changeUserCommand;
        private ICommand _deleteUserCommand;

        private ICommand _generateRecommendationsCommand;
        private ICommand _generateRecsForSingleNodeCommand;
        private ICommand _generateRecsForNodeArrayCommand;
        private ICommand _searchCommand;

        private ICommand _addToFavouriteCommand;
        private ICommand _addToNotShowCommand;
        private ICommand _removeFromFavouriteCommand;
        private ICommand _removeFromNotShowCommand;

        public ICommand OpenFileDialogCommand => _openFileDialogCommand ??= new RelayCommand<object>(_ =>
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog().Value)
            {
                Filename = fileDialog.FileName;
            }
        });

        public ICommand LoadTreeCommand => _loadTreeCommand ??= new RelayCommand<object>(_ =>
        {
            LoadTreeFromFile();
        });

        public ICommand AddUserCommand => _addUserCommand ??= new RelayCommand<string>(username =>
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show($"Некорректное имя пользователя.", "Ошибка");
            }
            else if (Users.Where(u => u.Name == username).Any())
            {
                MessageBox.Show($"Пользователь с именем {username} уже существует.", "Ошибка");
            }
            else
            {
                AddUser(new User(username));
            }
        });

        public ICommand ChangeUserCommand => _changeUserCommand ??= new RelayCommand<string>(username =>
        {
            User user = Users.Where(u => u.Name == username).SingleOrDefault();
            if (user == null)
            {
                MessageBox.Show($"Пользователя с именем {username} не существует.", "Ошибка");
            }
            else
            {
                ChangeUser(user);
            }
        });

        public ICommand DeleteUserCommand => _deleteUserCommand ??= new RelayCommand<string>(username =>
        {
            User user = Users.Where(u => u.Name ==
            (username ?? throw new ArgumentNullException(nameof(username)))).SingleOrDefault();
            if (user == null)
            {
                MessageBox.Show($"Пользователя с именем {username} не существует.", "Ошибка");
            }
            else RemoveUser(user);
        });

        public ICommand GenerateRecommendationsCommand =>
            _generateRecommendationsCommand ??= new RelayCommand<object>(_ =>
            {
                if (CurrentUser == null)
                {
                    MessageBox.Show("Не выбран пользователь.", "Ошибка");
                }
                else
                {
                    GenerateRecommendations();
                }
            });

        public ICommand GenerateRecsForSingleNodeCommand =>
            _generateRecsForSingleNodeCommand ??= new RelayCommand<TreeViewItem>(item =>
        {
            Node node = MemoryTree.GetNode(item.Header.ToString());

            if (node == null)
            {
                MessageBox.Show("Узел с указанным именем не найден в дереве.", "Ошибка");
            }
            else
            {
                GenerateRecsForSingleNode(node);
            }
        });


        public ICommand GenerateRecsForNodeArrayCommand => 
            _generateRecsForNodeArrayCommand ??= new RelayCommand<object>(_ => 
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Не выбран пользователь.", "Ошибка");
            }
            else if (!CurrentUser.Favourite.Any())
            {
                MessageBox.Show("Ничего не сохранено.", "Ошибка");
            }
            else
            {
                GenerateRecsForNodeArray();
            }
        });

        public ICommand SearchCommand => _searchCommand ??= new RelayCommand<object>(_ =>
        {
            try
            {
                IEnumerable<Node> result = ParametricSearch.Search(new SearchParams
                {
                    Tree = MemoryTree,
                    SpeedInterval = new Interval(MinSpeed, MaxSpeed),
                    CapacityInterval = new Interval(MinCapacity, MaxCapacity),
                    YearInterval = new Interval(MinReleaseYear, MaxReleaseYear),
                    CostInterval = new Interval(MinCost, MaxCost),
                    ApplicationType = (Application == 2) ? (bool?)null : (Application == 0),
                    MemoryType = (MemoryType == 5) ? null : (MemoryType?)MemoryType,
                    Count = RecsLength
                });

                Recommendations = new ObservableCollection<Node>(result);

                AddRecommendationsToItems();
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                MessageBox.Show(e.Message, "Ошибка");
            }
        });

        public ICommand AddToFavouriteCommand => _addToFavouriteCommand ??= new RelayCommand<TreeViewItem>(item =>
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Не выбран пользователь.", "Ошибка");
            }
            else
            {
                string name = item.Header.ToString();
                Node node = MemoryTree.GetNode(name);

                if (node == null)
                {
                    MessageBox.Show("Узел с указанным именем не найден в дереве.", "Ошибка");
                }
                else if (CurrentUser.Favourite.Where(n => n.Name == name).Any())
                {
                    MessageBox.Show("Узел с указанным именем уже в списке.", "Ошибка");
                }
                else AddToFavourite(node);
            }
        });

        public ICommand RemoveFromFavouriteCommand =>
            _removeFromFavouriteCommand ??= new RelayCommand<TreeViewItem>(item =>
            {
                if (CurrentUser == null)
                {
                    MessageBox.Show("Не выбран пользователь.", "Ошибка");
                }
                else
                {
                    Node node = CurrentUser.Favourite.Where(
                        n => n.Name == item.Header.ToString()).SingleOrDefault();

                    if (node == null)
                    {
                        MessageBox.Show("Узел с указанным именем не найден в списке.", "Ошибка");
                    }
                    else RemoveFromFavourite(node);
                }
            });

        public ICommand AddToNotShowCommand => _addToNotShowCommand ??= new RelayCommand<TreeViewItem>(item =>
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Не выбран пользователь.", "Ошибка");
            }
            else
            {
                string name = item.ToString();
                Node node = MemoryTree.GetNode(name);

                if (node == null)
                {
                    MessageBox.Show("Узел с указанным именем не найден в дереве.", "Ошибка");
                }
                else if (CurrentUser.NotShow.Where(n => n.Name == name).Any())
                {
                    MessageBox.Show("Узел с указанным именем уже в списке.", "Ошибка");
                }
                else AddToNotShow(node);
            }
        });

        public ICommand RemoveFromNotShowCommand =>
            _removeFromNotShowCommand ??= new RelayCommand<TreeViewItem>(item =>
        {
            if (CurrentUser == null)
            {
                MessageBox.Show("Не выбран пользователь.", "Ошибка");
            }
            else
            {
                Node node = CurrentUser.NotShow
                    .Where(n => n.Name == item.Header.ToString()).SingleOrDefault();

                if (node == null)
                {
                    MessageBox.Show("Узел с указанным именем не найден в списке.", "Ошибка");
                }
                else RemoveFromNotShow(node);
            }
        });
    }
}
