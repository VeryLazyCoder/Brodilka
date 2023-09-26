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
            new RandomEvents("Вы протёрли свои очки и увидели нового врага","придётся перемещаться осторожнее :)"),
            new RandomEvents("Вы нашли подозрительную синюю таблетку с лекарством",
                "Приняв её вы выяснили, что здоровье увеличилось"),
            new RandomEvents("Вы нашли совсем неподозрительную красную таблетку",
                "Выпив её ваше здоровье почему-то уменьшилось"),
            new RandomEvents("Мы решили вас похвалить","Прочитайте комплимент в ваш адрес :)")
        };
        public static bool InvokeEvent(GameMap map, Player player,
            List<IEnemy> enemies, bool flag = false)
        {
            if (flag)
                return false;
            
            if (new Random().Next(35) == 22)
            {
                ProcessEvent(map, player, enemies);
                return true;
            }
            return false;
        }

        private static void ProcessEvent(GameMap map, Player player, List<IEnemy> enemies)
        {
            int randomEventNumber = new Random().Next(_events.Count);
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
                case 0:
                    player.MovesAvailable -= 5;
                    break;
                case 1:
                    while (true)
                    {
                        var position = Program.GetEmptyPosition(map);
                        if (map[position] == ' ')
                        {
                            map[position] = 'X';
                            map.AddOneTreasure();
                            return;
                        }
                    }
                case 2:
                    player.MovesAvailable += 5;
                    break;
                case 3:
                    enemies.Add(new CommomEnemy(Program.GetEmptyPosition(map), map));
                    break;
                case 4:
                    player.ChangeHealth(player.Health / 10);
                    break;
                case 5:
                    player.ChangeHealth(-15);
                    break;
                case 6:
                    string[] compliments = { "Вы лучший",
                         "Вы очаровательны сегодня и весьма умны. Продолжайте играть, но не принимайте все всерьез.\n" +
                         "Помните, что это всего лишь игра.", "Вы пышите здоровьем и ваша броня крепка." +
                         " Продолжайте в том же духе.\n" +"Игра - ваша тема!",
                            "Сударь! А может быть, сударыня! Вы умны и здоровы, а можете стать богатым. " +
                            "\nПродолжайте собирать сокровища. Мы вас любим."
                            , "Вперёд, вам по силам пройти эту игру!!!" };
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(compliments[new Random().Next(compliments.Length)]);
                    break;
            }
            Console.ReadKey();
            Console.Clear();
            map.DrawMap();
        }
    }
}
