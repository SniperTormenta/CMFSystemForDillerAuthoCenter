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

namespace CMFSystemForDillerAuthoCenter.CallWindow
{
    public partial class AddContactPersonWindow : Window
    {
        private ContactPerson _contactPerson;

        public AddContactPersonWindow(ContactPerson contactPerson = null)
        {
            InitializeComponent();
            _contactPerson = contactPerson ?? new ContactPerson();

            if (contactPerson != null)
            {
                FullNameTextBox.Text = contactPerson.FullName;
                PhoneTextBox.Text = contactPerson.Phone;
                EmailTextBox.Text = contactPerson.Email;
                PositionTextBox.Text = contactPerson.Position;
                NotesTextBox.Text = contactPerson.Notes;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("Укажите ФИО контактного лица.");
                return;
            }

            _contactPerson.FullName = FullNameTextBox.Text;
            _contactPerson.Phone = PhoneTextBox.Text;
            _contactPerson.Email = EmailTextBox.Text;
            _contactPerson.Position = PositionTextBox.Text;
            _contactPerson.Notes = NotesTextBox.Text;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public ContactPerson GetContactPerson()
        {
            return _contactPerson;
        }
    }
}