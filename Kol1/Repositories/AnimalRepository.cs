using System.Data.SqlClient;
using Kol1.Models;
using Kol1.Models.DTOs;

namespace Kol1.Repositories;

public class AnimalRepository : IAnimalRepository
{
    
    private readonly IConfiguration _configuration;

    public AnimalRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool AnimalExists(int AnimalId)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                
                command.Parameters.AddWithValue("@AnimalId", AnimalId);
               

                command.CommandText =
                    "SELECT COUNT(*) FROM Animal WHERE Id = @AnimalId";
                
                Console.WriteLine((int)command.ExecuteScalar());
               
                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
    
    public InfoAnimal GetAnimalInfo(int AnimalId)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;

                command.Parameters.AddWithValue("@AnimalId", AnimalId);

                command.CommandText = "SELECT P.name, P.description, AP.date" +
                                      " FROM Procedure_Animal AP" +
                                      " JOIN [Procedure] P ON AP.Procedure_Id = P.id" +
                                      " WHERE AP.Animal_id = @AnimalId;";
                
                
                var reader = command.ExecuteReader();

                var listOfProcedure = new List<Procedure>();
                
                while (reader.Read())
                {
                    listOfProcedure.Add(new Procedure()
                    {
                        name = reader["Name"].ToString(),
                        description = reader["Description"].ToString(),
                        date =  DateTime.Parse(reader["Date"].ToString())
                        
                    });
                }
                
                reader.Close();

                command.CommandText = "SELECT O.Id, O.firstName, O.lastName" +
                                      " FROM Animal A" +
                                      " JOIN Owner O ON O.ID = A.Owner_ID" +
                                      " WHERE A.Id = @AnimalId";
                

                reader = command.ExecuteReader();

                Owner owner_ = null;
                
                while (reader.Read())
                {
                    owner_ = new Owner()
                    {
                      id = (int)reader["Id"],
                      firstName = reader["FirstName"].ToString(),
                      lastName = reader["LastName"].ToString()
                    };
                }
                
                command.CommandText = "SELECT ID, Name, Type, AdmissionDate" +
                                      " FROM Animal " +
                                      " WHERE Id = @AnimalId";

                reader.Close();
                reader = command.ExecuteReader();
                

                InfoAnimal infoAnimal = null;
                
                while (reader.Read())
                {
                    infoAnimal = new InfoAnimal()
                    {
                        id = (int)reader["Id"],
                        name = reader["Name"].ToString(),
                        type = reader["Type"].ToString(),
                        admissionDate = DateTime.Parse(reader["AdmissionDate"].ToString()),
                        owner = owner_,
                        procedures = listOfProcedure
                        
                    };
                }
                
                
                
                
                

                return infoAnimal;
            }
        }
    }

    public void AddAnimal(AddAnimal animal)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {



                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.Transaction = transaction;


                    command.Parameters.AddWithValue("@animalName", animal.name);
                    command.Parameters.AddWithValue("@type", animal.type);
                    command.Parameters.AddWithValue("@admissionDate", animal.admissionDate);
                    command.Parameters.AddWithValue("@ownerId", animal.ownerId);

                    command.CommandText = "INSERT INTO Animal VALUES(@animalName, @type, @admissionDate, @ownerId);";

                    command.ExecuteNonQuery();

                    command.CommandText = "SELECT MAX(ID) FROM Animal";

                    int Id = 0;
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        Id = Convert.ToInt32(result);
                    }




                    Console.WriteLine(Id);



                    var listOfProcedure = animal.procedures;

                    command.Parameters.AddWithValue("@animalId", Id);
                    command.Parameters.AddWithValue("@procedureId", null);
                    command.Parameters.AddWithValue("@date", null);
                    foreach (var procedure in listOfProcedure)
                    {
                        command.Parameters["@procedureId"].Value = procedure.procedureId;
                        command.Parameters["@date"].Value = procedure.date;


                        command.CommandText = "INSERT INTO Procedure_Animal VALUES(@procedureId, @animalId ,@date);";
                        command.ExecuteNonQuery();
                    }





                    transaction.Commit();



                }
            } catch (Exception e)
            {
                Console.WriteLine("Transaction failed. Rolling back.");
                transaction.Rollback();
            }

        }
    }
    
}