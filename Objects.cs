using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZPG_Server
{
    public class Objects
    {
        public class Players //Персонажи игроков
        {
            public struct Player //Структура каждого персонажа
            {
                public int STR; //Сила
                public int WIS; //Мудрость
                public int AGI; //Ловкость
                public int LEVEL; //Уровень
                public string CLASS; //Класс (Определяет основную характеристику и типы атаки)
                public string RACE; //Раса
                public string Name; //Имя
                public string Prof; //Профессия
                public int Moral; // (Добро) + : - (Зло)
                public static List<Item> Inventory = new List<Item>(); //Инвентарь 
                public int X;
                public int Y;
                public double Z;
            }
            public static List<Player> PlayersList = new List<Player>(); //Лист персонажей игроков
        }

        public class NPCs //Все другие персонажи 
        {
            public struct NPC
            {
                public string Name; //Имя
                public string Role; //Роль в мире
                public string Fraction; //Фракция персонажа
                public static List<Item> Inventory = new List<Item>();
            }
            public static List<NPC> NPCsList = new List<NPC>(); //Лист персонажей автономных
        }
        public class Item
        {
            public struct ItemIns
            {
                public string Name;
                public int ID;
                public int Cost;
            }
        }
        public class StaticObjects
        {
            public string Name;
        }
        
    }
}
