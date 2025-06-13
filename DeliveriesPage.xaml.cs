using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public partial class DeliveriesPage : Page
    {
        public DeliveriesPage()
        {
            InitializeComponent();

            btnAddDelivery.Click += BtnAddDelivery_Click;
            btnEditDelivery.Click += BtnEditDelivery_Click;
            btnDeleteDelivery.Click += BtnDeleteDelivery_Click;

            LoadDeliveries();
        }

        private void LoadDeliveries()
        {
            lstDeliveries.Items.Clear();
            lstDeliveries.Items.Add("OrderID: 1, DeliveryPerson: John, Status: Pending");
            lstDeliveries.Items.Add("OrderID: 2, DeliveryPerson: Mary, Status: Delivered");
        }

        private void BtnAddDelivery_Click(object sender, RoutedEventArgs e)
        {
            string orderId = txtOrderID.Text;
            string deliveryPerson = txtDeliveryPerson.Text;
            string deliveryStatus = (cmbDeliveryStatus.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(deliveryPerson) || string.IsNullOrEmpty(deliveryStatus))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            MessageBox.Show("Add delivery: " + orderId);

            LoadDeliveries();
        }

        private void BtnEditDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (lstDeliveries.SelectedItem == null)
            {
                MessageBox.Show("Select a delivery to edit.");
                return;
            }

            MessageBox.Show("Edit delivery.");
            LoadDeliveries();
        }

        private void BtnDeleteDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (lstDeliveries.SelectedItem == null)
            {
                MessageBox.Show("Select a delivery to delete.");
                return;
            }

            MessageBox.Show("Delete delivery.");
            LoadDeliveries();
        }
    }
}
