﻿using Final_Project.Models;

namespace Final_Project.Repository.WeightSettingRepo
{
    public class WeightSettingRepo : IWeightSettingRepo
    {
        private ProjContext _context;
        public WeightSettingRepo(ProjContext context)
        {
            _context = context;
        }
        public IEnumerable<WeightSetting> GetAll()
        {
            return _context.WeightSetting.ToList();
        }

        public WeightSetting GetById(int id)
        {
            return _context.WeightSetting.Find(id);
        }

        public void Update(WeightSetting weightSetting)
        {
            _context.WeightSetting.Update(weightSetting);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
