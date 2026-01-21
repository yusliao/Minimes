using System.Security.Cryptography;

namespace Minimes.Domain.Security;

/// <summary>
/// 密码哈希服务 - 使用PBKDF2算法
/// 这是一个领域层工具，可被任何层使用
/// </summary>
public static class PasswordHashService
{
    private const int IterationCount = 10000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const char Separator = ':';

    /// <summary>
    /// 对密码进行哈希处理并返回"salt:hash"格式的字符串
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("密码不能为空");
        }

        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                IterationCount,
                HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // 将salt和hash转换为Base64字符串，并合并为"salt:hash"格式
                string saltBase64 = Convert.ToBase64String(salt);
                string hashBase64 = Convert.ToBase64String(hash);

                return $"{saltBase64}{Separator}{hashBase64}";
            }
        }
    }

    /// <summary>
    /// 验证密码是否与存储的哈希值匹配
    /// </summary>
    public static bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            // 分离salt和hash
            var parts = passwordHash.Split(Separator);
            if (parts.Length != 2)
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            // 使用相同的salt对输入密码进行哈希处理
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                IterationCount,
                HashAlgorithmName.SHA256))
            {
                byte[] computedHash = pbkdf2.GetBytes(HashSize);

                // 比较哈希值（使用恒定时间比较防止时序攻击）
                return ConstantTimeEquals(computedHash, storedHash);
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 恒定时间比较 - 防止时序攻击
    /// </summary>
    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}
