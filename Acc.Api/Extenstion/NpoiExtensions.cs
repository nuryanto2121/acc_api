using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Extenstion
{
    public static class NpoiExtensions
    {
        public static string GetFormattedCellValue(this ICell cell, IFormulaEvaluator eval = null)
        {
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue;

                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                        {
                            
                            try
                            {
                                DateTime date = cell.DateCellValue;

                                ICellStyle style = cell.CellStyle;
                                // Excel uses lowercase m for month whereas .Net uses uppercase
                                string format = style.GetDataFormatString().Replace('m', 'M');
                                if (format== "M/d/yy")
                                {
                                    format = "dd/MM/yyyy HH:mm:ss";
                                }
                                return date.ToString(format);
                            }
                            catch (NullReferenceException)
                            {
                                var dt = DateTime.FromOADate(cell.NumericCellValue).ToString("dd/MM/yyyy HH:mm:ss");
                                return dt;
                            }
                        }
                        else
                        {
                            return cell.NumericCellValue.ToString();
                        }

                    case CellType.Boolean:
                        return cell.BooleanCellValue ? "TRUE" : "FALSE";

                    case CellType.Formula:
                        if (eval != null)
                            return GetFormattedCellValue(eval.EvaluateInCell(cell));
                        else
                            return cell.CellFormula;

                    case CellType.Error:
                        return FormulaError.ForInt(cell.ErrorCellValue).String;
                }
            }
            // null or blank cell, or unknown cell type
            return string.Empty;
        }
    }
}
