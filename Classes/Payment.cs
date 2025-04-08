using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class Payment
    {
        private readonly MySqlConnection mySqlConnection;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button btnPay;
        private Panel panelConfirmPayment;
        public Payment(MySqlConnection mySqlConnection, FlowLayoutPanel flowLayoutPanel1, FlowLayoutPanel flowLayoutPanel2, Button btnPay, Panel panelConfirmPayment)
        {
            this.mySqlConnection = mySqlConnection;
            this.flowLayoutPanel1 = flowLayoutPanel1;
            this.flowLayoutPanel2 = flowLayoutPanel2;
            this.btnPay = btnPay;
            this.panelConfirmPayment = panelConfirmPayment;
        }
        public void PanelConfirmPayment_Click(object sender, EventArgs e) 
        {
            try
            {
                DialogResult rez = MessageBox.Show("Confirm purchase", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rez == DialogResult.Yes)
                {
                    if (GlobalData.listPriceOfProducts.Count == 0) throw new Exception("You don't have product to buy");

                    flowLayoutPanel1.Visible = false;
                    flowLayoutPanel2.Visible = true;
                    btnPay.Visible = true;
                    panelConfirmPayment.Visible = false;

                    for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
                    {
                        Label label = new Label()
                        {
                            Size = new Size(477, 35),
                            Location = new Point(10, i * 40),
                        };
                        label.Text = GlobalData.listNameOfProducts[i] + "   " + GlobalData.listTypeOfProducts[i] + "   " + GlobalData.listManufacturerOfProducts[i] + "\n" + GlobalData.listQuantityOfProducts[i] + "x" + "   " + GlobalData.listPricePerUnitOfProducts[i] + "    " + GlobalData.listPriceOfProducts[i];
                        flowLayoutPanel2.Controls.Add(label);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public async Task Pay()
        {
            try
            {
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = false;
                btnPay.Visible = false;
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();
                MessageBox.Show("Payment completed successfully");
                string queryUpdateQuantityOfProduct = "UPDATE Product SET InStock = @Item WHERE Id = @Id";
                MySqlCommand cmdUpdateProduct1 = new MySqlCommand(queryUpdateQuantityOfProduct, mySqlConnection);
                for (int i = 0; i < GlobalData.listIdsOfProducts.Count; i++)
                {
                    cmdUpdateProduct1.Parameters.Clear();

                    cmdUpdateProduct1.Parameters.AddWithValue("@Item", GlobalData.listInStockOfProducts[i] - GlobalData.listQuantityOfProducts[i]);
                    cmdUpdateProduct1.Parameters.AddWithValue("@Id", GlobalData.listIdsOfProducts[i]);
                    await cmdUpdateProduct1.ExecuteNonQueryAsync();
                }

                SetReset.reset();
                
                await Fruit.loadFruit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
