using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для Создание_Конфигурации.xaml
    /// </summary>
    public partial class Создание_Конфигурации : Window
    {
        string targetPath;
        private string selectedImagePath = null;
        List<int> arrGadgets = new List<int>();
        int userId;
        public Создание_Конфигурации(int userId)
        {
            InitializeComponent();

            classCB.ItemsSource = DB.Context.Класс.ToList();
            classCB.DisplayMemberPath = "Наименование";
            classCB.SelectedValuePath = "Код_класса";
            classCB.SelectedIndex = -1;
            this.userId = userId;
        }
        public void loadAllComboBox(int idClass)
        {
            // 2. Оружие
            weaponCB.ItemsSource = DB.Context.Оружие.Where(z => z.Код_класса == idClass).ToList();
            weaponCB.DisplayMemberPath = "Наименование";
            weaponCB.SelectedValuePath = "Код_оружия";
            weaponCB.SelectedIndex = -1;
            weaponCB.Visibility = Visibility.Visible;

            // 3. Специализации
            specializationCB.ItemsSource = DB.Context.Специализация.Where(z => z.Код_класса == idClass).ToList();
            specializationCB.DisplayMemberPath = "Наименование";
            specializationCB.SelectedValuePath = "Код_специализации";
            specializationCB.SelectedIndex = -1;
            specializationCB.Visibility = Visibility.Visible;

            // 4. Гаджеты (три слота)
            //var zzz = DB.Context.Гаджет.Where(x => x.Код_класса );
            var gadgetsForClass = DB.Context.Гаджет_к_классу
            .Where(gk => gk.Код_класса == idClass)
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
            // Резерв может содержать любое оружие или гаджет — значит тоже всё
            var rezervItems = new List<object>();
            rezervItems.AddRange(DB.Context.Гаджет_к_классу
            .Where(gk => gk.Код_класса == idClass)
            .Select(gk => gk.Гаджет)
            .ToList());
            rezervItems.AddRange(DB.Context.Оружие.Where(z => z.Код_класса == idClass).ToList());

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
        private void Button_Click(object sender, RoutedEventArgs e) //загрузка фото
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

                MessageBox.Show("Фото загружено.",
                                "ОК", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
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

        private void specializationCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = specializationCB.SelectedItem as Специализация;
            if (item == null) return;
            LoadImage(specializationImg, item.Фото);
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

        private void Button_Click_1(object sender, RoutedEventArgs e) //создание конфигурации
        {
            if (selectedImagePath == null || txtZagolovok == null || txtOpisanie == null)
            {
                MessageBox.Show("Сначала выберите фото, заполните поля заголовка и описания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (classCB.SelectedIndex <= -1 || weaponCB.SelectedIndex <= -1 || gadget1CB.SelectedIndex <= -1 || gadget2CB.SelectedIndex <= -1 || gadget3CB.SelectedIndex <= -1 || rezerv1CB.SelectedIndex <= -1 || rezerv2CB.SelectedIndex <= -1 || rezerv3CB.SelectedIndex <= -1 || rezerv4CB.SelectedIndex <= -1 || specializationCB.SelectedIndex <= -1)
            {
                MessageBox.Show("Сначала выберите все предметы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string configDir = System.IO.Path.Combine("img", "configs");
                Directory.CreateDirectory(configDir);

                string fileName = $"config_{userId}_{DateTime.Today.Ticks}{System.IO.Path.GetExtension(selectedImagePath)}";
                targetPath = System.IO.Path.Combine(configDir, fileName);

                File.Copy(selectedImagePath, targetPath, true);

                // здесь записываешь путь в базу/конфиг
                // CurrentConfig.Фото = targetPath;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

            Конфигурация конфигурация = new Конфигурация
            {
                Название = txtZagolovok.Text,
                Описание = txtOpisanie.Text,
                Код_автора = userId,
                Код_класса = (int)classCB.SelectedValue,
                Код_специализации = (int)specializationCB.SelectedValue,
                Код_оружия = (int)weaponCB.SelectedValue,
                Код_гаджета_место1 = (int)gadget1CB.SelectedValue,
                Код_гаджета_место2 = (int)gadget2CB.SelectedValue,
                Код_гаджета_место3 = (int)gadget3CB.SelectedValue,
                Фото = targetPath,
                Дата_создания = DateTime.Now
            };


            DB.Context.Конфигурация.Add(конфигурация);
            DB.Context.SaveChanges();


            var rezervComboBoxes = new[] { rezerv1CB, rezerv2CB, rezerv3CB, rezerv4CB };

            for (int i = 0; i < rezervComboBoxes.Length; i++)
            {
                if (rezervComboBoxes[i].SelectedValue != null)
                {
                    int кодПредмета = GetItemId(rezervComboBoxes[i].SelectedValue);
                    string типПредмета = GetItemType(rezervComboBoxes[i].SelectedValue);
                    Конфигурация_резерв резерв = new Конфигурация_резерв
                    {
                        Код_конфигурации = конфигурация.Код_конфигурации,
                        Тип_предмета = типПредмета,
                        Код_предмета = кодПредмета,
                        Номер_слота = i + 1
                    };

                    DB.Context.Конфигурация_резерв.Add(резерв);
                }
            }

            DB.Context.SaveChanges();
            MessageBox.Show("Конфигурация сохранена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private void Button_Click_2(object sender, RoutedEventArgs e) //назад
        {
            new Мои_Конфигурации(userId).Show();
            this.Close();
        }
    }
}