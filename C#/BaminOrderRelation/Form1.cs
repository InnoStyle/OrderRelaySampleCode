﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace BaminOrderRelation
{
    public partial class FrmBaminOrderRelationMain : Form
    {
        const int S_OK = 0;
        const int PT_PREPAYED = 0;
        const int PT_MEET_CARD = 1;
        const int PT_MEET_CASH = 2;

        const int OS_NEW = 0;
        const int OS_RECEIPT = 1;
        const int OS_COMPLETED = 2;
        const int OS_CANCELED = 3;


        public delegate bool TOnNewDeliveryFunc([MarshalAs(UnmanagedType.LPWStr)] string AOrderNo, [MarshalAs(UnmanagedType.LPWStr)] string ARoadNameAddress,
            [MarshalAs(UnmanagedType.LPWStr)] string AAddress, [MarshalAs(UnmanagedType.LPWStr)] string AAddressDetail, [MarshalAs(UnmanagedType.LPWStr)] string APhoneNo,
            [MarshalAs(UnmanagedType.LPWStr)] string ALatitude, [MarshalAs(UnmanagedType.LPWStr)] string ALongitude, [MarshalAs(UnmanagedType.LPWStr)] string ATitle, 
            int AQuantity, int AAmount, int APaymentType);
        public delegate void TOnStatusChangedProc([MarshalAs(UnmanagedType.LPWStr)] string AOrderNo, int AOrderStatus);
        public delegate void TOnDisconnectedProc();

        [DllImport("BMOrderRelay.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int InitializeService(string ASignKey);

        [DllImport("BMOrderRelay.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int FinalizeService();

        [DllImport("BMOrderRelay.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterNewDeliveryFunction(TOnNewDeliveryFunc AEvent);

        [DllImport("BMOrderRelay.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterStatusChangedFunction(TOnStatusChangedProc AEvent);

        [DllImport("BMOrderRelay.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterDisconnectedFunction(TOnDisconnectedProc AEvent);

        [DllImport("BMOrderRelay.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDeliveryCompleted(string AOrderNo);

        public TOnNewDeliveryFunc s_newDeliveryFunc;
        public TOnStatusChangedProc s_statusChangedProc;
        public TOnDisconnectedProc s_disconnectedProc;

        public FrmBaminOrderRelationMain()
        {
            InitializeComponent();

            s_newDeliveryFunc = this.MyOnNewDeliveryFunc;
            s_statusChangedProc = this.MyOnStatusChangedProc;
            s_disconnectedProc = this.MyOnDisconnectedProc;

        }

        private void btnSetDeliveryCompleted_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0)
            {
                return;
            }
            int intselectedindex = listView1.SelectedIndices[0];
            if (intselectedindex >= 0)
            {
                String text = listView1.Items[intselectedindex].Text;
                SetDeliveryCompleted(text);

            }
        }

        private void btnInitializeService_Click(object sender, EventArgs e)
        {
            if (InitializeService("Test Mode Gear") != S_OK) {
                MessageBox.Show("InitializeService Failed");
            }
            richTextBox1.AppendText("Initialize Service" + Environment.NewLine);
        }

        private void btnFinalizeService_Click(object sender, EventArgs e)
        {
            if (FinalizeService() != S_OK)
            {
                MessageBox.Show("FinalizeService Failed");
            }
            richTextBox1.AppendText("Finalize Service" + Environment.NewLine);
        }

        private void btnRegCallback_Click(object sender, EventArgs e)
        {
            if (!RegisterNewDeliveryFunction(s_newDeliveryFunc))
                MessageBox.Show("Error RegisterNewDeliveryFunction!");
            if (!RegisterStatusChangedFunction(s_statusChangedProc))
                MessageBox.Show("Error RegisterStatusChangedFunction!");
            if (!RegisterDisconnectedFunction(s_disconnectedProc))
                MessageBox.Show("Error RegisterDisconnectedFunction");
            richTextBox1.AppendText("Register Callback Functions" + Environment.NewLine);
        }
         
        private bool MyOnNewDeliveryFunc(string AOrderNo, string ARoadNameAddress, string AAddress, string AAddressDetail, string APhoneNo,
            string ALatitude, string ALongitude, string ATitle, int AQuantity, int AAmount, int APaymentType)
        {
            //MessageBox.Show("AOrderNo:" + AOrderNo);
            Invoke(new MethodInvoker(delegate ()
            {
                richTextBox1.AppendText("[New Delivery]" + Environment.NewLine);
                richTextBox1.AppendText("AOrderNo: " + AOrderNo + Environment.NewLine);
                richTextBox1.AppendText("ARoadNameAddress: " + ARoadNameAddress + Environment.NewLine);
                richTextBox1.AppendText("AAddress: " + AAddress + Environment.NewLine);
                richTextBox1.AppendText("AAddressDetail: " + AAddressDetail + Environment.NewLine);
                richTextBox1.AppendText("APhoneNo: " + APhoneNo + Environment.NewLine);
                richTextBox1.AppendText("ALatitude: " + ALatitude + Environment.NewLine);
                richTextBox1.AppendText("ALongitude: " + ALongitude + Environment.NewLine);
                richTextBox1.AppendText("ATitle: " + ATitle + Environment.NewLine);
                richTextBox1.AppendText("AQuantity: " + AQuantity + Environment.NewLine);
                richTextBox1.AppendText("AAmount: " + AAmount + Environment.NewLine);
                switch (APaymentType)
                {
                    case PT_PREPAYED:
                        richTextBox1.AppendText("PaymentType: 바로결제" + Environment.NewLine);
                        break;
                    case PT_MEET_CARD:
                        richTextBox1.AppendText("PaymentType: 만나서 결제 카드" + Environment.NewLine);
                        break;
                    case PT_MEET_CASH:
                        richTextBox1.AppendText("PaymentType: 만나서 결제 현금" + Environment.NewLine);
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }


                Form2 addDelivery = new Form2();
                addDelivery.tbOrderNo.Text = AOrderNo;
                addDelivery.tbRoadName.Text = ARoadNameAddress;
                addDelivery.tbAddress.Text = AAddress;
                addDelivery.tbAddressDetail.Text = AAddressDetail;
                addDelivery.tbPhoneNo.Text = APhoneNo;
                addDelivery.tbLatitude.Text = ALatitude;
                addDelivery.tbLongitude.Text = ALongitude;
                addDelivery.tbTitle.Text = ATitle;
                addDelivery.tbTotalAmount.Text = AAmount.ToString();
                addDelivery.tbTotalCount.Text = AQuantity.ToString();

                switch (APaymentType)
                {
                    case PT_PREPAYED:
                        addDelivery.rbPrepaid.Checked = true;
                        break;
                    case PT_MEET_CARD:
                        addDelivery.rbMeetCard.Checked = true;
                        break;
                    case PT_MEET_CASH:
                        addDelivery.rbMeetCash.Checked = true;
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }


                DialogResult Res = addDelivery.ShowDialog(this);
                if (Res == DialogResult.OK)
                {
                    ListViewItem item = new ListViewItem(AOrderNo);
                    item.SubItems.Add(ARoadNameAddress + " " + AAddressDetail);
                    item.SubItems.Add(ALatitude + ", " + ALongitude);
                    if (addDelivery.rbPrepaid.Checked)
                        item.SubItems.Add("바로결제");
                    else if (addDelivery.rbMeetCard.Checked)
                        item.SubItems.Add("만나서 결제 카드");
                    else
                        item.SubItems.Add("만나서 결제 현금");
                    //item.SubItems.Add(ARoadNameAddress + " " + AAddressDetail);

                    listView1.Items.Add(item);
                }
            }));
            return true;

        }

        private void MyOnStatusChangedProc(string AOrderNo, int AOrderStatus)
        {
            Invoke(new MethodInvoker(delegate () 
            {
                richTextBox1.AppendText("StatusChanged: " + AOrderNo + " " + AOrderStatus + Environment.NewLine);

                for (int i = listView1.Items.Count-1; i >=0 ; i--)
                {
                    if (AOrderNo.Equals(listView1.Items[i].Text)) {
                        listView1.Items.RemoveAt(i);
                        break;
                    }
                }

            }));
        }

        private void MyOnDisconnectedProc()
        {
            Invoke(new MethodInvoker(delegate () { richTextBox1.AppendText("Disconnected!!!" + Environment.NewLine); }));
        }

    }
}
