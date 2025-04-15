using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class Fruit
    {
        private static MySqlConnection mySqlConnection;

        private static TcpClient client;
        private static NetworkStream stream;
        private static bool isRunning = false;
        private static float receivedValue;
        private static float value;

        private static Label lblTotalPrice;
        private static FlowLayoutPanel flpFruit;
        private static TextBox txtScannedBarcode;
        private Panel homePagePanel;
        private static FlowLayoutPanel flowLayoutPanel1;

        private static List<double> listFruitPrice = new List<double>();
        private static List<double> listFruitDiscountPrice = new List<double>();
        private static List<double> listFruitInStock = new List<double>();
        private static List<string> listFruitName = new List<string>();
        private static List<int> listFruitId = new List<int>();
        private static List<string> listFruitType = new List<string>();


        public Fruit(MySqlConnection mySqlConnection, FlowLayoutPanel flpFruit, Panel homePagePanel, Label lblTotalPrice, TextBox txtScannedBarcode, FlowLayoutPanel flowLayoutPanel1)
        {
            Fruit.mySqlConnection = mySqlConnection;
            Fruit.lblTotalPrice = lblTotalPrice;
            Fruit.flpFruit = flpFruit;
            Fruit.txtScannedBarcode = txtScannedBarcode;
            this.homePagePanel = homePagePanel;
            Fruit.flowLayoutPanel1 = flowLayoutPanel1;
        }

        public delegate void WeightReceivedHandler(float weight);
        public event WeightReceivedHandler OnWeightReceived;

        public async Task btnShowFruit()
        {
            flpFruit.Visible = true;

            flpFruit.Location = new Point(homePagePanel.Location.X, homePagePanel.Location.Y);
            flpFruit.Size = new Size(homePagePanel.Width, homePagePanel.Height);

            try
            {
                isRunning = true;
                await Task.Run(() => ReceiveWeightData());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška: " + ex.Message);
            }
        }

        public static async Task btnChooseFruitToBuy()
        {
            try
            {
                isRunning = false;
                await SendCommandToRaspberry("STOP");

                stream?.Close();
                client?.Close();
                client = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška: " + ex.Message);
            }
        }

        public static async Task SendCommandToRaspberry(string command)
        {
            try
            {
                if (client == null || !client.Connected)
                {
                    client = new TcpClient("127.0.0.1", 50001);
                    stream = client.GetStream();
                }

                byte[] message = Encoding.ASCII.GetBytes(command);
                await stream.WriteAsync(message, 0, message.Length);
                await stream.FlushAsync();

                /*byte[] buffer = new byte[4];


                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 4)
                {
                    receivedValue = BitConverter.ToSingle(buffer, 0);
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblFruitWeight.Text = $"Vrijednost: {receivedValue:F2} g";
                    });
                }
                //Task.Delay(1000).Wait();
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška u slanju podataka: " + ex.Message);
            }
        }

        public async Task ReceiveWeightData()
        {
            if (isRunning)
            {
                try
                {
                    if (!isRunning) throw new Exception("EEE");

                    if (client == null || !client.Connected)
                    {
                        client = new TcpClient("127.0.0.1", 50001);
                        stream = client.GetStream();
                    }

                    byte[] message = Encoding.ASCII.GetBytes("GET_WEIGHT");
                    stream.Write(message, 0, message.Length);
                    stream.Flush();

                    byte[] buffer = new byte[4];

                    while (isRunning)
                    {
                        if (!isRunning) break;
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 4)
                        {
                            receivedValue = BitConverter.ToSingle(buffer, 0);
                            if (receivedValue != 0.0)
                            {
                                value = receivedValue - 0.1f;
                                OnWeightReceived?.Invoke(value); // Pozivamo event!
                            }
                        }
                        //Task.Delay(1000).Wait();
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Greška pri primanju podataka: " + ex.Message);
                }
            }
            
        }

        public static void FruitClick(object sender, EventArgs e)
        {
            if (sender is Panel panel && panel.Tag != null)
            {
                string clickedTag = panel.Tag.ToString();
                //MessageBox.Show(clickedTag);
                for (int i = 0; i < listFruitName.Count; i++)
                {
                    if (listFruitName[i] == clickedTag)
                    {
                        if (listFruitDiscountPrice[i] == 0)
                        {
                            double priceOfProduct = listFruitPrice[i] * value / 1000;
                            GlobalData.listQuantityOfProducts.Add(Math.Round(value / 1000, 2) - 0.1);
                            GlobalData.listPricePerUnitOfProducts.Add(listFruitPrice[i]);
                            GlobalData.listPriceOfProducts.Add(priceOfProduct);
                            GlobalData.listNameOfProducts.Add(listFruitName[i]);
                            GlobalData.listIdsOfProducts.Add(listFruitId[i]);
                            GlobalData.listTypeOfProducts.Add(listFruitType[i]);
                            GlobalData.listManufacturerOfProducts.Add("1");
                            GlobalData.listInStockOfProducts.Add(listFruitInStock[i]);

                            double totalPriceOfProduct = listFruitPrice[i] * value / 1000;
                            UI.addNewProduct(listFruitName[i], totalPriceOfProduct.ToString(), listFruitPrice[i].ToString(), (value / 1000).ToString(), true, flowLayoutPanel1);
                        }
                        if (listFruitDiscountPrice[i] != 0)
                        {
                            double priceOfProduct = listFruitDiscountPrice[i] * value / 1000;
                            GlobalData.listQuantityOfProducts.Add(Math.Round(value / 1000, 2) - 0.1);
                            GlobalData.listPricePerUnitOfProducts.Add(listFruitPrice[i]);
                            GlobalData.listPriceOfProducts.Add(priceOfProduct);
                            GlobalData.listNameOfProducts.Add(listFruitName[i]);
                            GlobalData.listIdsOfProducts.Add(listFruitId[i]);
                            GlobalData.listTypeOfProducts.Add(listFruitType[i]);
                            GlobalData.listManufacturerOfProducts.Add("1");
                            GlobalData.listInStockOfProducts.Add(listFruitInStock[i]);

                            double totalPriceOfProduct = listFruitDiscountPrice[i] * value / 1000;
                            UI.addNewProduct(listFruitName[i], totalPriceOfProduct.ToString(), listFruitPrice[i].ToString(), (value / 1000).ToString(), true, flowLayoutPanel1);
                        }
                    }
                }

                EditProduct.CalculateTotalPrice();

                flpFruit.Visible = false;

                txtScannedBarcode.Focus();
            }
        }

        public static async Task loadFruit()
        {
            flpFruit.Visible = false;

            listFruitInStock.Clear();
            listFruitName.Clear();
            listFruitDiscountPrice.Clear();
            listFruitPrice.Clear();
            flpFruit.Controls.Clear();

            string getCategoryForFruit = "SELECT * FROM Category";

            int fruitCategoryID = 0;
            int vegetablesCategoryID = 0;

            using (MySqlCommand cmdProduct = new MySqlCommand(getCategoryForFruit, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    while (productReader.Read())
                    {
                        if (productReader["name"].ToString() == "Fruit") fruitCategoryID = (int)productReader["ID"];
                        if (productReader["name"].ToString() == "Vegetables") vegetablesCategoryID = (int)productReader["ID"];
                    }
                }
            }

            //MessageBox.Show(FruitCategoryID.ToString() + " " + VegetablesCategoryID.ToString());

            string getProductByFruit = $"SELECT id, type,name,price,discountPrice,inStock FROM Product WHERE CategoryID={vegetablesCategoryID} OR CategoryID={fruitCategoryID}";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByFruit, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    while (productReader.Read())
                    {
                        int columnIndex = productReader.GetOrdinal("DiscountPrice");

                        if (!productReader.IsDBNull(columnIndex)) listFruitDiscountPrice.Add((double)productReader["DiscountPrice"]);
                        else listFruitDiscountPrice.Add(0);

                        listFruitId.Add((int)productReader["ID"]);
                        listFruitInStock.Add((double)productReader["InStock"]);
                        listFruitName.Add(productReader["Name"].ToString().ToLower());
                        listFruitPrice.Add((double)productReader["Price"]);
                        listFruitType.Add(productReader["Type"].ToString().ToLower());
                    }
                }
            }

            for (int i = 0; i < listFruitDiscountPrice.Count; i++)
            {
                string imagePath = Path.Combine(Application.StartupPath, "/home/pi/Desktop/Pictures/", $"{listFruitName[i]}.jpg");
                //string imagePath = Path.Combine(Application.StartupPath, "Pictures/", $"{listFruitName[i]}.jpg");
               
                Panel panel = new Panel
                {
                    Size = new Size(100, 100),
                    Tag = listFruitName[i],
                    BackgroundImage = Image.FromFile(imagePath),
                    BackgroundImageLayout = ImageLayout.Stretch
                };
                panel.Click += async (sender, e) => await btnChooseFruitToBuy();

                panel.Click += FruitClick;

                Label label = new Label
                {
                    Text = $"{i + 1}"
                };
                panel.Controls.Add(label);
                flpFruit.Controls.Add(panel);
            }
        }
    }

}
