
namespace HodimBrodim
{
    public class RandomEvents
    {
        private string _nameOfEvent;
        private string _description;
        public RandomEvents(string nameOfEvent, string description)
        {
            _nameOfEvent = nameOfEvent;
            _description = description;
        }
        private static List<RandomEvents> _events = new List<RandomEvents>()
        {
            new RandomEvents("К вам пришла налоговая","вы не в состоянии думать ни о чём, кроме налогов. " +
                "\nПропустите 5 ходов и платите налоги вовремя"),
            new RandomEvents("К вам в руки попала новая карта лабиринта","на ней обнаружилось новое сокровище"),
            new RandomEvents("Вы нашли ковёр самолёт"," у вас стало больше времени. " +
                "Получите 5 ходов"),
            new RandomEvents("На вас напали конкуренты","придётся с ними сразиться"),
            new RandomEvents("Вы протёрли свои очки и увидели нового врага","придётся перемещаться осторожнее :)"),
            new RandomEvents("Вы нашли подозрительную синюю таблетку с лекарством",
                "Приняв её вы выяснили, что здоровье увеличилось"),
            new RandomEvents("Вы нашли совсем неподозрительную красную таблетку",
                "Выпив её ваше здоровье почему-то уменьшилось"),
            new RandomEvents("Мы решили вас похвалить","Прочитайте комплимент в ваш адрес :)")
        };
        public static void RandomEvent(GameMap map, Player player,
            List<IEnemy> enemies)
        {
            Random randomChance = new Random();
            int randomEventNumber = randomChance.Next(0, _events.Count);

            if (randomChance.Next(0, 25) == 12)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Случилось страшное!? вращайте барабан...");
                Console.ReadKey();
                Console.SetCursorPosition(0, 1);
                Console.WriteLine($"{_events[randomEventNumber]._nameOfEvent}. " +
                    $"В итоге {_events[randomEventNumber]._description}");
                Thread.Sleep(250);
                switch (randomEventNumber)
                {
                    case 0: // налоговая, - 5 ходов
                        Program.MovesAvailable -= 5;
                        break;
                    case 1: // новое сокровище
                        bool rightPostion = false;
                        while (rightPostion == false)
                        {
                            var position = Program.GetPosition(map);
                            if (map[position] == ' ')
                            {
                                map[position] = 'X';
                                map.AddOneTreasure();
                                rightPostion = true;
                            }
                        }
                        break;
                    case 2:
                        Program.MovesAvailable += 5;
                        break;
                    case 3:
                        Thread.Sleep(1000);
                        player.FightWithEnemy();
                        break;
                    case 4:
                        enemies.Add(new CommomEnemy(Program.GetPosition(map), map));
                        break;
                    case 5:
                        player.Health = player.Health / 10;
                        break;
                    case 6:
                        player.Health = -15;
                        break;
                    case 7:
                        string[] compliments = { "Вы лучший",
                         "Вы очаровательны сегодня и весьма умны. Продолжайте играть, но не принимайте все всерьез.\n" +
                         "Помните, что это всего лишь игра.", "Вы пышите здоровьем и ваша броня крепка." +
                         " Продолжайте в том же духе.\n" +"Игра - ваша тема!",
                            "Сударь! А может быть, сударыня! Вы умны и здоровы, а можете стать богатым. " +
                            "\nПродолжайте собирать сокровища. Мы вас любим."
                            , "Вперёд, вам по силам пройти эту игру!!!" };
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(compliments[randomChance.Next(0, compliments.Length)]);
                        break;
                }
                Console.ReadKey();
                Console.Clear();
                map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
            }
        }
    }
}
