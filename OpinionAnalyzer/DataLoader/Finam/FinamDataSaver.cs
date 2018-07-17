using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpinionAnalyzer.DataLoader.Classes;

namespace OpinionAnalyzer.DataLoader.Finam
{
    /// <summary>
    /// Класс отвечает за сохранение загруженной статьи
    /// Сохранение происходит по {путь}/{год}/{месяц}/{день}/{хеш от названия}.xml
    /// </summary>
    public class FinamDataSaver
    {
        private readonly string _path;

        public FinamDataSaver(string path)
        {
            _path = path;
        }

        public void Save(LoadedArticle article)
        {
            var folderPath = PreparePath(article.PublishDate);
            var fileName = GetFileName(article);
            var fullPath = Path.Combine(folderPath, fileName);

            var s = new XmlSerializer(typeof(LoadedArticle));

            using (var stream = File.OpenWrite(fullPath))
            {
               s.Serialize(stream, article);
            }            

        }

        private string GetFileName(LoadedArticle article)
        {
            byte[] hash;
            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(article.Content));
            }

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb + ".xml";
        }

        private string PreparePath(DateTime dateTime)
        {
            var folder = String.Format(@"{0}\{1}\{2}\{3}", _path, dateTime.Year, dateTime.Month, dateTime.Day);

            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }
    }
}
