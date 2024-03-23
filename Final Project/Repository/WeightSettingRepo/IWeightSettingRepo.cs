using Final_Project.Models;

namespace Final_Project.Repository.WeightSettingRepo
{
    public interface IWeightSettingRepo
    {
        List<WeightSetting> GetAll();
        WeightSetting GetById(int id);
        void Save();
        void Update(WeightSetting weightSetting);
    }
}