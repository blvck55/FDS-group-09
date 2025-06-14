using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btnRestaurants.Click += BtnRestaurants_Click;
            btnMenuItems.Click += (s, e) => MainContentFrame.Navigate(new MenuItemsPage());
            btnOrders.Click += BtnOrders_Click;
            btnDeliveries.Click += BtnDeliveries_Click;
            btnReviews.Click += BtnReviews_Click;

            // Optionally, load a default page on startup
            MainContentFrame.Navigate(new RestaurantPage());
        }

        private void BtnRestaurants_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new RestaurantPage());
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new OrdersPage());
        }

        private void BtnDeliveries_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new DeliveriesPage());
        }

        private void BtnReviews_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ReviewsPage());
        }
    }
}
