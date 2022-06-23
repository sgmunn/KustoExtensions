using System;
using System.Collections.Generic;
using System.Data;
using Kusto.Data.Net.Client;

namespace KustoExtensions.Kusto
{
	public class KustoTest
	{
		public KustoTest()
		{
		}

		public QueryResult GetTest(string query)
        {
            // todo: we need to be able to define a list of these
            var client = KustoClientFactory.CreateCslQueryProvider("https://help.kusto.windows.net/Samples;Fed=true");

            //var reader = client.ExecuteQuery(".show function VSMTI_VSMac_PublicVersionsWithFirstReleaseDate");
            var reader = client.ExecuteQuery(query);
            if (reader.RecordsAffected == 0)
            {
            }

            return QueryResult.LoadFromReader(reader);
        }
    }

    public sealed class QueryResult
    {
        private readonly List<DataRow> rows;

        public QueryResult()
        {
            this.rows = new List<DataRow>();
        }

        public Field[] Fields { get; private set; }

        public IEnumerable<DataRow> Rows => this.rows;

        internal static QueryResult LoadFromReader(IDataReader reader)
        {
            var result = CreateFromReader(reader);
            LoadRows(result, reader);
            return result;
        }

        private static QueryResult CreateFromReader(IDataReader reader)
        {
            var result = new QueryResult();
            var fields = new List<Field>(reader.FieldCount);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                fields.Add(new Field(i, reader.GetName(i), reader.GetFieldType(i)));
            }

            result.Fields = fields.ToArray();
            return result;
        }

        private static void LoadRows(QueryResult result, IDataReader reader)
        {
            while (reader.Read())
            {
                var row = new DataRow();
                var fieldData = new List<object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    fieldData.Add(reader.GetValue(i));
                }

                row.Values = fieldData.ToArray();

                result.rows.Add(row);
            }

        }
    }

    public class Field
    {
        public Field(int index, string name, Type type)
        {
            this.Index = index;
            this.Name = name;
            this.Type = type;
        }

        public int Index { get; }

        public Type Type { get; }

        public string Name { get; }
    }

    public class DataRow
    {
        public object[] Values { get; set; }
    }
}

