using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public ObservableCollection<Build> Builds { get; set; }
        public Window1()
        {
            InitializeComponent();
            LoadSampleData();
            DataContext = this;
        }
        private void LoadSampleData()
        {
            Builds = new ObservableCollection<Build>
            {
                new Build
                {
                    BuildName = "Штурмовой тяжелый",
                    Description = "Агрессивный билд для прорыва обороны и захвата точек. Идеально подходит для атаки на укрепленные позиции.",
                    Specialization = "Бронежилет",
                    SpecializationImage = "/Images/spec_armor.jpg",
                    Weapon = "M60",
                    WeaponImage = "/Images/weapon_m60.jpg",
                    BuildImage = "/Images/build1.jpg",
                    Gadgets = new ObservableCollection<GadgetItem>
                    {
                        new GadgetItem { Name = "Кувалда", Image = "/Images/gadget_hammer.jpg" },
                        new GadgetItem { Name = "Баллон с огнем", Image = "/Images/gadget_fire.jpg" },
                        new GadgetItem { Name = "Тактический щит", Image = "/Images/gadget_shield.jpg" }
                    },
                    ReserveItems = new ObservableCollection<ReserveItem>
                    {
                        new ReserveItem { Name = "Аптечка", Image = "/Images/reserve_medkit.jpg" },
                        new ReserveItem { Name = "Дымовая граната", Image = "/Images/reserve_smoke.jpg" },
                        new ReserveItem { Name = "Взрывчатка C4", Image = "/Images/reserve_c4.jpg" },
                        new ReserveItem { Name = "Тактический щит", Image = "/Images/reserve_shield.jpg" }
                    }
                },
                new Build
                {
                    BuildName = "Поддержка средний",
                    Description = "Поддерживающий билд для помощи команде. Фокус на исцелении и обеспечении тактических преимуществ.",
                    Specialization = "Лечебный луч",
                    SpecializationImage = "/Images/spec_heal.jpg",
                    Weapon = "AKM",
                    WeaponImage = "/Images/weapon_akm.jpg",
                    BuildImage = "/Images/build2.jpg",
                    Gadgets = new ObservableCollection<GadgetItem>
                    {
                        new GadgetItem { Name = "Дефибриллятор", Image = "/Images/gadget_defib.jpg" },
                        new GadgetItem { Name = "Газовая граната", Image = "/Images/gadget_gas.jpg" },
                        new GadgetItem { Name = "Сканер движения", Image = "/Images/gadget_scanner.jpg" }
                    },
                    ReserveItems = new ObservableCollection<ReserveItem>
                    {
                        new ReserveItem { Name = "Дымовая завеса", Image = "/Images/reserve_smoke2.jpg" },
                        new ReserveItem { Name = "Клейкая граната", Image = "/Images/reserve_glue.jpg" },
                        new ReserveItem { Name = "Аптечка", Image = "/Images/reserve_medkit2.jpg" },
                        new ReserveItem { Name = "Запасные патроны", Image = "/Images/reserve_ammo.jpg" }
                    }
                }
            };

            BuildsListBox.ItemsSource = Builds;
            StatsText.Text = $"Всего билдов: {Builds.Count}";
        }
    }

    public class Build
    {
        public string BuildName { get; set; }
        public string Description { get; set; }
        public string Specialization { get; set; }
        public string SpecializationImage { get; set; }
        public string Weapon { get; set; }
        public string WeaponImage { get; set; }
        public string BuildImage { get; set; }
        public ObservableCollection<GadgetItem> Gadgets { get; set; }
        public ObservableCollection<ReserveItem> ReserveItems { get; set; }
    }

    public class GadgetItem
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class ReserveItem
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
}

