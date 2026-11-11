using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FINALS
{
    /// <summary>
    /// Логика взаимодействия для Редактирование_Конфигурации.xaml
    /// </summary>
    public partial class Редактирование_Конфигурации : Window
    {
        Конфигурация build;
        private string selectedImagePath = null;
        string targetPath;

        public Редактирование_Конфигурации(Конфигурация build)
        {
            InitializeComponent();
            this.build = build;
            loadAllComboBox();
            loadBuild();


        }
        public void loadAllComboBox(int? idClass = null)
        {
            // Используем переданный idClass или берем из build
            int classId = (int)(idClass ?? build.Код_класса);

            // 1. Класс
            classCB.ItemsSource = DB.Context.Класс.ToList();
            classCB.DisplayMemberPath = "Наименование";
            classCB.SelectedValuePath = "Код_класса";
            // НЕ сбрасываем SelectedIndex здесь

            // 2. Оружие
            weaponCB.ItemsSource = DB.Context.Оружие.Where(z => z.Код_класса == classId).ToList();
            weaponCB.DisplayMemberPath = "Наименование";
            weaponCB.SelectedValuePath = "Код_оружия";
            weaponCB.SelectedIndex = -1;
            weaponCB.Visibility = Visibility.Visible;

            // 3. Специализации
            specializationCB.ItemsSource = DB.Context.Специализация.Where(z => z.Код_класса == classId).ToList();
            specializationCB.DisplayMemberPath = "Наименование";
            specializationCB.SelectedValuePath = "Код_специализации";
            specializationCB.SelectedIndex = -1;
            specializationCB.Visibility = Visibility.Visible;

            // 4. Гаджеты (три слота)
            var gadgetsForClass = DB.Context.Гаджет_к_классу
                .Where(gk => gk.Код_класса == classId)
                .Select(gk => gk.Гаджет)
                .ToList();

            gadget1CB.ItemsSource = gadgetsForClass;
            gadget1CB.DisplayMemberPath = "Наименование";
            gadget1CB.SelectedValuePath = "Код_гаджета";
            gadget1CB.Visibility = Visibility.Visible;

            gadget2CB.ItemsSource = gadgetsForClass;
            gadget2CB.DisplayMemberPath = "Наименование";
            gadget2CB.SelectedValuePath = "Код_гаджета";
            gadget2CB.Visibility = Visibility.Visible;

            gadget3CB.ItemsSource = gadgetsForClass;
            gadget3CB.DisplayMemberPath = "Наименование";
            gadget3CB.SelectedValuePath = "Код_гаджета";
            gadget3CB.Visibility = Visibility.Visible;

            gadget1CB.SelectedIndex = -1;
            gadget2CB.SelectedIndex = -1;
            gadget3CB.SelectedIndex = -1;

            // 5. Резерв (четыре слота)
            var rezervItems = new List<object>();
            rezervItems.AddRange(DB.Context.Гаджет_к_классу
                .Where(gk => gk.Код_класса == classId)
                .Select(gk => gk.Гаджет)
                .ToList());
            rezervItems.AddRange(DB.Context.Оружие.Where(z => z.Код_класса == classId).ToList());

            rezerv1CB.ItemsSource = rezervItems;
            rezerv1CB.DisplayMemberPath = "Наименование";
            rezerv1CB.SelectedIndex = -1;
            rezerv1CB.Visibility = Visibility.Visible;

            rezerv2CB.ItemsSource = rezervItems;
            rezerv2CB.DisplayMemberPath = "Наименование";
            rezerv2CB.SelectedIndex = -1;
            rezerv2CB.Visibility = Visibility.Visible;

            rezerv3CB.ItemsSource = rezervItems;
            rezerv3CB.DisplayMemberPath = "Наименование";
            rezerv3CB.SelectedIndex = -1;
            rezerv3CB.Visibility = Visibility.Visible;

            rezerv4CB.ItemsSource = rezervItems;
            rezerv4CB.DisplayMemberPath = "Наименование";
            rezerv4CB.SelectedIndex = -1;
            rezerv4CB.Visibility = Visibility.Visible;
        }

        public void loadBuild() 
        {
            if (this.build != null)
            {
                txtZagolovok.Text = build.Название;
                txtOpisanie.Text = build.Описание;
                classCB.SelectedValue = build.Код_класса;
                specializationCB.SelectedValue = build.Код_специализации;
                weaponCB.SelectedValue = build.Код_оружия;
                gadget1CB.SelectedValue = build.Код_гаджета_место1;
                gadget2CB.SelectedValue = build.Код_гаджета_место2;
                gadget3CB.SelectedValue = build.Код_гаджета_место3;

                // Для фото, если targetPath - это переменная для отображения
                if (!string.IsNullOrEmpty(build.Фото))
                {
                    string full = System.IO.Path.GetFullPath(build.Фото);
                    imgBuild.Source = new BitmapImage(new Uri(full, UriKind.Absolute)); ;
                    // Дополнительный код для загрузки изображения в PictureBox/Image
                }

                var rezervComboBoxes = new[] { rezerv1CB, rezerv2CB, rezerv3CB, rezerv4CB };

                var reserveItems = DB.Context.Конфигурация_резерв
                .Where(x => x.Код_конфигурации == build.Код_конфигурации)
                .OrderBy(x => x.Номер_слота)
                .ToList();

                foreach (var reserveItem in reserveItems)
                {
                    int slotIndex = reserveItem.Номер_слота - 1; // -1 потому что слоты начинаются с 1

                    if (slotIndex >= 0 && slotIndex < rezervComboBoxes.Length)
                    {
                        // Устанавливаем выбранное значение в комбобокс
                        SetComboBoxValue(rezervComboBoxes[slotIndex], reserveItem.Тип_предмета, reserveItem.Код_предмета);
                    }
                }
                // Дата создания обычно только для отображения, если нужно:
                // dateLabel.Text = build.Дата_создания.ToString();
            }
        }
        private void SetComboBoxValue(ComboBox comboBox, string itemType, int itemId)
        {
            // В зависимости от типа предмета ищем соответствующий элемент
            switch (itemType)
            {
                case "Оружие":
                    var weaponItem = comboBox.Items.Cast<dynamic>()
                        .FirstOrDefault(item => GetItemId(item) == itemId && GetItemType(item) == itemType);
                    if (weaponItem != null)
                        comboBox.SelectedItem = weaponItem;
                    break;
                case "Гаджет":
                    var gadgetItem = comboBox.Items.Cast<dynamic>()
                        .FirstOrDefault(item => GetItemId(item) == itemId && GetItemType(item) == itemType);
                    if (gadgetItem != null)
                        comboBox.SelectedItem = gadgetItem;
                    break;
                    // Добавьте другие типы предметов по необходимости
            }
        }
        private int GetItemId(object selectedValue)
        {
            switch (selectedValue)
            {
                case Оружие оружие:
                    return оружие.Код_оружия;
                case Гаджет гаджет:
                    return гаджет.Код_гаджета;
                default:
                    throw new InvalidOperationException($"Неизвестный тип предмета: {selectedValue.GetType()}");
            }
        }
        private string GetItemType(object selectedValue)
        {
            // Определяем тип объекта и получаем его ID
            switch (selectedValue)
            {
                case Оружие оружие:
                    return "Оружие";
                case Гаджет гаджет:
                    return "Гаджет";
                default:
                    throw new InvalidOperationException($"Неизвестный тип предмета: {selectedValue.GetType()}");
            }
        }
        private void LoadImage(Image img, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    img.Source = null;
                    return;
                }

                string full = System.IO.Path.GetFullPath(path);
                img.Source = new BitmapImage(new Uri(full, UriKind.Absolute));
            }
            catch
            {
                img.Source = null;
            }
        }
        private void weaponCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var item = weaponCB.SelectedItem as Оружие;
            if (item == null) return;
            LoadImage(weaponImg, item.Фото);
        }

        private void Gadget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb == null || cb.SelectedItem == null) return;

            if (IsItemAlreadySelected(cb.SelectedItem, cb))
            {
                MessageBox.Show("Этот предмет уже выбран в другом слоте!");
                cb.SelectedIndex = -1;
                GetGadgetImage(cb).Source = null;
                return;
            }

            var gadget = cb.SelectedItem as Гаджет;
            LoadImage(GetGadgetImage(cb), gadget?.Фото);
        }

        private void Rezerv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb == null) return;

            if (cb.SelectedItem != null && IsItemAlreadySelected(cb.SelectedItem, cb))
            {
                MessageBox.Show("Этот предмет уже выбран в другом слоте!");
                cb.SelectedIndex = -1;
                GetRezervImage(cb).Source = null;
                return;
            }

            string path = null;
            if (cb.SelectedItem is Гаджет g) path = g.Фото;
            else if (cb.SelectedItem is Оружие w) path = w.Фото;
            LoadImage(GetRezervImage(cb), path);
        }

        private void specializationCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = specializationCB.SelectedItem as Специализация;
            if (item == null) return;
            LoadImage(specializationImg, item.Фото);
        }

        private bool IsItemAlreadySelected(object selectedItem, ComboBox currentComboBox)
        {
            var allComboBoxes = new[] { gadget1CB, gadget2CB, gadget3CB, rezerv1CB, rezerv2CB, rezerv3CB, rezerv4CB };

            int currentItemId = GetItemId(selectedItem);

            var otherSelectedItemsIds = allComboBoxes
                .Where(c => c != currentComboBox && c.SelectedItem != null)
                .Select(c => GetItemId(c.SelectedItem));

            return otherSelectedItemsIds.Contains(currentItemId);
        }

        private Image GetGadgetImage(ComboBox cb)
        {
            if (cb == gadget1CB) return gadget1Img;
            if (cb == gadget2CB) return gadget2Img;
            if (cb == gadget3CB) return gadget3Img;
            return null;
        }

        private Image GetRezervImage(ComboBox cb)
        {
            if (cb == rezerv1CB) return rezerv1Img;
            if (cb == rezerv2CB) return rezerv2Img;
            if (cb == rezerv3CB) return rezerv3Img;
            if (cb == rezerv4CB) return rezerv4Img;
            return null;
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp",
                Title = "Выберите фото для конфигурации"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                selectedImagePath = dialog.FileName;   // сохраняем путь, но НЕ копируем

                imgBuild.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.Absolute));

                MessageBox.Show("Фото загружено. Теперь нажмите СОХРАНИТЬ.",
                                "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //редактирование 
        {
            if (imgBuild.Source == null || string.IsNullOrEmpty(txtZagolovok.Text) || string.IsNullOrEmpty(txtOpisanie.Text))
            {
                MessageBox.Show("Сначала выберите фото, заполните поля заголовка и описания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (classCB.SelectedIndex <= -1 || weaponCB.SelectedIndex <= -1 || gadget1CB.SelectedIndex <= -1 ||
                gadget2CB.SelectedIndex <= -1 || gadget3CB.SelectedIndex <= -1 || rezerv1CB.SelectedIndex <= -1 ||
                rezerv2CB.SelectedIndex <= -1 || rezerv3CB.SelectedIndex <= -1 || rezerv4CB.SelectedIndex <= -1 ||
                specializationCB.SelectedIndex <= -1)
            {
                MessageBox.Show("Сначала выберите все предметы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Копирование фото только если выбрано новое изображение
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    string configDir = System.IO.Path.Combine("img", "configs");
                    Directory.CreateDirectory(configDir);

                    // Создаем безопасное имя файла (убираем недопустимые символы)
                    string fileName = $"config_{build.Код_автора}_{DateTime.Today}{System.IO.Path.GetExtension(selectedImagePath)}";


                    // Заменяем недопустимые символы в имени файла
                    foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(invalidChar, '_');
                    }

                    targetPath = System.IO.Path.Combine(configDir, fileName);
                    File.Copy(selectedImagePath, targetPath, true);
                }
                else
                {
                    // Если фото не менялось, оставляем старый путь
                    targetPath = build.Фото;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при копировании фото: " + ex.Message);
                return;
            }

            // Обновление основной конфигурации
            build.Название = txtZagolovok.Text;
            build.Описание = txtOpisanie.Text;
            build.Код_класса = (int)classCB.SelectedValue;
            build.Код_специализации = (int)specializationCB.SelectedValue;
            build.Код_оружия = (int)weaponCB.SelectedValue;
            build.Код_гаджета_место1 = (int)gadget1CB.SelectedValue;
            build.Код_гаджета_место2 = (int)gadget2CB.SelectedValue;
            build.Код_гаджета_место3 = (int)gadget3CB.SelectedValue;
            build.Фото = targetPath;

            // Обновление резервных слотов
            var rezervComboBoxes = new[] { rezerv1CB, rezerv2CB, rezerv3CB, rezerv4CB };
            var existingReserves = DB.Context.Конфигурация_резерв
                .Where(z => z.Код_конфигурации == build.Код_конфигурации)
                .ToList();

            for (int i = 0; i < rezervComboBoxes.Length; i++)
            {
                int slotNumber = i + 1;
                var existingReserve = existingReserves.FirstOrDefault(z => z.Номер_слота == slotNumber);

                if (existingReserve != null)
                {
                    if (rezervComboBoxes[i].SelectedValue != null)
                    {
                        int кодПредмета = GetItemId(rezervComboBoxes[i].SelectedValue);
                        string типПредмета = GetItemType(rezervComboBoxes[i].SelectedValue);

                        existingReserve.Тип_предмета = типПредмета;
                        existingReserve.Код_предмета = кодПредмета;
                    }
                    else
                    {
                        // Если комбобокс пустой - удаляем запись
                        DB.Context.Конфигурация_резерв.Remove(existingReserve);
                    }
                }
            }

            try
            {
                DB.Context.SaveChanges();
                MessageBox.Show("Конфигурация отредактирована.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                selectedImagePath = "";
                targetPath = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void classCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (classCB.SelectedValue != null)
            {
                weaponCB.Visibility = Visibility.Visible;
                gadget1CB.Visibility = Visibility.Visible;
                gadget2CB.Visibility = Visibility.Visible;
                gadget3CB.Visibility = Visibility.Visible;
                rezerv1CB.Visibility = Visibility.Visible;
                rezerv2CB.Visibility = Visibility.Visible;
                rezerv3CB.Visibility = Visibility.Visible;
                rezerv4CB.Visibility = Visibility.Visible;
                specializationCB.Visibility = Visibility.Visible;
                loadAllComboBox((int)classCB.SelectedValue);
                weaponImg.Source = null;
                gadget1Img.Source = null;
                gadget2Img.Source = null;
                gadget3Img.Source = null;
                rezerv1Img.Source = null;
                rezerv2Img.Source = null;
                rezerv3Img.Source = null;
                rezerv4Img.Source = null;
                specializationImg.Source = null;

            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}