using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public partial class PaymentsPage : Page
    {
        private string connectionString = @"Data Source=BLVCK-LOQ\SQLEXPRESS01;Initial Catalog=FoodDeliverySystem;Integrated Security=True;Encrypt=False";

        public PaymentsPage()
        {
            InitializeComponent();
            LoadOrderIDs();
            LoadPayments();

            btnRecord.Click += BtnRecord_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            dgPayments.SelectionChanged += DgPayments_SelectionChanged;
        }

        private void LoadOrderIDs()
        {
            cmbOrderID.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT OrderID FROM Orders", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbOrderID.Items.Add(reader["OrderID"].ToString());
                }
            }
        }

        private void LoadPayments()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Payments", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgPayments.ItemsSource = dt.DefaultView;
            }
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (cmbOrderID.SelectedItem == null || string.IsNullOrWhiteSpace(txtAmount.Text) || string.IsNullOrWhiteSpace(txtMethod.Text))
            {
                MessageBox.Show("Fill all fields.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Payments (OrderID, Amount, Method, Status) VALUES (@OrderID, @Amount, @Method, @Status)", conn);
                cmd.Parameters.AddWithValue("@OrderID", cmbOrderID.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(txtAmount.Text));
                cmd.Parameters.AddWithValue("@Method", txtMethod.Text);
                cmd.Parameters.AddWithValue("@Status", "Paid");
                cmd.ExecuteNonQuery();
            }

            lblStatus.Content = "Paid";
            lblStatus.Foreground = System.Windows.Media.Brushes.Green;
            LoadPayments();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgPayments.SelectedItem == null)
            {
                MessageBox.Show("Select a payment to update.");
                return;
            }

            DataRowView row = dgPayments.SelectedItem as DataRowView;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Payments SET Amount=@Amount, Method=@Method WHERE PaymentID=@PaymentID", conn);
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(txtAmount.Text));
                cmd.Parameters.AddWithValue("@Method", txtMethod.Text);
                cmd.Parameters.AddWithValue("@PaymentID", row["PaymentID"]);
                cmd.ExecuteNonQuery();
            }

            LoadPayments();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgPayments.SelectedItem == null)
            {
                MessageBox.Show("Select a payment to delete.");
                return;
            }

            DataRowView row = dgPayments.SelectedItem as DataRowView;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Payments WHERE PaymentID=@PaymentID", conn);
                cmd.Parameters.AddWithValue("@PaymentID", row["PaymentID"]);
                cmd.ExecuteNonQuery();
            }

            LoadPayments();
        }

        private void DgPayments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPayments.SelectedItem is DataRowView row)
            {
                cmbOrderID.SelectedItem = row["OrderID"].ToString();
                txtAmount.Text = row["Amount"].ToString();
                txtMethod.Text = row["Method"].ToString();
                lblStatus.Content = row["Status"].ToString();
                lblStatus.Foreground = row["Status"].ToString() == "Paid" ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            }
        }
    }
}
