/*
 *  Copyright (c) 2015-2016 Vitaleks
 *  Copyright (c) 2015 Edmundas919
 *  Copyright (c) 2015 mc3Dcm
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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using MySql.Data.MySqlClient;

namespace FinalServerSolution
{
    class Program
    {
        private static bool debug = true;

        private static string protocol = "http";
        private static string prefix = "";

        private static string serverIP = "";
        private static string port = "";
 
        public static string serverPrefix = "nfsw";

        private static bool isListening = false;
        private static HttpListener listener;

        static void Main(string[] args)
        {           
            prefix = String.Format("{0}://{1}:{2}/", protocol, serverIP, port);

            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            Console.WriteLine("Server runned {0}", prefix);
            Console.WriteLine("============");

            isListening = true;

            while (isListening)
            {
                if (debug)
                {
                    Console.WriteLine();
                    Console.WriteLine("Main logical point");
                    Console.WriteLine("=====");
                }
                try
                {
                    ThreadPool.QueueUserWorkItem(ProcessRequest, listener.GetContext());
                }
                catch
                {

                }
            }

            isListening = false;
            listener.Stop();
            listener.Close();

            MsSQL.Instance.CloseConnection();
        }

        public static void ProcessRequest(object contextTemp)
        {
            MySqlConnection connection = new MySqlConnection(MsSQL.connetionString);
            connection.Open();

            HttpListenerRequest request;
            HttpListenerResponse response;

            string path = "";
            bool isGzip = true;

            string requestPacket = "";
            string responsePacket = "";

            bool isAuthorizated = false;
            bool isGoodResponse = false;

            string session = "";
            int userId = 0;

            Dictionary<string, string> arguments = new Dictionary<string, string>();

            HttpListenerContext context = contextTemp as HttpListenerContext;

            request = context.Request;
            response = context.Response;
            
            //Read arguments
            {
                string[] pathTemp = request.RawUrl.Replace("/" + serverPrefix + "/", "").Replace("Engine.svc/", "").Split('?');
                path = pathTemp[0];

                if (pathTemp.Length > 1)
                {
                    string[] argumentsTemp = pathTemp[1].Split('&');
                    foreach (string argument in argumentsTemp)
                    {
                        try
                        {
                            string[] temp = argument.Split('=');
                            arguments.Add(temp[0], temp[1].Replace("'",""));
                        }
                        catch
                        {

                        }
                    }
                }
            }

            //Get post or put
            if (request.HttpMethod == "POST" || request.HttpMethod == "PUT")
            {
                int lenght = 0;

                foreach (string key in request.Headers.AllKeys)
                {
                    if (key == "Content-Length")
                        lenght = Convert.ToInt32(request.Headers.GetValues(key)[0]);
                }
				
				Console.WriteLine("Length: {0}", lenght);

                if (lenght > 0)
                {
                    //char[] requestBuffer = new char[lenght];
                    //string x = "";
                    using (var reader = new StreamReader(request.InputStream,
                             request.ContentEncoding))
                    {
                        requestPacket = reader.ReadToEnd();

                        //Thread.Sleep(100);
                        //try {
                            //reader.Read(requestBuffer, 0, lenght);
                        //} catch
                        //{

                        //}
                    }

					//Console.WriteLine("ReadLength: {0}", requestBuffer.Length);

                    //Console.WriteLine("Pack: {0}", new String(requestBuffer));

                    //requestPacket = x; //new String(requestBuffer).Replace("'", "");
                }


                //File.WriteAllText(path.Replace("/", ""), requestPacket);
            }


            //Check authorization
            foreach (string key in request.Headers.AllKeys)
            {
                if (key == "securityToken")
                    session = request.Headers.GetValues(key)[0];
                if (key == "userId")
                    userId = Convert.ToInt32(request.Headers.GetValues(key)[0]);
            }
            if (Authorization.IsAuthorizated(userId, session, connection))
                isAuthorizated = true;

            //-------------------------------------------
            if (debug)
            {
                Console.WriteLine("USER: {0}; IP: {1}", userId, request.RemoteEndPoint);

                Console.WriteLine("Method: {0}; Access point: {1}", request.HttpMethod, request.RawUrl);

                Console.WriteLine("Packet: {0}", requestPacket);
            }
            if (path == "Login" || path == "Status" || path == "Continue" || path.Contains("NewsImages") || path == "debugger")
                isAuthorizated = true;

            if (path == "debugger")
                debug = !debug;

            foreach (KeyValuePair<string, string> pair in arguments)
            {
                if (debug)
                {
                    Console.WriteLine(" {0} {1}", pair.Key, pair.Value);
                }
            }
            //isAuthorizated = true;
            //Link logic
            if (isAuthorizated)
            {
                if (path.Contains("personas/"))
                {
                    string[] temp = path.Split('/');
                    arguments.Add("personaId", temp[1]);
                    switch (temp[2])
                    {
                        case "carslots": path = "personas/carslots"; break;
                        case "cars":
                            if (request.HttpMethod == "PUT")
                                path = "personas/commerceUpdate";
                            if (request.HttpMethod == "POST")
                                path = "personas/sellCar";
                            if (request.HttpMethod == "GET")
                                path = "personas/getMultiplayerPersonaCars";
                            break;

                        case "defaultcar":
                            if (temp.Length == 4)
                            {
                                path = "personas/setdefaultcar";
                                arguments.Add("carId", temp[3]);
                                break;
                            }
                            else
                            {
                                path = "personas/getdefaultcar";
                                break;
                            }
                        case "commerce": path = "personas/commerce"; break;
                        case "baskets": path = "personas/baskets"; break;
                        default:
                            break;
                    }

                }

                if (path.Contains("matchmaking/launchevent/"))
                {
                    string[] temp = path.Split('/');
                    arguments.Add("raceId", temp[2]);
                    path = "matchmaking/launchevent";
                }

                switch (path)
                {
                    case "Status": Server.IsOnline(out responsePacket, out isGoodResponse); isGzip = false; break;
                    case "Continue": Server.ContinueSession(requestPacket, connection, out responsePacket, out isGoodResponse); isGzip = false; break;
                    
                    case "Login": Authorization.Login(requestPacket, connection, out responsePacket, out isGoodResponse); isGzip = false; break;
                    case "User/GetPermanentSession": Authorization.GetPermanentSession(userId, session, connection, out responsePacket, out isGoodResponse); break;

                    case "getusersettings": Settings.userSettings(userId, connection, out responsePacket, out isGoodResponse); break;
                    case "systeminfo": Settings.systemInfo(connection, out responsePacket, out isGoodResponse); break;
                    case "security/fraudConfig": Settings.fraudConfig(userId, connection, out responsePacket, out isGoodResponse); break;

                    case "DriverPersona/GetPersonaInfo": Persona.getpersonainfo(Convert.ToInt32(arguments["personaId"]), connection, out responsePacket, out isGoodResponse); break;
                    case "DriverPersona/GetPersonaBaseFromList": Persona.getPersonaBaseFromList(requestPacket, connection, out responsePacket, out isGoodResponse); break;
                    case "DriverPersona/GetPersonaPresenceByName": Persona.getPersonaPresenceByName(arguments["displayName"], connection, out responsePacket, out isGoodResponse); break;

                    case "personas/carslots": Cars.getPersonasCars(Convert.ToInt32(arguments["personaId"]), connection, out responsePacket, out isGoodResponse); break;
                    case "personas/commerce": Cars.updatePersonaCar(Convert.ToInt32(arguments["personaId"]), userId, requestPacket, connection, out responsePacket, out isGoodResponse); break;
                    case "personas/commerceUpdate": Cars.updatePersonaCar_car(Convert.ToInt32(arguments["personaId"]), userId, requestPacket, connection, out responsePacket, out isGoodResponse); break;

                    case "personas/setdefaultcar": Cars.updatePersonaDefaultCar(Convert.ToInt32(arguments["carId"]), Convert.ToInt32(arguments["personaId"]), userId, connection, out responsePacket, out isGoodResponse); break;
                    case "personas/getdefaultcar": Cars.getPersonaDefaultCar(Convert.ToInt32(arguments["personaId"]), userId, connection, out responsePacket, out isGoodResponse); break;
                    
                    case "personas/baskets": Cars.buyCar(requestPacket, Convert.ToInt32(arguments["personaId"]), userId, connection, out responsePacket, out isGoodResponse); break;
                    case "personas/sellCar": Cars.sellCar(Convert.ToInt32(arguments["serialNumber"]), Convert.ToInt32(arguments["personaId"]), userId, connection, out responsePacket, out isGoodResponse); break;

                    case "personas/getMultiplayerPersonaCars": Cars.getOtherPersonasCars(Convert.ToInt32(arguments["personaId"]), connection, out responsePacket, out isGoodResponse); break;

                    case "car/repair": Cars.repair(userId, Convert.ToInt32(arguments["personaId"]), connection, out responsePacket, out isGoodResponse); break;

                    case "matchmaking/launchevent": Tracks.singleRace(Convert.ToInt32(arguments["raceId"]), connection, out responsePacket, out isGoodResponse); break;
                    
                    case "event/arbitration":
                    case "event/bust": 
                    case "event/abort": Tracks.completedRace(requestPacket, userId, connection, out responsePacket, out isGoodResponse); break;

                    case "getfriendlistfromuserid": Friends.getFriendList(userId, connection, out responsePacket, out isGoodResponse); break;
                    case "addfriendrequest": Friends.addFriend(Convert.ToInt32(arguments["personaId"]), userId, arguments["displayName"], connection, out responsePacket, out isGoodResponse); break;
                    case "removefriend": Friends.removeFriend(Convert.ToInt32(arguments["personaId"]), userId, Convert.ToInt32(arguments["friendPersonaId"]), connection, out responsePacket, out isGoodResponse); break;
                    
                    /////////////////////
                    default:


                        if (path.Contains("catalog/productsInCategory"))
                        {
                            path = "catalog/" + arguments["categoryName"];
                        }

                        if (path.Contains("catalog/categories"))
                        {
                            path = "catalog/" + arguments["categoryName"];
                        }

                        string pathRead = serverPrefix + "/Engine.svc/" + path;

                        if (!File.Exists(pathRead + ".xml") && !File.Exists(pathRead))
                            pathRead = serverPrefix + "/Engine.svc/empty";

                        if (File.Exists(pathRead + ".xml"))
                            responsePacket = File.ReadAllText(pathRead + ".xml");
                        else
                            responsePacket = File.ReadAllText(pathRead);

                        path = pathRead;

                        //Console.WriteLine(pathRead);

                        isGoodResponse = true;

                        break;
                }
            }
            else
            {
                responsePacket = "Unbelievable! U hacked my server :'( Amazing man, amazing!!! :D";
                path = "No authorization message";
                //isGzip = false;
            
                //no authorization
            }
            if(debug)
            Console.WriteLine("Final path: {0}", path);

            //isGzip = false;

            //Packet send
            byte[] buffer = Encoding.UTF8.GetBytes(responsePacket);

            //ImageFix
            if (request.RawUrl.Contains(".jpg"))
            {
                buffer = File.ReadAllBytes(path);
                isGzip = false;
            }

            if (isGzip)
            {
                using (var ms = new MemoryStream())
                {
                    using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                        zip.Write(buffer, 0, buffer.Length);
                    buffer = ms.ToArray();
                }
                response.AddHeader("Content-Encoding", "gzip");
            }

            if (request.RawUrl.Contains(".jpg")) { response.AddHeader("ContentType", "image/jpeg"); }

            if (isAuthorizated && isGoodResponse)
                response.StatusCode = (int)HttpStatusCode.OK;
            else
                response.StatusCode = (int)HttpStatusCode.Continue;

            response.KeepAlive = false;

            try {
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            } catch
            {
                response.OutputStream.Close();
            }

            connection.Close();

            if(debug)
            Console.WriteLine("Response end, user: {0}; path: {1}", userId, path);
        }
    }
}
