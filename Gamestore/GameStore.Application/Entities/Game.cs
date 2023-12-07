using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Application.Entities
{
    public class Game
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Key { get; private set; }

        public string Description { get; set; }

        public Game()
        {
        }

        public Game(Guid id, string name, string key)
        {
            Id = id;
            Name = name;
            Key = key;
        }

        public Game(Guid id, string name, string key, string description)
        {
            Id = id;
            Name = name;
            Key = key;
            Description = description;
        }
    }
}
