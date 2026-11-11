using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace FINALS
{
    public static class Load
    {
        public static List<dynamic> LoadBuilds(List<Конфигурация> buildsData)
        {
            try
            {
                var buildsWithData = new List<dynamic>();

                foreach (var build in buildsData)
                {
                    var автор = DB.Context.Пользователь.FirstOrDefault(k => k.Код_пользователя == build.Код_автора);
                    var класс = DB.Context.Класс.FirstOrDefault(k => k.Код_класса == build.Код_класса);
                    var специализация = DB.Context.Специализация.FirstOrDefault(s => s.Код_специализации == build.Код_специализации);
                    var оружие = DB.Context.Оружие.FirstOrDefault(w => w.Код_оружия == build.Код_оружия);
                    var гаджет1 = DB.Context.Гаджет.FirstOrDefault(g => g.Код_гаджета == build.Код_гаджета_место1);
                    var гаджет2 = DB.Context.Гаджет.FirstOrDefault(g => g.Код_гаджета == build.Код_гаджета_место2);
                    var гаджет3 = DB.Context.Гаджет.FirstOrDefault(g => g.Код_гаджета == build.Код_гаджета_место3);

                    // Загружаем резервные предметы
                    var резервы = DB.Context.Конфигурация_резерв
                        .Where(cr => cr.Код_конфигурации == build.Код_конфигурации)
                        .ToList()
                        .Select(cr => new
                        {
                            cr.Код_резерва,
                            cr.Код_конфигурации,
                            cr.Тип_предмета,
                            cr.Код_предмета,
                            cr.Номер_слота,
                            Предмет = GetItemById(cr.Тип_предмета, cr.Код_предмета)
                        })
                        .ToList();

                    buildsWithData.Add(new
                    {
                        build.Код_конфигурации,
                        build.Название,
                        build.Описание,
                        Фотобилд = LoadImageFromPath(build.Фото),
                        build.Дата_создания,
                        Класс = new { класс?.Наименование },
                        Специализация = new
                        {
                            специализация?.Наименование,
                            Фото = LoadImageFromPath(специализация?.Фото)
                        },
                        Оружие = new
                        {
                            оружие?.Наименование,
                            Фото = LoadImageFromPath(оружие?.Фото)
                        },
                        Гаджет1 = new
                        {
                            гаджет1?.Наименование,
                            Фото = LoadImageFromPath(гаджет1?.Фото)
                        },
                        Гаджет2 = new
                        {
                            гаджет2?.Наименование,
                            Фото = LoadImageFromPath(гаджет2?.Фото)
                        },
                        Гаджет3 = new
                        {
                            гаджет3?.Наименование,
                            Фото = LoadImageFromPath(гаджет3?.Фото)
                        },
                        Автор = new { автор?.Фамилия, автор?.Имя, автор?.Отчество },
                        Конфигурация_резерв = резервы.Select(r => new
                        {
                            r.Тип_предмета,
                            r.Код_предмета,
                            r.Номер_слота,
                            Предмет = new
                            {
                                Наименование = GetItemName(r.Тип_предмета, r.Код_предмета),
                                Фото = LoadImageFromPath(GetItemPhoto(r.Тип_предмета, r.Код_предмета))
                            }
                        }).ToList()
                    });
                }

                return buildsWithData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки конфигураций: {ex.Message}", ex);
            }
        }

        // Вспомогательные методы
        private static object GetItemById(string типПредмета, int кодПредмета)
        {
            switch (типПредмета)
            {
                case "Оружие":
                    return DB.Context.Оружие.FirstOrDefault(w => w.Код_оружия == кодПредмета);
                case "Гаджет":
                    return DB.Context.Гаджет.FirstOrDefault(g => g.Код_гаджета == кодПредмета);
                default:
                    return null;
            }
        }

        private static string GetItemName(string типПредмета, int кодПредмета)
        {
            if (типПредмета == "Оружие")
                return DB.Context.Оружие.FirstOrDefault(o => o.Код_оружия == кодПредмета)?.Наименование;
            else if (типПредмета == "Гаджет")
                return DB.Context.Гаджет.FirstOrDefault(g => g.Код_гаджета == кодПредмета)?.Наименование;
            else
                return "Неизвестный предмет";
        }

        private static string GetItemPhoto(string типПредмета, int кодПредмета)
        {
            if (типПредмета == "Оружие")
                return DB.Context.Оружие.FirstOrDefault(o => o.Код_оружия == кодПредмета)?.Фото;
            else if (типПредмета == "Гаджет")
                return DB.Context.Гаджет.FirstOrDefault(g => g.Код_гаджета == кодПредмета)?.Фото;
            else
                return null;
        }

        private static BitmapImage LoadImageFromPath(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;

                string full = System.IO.Path.GetFullPath(path);
                return new BitmapImage(new Uri(full, UriKind.Absolute));
            }
            catch
            {
                return null;
            }
        }
    }
}