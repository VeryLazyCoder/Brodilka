namespace HodimBrodim
{
    public class RandomEventsHandler
    {
        private readonly List<Event> _events = new()
        {
            new Event("К вам пришла налоговая","вы не в состоянии думать ни о чём, кроме налогов. " +
                "\nПропустите 5 ходов и платите налоги вовремя"),
            new Event("Вы нашли ковёр самолёт"," у вас стало больше времени. " +
                "Получите 5 ходов"),
            new Event("К вам в руки попала новая карта лабиринта","на ней обнаружилось новое сокровище"),
            new Event("Вы протёрли свои очки и увидели нового врага","придётся перемещаться осторожнее :)"),
            new Event("Вы нашли подозрительную синюю таблетку с лекарством",
                "Приняв её вы выяснили, что здоровье увеличилось"),
            new Event("Вы нашли совсем неподозрительную красную таблетку",
                "Выпив её ваше здоровье почему-то уменьшилось"),
        };
        private readonly List<Action<GameRound>> _actions = new()
        {
            game => game.Player.MovesAvailable -= 5,
            game => game.Player.MovesAvailable += 5,
            game => game.Map.AddAdditionalTreasure(),
            game => game.AddAdditionalEnemy(),
            game => game.Player.ChangeHealthFor(25),
            game => game.Player.ChangeHealthFor(-15),
        };
        private readonly Random _random = new();
        private int _eventNumber;

        public bool TryRaiseEvent(GameRound round)
        {
            if (_random.Next(35) != 22) return false;
            InvokeEvent(round);
            return true;
        }

        public void InvokeEvent(GameRound round)
        {
            _eventNumber = _random.Next(_events.Count);
            DisplayEvent();
            _actions[_eventNumber].Invoke(round);
            Console.ReadKey();
            Console.Clear();
        }

        private void DisplayEvent()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(@"Случилось страшное!? вращайте барабан...");
            Console.ReadKey(true);
            Console.SetCursorPosition(0, 1);
            Console.WriteLine(_events[_eventNumber]);
            Thread.Sleep(250);
        }
    }

    internal readonly struct Event
    {
        public string Description { get; init; }
        public string Consequence { get; init; }

        public Event(string description, string consequence)
        {
            Description = description;
            Consequence = consequence;
        }

        public override string ToString()
        {
            return $"{Description}. В результате {Consequence}";
        }
    }
}
