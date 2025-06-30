using StreebogLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StreebogConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Тестирование примера из стандарта
            TestStandardExample();

            // Тестирование производительности
            TestPerformance();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void TestStandardExample()
        {
            Console.WriteLine("Тестирование алгоритма Streebog (Пример A.1 из стандарта)");

            string messageHex = "323130393837363534333231303938373635343332313039383736353433323130393837363534333231303938373635343332313039383736353433323130";
            Console.WriteLine($"Исходное сообщение (hex): {messageHex}");

            byte[] message = HexToBytes(messageHex);
            var streebog = new Streebog(Streebog.lengthHash.Length_512);
            byte[] hash = streebog.GetHash(message);
            string hashHex = BytesToHex(hash);

            Console.WriteLine($"\nВычисленный хеш (512 бит):");
            Console.WriteLine(hashHex);

            string expectedHash = "486f64c1917879417fef082b3381a4e211c324f074654c38823a7b76f830ad00fa1fbae42b1285c0352f227524bc9ab16254288dd6863dccd5b9f54a1ad0541b";
            Console.WriteLine("\nОжидаемый результат (из стандарта):");
            Console.WriteLine(expectedHash);

            if (hashHex.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nРезультат совпадает с ожидаемым!");
            }
            else
            {
                Console.WriteLine("\nРезультат НЕ совпадает с ожидаемым!");
            }
        }

        static void TestPerformance()
        {
            Console.WriteLine("\nТестирование производительности...");

            // Чтение входных данных из файла
            string[] inputLines = File.ReadAllLines("input.txt");
            string[] hash256Lines = File.ReadAllLines("hash256.txt");
            string[] hash512Lines = File.ReadAllLines("hash512.txt");

            // Создаем экземпляры хеш-функций (здесь же идут предвычисления)
            var streebog256 = new Streebog(Streebog.lengthHash.Length_256);
            var streebog512 = new Streebog(Streebog.lengthHash.Length_512);

            // Тестируем для каждой строки входных данных
            for (int i = 0; i < inputLines.Length; i++)
            {
                Console.WriteLine($"\nТест #{i + 1}");

                byte[] message = HexToBytes(inputLines[i]);

                // Тестируем 256-битную версию
                TestHashImplementation(
                    "Оригинальная 256-bit",
                    () => streebog256.GetHash(message),
                    hash256Lines[i]);

                TestHashImplementation(
                    "Оптимизированная 256-bit",
                    () => streebog256.GetHashOptimized(message),
                    hash256Lines[i]);

                // Тестируем 512-битную версию
                TestHashImplementation(
                    "Оригинальная 512-bit",
                    () => streebog512.GetHash(message),
                    hash512Lines[i]);

                TestHashImplementation(
                    "Оптимизированная 512-bit",
                    () => streebog512.GetHashOptimized(message),
                    hash512Lines[i]);
            }
        }

        static void TestHashImplementation(string name, Func<byte[]> hashFunc, string expectedHash)
        {
            Stopwatch sw = Stopwatch.StartNew();
            byte[] hash = hashFunc();
            sw.Stop();

            string hashHex = BytesToHex(hash);
            bool isCorrect = hashHex.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);

            Console.WriteLine($"{name}:");
            Console.WriteLine($"Время: {sw.ElapsedTicks} мс");
            Console.WriteLine($"Результат: {(isCorrect ? "верный" : "НЕВЕРНЫЙ")}");
            if (!isCorrect)
            {
                Console.WriteLine($"Ожидалось: {expectedHash}");
                Console.WriteLine($"Получено:  {hashHex}");
            }
        }

        static byte[] HexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        static string BytesToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}