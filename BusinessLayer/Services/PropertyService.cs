using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.DataAccess;
using DataLayer.Models;
using DealRealState.Data.DataAccess;

namespace BusinessLayer.Services
{
   
   public class PropertyService
        {

        public async Task<Property?> GetPropertyByIDAsync(int propertyID)
        {
            if (propertyID <= 0)
                return null;

            return await PropertyData.GetPropertyByIDAsync(propertyID);
        }
        public static async Task<List<Property>> GetAllProperties(string propertyType = null)
        {
            // أضيفي .ToList() هنا إذا كانت تعود كـ IEnumerable
            var result = await PropertyData.GetAllPropertiesAsync(propertyType);
            return result.ToList();
        }
        public async Task<bool> AddPanoramaToPropertyAsync(int propertyId, string fileUrl)
        {
            if (propertyId <= 0 || string.IsNullOrWhiteSpace(fileUrl))
                return false;

            // استدعاء الدالة من كلاس البيانات الصحيح وتمرير البارامترات
            return await PropertyPanoramaData.AddPanoramaToPropertyAsync(propertyId, fileUrl);
        }

        public int AddProperty(Property property)
        {
            if (property == null)
                return 0;

            if (string.IsNullOrWhiteSpace(property.Title))
                return 0;

            if (property.Price <= 0)
                return 0;


            return PropertyData.AddProperty(property);
        }
        public bool UpdateProperty(Property property)
        {
            if (property == null)
                return false;

            if (property.PropertyID <= 0)
                return false;

            return PropertyData.UpdateProperty(property);
        }
        public bool DeleteProperty(int propertyID)
        {
            if (propertyID <= 0)
                return false;

            

            return PropertyData.DeleteProperty(propertyID);
        }
    }
   
}
