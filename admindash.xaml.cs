using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FoodDeliverySystem
{
    public partial class AdminDash : Page
    {
        private List<Order> orders = new List<Order>();
        private DispatcherTimer statusTimer;
        private int statusStep;
        private Order? currentTrackingOrder;

        public AdminDash()
        {
            InitializeComponent();

            LoadOrders("Daily");

            btnDaily.Click += (s, e) => LoadOrders("Daily");
            btnWeekly.Click += (s, e) => LoadOrders("Weekly");
            btnMonthly.Click += (s, e) => LoadOrders("Monthly");
            btnCheckStatus.Click += BtnCheckStatus_Click;

            statusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            statusTimer.Tick += StatusTimer_Tick;
        }

        private void LoadOrders(string filter)
        {
            try
            {
                // Load orders from DB and filter by date
                string query = "SELECT * FROM orders";
                DataTable dt = DatabaseHelper.ExecuteQuery(query);

                orders.Clear();

                DateTime filterDate = filter switch
                {
                    "Daily" => DateTime.Today,
                    "Weekly" => DateTime.Today.AddDays(-7),
                    "Monthly" => DateTime.Today.AddMonths(-1),
                    _ => DateTime.Today,
                };

                foreach (DataRow row in dt.Rows)
                {
                    DateTime orderDate = Convert.ToDateTime(row["order_date"]); // assuming there's order_date column
                    if (orderDate >= filterDate)
                    {
                        orders.Add(new Order(
                            row["id"].ToString() ?? "",
                            row["customer_name"].ToString() ?? "",
                            orderDate,
                            Convert.ToDecimal(row["amount"]),
                            row["status"].ToString() ?? ""
                        ));
                    }
                }

                // Show summary
                lblTotalOrders.Text = orders.Count.ToString();

                decimal totalRevenue = 0m;
                int pendingDeliveries = 0;

                foreach (var order in orders)
                {
                    totalRevenue += order.Amount;
                    if (!order.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                        pendingDeliveries++;
                }

                lblRevenue.Text = $"${totalRevenue:F2}";
                lblPending.Text = pendingDeliveries.ToString();

                // Bind to ListBox (or DataGrid)
                lstOrders.Items.Clear();
                foreach (var order in orders)
                {
                    lstOrders.Items.Add($"{order.OrderID} - {order.CustomerName} ordered on {order.Date.ToShortDateString()} - ${order.Amount:F2} [{order.Status}]");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load orders: " + ex.Message);
            }
        }

        private void BtnCheckStatus_Click(object sender, RoutedEventArgs e)
        {
            string orderId = txtOrderIDTrack.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(orderId))
            {
                lblCurrentStatus.Text = "Status: Please enter a valid Order ID.";
                progressStatus.Value = 0;
                statusTimer.Stop();
                return;
            }

            currentTrackingOrder = orders.Find(o => o.OrderID.Equals(orderId, StringComparison.OrdinalIgnoreCase));

            if (currentTrackingOrder == null)
            {
                lblCurrentStatus.Text = "Status: Order ID not found.";
                progressStatus.Value = 0;
                statusTimer.Stop();
                return;
            }

            // Start status simulation
            statusStep = 0;
            progressStatus.Value = 0;
            lblCurrentStatus.Text = "Status: Checking...";
            statusTimer.Start();
        }

        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            if (currentTrackingOrder == null)
            {
                statusTimer.Stop();
                return;
            }

            string[] statuses = { "Placed", "Processing", "Out for Delivery", "Delivered" };

            if (statusStep < statuses.Length)
            {
                lblCurrentStatus.Text = $"Status: {statuses[statusStep]}";
                progressStatus.Value = ((statusStep + 1) * 100) / statuses.Length;
                statusStep++;
            }
            else
            {
                statusTimer.Stop();
                // Optionally update currentTrackingOrder.Status here
                lblCurrentStatus.Text = $"Status: {currentTrackingOrder.Status} (Final)";
            }
        }
    }

    // Reuse Order class as you used in OrdersPage
    public class Order
    {
        public string OrderID { get; }
        public string CustomerName { get; }
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Status { get; }

        public Order(string id, string customerName, DateTime date, decimal amount, string status)
        {
            OrderID = id;
            CustomerName = customerName;
            Date = date;
            Amount = amount;
            Status = status;
        }
    }
}
