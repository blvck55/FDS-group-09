using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            LoadOrders();

            btnAddOrder.Click += BtnAddOrder_Click;
            btnEditOrder.Click += BtnEditOrder_Click;
            btnDeleteOrder.Click += BtnDeleteOrder_Click;
            lstOrders.SelectionChanged += LstOrders_SelectionChanged;

            // Initially disable edit/delete until selection
            btnEditOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
        }

        private void LoadOrders()
        {
            lstOrders.Items.Clear();
            try
            {
                string query = "SELECT * FROM orders";
                DataTable data = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in data.Rows)
                {
                    lstOrders.Items.Add($"{row["id"]} - {row["customer_name"]} ordered {row["quantity"]} x {row["food_item"]} [{row["status"]}]");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}");
            }
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            string name = txtCustomerName.Text.Trim();
            string item = txtFoodItem.Text.Trim();
            string qtyText = txtQuantity.Text.Trim();
            var selectedStatus = cmbStatus.SelectedItem as ComboBoxItem;

            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(item) ||
                string.IsNullOrEmpty(qtyText) ||
                selectedStatus == null)
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (!int.TryParse(qtyText, out int qty) || qty <= 0)
            {
                MessageBox.Show("Quantity must be a positive integer.");
                return;
            }

            string status = selectedStatus.Content.ToString();

            // Note: Parameterized queries should be used here instead for safety.
            string query = $"INSERT INTO orders (customer_name, food_item, quantity, status) " +
                           $"VALUES ('{name.Replace("'", "''")}', '{item.Replace("'", "''")}', {qty}, '{status}')";

            try
            {
                DatabaseHelper.ExecuteNonQuery(query);
                LoadOrders();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding order: {ex.Message}");
            }
        }

        private void BtnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (lstOrders.SelectedItem == null)
            {
                MessageBox.Show("Select an order to edit.");
                return;
            }

            string selected = lstOrders.SelectedItem.ToString();
            string id = selected.Split('-')[0].Trim();

            string name = txtCustomerName.Text.Trim();
            string item = txtFoodItem.Text.Trim();
            string qtyText = txtQuantity.Text.Trim();
            var selectedStatus = cmbStatus.SelectedItem as ComboBoxItem;

            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(item) ||
                string.IsNullOrEmpty(qtyText) ||
                selectedStatus == null)
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (!int.TryParse(qtyText, out int qty) || qty <= 0)
            {
                MessageBox.Show("Quantity must be a positive integer.");
                return;
            }

            string status = selectedStatus.Content.ToString();

            string query = $"UPDATE orders SET customer_name='{name.Replace("'", "''")}', " +
                           $"food_item='{item.Replace("'", "''")}', quantity={qty}, status='{status}' WHERE id={id}";

            try
            {
                DatabaseHelper.ExecuteNonQuery(query);
                LoadOrders();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order: {ex.Message}");
            }
        }

        private void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (lstOrders.SelectedItem == null)
            {
                MessageBox.Show("Select an order to delete.");
                return;
            }

            string selected = lstOrders.SelectedItem.ToString();
            string id = selected.Split('-')[0].Trim();

            string query = $"DELETE FROM orders WHERE id={id}";

            try
            {
                DatabaseHelper.ExecuteNonQuery(query);
                LoadOrders();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting order: {ex.Message}");
            }
        }

        private void LstOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstOrders.SelectedItem != null)
            {
                btnEditOrder.IsEnabled = true;
                btnDeleteOrder.IsEnabled = true;

                string item = lstOrders.SelectedItem.ToString();
                try
                {
                    string[] parts = item.Split('-');
                    string customerPart = parts[1].Trim(); // e.g., "John Doe ordered 2 x Burger [Pending]"
                    string[] orderDetails = customerPart.Split(new string[] { " ordered " }, StringSplitOptions.None);
                    string customerName = orderDetails[0].Trim();
                    string[] itemParts = orderDetails[1].Split('x');
                    string qty = itemParts[0].Trim();
                    string food = itemParts[1].Split('[')[0].Trim();
                    string status = orderDetails[1].Split('[')[1].Replace("]", "").Trim();

                    txtCustomerName.Text = customerName;
                    txtQuantity.Text = qty;
                    txtFoodItem.Text = food;
                    cmbStatus.SelectedIndex = status == "Pending" ? 0 :
                                              status == "Preparing" ? 1 : 2;
                }
                catch
                {
                    ClearInputs();
                }
            }
            else
            {
                btnEditOrder.IsEnabled = false;
                btnDeleteOrder.IsEnabled = false;
                ClearInputs();
            }
        }

        private void ClearInputs()
        {
            txtCustomerName.Clear();
            txtFoodItem.Clear();
            txtQuantity.Clear();
            cmbStatus.SelectedIndex = -1;

            btnEditOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
        }
    }
}
