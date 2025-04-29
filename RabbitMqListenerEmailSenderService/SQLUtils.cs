using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMqListenerEmailSenderService
{
    public class SQLUtils
    {
        private static readonly string _postgreConnectionString = DatabaseParameters.GetConnectionString("PostgreSqlConnection");
        //private static readonly string _sqlConnectionString = DatabaseParameters.GetConnectionString("MsSqlConnection");

        private static readonly string _sqlConnectionString = ConnectionStrinManager.etDecryptedConnectionString();

        // ** SQL Server Metotları **
        public static DataTable GetSqlQueuedEmails()
        {
            return ExecuteSqlStoredProcedure("GetSqlQueuedEmails");
        }

        public static DataTable GetEmailFromId(int emailId)
        {
            return ExecuteSqlStoredProcedure("GetEmailFromId", new SqlParameter("@EmailId", emailId));
        }

        public static DataTable UpdateSqlEmailStatus(int emailId, int result)
        {
            return ExecuteSqlStoredProcedure("UpdateSqlEmailStatus", new SqlParameter("@EmailId", emailId), new SqlParameter("@Result", result));
        }
        public static DataTable GetHtmlTemplate(string jsonData)
        {
            return ExecuteSqlStoredProcedure("GetHtmlTemplate", new SqlParameter("@JsonData", jsonData));
            Console.WriteLine(jsonData);

        }

        public static DataTable GetSqlEmailsByStatus(int state)
        {
            return ExecuteSqlStoredProcedure("GetEmailsByStatus", new SqlParameter("@State", state));
        }


        public static string GetStudentPicturePath(string databaseType, int studentId)
        {
            string query = "SELECT PicturePath FROM Students WHERE Id = @StudentId";

            if (databaseType == "MsSqlConnection")
            {
                using (SqlConnection conn = new SqlConnection(_sqlConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        conn.Open();
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }
            }

            return null;
        }


        // ** PostgreSQL Metotları **
        public static DataTable GetPostgreQueuedEmails()
        {
            return ExecutePostgreQuery("SELECT * FROM get_postgre_queued_emails();");
        }

        public static DataTable GetPostgreEmailFromId(int emailId)
        {
            return ExecutePostgreQuery("SELECT * FROM get_postgre_email_from_id(@EmailId);", new NpgsqlParameter("@EmailId", emailId));
        }

        public static void UpdatePostgreEmailStatus(int emailId, int result)
        {
            ExecutePostgreNonQuery("SELECT * FROM update_postgre_email_state(@p_email_id, @p_result);", new NpgsqlParameter("@p_email_id", emailId), new NpgsqlParameter("@p_result", result));
        }

        // ** SQL Server - Tekil Metot ile Stored Procedure Çalıştırma **
        private static DataTable ExecuteSqlStoredProcedure(string procedureName, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    return ExecuteSqlCommand(cmd, connection);
                }

            }
        }

        private static DataTable ExecuteSqlCommand(SqlCommand cmd, SqlConnection connection)
        {
            connection.Open();
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // ** PostgreSQL - Tekil Metot ile Query Çalıştırma **
        private static DataTable ExecutePostgreQuery(string query, params NpgsqlParameter[] parameters)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_postgreConnectionString))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);

                    return ExecutePostgreCommand(cmd, connection);
                }
            }
        }

        private static void ExecutePostgreNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_postgreConnectionString))
            {
                connection.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static DataTable ExecutePostgreCommand(NpgsqlCommand cmd, NpgsqlConnection connection)
        {
            connection.Open();
            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public static DataTable GetEmailsByStatus(int state)
        {
            using (SqlConnection conn = new SqlConnection(_sqlConnectionString))
            {
                string query = "SELECT Id, JsonData FROM Emails WHERE State = @State";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@State", state);
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable resultTable = new DataTable();
                        adapter.Fill(resultTable);
                        return resultTable;
                    }
                }
            }
        }

        public static DataTable GetPostgreEmailsByStatus(int state)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_postgreConnectionString))
            {
                string query = "SELECT id AS Id, jsondata AS JsonData FROM Emails WHERE state = @State";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@State", state);
                    conn.Open();

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable resultTable = new DataTable();
                        adapter.Fill(resultTable);
                        return resultTable;
                    }
                }
            }
        }
    }
}
