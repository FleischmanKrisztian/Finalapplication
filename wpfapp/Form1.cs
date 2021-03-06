﻿using ChoETL;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Xceed.Words.NET;

namespace wpfapp
{
    public partial class Form1 : Form
    {
        private string filename;
        private string value;
        public Form1()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            //args[0] is always the path to the application
            RegisterMyProtocol(args[0]);
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            //^the method posted before, that edits registry
            additionalInfo1.Hide();
            panel2.Hide();
            panel1.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream;
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    string strfilename = openFileDialog1.FileName;
                    richTextBox1.Text = strfilename;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Dictionary<string, string> keyValuePairs = CSVExporter.StringtoCsv.Methods.Xmldecoder();
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                    if (richTextBox1.Text.Contains("ContractBeneficiar") == true || richTextBox1.Text.Contains("Contract_cadru_asistati_Fundatie") == true || richTextBox1.Text.Contains("beneficiar") == true || richTextBox1.Text.Contains("Beneficiar") == true)
                    {
                        string[] args = Environment.GetCommandLineArgs();
                        RegisterMyProtocol(args[0]);
                        var doc = DocX.Load(richTextBox1.Text);
                        HttpClient httpClient = new HttpClient();
                        args[1] = args[1].Remove(0, 16);
                        string url = keyValuePairs.GetValue("Beneficiarylink") + args[1];
                        var result = httpClient.GetStringAsync(url).Result.Normalize();
                        result = result.Replace("[", "");
                        result = result.Replace("]", "");
                        beneficiarycontract volc = new beneficiarycontract();
                        volc = JsonConvert.DeserializeObject<beneficiarycontract>(result);
                        try
                        {
                            string phrase = volc.Address;
                            string[] words = phrase.Split(',');
                        }
                        catch { }
                        doc.ReplaceText("<nrreg>", volc.NumberOfRegistration);
                        doc.ReplaceText("<todaydate>", volc.RegistrationDate.ToShortDateString());
                        doc.ReplaceText("<Fullname>", volc.Fullname);
                        if (volc.CNP != null)
                            doc.ReplaceText("<CNP>", volc.CNP);
                        if (volc.CIinfo != null)
                            doc.ReplaceText("<CiInfo>", volc.CIinfo);
                        if (volc.Address != null)
                            doc.ReplaceText("<Address>", volc.Address);
                        if (volc.Nrtel != null)
                            doc.ReplaceText("<tel>", volc.Nrtel);
                        if (volc.IdApplication != null)
                            doc.ReplaceText("<IdAplication>", volc.IdApplication);
                        if (volc.IdInvestigation != null)
                            doc.ReplaceText("<IdInvestigation>", volc.IdInvestigation);

                        
                        volc.myOption = value;
                        

                        doc.ReplaceText("<option>", volc.myOption);
                        doc.ReplaceText("<NumberOfPortions>", volc.NumberOfPortion);
                        doc.ReplaceText("<RegistrationDate> ", volc.RegistrationDate.ToShortDateString());
                        doc.ReplaceText("<ExpirationDate>", volc.ExpirationDate.ToShortDateString());
                        if (saveFileDialog1.FileName.Contains(".docx") == true)
                        {
                            doc.SaveAs(saveFileDialog1.FileName);
                        }
                        else
                        {
                            doc.SaveAs(saveFileDialog1.FileName + "." + "docx");
                            saveFileDialog1.FileName = saveFileDialog1.FileName + ".docx";
                        }

                        filename = saveFileDialog1.FileName;
                        richTextBox2.Text = saveFileDialog1.FileName;
                        richTextBox3.Text = "File Saved succesfully";
                    }
                    else
                    {
                        string[] args = Environment.GetCommandLineArgs();
                        RegisterMyProtocol(args[0]);
                        var doc = DocX.Load(richTextBox1.Text);
                        HttpClient httpClient = new HttpClient();
                        args[1] = args[1].Remove(0, 16);
                        string url = keyValuePairs.GetValue("Volunteerlink") + args[1];
                        var result = httpClient.GetStringAsync(url).Result.Normalize();
                        result = result.Replace("[", "");
                        result = result.Replace("]", "");
                        volcontract volc = new volcontract();
                        volc = JsonConvert.DeserializeObject<volcontract>(result);
                        if (volc.Address != null)
                            doc.ReplaceText("<Address>", volc.Address);
                        doc.ReplaceText("<nrreg>", volc.NumberOfRegistration);
                        doc.ReplaceText("<todaydate>", volc.RegistrationDate.ToShortDateString());
                        doc.ReplaceText("<Fullname>", volc.Fullname);
                        if (volc.CNP != null)
                            doc.ReplaceText("<CNP>", volc.CNP);
                        if (volc.CIseria != null)
                            doc.ReplaceText("<Seria>", volc.CIseria);
                        if (volc.CINr != null)
                            doc.ReplaceText("<Nr>", volc.CINr);
                        if (volc.CIEliberat != null)
                            doc.ReplaceText("<eliberat>", volc.CIEliberat.ToShortDateString());
                        if (volc.CIeliberator != null)
                            doc.ReplaceText("<eliberator>", volc.CIeliberator);
                        if (volc.Nrtel != null)
                            doc.ReplaceText("<tel>", volc.Nrtel);
                        doc.ReplaceText("<startdate>", volc.RegistrationDate.ToShortDateString());
                        doc.ReplaceText("<finishdate>", volc.ExpirationDate.ToShortDateString());
                        doc.ReplaceText("<hourcount>", volc.HourCount.ToString());
                        if (saveFileDialog1.FileName.Contains(".docx") == true)
                        {
                            doc.SaveAs(saveFileDialog1.FileName);
                        }
                        else
                        {
                            doc.SaveAs(saveFileDialog1.FileName + "." + "docx");
                            saveFileDialog1.FileName = saveFileDialog1.FileName + ".docx";
                        }
                        filename = saveFileDialog1.FileName;
                        richTextBox2.Text = saveFileDialog1.FileName;
                        richTextBox3.Text = "File Saved succesfully";
                    }
                }
            }
            catch
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                richTextBox2.Text = saveFileDialog1.FileName;
                richTextBox3.Text = "an error has occured";
            }

            PanelVisibility(richTextBox3.Text);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private static void RegisterMyProtocol(string ContractPrinterPath)  //myAppPath = full path to your application
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey("ContractPrinter");  //open myApp protocol's subkey

            if (key == null)  //if the protocol is not registered yet...we register it
            {
                key = Registry.ClassesRoot.CreateSubKey("ContractPrinter");
                key.SetValue(string.Empty, "URL:ContractPrinter Protocol");
                key.SetValue("URL Protocol", string.Empty);
                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, ContractPrinterPath + " " + "%1");
                //%1 represents the argument - this tells windows to open this program with an argument / parameter
            }

            key.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Contains("ContractBeneficiar") == true || richTextBox1.Text.Contains("Contract_cadru_asistati_Fundatie") == true || richTextBox1.Text.Contains("beneficiar") == true || richTextBox1.Text.Contains("Beneficiar") == true)
            {
                panel1.Show();
                additionalInfo1.Show();
            }
           
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

       

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Contains("ContractBeneficiar") == true || richTextBox1.Text.Contains("Contract_cadru_asistati_Fundatie") == true || richTextBox1.Text.Contains("beneficiar") == true || richTextBox1.Text.Contains("Beneficiar") == true)
            {
                additionalInfo1.Show();
                panel1.Show();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(filename);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string my_directory = Path.GetDirectoryName(filename);
            Process.Start(my_directory);
        }

        private void PanelVisibility(string message)
        {
            if (message.Contains("succesfully") == true)
            {
                panel2.Show();
            }
            else
            { panel2.Hide(); }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void additionalInfo1_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
           
            value = additionalInfo1.MyVal;
            additionalInfo1.Hide();
            panel1.Hide();
        }
    }
}