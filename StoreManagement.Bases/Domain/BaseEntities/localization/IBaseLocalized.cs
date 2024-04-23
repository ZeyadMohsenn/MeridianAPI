using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;

namespace StoreManagement.Bases.Domain.BaseEntities.localization
{
    /// <summary>
    /// Defines The Language Entity Which Will holds language data definition
    /// </summary>
    public interface ILanguage
    {
    }

    /// <summary>
    /// Interface in Which any class will implement us will used for Our Content Localization Module
    /// </summary>
    /// <typeparam name="Ttranslation"> Translation Object Used for Serialization & DeSerilization ** [DON'T MAPPED TO ANY DbSet]></typeparam>
    public interface ITranslatedEntity<Ttranslation> where Ttranslation : IEntityLocalizationDescription
    {
        public string Entity_Translation { get; set; }

        public IDictionary<string,Ttranslation> GetTranslation()
        {
            return JsonSerializer.Deserialize<IDictionary<string,Ttranslation>>(Entity_Translation)??new Dictionary<string,Ttranslation>();
        }

    }
    /// <summary>
    /// Translation Object Used for Serialization & DeSerialization ** [DON'T MAPPED TO ANY DbSet]
    /// </summary>
    public interface IEntityLocalizationDescription
    {
        public string LanguageCode { get; set; }
    }


    public static class LocalizationDescription
    {

        /// <summary>
        /// converts Collection of translation object into Dictionary
        /// </summary>
        /// <typeparam name="T"> the type class that implements IEntityLocalizationDescription </typeparam>
        /// <param name="values"> Translation Data </param>
        /// <returns> the converted object and ready to parse it Json Object </returns>
        public static dynamic Map<T>(this IEnumerable<T> values) where T : class, IEntityLocalizationDescription
        {
            dynamic obj = new ExpandoObject();
            foreach (var item in values)
            {
                ((IDictionary<string, object>)obj).Add(item.LanguageCode, item);
            }
            return obj??default;
        }

        /// <summary>
        ///  converts Dictionary of  translation object into Collection
        /// </summary>
        /// <typeparam name="T"> Entity </typeparam>
        /// <typeparam name="td">Translation entity</typeparam>
        /// <param name="value"></param>
        /// <returns> returns Deserialized Collection From Json String  </returns>
        public static IEnumerable<TTranslation> ReverseMap<TEntity,TTranslation>(this TEntity value) where TEntity : class, ITranslatedEntity<TTranslation> where TTranslation:IEntityLocalizationDescription
        {
            var data= JsonSerializer.Deserialize<IDictionary<string, TTranslation>>(value.Entity_Translation);

            return data?.Values?.ToList()??new List<TTranslation> () ;
        }


    }
}
