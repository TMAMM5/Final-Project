using Final_Project.Models;

namespace Final_Project.Repository.WeightSettingRepo
{
    public interface IWeightSettingRepo
    {
        IEnumerable<WeightSetting> GetAll();
        WeightSetting GetById(int id);
        void Save();
        void Update(WeightSetting weightSetting);
    }
}