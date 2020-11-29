using SII5.Helpers;
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
        /// <summary> Добавить нового пользователя. </summary>
        /// <param name="user"> Пользователь для добавления. </param>
        private void AddUser(User user)
        {
            Users.Add(user ?? throw new ArgumentNullException(nameof(user)));

            var menu = new ContextMenu();

            menu.Items.Add(new MenuItem
            {
                Header = "Сменить пользователя",
                Command = ChangeUserCommand,
                CommandParameter = user.Name
            });
            menu.Items.Add(new MenuItem
            {
                Header = "Удалить пользователя",
                Command = DeleteUserCommand,
                CommandParameter = user.Name
            });

            UserItems.Add(new ListViewItem { Content = user, ContextMenu = menu });
        }

        private void ChangeUser(User user)
        {
            CurrentUserName = user.Name;
            CurrentUser = user;

            FavouriteItems = new ObservableCollection<TreeViewItem>();
            NotShowItems = new ObservableCollection<TreeViewItem>();
            
            foreach (Node node in user.Favourite)
            {
                var item = new TreeViewItem { Header = node.Name };
                var menu = new ContextMenu();

                menu.Items.Add(new MenuItem
                {
                    Header = "Сгенерировать рекомендации",
                    Command = GenerateRecsForSingleNodeCommand,
                    CommandParameter = item,
                });
                menu.Items.Add(new MenuItem
                {
                    Header = "Удалить",
                    Command = RemoveFromFavouriteCommand,
                    CommandParameter = item,
                });

                item.ContextMenu = menu;
                Tree.AddNodeToTreeView(item, node);
                FavouriteItems.Add(item);
            }

            foreach (Node node in user.NotShow)
            {
                var item = new TreeViewItem { Header = node.Name };
                var menu = new ContextMenu();

                menu.Items.Add(new MenuItem
                {
                    Header = "Удалить",
                    Command = RemoveFromNotShowCommand,
                    CommandParameter = item,
                });

                item.ContextMenu = menu;
                Tree.AddNodeToTreeView(item, node);
                NotShowItems.Add(item);
            }
        }

        /// <summary> Удалить пользователя. </summary>
        /// <param name="user"> Пользователь для удаления. </param>
        private void RemoveUser(User user)
        {
            ListViewItem userItem = UserItems.Where(u => u.Content == user).SingleOrDefault();

            Users.Remove(user);
            UserItems.Remove(userItem);
        }

        /// <summary> Добавить узел в список сохраненных. </summary>
        /// <param name="node"> Узел, который нужно добавить. </param>
        private void AddToFavourite(Node node)
        {
            var item = new TreeViewItem { Header = node.Name };
            var menu = new ContextMenu();

            menu.Items.Add(new MenuItem
            {
                Header = "Сгенерировать рекомендации",
                Command = GenerateRecsForSingleNodeCommand,
                CommandParameter = item,
            });
            menu.Items.Add(new MenuItem
            {
                Header = "Удалить",
                Command = RemoveFromFavouriteCommand,
                CommandParameter = item,
            });

            item.ContextMenu = menu;
            Tree.AddNodeToTreeView(item, node);
            FavouriteItems.Add(item);
            CurrentUser.Favourite.Add(node);
        }

        /// <summary> Удалить узел из списка сохраненных. </summary>
        /// <param name="node"> Узел, который нужно удалить. </param>
        private void RemoveFromFavourite(Node node)
        {
            TreeViewItem nodeItem = FavouriteItems
                .Where(i => i.Header.ToString() == node.Name).SingleOrDefault();

            FavouriteItems.Remove(nodeItem);
            CurrentUser.Favourite.Remove(node);
        }

        /// <summary> Добавить узул в список "Больше не предлагать". </summary>
        /// <param name="node"> Узел, который нужно добавить. </param>
        private void AddToNotShow(Node node)
        {
            var item = new TreeViewItem { Header = node.Name };
            var menu = new ContextMenu();

            menu.Items.Add(new MenuItem
            {
                Header = "Удалить",
                Command = RemoveFromNotShowCommand,
                CommandParameter = item,
            });

            item.ContextMenu = menu;
            Tree.AddNodeToTreeView(item, node);
            NotShowItems.Add(item);
            CurrentUser.NotShow.Add(node);
        }

        /// <summary> Удалить элемент из списка "Больше не предлагать". </summary>
        /// <param name="node"> Узел, который нужно удалить. </param>
        private void RemoveFromNotShow(Node node)
        {
            TreeViewItem nodeItem = NotShowItems
                .Where(i => i.Header.ToString() == node.Name).SingleOrDefault();

            NotShowItems.Remove(nodeItem);
            CurrentUser.NotShow.Remove(node);
        }

        /// <summary> Загрузка дерева из JSON-файла. </summary>
        private void LoadTreeFromFile()
        {
            try
            {
                string fileContent = string.IsNullOrWhiteSpace(Filename) ? 
                    SII5.Properties.Resources.tree : File.ReadAllText(Filename);

                MemoryTree = JsonSerializer.Deserialize<Tree>(fileContent);
                MemoryTree.SetParents();

                var root = new TreeViewItem();
                root.Header = "Дерево";
                MemoryTree.AddToTreeView(root, new ICommand[] { AddToFavouriteCommand, AddToNotShowCommand });

                var items = new ObservableCollection<TreeViewItem>();
                items.Add(root);
                MemoryTreeViewItems = items;
            }
            catch (ArgumentException)
            {
                MessageBox.Show($"Ошибка при загрузке дерева: неверный путь к файлу.\n{Filename}", "Ошибка");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show($"Ошибка при загрузке дерева: указанный путь не существует.\n{Filename}", "Ошибка");
            }
            catch (JsonException)
            {
                MessageBox.Show("Ошибка при загрузке дерева: неверный формат данных.", "Ошибка");
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                MessageBox.Show($"Ошибка при загрузке дерева: {e.Message}", "Ошибка");
            }
        }
    }
}
