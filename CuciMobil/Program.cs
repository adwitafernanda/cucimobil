using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace CuciMobil
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program pr = new Program();
            while (true)
            {
                string db = "CuciMobilDB";
                SqlConnection conn = null;
                string strKoneksi = "Data Source = DESKTOP-NVMQ17T\\ADWITAFERNANDA;" +
                    "Initial Catalog = {0};" +
                    "User ID = sa ; Password = Oksip.123;";
                conn = new SqlConnection(string.Format(strKoneksi, db));
                conn.Open();
                Console.Clear();
                while (true)
                {
                    Console.WriteLine("1. read transaksi ");
                    Console.WriteLine("2. create transaksi ");
                    Console.WriteLine("3. update transaksi ");
                    Console.WriteLine("4. read user ");
                    Console.WriteLine("5. create user ");
                    Console.WriteLine("6. read data mobil");
                    Console.WriteLine("7. update data mobil");
                    Console.WriteLine("8. delete data mobil");
                    Console.WriteLine("9. create data mobil");
                    Console.WriteLine("\n enter your choice (1-8): ");
                    char ch = Convert.ToChar(Console.ReadLine());

                    switch (ch)
                    {
                        case '1':
                            pr.ReadTransaksi(conn);
                            break;
                        case '2':
                            pr.CreateTransaksi(conn);
                            break;
                        case '3':
                            pr.UpdateTransaksi(conn);
                            break;
                        case '4':
                            pr.ReadUser(conn);
                            break;
                        case '5':
                            pr.CreateUser(conn);
                            break;
                        case '6':
                            pr.ReadDataMobil(conn);
                            break;
                        case '7':
                            pr.UpdateDataMobil(conn);
                            break;
                        case '8':
                            pr.DeleteDataMobil(conn);
                            break;
                        case '9':
                            pr.CreateDataMobil(conn);
                            break;
                    }
                }

            }
        }

        public void ReadTransaksi(SqlConnection conn)
        {
            try
            {
                string query = "SELECT * FROM Transaksi";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Baca data transaksi dan tampilkan
                    Console.WriteLine($"Kode Transaksi: {reader.GetString(0)}");
                    Console.WriteLine($"Tanggal Transaksi: {reader.GetDateTime(1)}");
                    Console.WriteLine($"Total Biaya: {reader.GetDecimal(2)}");
                    Console.WriteLine($"ID User: {reader.GetString(3)}");
                    Console.WriteLine($"Layanan: {reader.GetString(4)}");
                    Console.WriteLine($"ID Mobil: {reader.GetString(5)}");
                    Console.WriteLine();
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public void CreateTransaksi(SqlConnection conn)
        {
            try
            {
                Console.WriteLine("Data pengguna:");

                // Query SQL untuk mendapatkan data pengguna yang tersedia
                string selectUserQuery = "SELECT ID_User, Nama FROM users";
                SqlCommand selectUserCmd = new SqlCommand(selectUserQuery, conn);

                // Membaca data pengguna yang tersedia
                using (SqlDataReader userReader = selectUserCmd.ExecuteReader())
                {
                    while (userReader.Read())
                    {
                        Console.WriteLine($"ID_User: {userReader["ID_User"]}, Nama: {userReader["Nama"]}");
                    }
                }

                Console.WriteLine("Data mobil:");

                // Query SQL untuk mendapatkan data mobil yang tersedia
                string selectMobilQuery = "SELECT ID_Mobil, Merek, Warna, Tipe FROM Mobil";
                SqlCommand selectMobilCmd = new SqlCommand(selectMobilQuery, conn);

                // Membaca data mobil yang tersedia
                using (SqlDataReader mobilReader = selectMobilCmd.ExecuteReader())
                {
                    while (mobilReader.Read())
                    {
                        Console.WriteLine($"ID Mobil: {mobilReader["ID_Mobil"]}, Merek: {mobilReader["Merek"]}, Warna: {mobilReader["Warna"]}, Tipe: {mobilReader["Tipe"]}");
                    }
                }


                // Meminta input dari pengguna
                Console.Write("Kode Transaksi: ");
                string kdTransaksi = Console.ReadLine();

                // Query SQL untuk memeriksa apakah kode transaksi sudah ada
                string checkQuery = "SELECT COUNT(*) FROM Transaksi WHERE Kd_Transaksi = @KdTransaksi";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@KdTransaksi", kdTransaksi);

                int existingCount = (int)checkCmd.ExecuteScalar();

                if (existingCount > 0)
                {
                    Console.WriteLine("Data dengan kode transaksi tersebut sudah ada. Silakan masukkan kode transaksi yang berbeda.");
                    return; // Keluar dari method karena data sudah ada
                }

                Console.Write("Total Biaya: ");
                decimal totalBiaya = Convert.ToDecimal(Console.ReadLine());
                Console.Write("ID User: ");
                string idUser = Console.ReadLine();
                Console.Write("Layanan: ");
                string layanan = Console.ReadLine();
                Console.Write("ID Mobil: ");
                string idMobil = Console.ReadLine();
                Console.Write("Tanggal Transaksi (dd/MM/yyyy HH:mm:ss): ");
                string inputTanggal = Console.ReadLine();
                DateTime tglTransaksi;

                // Validasi input tanggal
                if (string.IsNullOrWhiteSpace(inputTanggal) || !DateTime.TryParseExact(inputTanggal, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tglTransaksi))
                {
                    Console.WriteLine("Format tanggal berhasil di masukkan. Menggunakan tanggal dan waktu saat ini.");
                    tglTransaksi = DateTime.Now;
                }

                // Query SQL untuk menyisipkan data transaksi baru
                string query = "INSERT INTO Transaksi (Kd_Transaksi, Tgl_Transaksi, Total_biaya, Id_User, Layanan, Id_Mobil) VALUES (@KdTransaksi, @TglTransaksi, @TotalBiaya, @IdUser, @Layanan, @IdMobil)";

                // Membuat command untuk eksekusi query
                SqlCommand cmd = new SqlCommand(query, conn);

                // Menambahkan parameter ke command
                cmd.Parameters.AddWithValue("@KdTransaksi", kdTransaksi);
                cmd.Parameters.AddWithValue("@TglTransaksi", tglTransaksi);
                cmd.Parameters.AddWithValue("@TotalBiaya", totalBiaya);
                cmd.Parameters.AddWithValue("@IdUser", idUser);
                cmd.Parameters.AddWithValue("@Layanan", layanan);
                cmd.Parameters.AddWithValue("@IdMobil", idMobil);

                // Eksekusi command
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Transaksi berhasil dibuat.");
                }
                else
                {
                    Console.WriteLine("Gagal membuat transaksi.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Angka ini menunjukkan kode kesalahan untuk kesalahan unik kunci ganda
                {
                    Console.WriteLine("Data sudah ada dan gagal untuk menambah data.");
                }
                else
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        public void UpdateTransaksi(SqlConnection conn)
        {
            try
            {
                Console.WriteLine("Masukkan kode transaksi yang akan diperbarui:");
                string kdTransaksi = Console.ReadLine();

                // Periksa apakah transaksi dengan kode yang dimasukkan pengguna ada dalam database
                string checkQuery = "SELECT COUNT(*) FROM Transaksi WHERE Kd_Transaksi = @KdTransaksi";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@KdTransaksi", kdTransaksi);
                int transaksiCount = (int)checkCmd.ExecuteScalar();

                if (transaksiCount == 0)
                {
                    Console.WriteLine("Transaksi tidak ditemukan.");
                    return;
                }

                // Meminta input baru dari pengguna
                Console.WriteLine("Masukkan informasi baru untuk transaksi:");

                // Meminta input dari pengguna
                Console.Write("Tanggal Transaksi (yyyy-MM-dd): ");
                DateTime tglTransaksi = Convert.ToDateTime(Console.ReadLine());
                Console.Write("Total Biaya: ");
                decimal totalBiaya = Convert.ToDecimal(Console.ReadLine());
                Console.Write("ID User: ");
                string idUser = Console.ReadLine();
                Console.Write("Layanan: ");
                string layanan = Console.ReadLine();
                Console.Write("ID Mobil: ");
                string idMobil = Console.ReadLine();

                // Query SQL untuk memperbarui data transaksi
                string updateQuery = "UPDATE Transaksi SET Tgl_Transaksi = @TglTransaksi, Total_biaya = @TotalBiaya, Id_User = @IdUser, Layanan = @Layanan, Id_Mobil = @IdMobil WHERE Kd_Transaksi = @KdTransaksi";

                // Membuat command untuk eksekusi query
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);

                // Menambahkan parameter ke command
                updateCmd.Parameters.AddWithValue("@TglTransaksi", tglTransaksi);
                updateCmd.Parameters.AddWithValue("@TotalBiaya", totalBiaya);
                updateCmd.Parameters.AddWithValue("@IdUser", idUser);
                updateCmd.Parameters.AddWithValue("@Layanan", layanan);
                updateCmd.Parameters.AddWithValue("@IdMobil", idMobil);
                updateCmd.Parameters.AddWithValue("@KdTransaksi", kdTransaksi);

                // Eksekusi command
                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Transaksi berhasil diperbarui.");
                }
                else
                {
                    Console.WriteLine("Gagal memperbarui transaksi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void ReadUser(SqlConnection conn)
        {
            try
            {
                // Query SQL untuk membaca data user
                string query = "SELECT * FROM Users";

                // Membuat command untuk eksekusi query
                SqlCommand cmd = new SqlCommand(query, conn);

                // Membaca data menggunakan SqlDataReader
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Mengecek apakah ada baris data untuk dibaca
                    if (reader.HasRows)
                    {
                        // Membaca dan menampilkan data user
                        Console.WriteLine("Data User:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID User: {reader.GetString(0)}");
                            Console.WriteLine($"Nama: {reader.GetString(1)}");
                            Console.WriteLine($"Jenis Kelamin: {reader.GetString(2)}");
                            Console.WriteLine($"Alamat: {reader.GetString(3)}");
                            Console.WriteLine($"No Telepon: {reader.GetString(4)}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Tidak ada data user.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void CreateUser(SqlConnection conn)
        {
            try
            {
                Console.WriteLine("Masukkan informasi user baru:");

                // Meminta input dari pengguna
                Console.Write("ID User: ");
                string idUser = Console.ReadLine();
                Console.Write("Nama: ");
                string nama = Console.ReadLine();
                Console.Write("Jenis Kelamin: ");
                string jenisKelamin = Console.ReadLine();
                Console.Write("Alamat: ");
                string alamat = Console.ReadLine();
                Console.Write("No Telepon: ");
                string noTlpn = Console.ReadLine();

                // Query SQL untuk menyisipkan data user baru
                string query = "INSERT INTO Users (ID_User, Nama, Jenis_Kelamin, Alamat, No_Tlpn) VALUES (@IDUser, @Nama, @JenisKelamin, @Alamat, @NoTlpn)";

                // Membuat command untuk eksekusi query
                SqlCommand cmd = new SqlCommand(query, conn);

                // Menambahkan parameter ke command
                cmd.Parameters.AddWithValue("@IDUser", idUser);
                cmd.Parameters.AddWithValue("@Nama", nama);
                cmd.Parameters.AddWithValue("@JenisKelamin", jenisKelamin);
                cmd.Parameters.AddWithValue("@Alamat", alamat);
                cmd.Parameters.AddWithValue("@NoTlpn", noTlpn);

                // Eksekusi command
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("User berhasil dibuat.");
                }
                else
                {
                    Console.WriteLine("Gagal membuat user.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Angka ini menunjukkan kode kesalahan untuk kesalahan unik kunci ganda
                {
                    Console.WriteLine("Data sudah ada dan gagal untuk menambah data.");
                }
                else
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void ReadDataMobil(SqlConnection conn)
        {
            try
            {
                // Query SQL untuk membaca data mobil
                string query = "SELECT * FROM Mobil";

                // Membuat command untuk eksekusi query
                SqlCommand cmd = new SqlCommand(query, conn);

                // Membaca data menggunakan SqlDataReader
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Mengecek apakah ada baris data untuk dibaca
                    if (reader.HasRows)
                    {
                        // Membaca dan menampilkan data mobil
                        Console.WriteLine("Data Mobil:");
                        while (reader.Read())
                        {
                            Console.WriteLine($"ID Mobil: {reader.GetString(0)}");
                            Console.WriteLine($"Merek: {reader.GetString(1)}");
                            Console.WriteLine($"Warna: {reader.GetString(2)}");
                            Console.WriteLine($"Tipe: {reader.GetString(3)}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Tidak ada data mobil.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateDataMobil(SqlConnection conn)
        {
            try
            {
                Console.WriteLine("Masukkan ID mobil yang akan diperbarui:");
                string idMobil = Console.ReadLine();

                // Periksa apakah mobil dengan ID yang dimasukkan pengguna ada dalam database
                string checkQuery = "SELECT COUNT(*) FROM Mobil WHERE ID_Mobil = @IdMobil";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@IdMobil", idMobil);
                int mobilCount = (int)checkCmd.ExecuteScalar();

                if (mobilCount == 0)
                {
                    Console.WriteLine("Mobil tidak ditemukan.");
                    return;
                }

                // Meminta input baru dari pengguna
                Console.WriteLine("Masukkan informasi baru untuk mobil:");

                // Meminta input dari pengguna
                Console.Write("Merek: ");
                string merek = Console.ReadLine();
                Console.Write("Warna: ");
                string warna = Console.ReadLine();
                Console.Write("Tipe: ");
                string tipe = Console.ReadLine();

                // Query SQL untuk memperbarui data mobil
                string updateQuery = "UPDATE Mobil SET Merek = @Merek, Warna = @Warna, Tipe = @Tipe WHERE ID_Mobil = @IdMobil";

                // Membuat command untuk eksekusi query
                SqlCommand updateCmd = new SqlCommand(updateQuery, conn);

                // Menambahkan parameter ke command
                updateCmd.Parameters.AddWithValue("@Merek", merek);
                updateCmd.Parameters.AddWithValue("@Warna", warna);
                updateCmd.Parameters.AddWithValue("@Tipe", tipe);
                updateCmd.Parameters.AddWithValue("@IdMobil", idMobil);

                // Eksekusi command
                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Data mobil berhasil diperbarui.");
                }
                else
                {
                    Console.WriteLine("Gagal memperbarui data mobil.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteDataMobil(SqlConnection conn)
        {
            try
            {
                Console.WriteLine("Masukkan ID mobil yang akan dihapus:");
                string idMobil = Console.ReadLine();

                // Periksa apakah mobil dengan ID yang dimasukkan pengguna ada dalam database
                string checkQuery = "SELECT COUNT(*) FROM Mobil WHERE ID_Mobil = @IdMobil";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@IdMobil", idMobil);
                int mobilCount = (int)checkCmd.ExecuteScalar();

                if (mobilCount == 0)
                {
                    Console.WriteLine("Mobil tidak ditemukan.");
                    return;
                }

                // Query SQL untuk menghapus data mobil
                string deleteQuery = "DELETE FROM Mobil WHERE ID_Mobil = @IdMobil";

                // Membuat command untuk eksekusi query
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);

                // Menambahkan parameter ke command
                deleteCmd.Parameters.AddWithValue("@IdMobil", idMobil);

                // Eksekusi command
                int rowsAffected = deleteCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Data mobil berhasil dihapus.");
                }
                else
                {
                    Console.WriteLine("Gagal menghapus data mobil.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        public void CreateDataMobil(SqlConnection conn)
        {
            try
            {
                Console.WriteLine("Masukkan data mobil:");

                // Meminta input dari pengguna
                Console.Write("ID Mobil: ");
                string idmbl = Console.ReadLine();
                Console.Write("merk: ");
                string merek = Console.ReadLine();
                Console.Write("warna: ");
                string warna = Console.ReadLine();
                Console.Write("tipe: ");
                string tipe = Console.ReadLine();

                // Query SQL untuk menyisipkan data user baru
                string query = "INSERT INTO Mobil (ID_Mobil, Merek, Warna, Tipe) VALUES (@IDmobil, @merek, @warna, @tipe)";

                // Membuat command untuk eksekusi query
                SqlCommand cmd = new SqlCommand(query, conn);

                // Menambahkan parameter ke command
                cmd.Parameters.AddWithValue("@IDmobil", idmbl);
                cmd.Parameters.AddWithValue("@merek", merek);
                cmd.Parameters.AddWithValue("@warna", warna);
                cmd.Parameters.AddWithValue("@tipe", tipe);

                // Eksekusi command
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("data berhasil dibuat.");
                }
                else
                {
                    Console.WriteLine("Gagal membuat data.");
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Angka ini menunjukkan kode kesalahan untuk kesalahan unik kunci ganda
                {
                    Console.WriteLine("Data sudah ada dan gagal untuk menambah data.");
                }
                else
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
