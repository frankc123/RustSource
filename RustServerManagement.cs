public class RustServerManagement : ServerManagement
{
    public static RustServerManagement Get()
    {
        return (RustServerManagement) ServerManagement.Get();
    }
}

