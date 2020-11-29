using System.Collections.Generic;

namespace SII5.Models
{
    /// <summary> Класс, представляющий данные пользователя рекомендательной системы. </summary>
    public class User
    {
        /// <summary> Имя пользователя. </summary>
        public string Name { get; set; } = "";
        /// <summary> Сохраненные узлы. </summary>
        public List<Node> Favourite { get; set; } = new List<Node>();
        /// <summary> Узлы, помеченные как "больше не предлагать". </summary>
        public List<Node> NotShow { get; set; } = new List<Node>();
        /// <summary> Рейтинг совпадения. </summary>
        public int Rate { get; set; } = 0;

        public User(string name)
        {
            Name = name;
            Favourite = new List<Node>();
            NotShow = new List<Node>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
