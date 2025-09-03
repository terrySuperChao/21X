public class Singleton<T> where T:new ()
{
    // 用来保存唯一的实例
    private static T instance;

    // 确保线程安全的锁对象
    private static readonly object lockobj = new object();

    //私有构造函数，防止实例化
    protected Singleton() { 
    }

    // 获取唯一实例的公有静态方法
    public static T Instance {
        get {
            // 双重检查锁定(double-checked locking)
            if (instance == null)
            {
                lock (lockobj)
                {
                    if (instance == null) {
                        instance = new T();
                    }  
                }
            }                    
            return instance;
        }
    }
}
