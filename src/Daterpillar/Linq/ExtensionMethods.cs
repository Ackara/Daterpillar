using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Linq
{
	/// <summary>
	/// Provides extension methods.
	/// </summary>
	public static class ExtensionMethods
	{
		public static string ToSQL(this object value)
		{
			if (value == null)
			{
				return "null";
			}
			else if (value is bool bit)
			{
				return bit ? "'1'" : "'0'";
			}
			else if (value is TimeSpan time)
			{
				return string.Format("{0:hh}:{0:mm}:{0:ss}", time);
			}
			else if (value is DateTime date)
			{
				return $"'{date.ToString("yyyy-MM-dd HH:mm:ss")}'";
			}
			else if (value.GetType().GetTypeInfo().IsEnum)
			{
				return $"'{(int)value}'";
			}
			else
			{
				return $"'{value.ToString().Replace("'", "''")}'";
			}
		}

		public static string Escape(this string name, Syntax syntax = Syntax.Generic)
		{
			switch (syntax)
			{
				default:
				case Syntax.Generic:
					return name;

				case Syntax.MSSQL:
				case Syntax.SQLite:
					return $"[{name}]";

				case Syntax.MySQL:
					return $"`{name}`";
			}
		}
		
		public static string GetTableName(this object entity)
		{
			TableAttribute attr = entity.GetType().GetTypeInfo().GetCustomAttribute<TableAttribute>();
			return attr?.Name;
		}

		public static IEnumerable<ColumnInfo> GetColumns(this object entity)
		{
			Type type = entity.GetType();
			bool objectIsaTable = type.GetTypeInfo().GetCustomAttribute<TableAttribute>() != null;

			if (objectIsaTable)
			{
				ColumnAttribute attribute;
				foreach (var property in type.GetRuntimeProperties())
				{
					attribute = property.GetCustomAttribute<ColumnAttribute>();
					if (attribute != null)
					{
						yield return new ColumnInfo()
						{
							Property = property,
							Attribute = attribute
						};
					}
				}
			}
		}

		public static string ToQuery(this object entity, Syntax syntax = Syntax.Generic)
		{
			Type type = entity.GetType();
			TableAttribute attribute = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
			if (attribute != null)
			{
				string table = Escape((string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name), syntax);
				return $"SELECT * FROM {table} WHERE {string.Join(" AND ", GetWhereClause(entity, syntax))};";
			}
			else return null;
		}

		public static string ToDeleteCommand(this object entity, Syntax syntax = Syntax.Generic)
		{
			Type type = entity.GetType();
			TableAttribute attribute = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
			if (attribute != null)
			{
				string table = Escape((string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name), syntax);
				return $"DELETE FROM {table} WHERE {string.Join(" AND ", GetWhereClause(entity, syntax))};";
			}
			else return null;
		}

		public static string ToInsertCommand(this object entity, Syntax syntax = Syntax.Generic)
		{
			Type type = entity.GetType();
			var tableAttribute = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
			if (tableAttribute != null)
			{
				var columns = GetColumns(entity);
				var columnNames = from c in columns
								  where c.Attribute.AutoIncrement == false
								  select (string.IsNullOrEmpty(c.Attribute.Name) ? c.Property.Name : c.Attribute.Name);

				var columnValues = from c in columns
								   where c.Attribute.AutoIncrement == false
								   select c.Property.GetValue(entity);

				string tableName = Escape((string.IsNullOrEmpty(tableAttribute.Name) ? type.Name : tableAttribute.Name), syntax);
				string fields = string.Join(", ", columnNames.Select(name => $"{Escape(name, syntax)}"));
				string values = string.Join(", ", columnValues.Select(v => $"{v.ToSQL()}"));

				return $"INSERT INTO {tableName} ({fields}) VALUES ({values});";
			}
			else return null;
		}

		public static string ToInsertCommand<T>(this IEnumerable<T> collection, Syntax syntax = Syntax.Generic)
		{
			Type type = typeof(T);
			var tableAttribute = type.GetTypeInfo().GetCustomAttribute<TableAttribute>();
			if (tableAttribute != null)
			{
				bool firstElement = true;
				var script = new StringBuilder();
				IEnumerable<ColumnInfo> columns = null;

				foreach (var item in collection)
				{
					columns = GetColumns(item);

					if (firstElement)
					{
						string tableName = (string.IsNullOrEmpty(tableAttribute.Name) ? type.Name : tableAttribute.Name);
						script.Append($"INSERT INTO {Escape(tableName, syntax)} (");
						var names = from col in columns
									where col.Attribute.AutoIncrement == false
									select (string.IsNullOrEmpty(col.Attribute.Name) ? Escape(col.Property.Name, syntax) : Escape(col.Attribute.Name, syntax));

						script.Append(string.Join(", ", names));
						script.Append(") VALUES" + Environment.NewLine);
						firstElement = false;
					}

					var values = from v in columns
								 where v.Attribute.AutoIncrement == false
								 select (v.Property.GetValue(item).ToSQL());

					script.AppendLine($"({string.Join(", ", values)}),");
				}

				if (script.Length > 0)
				{
					script = script.Remove((script.Length - 3), 3);
					script.Append(";");
				}

				return script.ToString();
			}
			else return null;
		}

		internal static IEnumerable<string> GetWhereClause(object entity, Syntax syntax)
		{
			var columns = GetColumns(entity);
			var autoKey = from c in columns
						  where c.Attribute.AutoIncrement
						  select (string.Format("{0}={1}", Escape((string.IsNullOrEmpty(c.Attribute.Name) ? c.Property.Name : c.Attribute.Name), syntax), ToSQL(c.Property.GetValue(entity))));

			var primaryKeys = from c in columns
							  where c.Property.GetCustomAttribute<IndexAttribute>()?.Type == IndexType.PrimaryKey
							  select (string.Format("{0}={1}", Escape((string.IsNullOrEmpty(c.Attribute.Name) ? c.Property.Name : c.Attribute.Name), syntax), ToSQL(c.Property.GetValue(entity))));

			if (autoKey.FirstOrDefault() == null)
			{
				return primaryKeys;
			}
			else
			{
				return autoKey;
			}
		}
	}
}