using System;
using System.Data;

namespace eacc
{
    public enum ModuleType : int
    {
        None = 0,
        Pilot = 1,
        Weapon = 2,
        Engine = 4
    }

    public abstract class Module
    {
        public static int GetCost(ModuleType modType)
        {
            var mod = CreateOne(modType);
            if (mod == null)
                return 0;
            //else
            return mod.GetCost();
        }

        public int Id { get; set; }

        public int VehicleId { get; set; }

        public string Name { get; set; }

        public ModuleType ModuleType { get; private set; }

        protected Module(ModuleType mt)
        {
            this.ModuleType = mt;
            this.Id = -1;
        }

        public abstract int GetCost();

        public static Module Create(DataRow row)
        {
            ModuleType typ = (ModuleType)((int)row["MOD_TYPE_NU"]);
            Module res = null;
            switch (typ)
            {
                case ModuleType.Pilot:
                    res = new PilotModule();
                    break;
                case ModuleType.Weapon:
                    res = new WeaponModule();
                    break;
                case ModuleType.Engine:
                    res = new EngineModule();
                    break;
                case ModuleType.None:
                default:
                    break;
            }
            if (res != null)
            {
                res.Id = (int)row["MOD_ID"];
                res.Name = (string)row["MOD_NAME_CH"];
                res.VehicleId = (int)row["VCL_ID"];
            }
            return res;
        }

        public static Module CreateOne(ModuleType modType)
        {
            switch (modType)
            {
                case ModuleType.Pilot:
                    return new PilotModule();
                case ModuleType.Weapon:
                    return new WeaponModule();
                case ModuleType.Engine:
                    return new EngineModule();
                default:
                case ModuleType.None:
                    break;
            }
            return null;
        }
    }

    public class WeaponModule : Module
    {
        public WeaponModule() : base(ModuleType.Weapon)
        {
            Name = "WeaponModule";
        }

        public override int GetCost()
        {
            return 2;
        }
    }

    public class PilotModule : Module
    {
        public PilotModule() : base(ModuleType.Pilot)
        {
            Name = "PilotModule";
        }

        public override int GetCost()
        {
            return 1;
        }
    }

    public class EngineModule : Module
    {
        public EngineModule() : base(ModuleType.Engine)
        {
            Name = "EngineModule";
        }

        public override int GetCost()
        {
            return 2;
        }
    }
}