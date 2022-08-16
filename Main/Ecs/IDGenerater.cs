using YuoTools.Main.Ecs;

namespace YuoTools.ECS
{
    public class IDGenerater : YuoComponent
    {
        private static IDGenerater _instance;

        public static IDGenerater Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IDGenerater();
                }

                return _instance;
            }
        }

        public enum IDType
        {
            Scene,
        }

        public static long GetID(IDType type, long id)
        {
            switch (type)
            {
                case IDType.Scene:
                    return id + 10000;
                default:
                    return id;
            }
        }
        
        public static long GetID(Main.Ecs.YuoEntity entity)
        {
            return entity.GetHashCode();
        }

        public static long GetID(string name)
        {
            return name.GetHashCode() + 10000000000;
        }

        public void Init()
        {
        }
    }

    public class IDGeneraterSystem : YuoSystem<IDGenerater>, IAwake
    {
        protected override void Run(IDGenerater component)
        {
            component.Init();
        }
    }
}