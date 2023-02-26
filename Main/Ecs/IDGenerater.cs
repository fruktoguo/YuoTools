using YuoTools.Main.Ecs;

namespace YuoTools.ECS
{
    public class IDGenerate : YuoComponent
    {
        private static IDGenerate _instance;

        public static IDGenerate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IDGenerate();
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
        
        public static long GetID(YuoEntity entity)
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

    public class IDGenerateSystem : YuoSystem<IDGenerate>, IAwake
    {
        protected override void Run(IDGenerate component)
        {
            component.Init();
        }
    }
}