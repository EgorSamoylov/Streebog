using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace StreebogLib
{
    public class StreebogCollisionFinder
    {
        private readonly Streebog _streebog;

        public StreebogCollisionFinder(Streebog.lengthHash length = Streebog.lengthHash.Length_512)
        {
            _streebog = new Streebog(length);
        }

        // 2. Поиск коллизии базовым методом (перебор случайных сообщений)
        public (string, string) FindCollisionBasic(int n)
        {
            var hashes = new Dictionary<string, string>();

            while (true)
            {
                string message = GenerateRandomMessage();
                byte[] hash = _streebog.GetHash(Encoding.UTF8.GetBytes(message));
                string hashPrefix = BitConverter.ToString(hash, 0, n).Replace("-", "");

                if (hashes.TryGetValue(hashPrefix, out string existingMessage))
                {
                    if (!existingMessage.Equals(message))
                    {
                        return (existingMessage, message);
                    }
                }
                else
                {
                    hashes[hashPrefix] = message;
                }
            }
        }

        // 2. Поиск коллизии итеративным методом (парадокс дней рождения)
        public (byte[], byte[]) FindCollisionIterative(int n)
        {
            // Генерируем случайное начальное сообщение
            byte[] x0 = GenerateRandomBytes(n + 1);
            byte[] x = x0;
            byte[] xPrime = x0;

            // Фаза поиска цикла (черепаха и заяц)
            while (true)
            {
                x = _streebog.GetHash(x).Take(n).ToArray();
                xPrime = _streebog.GetHash(_streebog.GetHash(xPrime).Take(n).ToArray()).Take(n).ToArray();

                if (x.SequenceEqual(xPrime))
                {
                    break;
                }
            }

            // Фаза нахождения точки столкновения
            xPrime = x;
            x = x0;

            while (true)
            {
                byte[] hx = _streebog.GetHash(x).Take(n).ToArray();
                byte[] hxPrime = _streebog.GetHash(xPrime).Take(n).ToArray();

                if (hx.SequenceEqual(hxPrime) && !x.SequenceEqual(xPrime))
                {
                    return (x, xPrime);
                }

                x = hx;
                xPrime = hxPrime;
            }
        }

        // 3. Поиск осмысленной коллизии для изображений
        public (byte[], byte[]) FindMeaningfulCollision(string imagePath1, string imagePath2, int n)
        {
            byte[] image1Bytes = GetImageBytes(imagePath1);
            byte[] image2Bytes = GetImageBytes(imagePath2);

            // Хеш первого изображения
            byte[] mainHash = _streebog.GetHash(image1Bytes).Take(n).ToArray();

            // Пытаемся найти коллизию, изменя второе изображение
            for (int i = 0; i < image2Bytes.Length; i++)
            {
                byte[] modifiedBytes = (byte[])image2Bytes.Clone();
                modifiedBytes[i] = (byte)((modifiedBytes[i] + 1) % 256);

                byte[] currentHash = _streebog.GetHash(modifiedBytes).Take(n).ToArray();

                if (mainHash.SequenceEqual(currentHash))
                {
                    return (image1Bytes, modifiedBytes);
                }
            }

            throw new Exception("Коллизия не найдена");
        }

        private string GenerateRandomMessage()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            int length = random.Next(10, 50);

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            new Random().NextBytes(bytes);
            return bytes;
        }

        private byte[] GetImageBytes(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
