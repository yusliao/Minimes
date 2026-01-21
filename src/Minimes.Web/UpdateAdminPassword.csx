#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 8.0.11"
using System;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

// 密码哈希函数（与PasswordHashService相同）
string HashPassword(string password)
{
    const int IterationCount = 10000;
    const int SaltSize = 16;
    const int HashSize = 32;
    
    using (var rng = RandomNumberGenerator.Create())
    {
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);
        
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, IterationCount, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);
            string saltBase64 = Convert.ToBase64String(salt);
            string hashBase64 = Convert.ToBase64String(hash);
            return $"{saltBase64}:{hashBase64}";
        }
    }
}

// 生成新密码哈希
string newPasswordHash = HashPassword("Admin123456");
Console.WriteLine($"新密码哈希: {newPasswordHash}");

// 更新数据库
using (var connection = new SqliteConnection("Data Source=minimes.db"))
{
    connection.Open();
    using (var command = connection.CreateCommand())
    {
        command.CommandText = "UPDATE Users SET PasswordHash = @hash WHERE Username = 'admin'";
        command.Parameters.AddWithValue("@hash", newPasswordHash);
        int rows = command.ExecuteNonQuery();
        Console.WriteLine($"更新了 {rows} 行");
    }
}

Console.WriteLine("admin密码已更新为: Admin123456");
