using Population;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Population
{
    public partial class MainWindow : Window
    {
        private readonly List<Allampolgar> lakossag;
        const int feladatokSzama = 5;

        public MainWindow()
        {
            InitializeComponent();
            lakossag = new List<Allampolgar>();

            using var sr = new StreamReader(@"..\..\..\SRC\population.txt");
            _ = sr.ReadLine();

            while (!sr.EndOfStream)
            {
                lakossag.Add(new Allampolgar(sr.ReadLine()));
            }

            for (int i = 1; i <= feladatokSzama; i++)
            {
                feladatComboBox.Items.Add($"{i}.");
            }

            DataContext = this;
            MegoldasTeljes.ItemsSource = lakossag;
        }

        private void FeladatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MegoldasMondatos.Content = null;
            MegoldasLista.ItemsSource = null;
            MegoldasTeljes.ItemsSource = null;

            var methodName = $"Feladat{feladatComboBox.SelectedIndex + 1}";
            var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(this, null);
        }

        private void Feladat1()
        {
            MegoldasLista.ItemsSource = lakossag.GroupBy(l => l.IskolaiVegzettseg).ToDictionary(l => l.Key, l => l.Average(x => x.HaviBruttoJovedelem * 12)).Select(l => $"{l.Value} £");
        }

        private void Feladat2()
        {
            MegoldasTeljes.ItemsSource = lakossag.Where(l => l.IskolaiVegzettseg == "Bachelor's");
        }

        private void Feladat3()
        {
            MegoldasMondatos.Content = $"Az egészségbiztosítással rendelkezők között a minimális éves jövedelem: {lakossag.Where(l => l.Egeszsegbiztositas).Min(l => l.TeljesJovedelem())} £.";
        }

        private void Feladat4()
        {
            MegoldasMondatos.Content = $"{lakossag.Count(l => l.Nem == "egyéb")} ember nem nyilatkozott a neméről.";
        }

        private void Feladat5()
        {
            MegoldasLista.ItemsSource = lakossag.GroupBy(l => l.Megye).ToDictionary(l => l.Key, l => l.Count(x => x.Szavazokepesseg())).OrderByDescending(l => l.Value).Select(l => $"{l.Key}, {l.Value}");
        }
    }
}
