using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;

namespace Reg1
{
    public partial class Form1 : Form
    {
        public string ourkey = "Software\\safeqp";
        public string ourkeyfile = "licfile";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Get the licence key for safeqp.dll from the registry and display in hex form
            textBox2.ScrollBars = ScrollBars.None;
            textBox2.Multiline = false;
            Byte[] lic = null;
            try
            {
                System.Security.AccessControl.RegistryRights rights = System.Security.AccessControl.RegistryRights.FullControl;
                RegistryKeyPermissionCheck check = RegistryKeyPermissionCheck.ReadWriteSubTree;
                RegistryKey safekey;
                safekey = Registry.CurrentUser.OpenSubKey(ourkey, check, rights);

                if (safekey != null)
                {
                    lic = (Byte[])safekey.GetValue(ourkey);
                    if (lic == null)
                    {
                        throw new Exception("No lic");
                    }
                    safekey.Close();
                }
            }
            catch (Exception prob)
            {
                if(textBox2.Text != "")
                {
                    ourkeyfile = textBox2.Text;
                }
                using (FileStream stream = new FileStream(ourkeyfile, FileMode.OpenOrCreate))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        lic = new Byte[20];
                        reader.Read(lic, 0, 20);
                        reader.Close();
                    }
                }
            }
            if (lic != null)
            {
                string licence = "";
                string tlicence = "";
                for (int i = 0; i < lic.Length; ++i)
                {
                    licence += string.Format("{0:x2};", lic[i]);
                    tlicence += (char)lic[i];
                }
 
                textBox2.Multiline = true;
                textBox2.ScrollBars = ScrollBars.Vertical;
                textBox1.Text = licence;
                textBox2.Text = tlicence;
                string ll2=licence.Replace(';', ',');
                textBox2.Text += "\n\r\n\r\n\rWindows Registry Editor Version 5.00";
                textBox2.Text += "\n";
                textBox2.Text += "\n[HKEY_LOCAL_MACHINE\\Software\\safeqp]";
                textBox2.Text += "\n\"Software\\\\safeqp\"=hex:" + ll2;
                textBox2.Text += "\n";
                textBox2.Text += "\n[HKEY_CURRENT_USER\\Software\\safeqp]";
                textBox2.Text += "\n\"Software\\\\safeqp\"=hex:" + ll2;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //Write a new lecence key from textbox to registry
            string tlicence = "";
            string newlicence = textBox1.Text;
            string[] slicence = newlicence.Split(';');
            Byte[] licnew = new Byte[slicence.Length - 1];
            for (int i = 0;i < slicence.Length-1; ++i)
            {
                licnew[i] = Convert.ToByte(slicence[i], 16);
                tlicence += (char)licnew[i];
            }
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox2.Multiline = true;
            try
            {
                System.Security.AccessControl.RegistryRights rights = System.Security.AccessControl.RegistryRights.FullControl;
                RegistryKeyPermissionCheck check = RegistryKeyPermissionCheck.ReadWriteSubTree;

                RegistryKey safekey;
                safekey = Registry.CurrentUser.OpenSubKey(ourkey, check, rights);
                if (safekey == null)
                {
                    safekey = Registry.CurrentUser.CreateSubKey(ourkey);
                }
                safekey.SetValue(ourkey, licnew);
                safekey.Close();
                textBox2.Text = "echo " + "\"" + newlicence + "\"" + " | hostlist.exe -read -r";
            }
            catch(Exception prob)
            {
                using (FileStream stream = new FileStream(ourkeyfile, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(licnew);
                        writer.Close();
                    }
                }
                textBox2.Text = "hostlist -r < " + ourkeyfile;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
