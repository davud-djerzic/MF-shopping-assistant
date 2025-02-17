using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetEnv;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MF_Shopping_Assistant
{
    public partial class Form1 : Form
    {
        private string scannedBarcode;
        private bool isBarcodeScanned = false;
        private int passedSeconds = 0;
        private MySqlConnection mySqlConnection;

        private int locationLabelNameX = 17, locationLabelNameY = 21;
        private int locationLabelTotalPriceX = 298, locationLabelTotalPriceY = 48;
        private int locationLabelPricePerUnitX = 142, locationLabelPricePerUnitY = 48;
        private int locationLabelQuantityX = 17, locationLabelQuantityY = 48;

        private int numberOfProducts = 0;

        private List<int> listIdsOfProducts = new List<int>();
        private List<string> listNameOfProducts = new List<string>();
        private List<string> listTypeOfProducts = new List<string>();
        private List<string> listManufacturerOfProducts = new List<string>();
        private List<double> listPriceOfProducts = new List<double>();
        private List<double> listPricePerUnitOfProducts = new List<double>();
        private List<int> listQuantityOfProducts = new List<int>();
        private List<int> listInStockOfProducts = new List<int>();

        private int quantity = 1;
        private List<Panel> panelsOfProducts = new List<Panel>();

        private string selectedPanelTag;

        private int scrollStartY;
        private int scrollPosition;
        private bool isScrolling = false;

        private int updateQuantity = 1;
        private int quantityOfSelectedProduct;

        private bool isDoubleClicked = false;

        public Form1()
        {
            InitializeComponent();

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Env.Load("C:\\Users\\Korisnik\\Desktop\\MF Shopping Assistant\\.env");
            string connectionString = Env.GetString("Connection_String");

            mySqlConnection = new MySqlConnection(connectionString);
            try
            {
                await mySqlConnection.OpenAsync();
                MessageBox.Show("Connection opened succesfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtScannedBarcode.Focus();

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;
        }

        private void btnQuantityIncrease_Click(object sender, EventArgs e)
        {
            quantity++;
            lblNumberOfProducts.Text = quantity.ToString();
        }

        private void btnQuantityDecrease_Click(object sender, EventArgs e)
        {
            if (quantity > 1) quantity--;
            lblNumberOfProducts.Text = quantity.ToString();
        }

        private async void btnFinishProduct_Click(object sender, EventArgs e)
        {
            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            string getProductByBarcode = "SELECT * FROM Product";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByBarcode, mySqlConnection))
            {
                using(DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    bool productAlreadyExists = false;

                    while (productReader.Read())
                    {
                        if (productReader["Barcode"].ToString() == scannedBarcode)
                        {
                            double priceOfProduct = Convert.ToDouble(productReader["Price"]) * quantity;
                            int indexOfFoundProduct = 0;

                            for (int i = 0; i < listIdsOfProducts.Count; i++)
                            {
                                if (listIdsOfProducts[i] == Convert.ToInt32(productReader["ID"]))
                                {
                                    listQuantityOfProducts[i] += quantity;
                                    listPriceOfProducts[i] += priceOfProduct;
                                    indexOfFoundProduct = i;
                                    productAlreadyExists = true;
                                    break;
                                }
                            }

                            if (productAlreadyExists)
                            {
                                foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
                                {
                                    if ((int)panel.Tag == indexOfFoundProduct)
                                    {
                                        Label labelTotalPrice = panel.Controls.Find("lblTotalPrice", true).FirstOrDefault() as Label;
                                        Label labelQuantity = panel.Controls.Find("lblQuantity", true).FirstOrDefault() as Label;
                                        labelTotalPrice.Text = listPriceOfProducts[indexOfFoundProduct].ToString();
                                        labelQuantity.Text = listQuantityOfProducts[indexOfFoundProduct].ToString();
                                    }
                                }
                            }

                            if (!productAlreadyExists)
                            {
                                addNewProduct(productReader["Name"].ToString(), priceOfProduct.ToString(), productReader["Price"].ToString(), quantity.ToString());
                                listQuantityOfProducts.Add(quantity);
                                listPricePerUnitOfProducts.Add(Convert.ToDouble(productReader["Price"]));
                                listPriceOfProducts.Add(priceOfProduct);
                                listNameOfProducts.Add(productReader["Name"].ToString());
                                listIdsOfProducts.Add(Convert.ToInt32(productReader["ID"]));
                                listTypeOfProducts.Add(productReader["Type"].ToString());
                                listManufacturerOfProducts.Add(productReader["Manufacturer"].ToString());
                                listInStockOfProducts.Add(Convert.ToInt32(productReader["InStock"]));
                            }

                            break;
                        }
                    }
                    double totalPrice = 0;
                    for (int i = 0; i < listPriceOfProducts.Count; i++)
                    {
                        totalPrice += listPriceOfProducts[i];
                        lblTotalPrice.Text = totalPrice.ToString();
                    }

                    txtScannedBarcode.Clear();
                    txtScannedBarcode.Focus();
                    quantity = 1;
                    lblNumberOfProducts.Text = quantity.ToString();

                    isBarcodeScanned = false;
                }
            }  
        }

        private void txtScannedBarcode_TextChanged(object sender, EventArgs e)
        {
            if (txtScannedBarcode.Text != "" && !isDoubleClicked)
            {
                scannedBarcode = txtScannedBarcode.Text;
                isBarcodeScanned = true;
                timer1.Start();
            }
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            panelProductQuantity.Visible = true;
        }

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            scrollStartY = e.Y;
            scrollPosition = flowLayoutPanel1.AutoScrollPosition.Y;
            isScrolling = false;
        }

        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int deltaY=e.Y - scrollStartY;
                if (Math.Abs(deltaY) > 5)
                {
                    isScrolling = true;
                    flowLayoutPanel1.AutoScrollPosition = new Point(0, -(scrollPosition + deltaY));
                }
            }
        }

        private void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            flowLayoutPanel1.Capture = false;
        }
        //get data from gpioport
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            panelUpdateProductQuantity.Visible = false;

            foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
            {
                if (panel.Tag.ToString() == selectedPanelTag)
                {
                    Label labelTotalPrice = panel.Controls.Find("lblTotalPrice", true).FirstOrDefault() as Label;
                    Label labelPricePerUnit = panel.Controls.Find("lblPricePerUnit", true).FirstOrDefault() as Label;
                    Label labelQuantity = panel.Controls.Find("lblQuantity", true).FirstOrDefault() as Label;

                    double priceOfProduct = Convert.ToDouble(listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)] * updateQuantity);
                    double pricePerUnit = listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)];
                    string productName = listNameOfProducts[Convert.ToInt32(selectedPanelTag)];

                    labelTotalPrice.Text = priceOfProduct.ToString();
                    labelPricePerUnit.Text = pricePerUnit.ToString();
                    labelQuantity.Text = updateQuantity.ToString();
                    
                    listPriceOfProducts[Convert.ToInt32(selectedPanelTag)] = priceOfProduct;
                    listQuantityOfProducts[Convert.ToInt32(selectedPanelTag)] = updateQuantity;
                }
            }

            double totalPrice = 0;
            for (int i = 0; i < listPriceOfProducts.Count; i++)
            {
                totalPrice += listPriceOfProducts[i];
                lblTotalPrice.Text = totalPrice.ToString();
            }

            txtScannedBarcode.Clear();
            txtScannedBarcode.Focus();
            updateQuantity = 1;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();

            isDoubleClicked = false;
        }

        private void btnUpdateQuantityDecrease_Click(object sender, EventArgs e)
        {
            if (updateQuantity > 1) updateQuantity--;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString(); 
        }

        private void btnUpdateQuantityIncrease_Click(object sender, EventArgs e)
        {
            updateQuantity++;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();
        }

        private void panelConfirmPayment_Click(object sender, EventArgs e)
        {
            DialogResult rez = MessageBox.Show("Confirm purchase", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                flowLayoutPanel1.Visible = false;
                flowLayoutPanel2.Visible = true;
                btnPay.Visible = true;

                for (int i = 0; i < listPriceOfProducts.Count; i++)
                {
                    Label label = new Label()
                    {
                        Size = new Size(477, 35),
                        Location = new Point(10, i * 40),
                    };
                    label.Text = listNameOfProducts[i] + "   " + listTypeOfProducts[i] + "   " + listManufacturerOfProducts[i] + "\n" + listQuantityOfProducts[i] + "x" + "   " + listPricePerUnitOfProducts[i] + "    " + listPriceOfProducts[i];
                    flowLayoutPanel2.Controls.Add(label);
                }
            }
        }

        private async void btnPay_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel2.Visible = false;
            btnPay.Visible = false;
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel2.Controls.Clear();
            MessageBox.Show("Payment completed successfully");
            string queryUpdateQuantityOfProduct = "UPDATE Product SET InStock = @Item WHERE Id = @Id";
            MySqlCommand cmdUpdateProduct1 = new MySqlCommand(queryUpdateQuantityOfProduct, mySqlConnection);
            for (int i = 0; i < listIdsOfProducts.Count; i++)
            {
                cmdUpdateProduct1.Parameters.Clear();

                cmdUpdateProduct1.Parameters.AddWithValue("@Item", listInStockOfProducts[i] - listQuantityOfProducts[i]);
                cmdUpdateProduct1.Parameters.AddWithValue("@Id", listIdsOfProducts[i]);
                await cmdUpdateProduct1.ExecuteNonQueryAsync();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            passedSeconds++;
            if (passedSeconds == 1)
            {
                timer1.Stop();
                button1.Focus();
                passedSeconds = 0;
            }
        }

        private void addNewProduct(string productName, string totalPrice, string pricePerUnit, string quantity)
        {
            Panel panel;
            if (numberOfProducts == 0)
            {
                panel = new Panel
                {
                    Size = new Size(260, 68),
                    Location = new Point(6, 12),
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            } else
            {
                panel = new Panel
                {
                    Size = new Size(260, 68),
                    Location = new Point(6, 71 * numberOfProducts),
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            }
            panel.DoubleClick += PanelDoubleClick_DoubleClick;
            panel.MouseDown += flowLayoutPanel1_MouseDown;
            panel.MouseUp += flowLayoutPanel1_MouseUp;
            panel.MouseMove += flowLayoutPanel1_MouseMove;

            Label labelName = new Label
            {
                Text = productName,
                Location = new Point(17, 21),
                AutoSize = true
            };
            panel.Controls.Add(labelName); 

            Label labelTotalPrice = new Label
            {
                Name = "lblTotalPrice",
                Text = totalPrice,
                Location = new Point(230, 48), 
                AutoSize = true
            };

            panel.Controls.Add(labelTotalPrice); 

            Label labelPricePerUnit = new Label
            {
                Name = "lblPricePerUnit",
                Text = pricePerUnit,
                Location = new Point(142, 48), 
                AutoSize = true
            };

            panel.Controls.Add(labelPricePerUnit);

            Label labelQuantity = new Label
            {
                Name = "lblQuantity",
                Text = quantity,
                Location = new Point(17, 48), 
                AutoSize = true
            };

            panel.Controls.Add(labelQuantity); 

            System.Windows.Forms.Button closeButton = new System.Windows.Forms.Button
            {
                Location = new Point(220, 1),
                AutoSize = true,
                Tag = numberOfProducts,
                
            };
            closeButton.Click += ButtonClick_Click;
            panel.Controls.Add(closeButton);

            numberOfProducts++;

            panelsOfProducts.Add(panel);
            flowLayoutPanel1.Controls.Add(panel);
        }

        private void ButtonClick_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;
            if (clickedButton != null)
            {
                Panel parentPanel = clickedButton.Parent as Panel;
                if (parentPanel != null)
                {
                    double totalPrice = 0;
                    for (int i = 0; i < listPriceOfProducts.Count; i++)
                    {
                        totalPrice += listPriceOfProducts[i];
                    }
                    lblTotalPrice.Text = totalPrice.ToString();
                    flowLayoutPanel1.Controls.Remove(parentPanel);
                    parentPanel.Dispose();

                    numberOfProducts--; 

                    listNameOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listPriceOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listPricePerUnitOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listQuantityOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listIdsOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listManufacturerOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listTypeOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listInStockOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));

                    updatePanelTag();
                }
            }
        }

        private void PanelDoubleClick_DoubleClick(object sender, EventArgs e)
        {
            if (!isScrolling && !isBarcodeScanned)
            {
                isDoubleClicked = true;

                btnUpdateProduct.Focus();
                Panel panel = sender as Panel;
                if (panel != null)
                {
                    for (int i = 0; i < listQuantityOfProducts.Count; i++)
                    {
                        if (Convert.ToInt32(panel.Tag) == i)
                        {
                            quantityOfSelectedProduct = listQuantityOfProducts[i];
                            lblUpdateNumberOfProducts.Text = quantityOfSelectedProduct.ToString();
                            updateQuantity = listQuantityOfProducts[i];

                            selectedPanelTag = panel.Tag.ToString();

                            break;
                        }
                    }


                    updatePanelTag();

                    panelUpdateProductQuantity.Visible = true;
                }
            }
        }

        private void updatePanelTag()
        {
            int tagCounter = 0;
            foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
            {
                panel.Tag = tagCounter++;
            }
        }
    }
}

    

