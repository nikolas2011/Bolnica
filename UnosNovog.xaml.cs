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
using System.Windows.Shapes;

namespace BolnicaVezba
{
    /// <summary>
    /// Interaction logic for UnosNovog.xaml
    /// </summary>
    public partial class UnosNovog : Window
    {
        BolnicaBazaDataContext db = new BolnicaBazaDataContext();

        public UnosNovog()
        {
            InitializeComponent();
            puniOdeljenje();
           
        }
        void puniOdeljenje()
        {
            var odeljenje = from o in db.Odeljenjes
                            select o;

            cmbOdeljenje.ItemsSource = odeljenje;

        }

       

        private void cmbOdeljenje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sobe = from s in db.Sobas
                       where s.OdeljenjeID == ((Odeljenje)cmbOdeljenje.SelectedValue).OdeljenjeID
                       select s;
            cmbSoba.ItemsSource = sobe;

        }

        private void cmbSoba_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbSprat.Text = (((Soba)cmbSoba.SelectedValue).Sprat).ToString();
        }

        private void btnOdustani_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnUnesi_Click(object sender, RoutedEventArgs e)
        {
            Pacijent pacijent = new Pacijent();

            pacijent.IDPacijent = int.Parse(tbSifra.Text);
            pacijent.Ime = tbIme.Text;
            pacijent.Prezime = tbPrezime.Text;
            pacijent.Prioritet = tbPrioritet.Text;
            pacijent.OdeljenjeID = ((Odeljenje)cmbOdeljenje.SelectedValue).OdeljenjeID;
            pacijent.SobaID = ((Soba)cmbSoba.SelectedValue).SobaID;

            db.Pacijents.InsertOnSubmit(pacijent);

            try
            {
                db.SubmitChanges();
                MessageBox.Show("Uspesno ste dodali pacijenta");


            }
            catch(Exception ex)
            {
                MessageBox.Show("Greska" + ex);
            }

        }
    }
}
