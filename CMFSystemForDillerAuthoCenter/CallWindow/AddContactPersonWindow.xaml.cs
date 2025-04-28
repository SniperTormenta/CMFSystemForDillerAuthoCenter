using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("ФИО контактного лица обязательно для заполнения.");
                return;
            }

            // Проверка телефона
            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                string phone = PhoneTextBox.Text.Replace("+", "").Trim();
                if (!Regex.IsMatch(phone, @"^\d{11}$"))
                {
                    MessageBox.Show("Телефон должен содержать ровно 11 цифр (например, +79991234567 или 79991234567).");
                    return;
                }
            }

            // Проверка Email
            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) &&
                !Regex.IsMatch(EmailTextBox.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                MessageBox.Show("Email должен быть в формате example@domain.com");
                return;
            }

            // Проверка длины полей
            if (FullNameTextBox.Text.Length > 100 ||
                PositionTextBox.Text.Length > 50 ||
                NotesTextBox.Text.Length > 500)
            {
                MessageBox.Show("Превышена максимальная длина одного из полей:\n" +
                                "ФИО: до 100 символов\n" +
                                "Должность: до 50 символов\n" +
                                "Заметки: до 500 символов");
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