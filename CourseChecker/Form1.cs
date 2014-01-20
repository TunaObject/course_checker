//Made by Abdul Hage-ali
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace SwagBot3000
{
    public partial class Form1 : Form
    {
        string courseCode;
        string studentID;
        public Form1()
        {
            InitializeComponent();
        }

        private void LogWrite(string message)
        {
            string formatted = DateTime.UtcNow.ToShortTimeString() + " " + message + Environment.NewLine;
            if (txtLog.MaxLength <= (txtLog.Text + formatted).Length)
            {
                txtLog.Text = "";
            }
            txtLog.AppendText(formatted);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            courseCode = txtCourseCode.Text;
            studentID = txtStudentId.Text;
            label1.Hide();
            label2.Hide();
            label3.Hide();
            txtStudentId.Hide();
            txtCourseCode.Hide();
            txtEmail.Hide();
            webBrowser1.Hide();
            button1.Hide();
            txtLog.Show();
            LogWrite("--- BOT ACTIVATED ---");
            webBrowser1.Navigate("https://quest.pecs.uwaterloo.ca/psc/SS/ACADEMIC/SA/c/SA_LEARNER_SERVICES.SSR_SSENRL_CART.GBL?Page=SSR_SSENRL_CART&Action=A&ACAD_CAREER=UG&EMPLID=" + studentID + "&ENRL_REQUEST_ID=&INSTITUTION=UWATR&STRM=1141");
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            String code = webBrowser1.DocumentText;
            int index = code.IndexOf(courseCode);

            if (index < 0)
            {
                LogWrite("ERROR - Couldn't find " + courseCode + " on page - try to restart the application and check your information or make sure you're on the correct page!");
            }
            else
            {
                code = code.Substring(index);
                index = code.IndexOf("STATUS_CLOSED");
                if (index < 0)
                {
                        //Somehow communicate that you found it
                        Send_Success_Mail();
                        MessageBox.Show("THE STATUS IS OPEN; DO IT NOW!!!");
                }
                else
                {
                    LogWrite("STILL CLOSED - RETRYING");
                    //REFRESH
                    webBrowser1.Navigate("https://quest.pecs.uwaterloo.ca/psc/SS/ACADEMIC/SA/c/SA_LEARNER_SERVICES.SSR_SSENRL_CART.GBL?Page=SSR_SSENRL_CART&Action=A&ACAD_CAREER=UG&EMPLID=" + studentID + "&ENRL_REQUEST_ID=&INSTITUTION=UWATR&STRM=1141");
                    //WAIT 3 SECONDS
                    timer1.Start();
                    //TRY AGAIN
                }
            }
        }

        private void Send_Success_Mail()
        {
            var fromAddress = new MailAddress("swagbot30001@gmail.com", "COURSE CHECKER"); //this account will probably get stolen so you might want to put in your own gmail information and not upload it anywhere
            var toAddress = new MailAddress(txtEmail.Text, "To Whomever");
            const string fromPassword = "swagbottemp";
            const string body = "NOW OPEN!!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = courseCode,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch (Exception e)
                {
                    LogWrite("EMAIL -- COULDN'T SEND");
                    LogWrite(e.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("HOW TO USE:" + Environment.NewLine + "This program is for the University of Waterloo's Quest website. Add the course to your shopping cart in enroll and go to the page Enroll -> Add. Then type in the course code and your student ID and click activate!");
        }
    }
}
