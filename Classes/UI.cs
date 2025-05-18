using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class UI
    {
        private static int scrollStartY;
        private static int scrollPosition;
        //private static bool isScrolling = false;
        public static int numberOfProducts = 0;
        private FlowLayoutPanel flowLayoutPanel1;

        public UI(FlowLayoutPanel flowLayoutPanel1)
        {
            this.flowLayoutPanel1 = flowLayoutPanel1;
        }
        public static void addNewProduct(string productName, string totalPrice, string pricePerUnit, string quantity, bool isFruit, FlowLayoutPanel flowLayoutPanel1)
        {
            Panel panel;
            if (numberOfProducts == 0)
            {
                panel = new Panel
                {
                    //Size = new Size(510, 130),
                    Size = new Size(595, 130),
                    Location = new Point(6, 12),
                    BackColor = Color.Wheat,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            }
            else
            {
                panel = new Panel
                {
                    //Size = new Size(510, 130),
                    Size = new Size(595, 130),
                    Location = new Point(6, 71 * numberOfProducts),
                    BackColor = Color.Wheat,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            }
           // if (!isFruit) panel.DoubleClick += EditProduct.PanelDoubleClick;

            panel.MouseDown += flowLayoutPanel1Y_MouseDown;
            panel.MouseUp += flowLayoutPanel1Y_MouseUp;
            panel.MouseMove += flowLayoutPanel1Y_MouseMove;

            Label labelName = new Label
            {
                Text = productName,
                Location = new Point(10, 5),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Regular)
            };
            panel.Controls.Add(labelName);

            Label labelTotalPrice = new Label
            {
                Name = "lblTotalPrice",
                Text = totalPrice,
                Location = new Point(370, 96),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Regular)
            };

            panel.Controls.Add(labelTotalPrice);

            Label labelPricePerUnit = new Label
            {
                Name = "lblPricePerUnit",
                Text = pricePerUnit,
                Location = new Point(200, 96),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Regular)
            };

            panel.Controls.Add(labelPricePerUnit);

            Label labelQuantity = new Label
            {
                Name = "lblQuantity",
                Text = quantity,
                Location = new Point(30, 96),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Regular)
            };

            panel.Controls.Add(labelQuantity);

            System.Windows.Forms.Button closeButton = new System.Windows.Forms.Button
            {
                Text = "X",
                ForeColor = Color.White,
                //Location = new Point(430, 1),
                Location = new Point(512, 1),
                Size = new Size(75, 30),
                AutoSize = true,
                Tag = numberOfProducts,
                BackColor = Color.Maroon
            };
            closeButton.Click += EditProduct.RemoveProduct;
            panel.Controls.Add(closeButton);

            //string imagePath = @"C:\Users\Korisnik\Downloads\settingsIcon2.png";
            string imagePath = Path.Combine(Application.StartupPath, "/home/pi/Desktop/Pictures/settingsIcon2.png");

            Button updateProductButton = new Button
            {
                ForeColor = Color.White,
                //Location = new Point(430, 90),
                Location = new Point(507, 90),
                Size = new Size(75, 30),
                AutoSize = true,
                Tag = numberOfProducts,
                BackColor = Color.Maroon,
                Name = "Update"
            };
            Image originalImage = Image.FromFile(imagePath);
            Image resizedImage = new Bitmap(originalImage, updateProductButton.Size); // Skaliraj na veličinu buttona

            updateProductButton.Image = resizedImage;
            updateProductButton.ImageAlign = ContentAlignment.MiddleCenter;

            updateProductButton.Click += EditProduct.UpdateProductByButton;

            panel.Name = "Product";
            panel.Controls.Add(updateProductButton);
            if (isFruit)
            {
                updateProductButton.Visible = false;
                panel.Name = "Fruit";
            }
           

            numberOfProducts++;

            GlobalData.panelsOfProducts.Add(panel);
            flowLayoutPanel1.Controls.Add(panel);
        }

        public static void flowLayoutPanel1Y_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            scrollStartY = e.Y;
            scrollPosition = flowLayoutPanel1.AutoScrollPosition.Y;
            isScrolling = false;*/

            scrollStartY = e.Y;

            if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp)
            {
                scrollPosition = flp.AutoScrollPosition.Y;
            }

            Form1.isScrolling = false;
        }

        public static void flowLayoutPanel1Y_MouseMove(object sender, MouseEventArgs e)
        {/*
     if (e.Button == MouseButtons.Left)
     {
         int deltaY = e.Y - scrollStartY;
         if (Math.Abs(deltaY) > 5)
         {
             isScrolling = true;
             flowLayoutPanel1.AutoScrollPosition = new Point(0, -(scrollPosition + deltaY));
         }
     }*/
            if (e.Button == MouseButtons.Left)
            {
                int deltaY = e.Y - scrollStartY;
                if (Math.Abs(deltaY) > 5)
                {
                    Form1.isScrolling = true;

                    if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp)
                    {
                        flp.AutoScrollPosition = new Point(0, -(scrollPosition + deltaY));
                    }
                }
            }
        }

        public static void flowLayoutPanel1Y_MouseUp(object sender, MouseEventArgs e)
        {
            // flowLayoutPanel1.Capture = false;

            if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp)
            {
                flp.Capture = false;
            }
        }

        /*public static void flowLayoutPanel1Y_MouseDown(object sender, MouseEventArgs e)
        {
            scrollStartY = e.Y;

            if (sender is FlowLayoutPanel flp)
            {
                scrollPosition = Math.Abs(flp.AutoScrollPosition.Y);
            }
            else if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp2)
            {
                scrollPosition = Math.Abs(flp2.AutoScrollPosition.Y);
            }

            Form1.isScrolling = false;
        }

        public static void flowLayoutPanel1Y_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int deltaY = e.Y - scrollStartY;

                if (Math.Abs(deltaY) > 5)
                {
                    Form1.isScrolling = true;

                    FlowLayoutPanel flp = null;

                    if (sender is FlowLayoutPanel flowPanel)
                    {
                        flp = flowPanel;
                    }
                    else if (sender is Panel panel && panel.Parent is FlowLayoutPanel parentFlow)
                    {
                        flp = parentFlow;
                    }

                    if (flp != null)
                    {
                        int newScroll = scrollPosition - deltaY;

                        // Ograniči scrolling da ne ide ispod 0
                        if (newScroll < 0) newScroll = 0;

                        flp.AutoScrollPosition = new Point(0, newScroll);
                    }
                }
            }
        }

        public static void flowLayoutPanel1Y_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp)
            {
                flp.Capture = false;
            }
            else if (sender is FlowLayoutPanel flp2)
            {
                flp2.Capture = false;
            }
        }*/

        public static void HideBackground(Panel panel, Panel panelDisableBackground)
        {
            panelDisableBackground.Location = new Point(0, 0);
            //panelDisableBackground.Size = new Size(518, 717);
            panelDisableBackground.Size = new Size(690, 782);
           // panelDisableBackground.BackColor = Color.FromArgb(20, Color.Black);
            panel.Location = new Point(90, 100);
            panel.BringToFront();
        }

        public static void ShowBackground(Panel panel, Panel panelDisableBackground)
        {
            panelDisableBackground.Location = new Point(0, 0);
            panelDisableBackground.Size = new Size(0, 0);
            //panelDisableBackground.BackColor = Color.FromArgb(20, Color.Black);
            panel.Location = new Point(800, 0);
        }

     

        public static void flowLayoutPanel1X_MouseDown(object sender, MouseEventArgs e)
        {
            scrollStartY = e.X; 

            if (sender is FlowLayoutPanel flp)
            {
                scrollPosition = Math.Abs(flp.AutoScrollPosition.X); 
            }
            else if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp2)
            {
                scrollPosition = Math.Abs(flp2.AutoScrollPosition.X); 
            }

            Form1.isScrolling = false;
        }

        public static void flowLayoutPanel1X_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int deltaX = e.X - scrollStartY; 

                if (Math.Abs(deltaX) > 5)
                {
                    Form1.isScrolling = true;

                    FlowLayoutPanel flp = null;

                    if (sender is FlowLayoutPanel flowPanel)
                    {
                        flp = flowPanel;
                    }
                    else if (sender is Panel panel && panel.Parent is FlowLayoutPanel parentFlow)
                    {
                        flp = parentFlow;
                    }

                    if (flp != null)
                    {
                        int newScroll = scrollPosition - deltaX;

                        if (newScroll < 0) newScroll = 0;

                        flp.AutoScrollPosition = new Point(newScroll, 0); 
                    }
                }
            }
        }

        public static void flowLayoutPanel1X_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp)
            {
                flp.Capture = false;
            }
            else if (sender is FlowLayoutPanel flp2)
            {
                flp2.Capture = false;
            }
        }


        public static void updatePanelTag(FlowLayoutPanel flowLayoutPanel)
        {
            int tagCounter = 0;
            foreach (Panel outerPanel in flowLayoutPanel.Controls.OfType<Panel>())
            {
                Button updateButton = outerPanel.Controls.Find("Update", true).FirstOrDefault() as Button;
                if (updateButton != null)
                {
                    updateButton.Tag = tagCounter++;
                }
                /* if (outerPanel.Name == "Product")
                 {
                     Button updateButton = outerPanel.Controls.Find("Update", true).FirstOrDefault() as Button;
                     if (updateButton != null)
                     {
                         updateButton.Tag = tagCounter++;
                     }
                 } else
                 {
                     tagCounter++;
                 }*/
            }
            tagCounter = 0;
            foreach (Panel panel in flowLayoutPanel.Controls.OfType<Panel>())
            {
                panel.Tag = tagCounter++;
            }
        }
    }
}
