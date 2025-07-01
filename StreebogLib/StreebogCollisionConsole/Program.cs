using StreebogLib;
using System;

namespace StreebogCollisionConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var collisionFinder = new StreebogCollisionFinder();
            int[] nValues = { 1, 2, 4, 6, 8, 10, 12, 14, 16 };

            // Тестирование базового метода
            Console.WriteLine("Базовый метод поиска коллизий:");
            foreach (int n in nValues)
            {
                Console.WriteLine($"n = {n}");
                var (msg1, msg2) = collisionFinder.FindCollisionBasic(n);
                Console.WriteLine($"Коллизия найдена:\n{msg1}\n{msg2}\n");
            }

            // Тестирование итеративного метода
            Console.WriteLine("\nИтеративный метод поиска коллизий:");
            foreach (int n in nValues)
            {
                Console.WriteLine($"n = {n}");
                var (bytes1, bytes2) = collisionFinder.FindCollisionIterative(n);
                Console.WriteLine($"Коллизия найдена:\n{BitConverter.ToString(bytes1)}\n{BitConverter.ToString(bytes2)}\n");
            }

            // Тестирование осмысленных коллизий
            Console.WriteLine("\nПоиск осмысленных коллизий:");
            string catPath = "cat.jpg";
            string dogPath = "dog.jpg";

            foreach (int n in nValues)
            {
                Console.WriteLine($"n = {n}");
                try
                {
                    var (img1, img2) = collisionFinder.FindMeaningfulCollision(catPath, dogPath, n);
                    Console.WriteLine($"Осмысленная коллизия найдена для n={n}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Для n={n}: {ex.Message}");
                }
            }
        }
    }
}
