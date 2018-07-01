using System;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

public class JobLogger
{
    public static void LogAMessage(bool LogToFile, bool LogToConsole, bool LogToDatabase, bool LogMessage, bool LogWarning, bool LogError, string message, bool isMessage, bool isWarning, bool isError)
    {
        //Check if message has content
        if (message == null || message.Trim().Length == 0) return;

        //Check configuration is OK
        if (!LogToConsole && !LogToFile && !LogToDatabase)
        {
            throw new Exception("Invalid configuration");
        }

        if ((!LogError && !LogMessage && !LogWarning) || (!isMessage && !isWarning && !isError))
        {
            throw new Exception("Error or Warning or Message must be specified");
        }

        //If user chose to Log To Database
        if (LogToDatabase)
        {
            //Select which type is the message
            string typeOfMessage = "";
            if (isMessage && LogMessage)
            {
                typeOfMessage = "1";
            }
            if (isError && LogError)
            {
                typeOfMessage = "2";
            }
            if (isWarning && LogWarning)
            {
                typeOfMessage = "3";
            }

            //Connect and insert to DB
            try
            {
                using (var connection = new SqlConnection(@"server = 192.168.1.5,1433; database = master; uid = sa; password = PASSW0R$"))
                {
                    SqlDataAdapter cmd = new SqlDataAdapter();
                    using (var insertCommand = new SqlCommand("Insert into Log Values('" + message + "' , " + typeOfMessage + ")"))
                    {
                        insertCommand.Connection = connection;
                        cmd.InsertCommand = insertCommand;
                        connection.Open();
                        cmd.InsertCommand.ExecuteNonQuery();
                    }
                }
            }catch (Exception ex){
                throw new Exception("Log to database failed. " + ex.Message);
            }
        }

        //If user chose to Log to File
        if (LogToFile)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var file = System.IO.Path.Combine(documents, "LogFile" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                string fileText = "";

                //If file already exits, add the new message
                if (File.Exists(file))
                {
                    fileText = File.ReadAllText(file) + DateTime.Now.ToString("yyyyMMdd") + message;
                    //If file doesn't exists, create it with the message.
                }
                else
                {
                    fileText = DateTime.Now.ToString("yyyyMMdd") + message;
                }

                File.WriteAllText(file, fileText);
            }
            catch (Exception ex)
            {
                throw new Exception("Log to file failed. " + ex.Message);
            }
        }

        //If user chose to log to console
        if (LogToConsole)
        {

            //Change the color of the console depending on type of message
            if (isError && LogError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (isWarning && LogWarning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (isMessage && LogMessage)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            //Write on console
            Console.WriteLine(DateTime.Now.ToString("yyyyMMdd") + message);
        }
    }
}