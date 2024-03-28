using System.Linq.Expressions;

namespace CustomizableEventCalendar.src.CustomizableEventCalendar.ConsoleApp
{
    public static class CollectionExtensions
    {
        public static List<List<string>> InsertInto2DList<T>(this List<T> items, string[] columnHeadings, Expression<Func<T, object>>[] propertySelectors)
        {
            List<List<string>> tableContent = [];

            tableContent.Add(new List<string>(columnHeadings));

            foreach (var item in items)
            {
                List<string> rowData = [];

                foreach (var selectorExpr in propertySelectors)
                {
                    var value = selectorExpr.Compile().Invoke(item);
                    rowData.Add(value == null ? "-" : value.ToString());
                }

                tableContent.Add(rowData);
            }

            return tableContent;
        }
    }
}