namespace Minimes.Infrastructure.Devices.Management;

/// <summary>
/// 循环缓冲区（固定大小的环形缓冲区）
/// 艹，这个SB类用于存储设备日志，满了就覆盖最旧的数据
/// </summary>
/// <typeparam name="T">缓冲区元素类型</typeparam>
public class CircularBuffer<T>
{
    private readonly T[] _buffer;
    private readonly int _capacity;
    private int _head;  // 写入位置
    private int _tail;  // 读取位置
    private int _count; // 当前元素数量
    private readonly object _lock = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacity">缓冲区容量</param>
    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("容量必须大于0", nameof(capacity));
        }

        _capacity = capacity;
        _buffer = new T[capacity];
        _head = 0;
        _tail = 0;
        _count = 0;
    }

    /// <summary>
    /// 当前元素数量
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _count;
            }
        }
    }

    /// <summary>
    /// 缓冲区容量
    /// </summary>
    public int Capacity => _capacity;

    /// <summary>
    /// 是否已满
    /// </summary>
    public bool IsFull
    {
        get
        {
            lock (_lock)
            {
                return _count == _capacity;
            }
        }
    }

    /// <summary>
    /// 是否为空
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            lock (_lock)
            {
                return _count == 0;
            }
        }
    }

    /// <summary>
    /// 添加元素（如果满了就覆盖最旧的元素）
    /// </summary>
    public void Add(T item)
    {
        lock (_lock)
        {
            _buffer[_head] = item;
            _head = (_head + 1) % _capacity;

            if (_count < _capacity)
            {
                _count++;
            }
            else
            {
                // 缓冲区已满，移动tail指针
                _tail = (_tail + 1) % _capacity;
            }
        }
    }

    /// <summary>
    /// 获取所有元素（从最旧到最新）
    /// </summary>
    public List<T> GetAll()
    {
        lock (_lock)
        {
            var result = new List<T>(_count);

            if (_count == 0)
            {
                return result;
            }

            int index = _tail;
            for (int i = 0; i < _count; i++)
            {
                result.Add(_buffer[index]);
                index = (index + 1) % _capacity;
            }

            return result;
        }
    }

    /// <summary>
    /// 获取最近N个元素（从最新到最旧）
    /// </summary>
    public List<T> GetRecent(int count)
    {
        lock (_lock)
        {
            if (count <= 0)
            {
                return new List<T>();
            }

            int actualCount = Math.Min(count, _count);
            var result = new List<T>(actualCount);

            if (actualCount == 0)
            {
                return result;
            }

            // 从最新的元素开始（head - 1）
            int index = (_head - 1 + _capacity) % _capacity;
            for (int i = 0; i < actualCount; i++)
            {
                result.Add(_buffer[index]);
                index = (index - 1 + _capacity) % _capacity;
            }

            return result;
        }
    }

    /// <summary>
    /// 清空缓冲区
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            Array.Clear(_buffer, 0, _capacity);
            _head = 0;
            _tail = 0;
            _count = 0;
        }
    }
}
