using System;
using System.Collections.Generic;
using System.Drawing;
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
                    Size = new Size(260, 68),
                    Location = new Point(6, 12),
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            }
            else
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
            if (!isFruit) panel.DoubleClick += EditProduct.PanelDoubleClick;

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
            closeButton.Click += EditProduct.RemoveProduct;
            panel.Controls.Add(closeButton);

            numberOfProducts++;

            GlobalData.panelsOfProducts.Add(panel);
            flowLayoutPanel1.Controls.Add(panel);
        }

        public static void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
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

        public static void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
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

        public static void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            // flowLayoutPanel1.Capture = false;

            if (sender is Panel panel && panel.Parent is FlowLayoutPanel flp)
            {
                flp.Capture = false;
            }
        }

        public static void updatePanelTag(FlowLayoutPanel flowLayoutPanel)
        {
            int tagCounter = 0;
            foreach (Panel panel in flowLayoutPanel.Controls.OfType<Panel>())
            {
                panel.Tag = tagCounter++;
            }
        }
    }
}
