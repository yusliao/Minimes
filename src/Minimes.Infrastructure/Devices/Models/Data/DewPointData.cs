namespace Minimes.Infrastructure.Devices.Models.Data;

/// <summary>
/// 数据质量枚举
/// </summary>
public enum DataQuality
{
    /// <summary>未知</summary>
    Unknown = 0,

    /// <summary>优秀（90-100%）</summary>
    Excellent = 1,

    /// <summary>良好（80-90%）</summary>
    Good = 2,

    /// <summary>一般（60-80%）</summary>
    Fair = 3,

    /// <summary>差（<60%）</summary>
    Poor = 4
}

/// <summary>
/// 露点仪数据
/// </summary>
public class DewPointData
{
    /// <summary>温度（摄氏度）</summary>
    public decimal Temperature { get; set; }

    /// <summary>相对湿度（%）</summary>
    public decimal Humidity { get; set; }

    /// <summary>露点温度（摄氏度）</summary>
    public decimal DewPoint { get; set; }

    /// <summary>气压（hPa）</summary>
    public decimal? Pressure { get; set; }

    /// <summary>绝对湿度（g/m³）</summary>
    public decimal? AbsoluteHumidity { get; set; }

    /// <summary>数据质量</summary>
    public DataQuality DataQuality { get; set; } = DataQuality.Unknown;

    /// <summary>时间戳</summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// 计算露点温度（Magnus公式）
    /// </summary>
    /// <param name="temperature">温度（摄氏度）</param>
    /// <param name="humidity">相对湿度（%）</param>
    /// <returns>露点温度（摄氏度）</returns>
    public static decimal CalculateDewPoint(decimal temperature, decimal humidity)
    {
        // Magnus公式常数
        const decimal a = 17.27m;
        const decimal b = 237.7m;

        // 计算中间值
        var alpha = ((a * temperature) / (b + temperature)) + (decimal)Math.Log((double)(humidity / 100m));

        // 计算露点温度
        var dewPoint = (b * alpha) / (a - alpha);

        return Math.Round(dewPoint, 2);
    }

    /// <summary>
    /// 根据温度和湿度创建露点数据
    /// </summary>
    public static DewPointData Create(decimal temperature, decimal humidity, decimal? pressure = null)
    {
        var dewPoint = CalculateDewPoint(temperature, humidity);

        return new DewPointData
        {
            Temperature = temperature,
            Humidity = humidity,
            DewPoint = dewPoint,
            Pressure = pressure,
            DataQuality = DataQuality.Good
        };
    }

    public override string ToString()
    {
        return $"温度: {Temperature:F1}°C, 湿度: {Humidity:F1}%, 露点: {DewPoint:F1}°C";
    }
}
