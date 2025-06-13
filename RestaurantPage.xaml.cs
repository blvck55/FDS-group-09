using System;
using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public partial class RestaurantPage : Page
    {
        public RestaurantPage()
        {
            InitializeComponent();

            btnAddRestaurant.Click += BtnAddRestaurant_Click;
            btnEditRestaurant.Click += BtnEditRestaurant_Click;
            btnDeleteRestaurant.Click += BtnDeleteRestaurant_Click;
            lstRestaurants.SelectionChanged += LstRestaurants_SelectionChanged;

            // Initially disable Edit and Delete buttons
            btnEditRestaurant.IsEnabled = false;
            btnDeleteRestaurant.IsEnabled = false;
        }

        private void LstRestaurants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRestaurants.SelectedItem != null)
            {
                txtRestaurantName.Text = lstRestaurants.SelectedItem.ToString();

                // Enable Edit and Delete buttons only when selection is made
                btnEditRestaurant.IsEnabled = true;
                btnDeleteRestaurant.IsEnabled = true;
            }
            else
            {
                ClearInputAndButtons();
            }
        }

        private void BtnAddRestaurant_Click(object sender, RoutedEventArgs e)
        {
            string name = txtRestaurantName.Text.Trim();

            if (!string.IsNullOrEmpty(name))
            {
                // Optional: prevent duplicate restaurant names
                if (!lstRestaurants.Items.Contains(name))
                {
                    lstRestaurants.Items.Add(name);
                    txtRestaurantName.Clear();
                }
                else
                {
                    MessageBox.Show("This restaurant already exists.");
                }
            }
            else
            {
                MessageBox.Show("Please enter a restaurant name.");
            }
        }

        private void BtnEditRestaurant_Click(object sender, RoutedEventArgs e)
        {
            int index = lstRestaurants.SelectedIndex;

            if (index >= 0)
            {
                string newName = txtRestaurantName.Text.Trim();

                if (!string.IsNullOrEmpty(newName))
                {
                    // Optional: check for duplicate names except the currently selected one
                    bool isDuplicate = false;
                    for (int i = 0; i < lstRestaurants.Items.Count; i++)
                    {
                        if (i != index && lstRestaurants.Items[i].ToString().Equals(newName, StringComparison.OrdinalIgnoreCase))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (!isDuplicate)
                    {
                        lstRestaurants.Items[index] = newName;
                        lstRestaurants.SelectedIndex = -1; // Deselect
                        ClearInputAndButtons();
                    }
                    else
                    {
                        MessageBox.Show("Another restaurant with this name already exists.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a new restaurant name.");
                }
            }
            else
            {
                MessageBox.Show("Please select a restaurant to edit.");
            }
        }

        private void BtnDeleteRestaurant_Click(object sender, RoutedEventArgs e)
        {
            int index = lstRestaurants.SelectedIndex;

            if (index >= 0)
            {
                var result = MessageBox.Show("Are you sure you want to delete this restaurant?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    lstRestaurants.Items.RemoveAt(index);
                    ClearInputAndButtons();
                }
            }
            else
            {
                MessageBox.Show("Please select a restaurant to delete.");
            }
        }

        private void ClearInputAndButtons()
        {
            txtRestaurantName.Clear();
            lstRestaurants.SelectedIndex = -1;
            btnEditRestaurant.IsEnabled = false;
            btnDeleteRestaurant.IsEnabled = false;
        }
    }
}
