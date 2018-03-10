using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SarData.Api.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SarData.Api.Data
{
  public class DataSeeder
  {
    private readonly ILogger log;
    public DataSeeder(ILogger<DataSeeder> log)
    {
      this.log = log;
    }

    public void Seed(StoreContext db, IFileInfo seedFile)
    {
      if (!seedFile.Exists)
      {
        log.LogInformation($"{seedFile.Name} does not exist. Skipping seed database.");
        return;
      }

      if (db.Organizations.Any() || db.Jurisdictions.Any())
      {
        log.LogInformation($"Database contains data. Skipping seed database.");
        return;
      }

      log.LogInformation("Seeding database ...");

      JObject data;
      using (var fileStream = seedFile.CreateReadStream())
      {
        using (var textReader = new StreamReader(fileStream))
        {
          using (var jsonReader = new JsonTextReader(textReader))
          {
            data = JObject.Load(jsonReader);
          }
        }
      }

      LoadTable(db.Jurisdictions, data);
      LoadTable(db.Organizations, data);

      FixLinks(db.Jurisdictions, data);
      FixLinks(db.Organizations, data);
      db.SaveChanges();

      log.LogInformation("Finished database seed.");
    }
    
    private void FixLinks<T>(DbSet<T> table, JObject json) where T : class, IHasId
    {
      List<T> list = GetJsonRows<T>(json, null);
      foreach (T row in list)
      {
        T dbRow = table.Find(row.Id);
        foreach (var property in typeof(T).GetProperties())
        {
          var foreignKey = property.GetCustomAttributes(typeof(ForeignKeyAttribute), true).Cast<ForeignKeyAttribute>().SingleOrDefault();
          if (foreignKey == null) continue;

          var foreignKeyProperty = typeof(T).GetProperty(foreignKey.Name);
          foreignKeyProperty?.SetValue(dbRow, foreignKeyProperty?.GetValue(row));
        }
      }
    }

    private void LoadTable<T>(DbSet<T> table, JObject json) where T : class, IHasId
    {
      List<T> list = GetJsonRows<T>(json, log);
      foreach (T row in list)
      {
        foreach (var property in typeof(T).GetProperties())
        {
          var foreignKey = property.GetCustomAttributes(typeof(ForeignKeyAttribute), true).Cast<ForeignKeyAttribute>().SingleOrDefault();

          if (!IsCollection(property.PropertyType) && foreignKey == null)
            continue;

          property.SetValue(row, null);

          if (foreignKey == null) continue;

          var foreignKeyProperty = typeof(T).GetProperty(foreignKey.Name);
          foreignKeyProperty?.SetValue(row, foreignKeyProperty.PropertyType.IsValueType
                ? Activator.CreateInstance(foreignKeyProperty.PropertyType)
                : null);
        }
        table.Add(row);
      }
    }

    private static List<T> GetJsonRows<T>(JObject json, ILogger log) where T : class, IHasId
    {
      List<T> list = new List<T>();

      if (!json.TryGetValue(typeof(T).Name, out JToken token))
      {
        log?.LogInformation($"Found no {typeof(T).Name} while seeding");
        return list;
      }

      if (token.Type != JTokenType.Array)
      {
        log?.LogError($"Did not find array while seeding {typeof(T).Name}");
        return list;
      }

      list = ((JArray)token).ToObject<List<T>>();
      return list;
    }

    static bool IsCollection(Type type)
    {
      return IsCollectionShallow(type) || type.GetInterfaces().Any(IsCollectionShallow);
    }

    static bool IsCollectionShallow(Type type)
    {
      if (!type.IsGenericType)
        return false;
      Type generic = type.GetGenericTypeDefinition();
      if (generic != typeof(ICollection<>))
        return false;
      return true;
    }
  }
}
