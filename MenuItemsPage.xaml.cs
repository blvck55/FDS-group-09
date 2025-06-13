using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public static class DatabaseHelper
    {
        private static readonly string dbFile = "fooddelivery.db";
        private static readonly string connectionString = $"Data Source={dbFile};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS menu_items (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        price REAL NOT NULL,
                        available INTEGER NOT NULL
                    );";

                using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable ExecuteQuery(string query, params SQLiteParameter[] parameters)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }

    public partial class MenuItemsPage : Page
    {
        public MenuItemsPage()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            LoadMenuItems();

            btnAddMenuItem.Click += BtnAddMenuItem_Click;
            btnEditMenuItem.Click += BtnEditMenuItem_Click;
            btnDeleteMenuItem.Click += BtnDeleteMenuItem_Click;
            lstMenuItems.SelectionChanged += LstMenuItems_SelectionChanged;
        }

        private void LoadMenuItems()
        {
            lstMenuItems.Items.Clear();
            string query = "SELECT * FROM menu_items";
            DataTable data = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in data.Rows)
            {
                string availability = Convert.ToBoolean(row["available"]) ? "[Available]" : "[Unavailable]";
                lstMenuItems.Items.Add($"{row["id"]} - {row["name"]} - ${row["price"]} {availability}");
            }
        }

        private void BtnAddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtItemName.Text) && !string.IsNullOrWhiteSpace(txtItemPrice.Text))
            {
                string name = txtItemName.Text.Trim();
                if (!decimal.TryParse(txtItemPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Please enter a valid price.");
                    return;
                }
                bool available = chkAvailable.IsChecked == true;

                string query = "INSERT INTO menu_items (name, price, available) VALUES (@name, @price, @available)";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@name", name),
                    new SQLiteParameter("@price", price),
                    new SQLiteParameter("@available", available ? 1 : 0)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadMenuItems();
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Please enter both item name and price.");
            }
        }

        private void BtnEditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstMenuItems.SelectedItem != null)
            {
                string selected = lstMenuItems.SelectedItem.ToString();
                string id = selected.Split('-')[0].Trim();

                string name = txtItemName.Text.Trim();
                if (!decimal.TryParse(txtItemPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Please enter a valid price.");
                    return;
                }
                bool available = chkAvailable.IsChecked == true;

                string query = "UPDATE menu_items SET name = @name, price = @price, available = @available WHERE id = @id";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@name", name),
                    new SQLiteParameter("@price", price),
                    new SQLiteParameter("@available", available ? 1 : 0),
                    new SQLiteParameter("@id", id)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadMenuItems();
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Please select an item to edit.");
            }
        }

        private void BtnDeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstMenuItems.SelectedItem != null)
            {
                string selected = lstMenuItems.SelectedItem.ToString();
                string id = selected.Split('-')[0].Trim();

                string query = "DELETE FROM menu_items WHERE id = @id";
                var parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@id", id)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadMenuItems();
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }

        private void LstMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMenuItems.SelectedItem != null)
            {
                string item = lstMenuItems.SelectedItem.ToString();
                try
                {
                    string[] parts = item.Split('-');
                    string name = parts[1].Trim();
                    string[] priceStatus = parts[2].Trim().Split(' ');
                    string price = priceStatus[0].Replace("$", "").Trim();
                    bool isAvailable = parts[2].Contains("Available");

                    txtItemName.Text = name;
                    txtItemPrice.Text = price;
                    chkAvailable.IsChecked = isAvailable;
                }
                catch
                {
                    ClearInputFields();
                }
            }
        }

        private void ClearInputFields()
        {
            txtItemName.Clear();
            txtItemPrice.Clear();
            chkAvailable.IsChecked = false;
        }
    }
}
