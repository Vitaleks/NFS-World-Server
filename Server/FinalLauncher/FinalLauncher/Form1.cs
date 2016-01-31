/*
 *  Copyright (c) 2015-2016 Vitaleks
 *  Copyright (c) 2015 Edmundas919
 *  Copyright (c) 2015 mc3dcm
 * 
 *  This file is part of NFS World Server.
 *
 *  NFS World Server is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  NFS World Server is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with NFS World Server.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace FinalLauncher
{
    public partial class Form1 : Form
    {
        private bool isLoginWrite = false;
        private bool isPasswordWrite = false;

        private delegate void ServerStatus(bool status);
        private ServerStatus deletageServerStatus;

        private Thread serverCheckerThread;

        private bool isAuthorizated = false;

        private string ip = ":";

        private bool isClosed = false;

        private string userId = "";
        private string session = "";

        ModifyRegistry registry = new ModifyRegistry();

        private string gameExe = "";

        public Form1()
        {
            InitializeComponent();
            deletageServerStatus = new ServerStatus(ServerStatusMethod);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                registry.SubKey = "SOFTWARE\\Vitaleks\\Launcher";
                gameExe = registry.Read("path");
            }
            catch { }

            serverCheckerThread = new Thread(serverChecker);
            serverCheckerThread.IsBackground = false;
            serverCheckerThread.Start();
        }

        private void ServerStatusMethod(bool status)
        {
            if (status)
                pictureBox2.Image = Properties.Resources.logo;
            else
                pictureBox2.Image = Properties.Resources.logooff;
        }

        private void serverChecker()
        {
            while (!isClosed)
            {
                if (isAuthorizated)
                    continueServerSession();
                else
                    checkStatusServer();

                Thread.Sleep(60000);
            }
        }

        private void checkStatusServer()
        {
            HttpWebRequest request;
            HttpWebResponse response;

            Uri newUri = new Uri("http://" + ip + "/quadcopter_vision_base/Status");
            request = (HttpWebRequest)WebRequest.Create(newUri);

            request.Method = "GET";
            request.ContentType = "text/xml;charset=utf-8";

            string answer = "";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(stream, Encoding.UTF8);
                answer = readStream.ReadToEnd();
            }
            catch
            {
                answer = "<status>OFFLINE</status>";
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(answer);

            string status = document.ChildNodes[0].InnerText;

            bool serverStatus = false;

            if (status == "ONLINE")
                serverStatus = true;

            Invoke(deletageServerStatus, serverStatus);
        }

        private void continueServerSession()
        {
            HttpWebRequest request;
            HttpWebResponse response;

            Uri newUri = new Uri("http://" + ip + "/nfsw/Continue");
            request = (HttpWebRequest)WebRequest.Create(newUri);

            string packet = String.Format("<Continue><Id>{0}</Id><Session>{1}</Session></Continue>", userId, session);

            byte[] buffer = Encoding.UTF8.GetBytes(packet);
            
            request.Method = "POST";
            request.ContentLength = buffer.Length;
            request.ContentType = "text/xml;charset=utf-8";
            string answer = "";

            try
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(stream, Encoding.UTF8);
                answer = readStream.ReadToEnd();
            }
            catch
            {
                answer = "<status>OFFLINE</status>";
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(answer);

            string status = document.ChildNodes[0].InnerText;

            bool serverStatus = false;

            if (status == "ONLINE")
                serverStatus = true;

            Invoke(deletageServerStatus, serverStatus);
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (!isLoginWrite)
                textBox1.Text = "";
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                isLoginWrite = false;

            if (!isLoginWrite)
                textBox1.Text = "Login";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            isLoginWrite = true;
        }


        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (!isPasswordWrite)
            {
                textBox2.Text = "";
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
                isPasswordWrite = false;

            if (!isPasswordWrite)
            {
                textBox2.Text = "Password";
                textBox2.UseSystemPasswordChar = false;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            isPasswordWrite = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpWebRequest request;
            HttpWebResponse response;

            userId = "";
            session = "";
            string message = "";
            Boolean isError = false;

            string packet = String.Format("<Login><Email>{0}</Email><Password>{1}</Password></Login>", textBox1.Text, textBox2.Text);
            byte[] buffer = Encoding.UTF8.GetBytes(packet);

            Uri newUri = new Uri("http://" + ip + "/nfsw/Login");
            request = (HttpWebRequest)WebRequest.Create(newUri);

            request.Method = "POST";
            request.ContentLength = buffer.Length;
            request.ContentType = "text/xml;charset=utf-8";

            try
            {
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();
         
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(stream, Encoding.UTF8);
                string answer = readStream.ReadToEnd();

                //answer = "<success><ID>123</ID><session>dsafsdfsdfdgfdgdfg4546</session></success>";
                //answer = "<fail><message>BANNED</message></fail>";

                XmlDocument document = new XmlDocument();
                document.LoadXml(answer);

                if (document.FirstChild.Name == "success")
                {
                    foreach (XmlNode node in document.FirstChild.ChildNodes)
                    {
                        if (node.Name == "ID") userId = node.InnerText;
                        if (node.Name == "session") session = node.InnerText;
                    }
                }
                else if (document.FirstChild.Name == "fail")
                {
                    foreach (XmlNode node in document.FirstChild.ChildNodes)
                    {
                        if (node.Name == "message") message = node.InnerText;
                    }
                }
                else
                {
                    message = "Server communication error";
                }

                if (message != "")
                    isError = true;
            }
            catch
            {
                isError = true;
                message = "No connection with server";
            }

            if (isError == true)
            {
                MessageBox.Show(message);
                isAuthorizated = false;
            }
            else
            {
                isAuthorizated = true;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = gameExe;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "SS http://" + ip + "/nfsw/Engine.svc " + session + " " + userId;
                Process.Start(startInfo);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                gameExe = openFileDialog1.FileName;
                registry.SubKey = "SOFTWARE\\Vitaleks\\Launcher";
                registry.Write("path", gameExe);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serverCheckerThread.Abort();
        }

    }
}
