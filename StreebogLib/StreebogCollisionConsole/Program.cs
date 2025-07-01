using StreebogLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StreebogCollisionConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var collisionFinder = new StreebogCollisionFinder();
            int[] nValues = { 1, 2, 3 };
            var stopwatch = new Stopwatch();

            // Создаем папку для результатов, если ее нет
            string resultsDir = "CollisionResults";
            Directory.CreateDirectory(resultsDir);

            // Тестирование базового метода
            Console.WriteLine("Базовый метод поиска коллизий:");
            foreach (int n in nValues)
            {
                Console.WriteLine($"n = {n}");
                stopwatch.Restart();
                var (msg1, msg2) = collisionFinder.FindCollisionBasic(n);
                stopwatch.Stop();

                // Получаем хеши через внутренний Streebog
                byte[] hash1 = collisionFinder.GetHashInternal(Encoding.UTF8.GetBytes(msg1));
                byte[] hash2 = collisionFinder.GetHashInternal(Encoding.UTF8.GetBytes(msg2));

                Console.WriteLine($"Коллизия найдена за {stopwatch.Elapsed.TotalSeconds} сек:");
                Console.WriteLine($"Сообщение 1: {msg1}");
                Console.WriteLine($"Сообщение 2: {msg2}");
                Console.WriteLine($"Хеш 1 (первые {n} байт): {BitConverter.ToString(hash1, 0, n)}");
                Console.WriteLine($"Хеш 2 (первые {n} байт): {BitConverter.ToString(hash2, 0, n)}");
                Console.WriteLine();
            }

            // Тестирование итеративного метода
            Console.WriteLine("\nИтеративный метод поиска коллизий:");
            foreach (int n in nValues)
            {
                Console.WriteLine($"n = {n}");
                stopwatch.Restart();
                var (bytes1, bytes2) = collisionFinder.FindCollisionIterative(n);
                stopwatch.Stop();

                // Получаем хеши через внутренний Streebog
                byte[] hash1 = collisionFinder.GetHashInternal(bytes1);
                byte[] hash2 = collisionFinder.GetHashInternal(bytes2);

                Console.WriteLine($"Коллизия найдена за {stopwatch.Elapsed.TotalSeconds} сек:");
                Console.WriteLine($"Байты 1: {BitConverter.ToString(bytes1)}");
                Console.WriteLine($"Байты 2: {BitConverter.ToString(bytes2)}");
                Console.WriteLine($"Хеш 1 (первые {n} байт): {BitConverter.ToString(hash1, 0, n)}");
                Console.WriteLine($"Хеш 2 (первые {n} байт): {BitConverter.ToString(hash2, 0, n)}");
                Console.WriteLine();
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
                    stopwatch.Restart();
                    var (img1, img2) = collisionFinder.FindMeaningfulCollision(catPath, dogPath, n);
                    stopwatch.Stop();

                    // Сохраняем коллизионные изображения
                    string outputPath1 = Path.Combine(resultsDir, $"collision_cat_n{n}.jpg");
                    string outputPath2 = Path.Combine(resultsDir, $"collision_dog_n{n}.jpg");

                    File.WriteAllBytes(outputPath1, img1);
                    File.WriteAllBytes(outputPath2, img2);

                    // Получаем хеши через внутренний Streebog
                    byte[] hash1 = collisionFinder.GetHashInternal(img1);
                    byte[] hash2 = collisionFinder.GetHashInternal(img2);

                    Console.WriteLine($"Осмысленная коллизия найдена за {stopwatch.Elapsed.TotalSeconds} сек для n={n}");
                    Console.WriteLine($"Хеш оригинального кота (первые {n} байт): {BitConverter.ToString(hash1, 0, n)}");
                    Console.WriteLine($"Хеш модифицированной собаки (первые {n} байт): {BitConverter.ToString(hash2, 0, n)}");
                    Console.WriteLine($"Сохраненные файлы:");
                    Console.WriteLine($"- {outputPath1}");
                    Console.WriteLine($"- {outputPath2}");
                    Console.WriteLine($"Размер файлов: {img1.Length} байт и {img2.Length} байт\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Для n={n}: {ex.Message} (затрачено времени: {stopwatch.Elapsed.TotalSeconds} сек)");
                }
            }

            Console.WriteLine($"\nВсе результаты сохранены в папке: {Path.GetFullPath(resultsDir)}");
        }
    }
}