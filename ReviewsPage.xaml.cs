using System.Windows;
using System.Windows.Controls;

namespace FoodDeliverySystem
{
    public partial class ReviewsPage : Page
    {
        public ReviewsPage()
        {
            InitializeComponent();
            btnSubmitReview.Click += BtnSubmitReview_Click;
        }

        private void BtnSubmitReview_Click(object sender, RoutedEventArgs e)
        {
            // Validate input fields
            if (!string.IsNullOrWhiteSpace(txtReviewerName.Text) &&
                !string.IsNullOrWhiteSpace(txtTargetName.Text) &&
                !string.IsNullOrWhiteSpace(txtReview.Text) &&
                cmbTarget.SelectedItem is ComboBoxItem selectedItem)
            {
                // Get target type from ComboBox
                string targetType = selectedItem.Content.ToString();

                // Format the review entry
                string entry = $"{txtReviewerName.Text.Trim()} reviewed {targetType} '{txtTargetName.Text.Trim()}': {txtReview.Text.Trim()}";

                // Add review to ListBox
                lstReviews.Items.Add(entry);

                // Clear fields after submission
                ClearFields();
            }
            else
            {
                MessageBox.Show("Please fill in all fields before submitting your review.", "Input Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearFields()
        {
            txtReviewerName.Clear();
            txtTargetName.Clear();
            txtReview.Clear();
            cmbTarget.SelectedIndex = -1;
        }
    }
}
