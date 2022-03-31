using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BolnicaVezba
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BolnicaBazaDataContext db = new BolnicaBazaDataContext();
        public MainWindow()
        {
            InitializeComponent();
            puniGrid();
            puniOdeljenja();
            puniPrioritet();

        }
        void puniPrioritet()
        {
            var prioriteti = from p in db.Pacijents
                             select p;

            cmbPrioritet.ItemsSource = prioriteti;
        }
        void puniGrid()
        {
            var pacijenti = from p in db.Pacijents
                            select p;

            dataGridBolnica.ItemsSource = pacijenti;
        }

        void puniOdeljenja()
        {
            var odeljenja = from o in db.Odeljenjes
                            select o;

            cmbPretragaPoOdeljenju.ItemsSource = odeljenja;
        }
        private void cmbPretragaPoOdeljenju_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pacijenti = from p in db.Pacijents
                            where p.OdeljenjeID == ((Odeljenje)cmbPretragaPoOdeljenju.SelectedValue).OdeljenjeID
                            select p;

            lbListaBolesnika.Visibility = Visibility.Visible;
            lbPacijentiPrioritet.Visibility = Visibility.Hidden;

            lbListaBolesnika.ItemsSource = pacijenti;
        }

        private void dataGridBolnica_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            naplata();
        }
        void naplata()
        {
            tbBrDana.Text = (((Pacijent)dataGridBolnica.SelectedValue).BrDana).ToString();
        }

        private void btnIzracunaj_Click(object sender, RoutedEventArgs e)
        {
            int participacija = int.Parse(tbParticipacija.Text);
            tbNaplata.Text = (participacija * (int.Parse(tbBrDana.Text))).ToString();

            var dan = (from d in db.Pacijents
                       where d.IDPacijent == ((Pacijent)dataGridBolnica.SelectedValue).IDPacijent
                       select d).FirstOrDefault();
             
            try
            {
                db.SubmitChanges();
                MessageBox.Show("Uspesno ste uplatili" + tbNaplata.Text + "dinara", "Poruka", MessageBoxButton.OK);
                tbBrDana.Clear();
                tbParticipacija.Clear();
                tbNaplata.Clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Neuspeh!" + ex);
            }

        }

     
        private void btnOtpusti_Click(object sender, RoutedEventArgs e)
        {
            Pacijent pacijent = (from p in db.Pacijents
                                 where p.IDPacijent == int.Parse(tbSifraPacijenta.Text)
                                 select p).First();

            db.Pacijents.DeleteOnSubmit(pacijent);

            try
            {
                db.SubmitChanges();
                MessageBox.Show("Uspesno otpisan pacijent");
                puniGrid();
                tbSifraPacijenta.Clear();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Neuspeh" + ex);
            }
        }

        private void btnMaxDana_Click(object sender, RoutedEventArgs e)
        {
            var maxDana = (from p in db.Pacijents
                           where p.OdeljenjeID == ((Odeljenje)cmbPretragaPoOdeljenju.SelectedValue).OdeljenjeID
                           select p).Max(p => p.BrDana);

            var selektujPacijenta = from s in db.Pacijents
                                    where s.BrDana == maxDana
                                    select s;

            lbListaBolesnika.ItemsSource = selektujPacijenta;
           
        }

        private void cmbPrioritet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var prioritet = (from p in db.Pacijents
                             join d in db.Odeljenjes
                             on p.OdeljenjeID equals d.OdeljenjeID
                             where p.Prioritet == ((Pacijent)cmbPrioritet.SelectedValue).Prioritet
                             select new { p.Ime, p.Prezime, d.Naziv }
                             ).ToList();

            lbListaBolesnika.Visibility = Visibility.Hidden;
            lbPacijentiPrioritet.Visibility = Visibility.Visible;



            lbPacijentiPrioritet.ItemsSource = prioritet;

        }

        private void UnosNovog_Click(object sender, RoutedEventArgs e)
        {
            UnosNovog nov = new UnosNovog();
            nov.ShowDialog();
            puniGrid();
        }
    }
        
}
