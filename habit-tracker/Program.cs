using System.Globalization;
using Microsoft.Data.Sqlite;
using SQLitePCL;

internal class Program
{
    static string connectionString = @"Data Source=habit-Tracker.db";
    private static void Main(string[] args)
    {
        

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS drinking_water(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT,
            Quantity INTEGER
        )";
            tableCmd.ExecuteNonQuery();

            connection.Close();
        }
        GetData();

        static void GetData()
        {
            Console.Clear();
            bool closeApplication = false;
            while (closeApplication == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to close application");
                Console.WriteLine("\nType 1 to view all records");
                Console.WriteLine("\nType 2 to insert record");
                Console.WriteLine("\nType 3 to delete record");
                Console.WriteLine("\nType 4 to update record");
                Console.WriteLine("\n---------------------------------------\n");

                int command = Convert.ToInt32(Console.ReadLine());

                switch (command)
                {
                    case 0:
                        Console.WriteLine("\nGoodbye!\n");
                        closeApplication = true;
                        Environment.Exit(0);
                        break;
                    case 1:
                        GetAllRecords();
                        break;    
                    case 2:
                        Insert();
                        break;
                    case 3:
                        Delete();
                        break; 
                    case 4:
                        Update();
                        break;          
                    default:
                        Console.WriteLine("\nInvalid command. Plase type a number between 0 and 4\n");
                        break;
                }
            }
        }
        static void GetAllRecords()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @"
                SELECT * FROM drinking_water";
                List<DrinkingWater> dwList = new();
                SqliteDataReader reader = tableCmd.ExecuteReader();

                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        dwList.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1),"dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None),
                                Quantity = reader.GetInt32(2)
                            }
                        ); ;
                    }
                } 
                else
                {
                    Console.WriteLine("no rows find");
                }

                connection.Close();
                Console.WriteLine("\n-------------------------------------\n");
                foreach(var dw in dwList)
                {
                    Console.WriteLine($"Id:{dw.Id}, Date:{dw.Date.ToString()}, Quantity:{dw.Quantity}");
                }
                Console.WriteLine("\n-------------------------------------\n");
            }      
        }
        static void Insert()
        {
            int quantity = GetQuantity();
            string date = GetDate();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"
                    INSERT INTO drinking_water(date,quantity) VALUES('{date}',{quantity})";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }
        static void Delete()
        {
            Console.Clear();
            GetAllRecords();
            Console.WriteLine("What id you want to delete?\n");
            int recordId= Convert.ToInt32(Console.ReadLine());
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"
                    DELETE FROM drinking_water WHERE Id = '{recordId}'";
                int row  = tableCmd.ExecuteNonQuery();
                if(row == 0)
                {
                    Console.WriteLine("No more rows");
                    Delete();
                }
                Console.WriteLine($"The record with id:{recordId} was deleted");
                connection.Close();
            }
        }
        static void Update()
        {
         Console.Clear();
         GetAllRecords();   
         Console.WriteLine("What id you want to update?\n");
         int recordId = Convert.ToInt32(Console.ReadLine());

         using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = @$"
                    SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if(checkQuery == 0){
                    Console.WriteLine("There is no such id");
                    connection.Close();
                    Update();
                }

                int quantity = GetQuantity();
                string date = GetDate();

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @$"
                    UPDATE drinking_water SET date='{date}',quantity={quantity} WHERE Id={recordId}";
                int row  = tableCmd.ExecuteNonQuery();
                
                connection.Close();
            }

        }
        static int GetQuantity()
        {
            Console.WriteLine("\nPlease insert number of glass of water(no decimal allowed) or type 0 to return to main menu\n");
            string? choice = Console.ReadLine();
            while(!Int32.TryParse(choice,out _) || Convert.ToInt32(choice) < 0)
            {
                Console.WriteLine("Invalid input. Please insert a number");
                choice = Console.ReadLine();
            }
            if (choice == "0") GetData();
            int quantity = Convert.ToInt32(choice);
            return quantity;
        }

        static string GetDate()
        {
            Console.WriteLine("\nPlese insert date in (dd-mm-yy) format or type 0 to return to main menu\n");
            string? date = Console.ReadLine();
            while(!DateTime.TryParseExact(date,"dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("Invalid input. Please insert correct date");
                date = Console.ReadLine();
            }
            return date;
        }
    }
}
public class DrinkingWater
{
    public int Id {get;set;}
    public DateTime Date{get;set;}
    public int Quantity{get;set;}
}