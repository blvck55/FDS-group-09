using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FoodDeliverySystem
{
    public partial class OrderTrackingPage : Page
    {
        private DispatcherTimer statusTimer;
        private int currentStep = 0;
        private string[] statuses = { "Order Placed", "Processing", "Out for Delivery", "Delivered" };

        public OrderTrackingPage()
        {
            InitializeComponent();

            statusTimer = new DispatcherTimer();
            statusTimer.Interval = TimeSpan.FromSeconds(1);
            statusTimer.Tick += StatusTimer_Tick;
        }

        private void btnTrack_Click(object sender, RoutedEventArgs e)
        {
            string orderId = txtOrderId.Text.Trim();

            if (string.IsNullOrEmpty(orderId))
            {
                lblStatus.Text = "Please enter an Order ID.";
                return;
            }

            // Start progress simulation
            currentStep = 0;
            progressStatus.Value = 0;
            lblStatus.Text = "Checking...";
            statusTimer.Start();
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            if (currentStep < statuses.Length)
            {
                lblStatus.Text = statuses[currentStep];
                progressStatus.Value = ((currentStep + 1) * 100) / statuses.Length;
                currentStep++;
            }
            else
            {
                statusTimer.Stop();
                lblStatus.Text = "Order Delivered!";
            }
        }
    }
}
